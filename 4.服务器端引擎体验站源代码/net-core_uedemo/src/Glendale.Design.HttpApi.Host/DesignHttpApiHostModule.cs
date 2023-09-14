using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Glendale.Design.EntityFrameworkCore;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic;
using Microsoft.OpenApi.Models;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.Authentication.JwtBearer;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Swashbuckle.AspNetCore.SwaggerUI;
using Volo.Abp.Json.SystemTextJson;
using Volo.Abp.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Text.Json;
using Volo.Abp.BlobStoring.FileSystem;
using Volo.Abp.BlobStoring;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Json.SystemTextJson.JsonConverters;
using Volo.Abp.Json.Newtonsoft;
using System.Text;
using Glendale.Design.Services; 
using IdentityServer4.Models;
using IdentityServer4.Validation;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace Glendale.Design
{
    [DependsOn(
        typeof(DesignHttpApiModule),
        typeof(DesignApplicationModule),
        typeof(DesignEntityFrameworkCoreModule),
        typeof(DesignModelEntityFrameworkCoreModule),        
        typeof(AbpAutofacModule),
        typeof(AbpAspNetCoreMvcUiBasicThemeModule),
        typeof(AbpAspNetCoreAuthenticationJwtBearerModule),
        typeof(AbpAccountWebIdentityServerModule),
        typeof(AbpAspNetCoreSerilogModule),
        typeof(AbpSwashbuckleModule),
        typeof(AbpBlobStoringFileSystemModule)
    )]
    public class DesignHttpApiHostModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            
            Volo.Abp.PermissionManagement.AbpPermissionManagementDbProperties.DbTablePrefix = "Sys_";
            Volo.Abp.SettingManagement.AbpSettingManagementDbProperties.DbTablePrefix = "Sys_";
            Volo.Abp.BackgroundJobs.BackgroundJobsDbProperties.DbTablePrefix = "Sys_";
            Volo.Abp.AuditLogging.AbpAuditLoggingDbProperties.DbTablePrefix = "Sys_";
            Volo.Abp.Identity.AbpIdentityDbProperties.DbTablePrefix = "Sys_";
            Volo.Abp.FeatureManagement.FeatureManagementDbProperties.DbTablePrefix = "Sys_";
            Volo.Abp.IdentityServer.AbpIdentityServerDbProperties.DbTablePrefix = "Ids_";
            //解决文件上传Multipart body length limit 134217728 exceeded
            context.Services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
                x.MemoryBufferThreshold = int.MaxValue;
            });

            // 路由配置
            context.Services.AddRouting(options =>
            {
                // 设置URL为小写
                options.LowercaseUrls = true;
                // 在生成的URL后面添加斜杠
                options.AppendTrailingSlash = true;
            });
            // Http请求
            context.Services.AddHttpClient("IgnoreSSL").ConfigurePrimaryHttpMessageHandler(x =>
            new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, error) => true,
                //AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                //UseCookies = false,
                //AllowAutoRedirect = false,
                //UseDefaultCredentials = true,
            });
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

#if UERELEASE || UEDEBUG
            context.Services.PreConfigure<IIdentityServerBuilder>(builder =>
            {   
                //重写密码登录
                builder.AddResourceOwnerValidator<PasswordGrantValidator>();

                //扩展手机验证码登录
                builder.AddExtensionGrantValidator<PhoneGrantValidator>(); 
            });
#endif

        }
        public override void ConfigureServices(ServiceConfigurationContext context)
        { 
            var configuration = context.Services.GetConfiguration();
            var hostingEnvironment = context.Services.GetHostingEnvironment();

            ConfigureBundles();
            ConfigureBlobStoring(hostingEnvironment);
            ConfigureConventionalControllers();
            ConfigureAuthentication(context, configuration);
            ConfigureLocalization();
            ConfigureVirtualFileSystem(context);
            ConfigureCors(context, configuration);
            ConfigureJsonOptions();
            ConfigureSwaggerServices(context, configuration);             
        }

        private void ConfigureBundles()
        {
            Configure<AbpBundlingOptions>(options =>
            {
                options.StyleBundles.Configure(
                    BasicThemeBundles.Styles.Global,
                    bundle => { bundle.AddFiles("/global-styles.css"); }
                );
            });
        }
        private void ConfigureBlobStoring(IWebHostEnvironment hostingEnvironment)
        {
            Configure<AbpBlobStoringOptions>(options =>
            {
                options.Containers.ConfigureDefault(container =>
                {
                    container.UseFileSystem(fileSystem =>
                    {
                        fileSystem.BasePath = Path.Combine(hostingEnvironment.ContentRootPath, "uploads");
                    });
                });
            });
        }        

        private void ConfigureVirtualFileSystem(ServiceConfigurationContext context)
        {
            var hostingEnvironment = context.Services.GetHostingEnvironment();

            if (hostingEnvironment.IsDevelopment())
            {
                Configure<AbpVirtualFileSystemOptions>(options =>
                {
                    options.FileSets.ReplaceEmbeddedByPhysical<DesignDomainSharedModule>(
                        Path.Combine(hostingEnvironment.ContentRootPath,
                            $"..{Path.DirectorySeparatorChar}Glendale.Design.Domain.Shared"));
                    options.FileSets.ReplaceEmbeddedByPhysical<DesignDomainModule>(
                        Path.Combine(hostingEnvironment.ContentRootPath,
                            $"..{Path.DirectorySeparatorChar}Glendale.Design.Domain"));
                    options.FileSets.ReplaceEmbeddedByPhysical<DesignApplicationContractsModule>(
                        Path.Combine(hostingEnvironment.ContentRootPath,
                            $"..{Path.DirectorySeparatorChar}Glendale.Design.Application.Contracts"));
                    options.FileSets.ReplaceEmbeddedByPhysical<DesignApplicationModule>(
                        Path.Combine(hostingEnvironment.ContentRootPath,
                            $"..{Path.DirectorySeparatorChar}Glendale.Design.Application"));
                });
            }
        }

        private void ConfigureConventionalControllers()
        {
            Configure<AbpAspNetCoreMvcOptions>(options =>
            {
                options.ConventionalControllers.Create(typeof(DesignApplicationModule).Assembly);
            });
        }

        private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddAuthentication()
                .AddJwtBearer(options =>
                {
                    options.Authority = configuration["AuthServer:Authority"];
                    options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
                    options.TokenValidationParameters.ValidateIssuer = Convert.ToBoolean(configuration["AuthServer:ValidateIssuer"]);
                    options.Audience = "Design";
                    options.TokenValidationParameters.ValidateIssuer = Convert.ToBoolean(configuration["AuthServer:ValidateIssuer"]);
                    options.BackchannelHttpHandler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback =
                            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };
                });
        }
        private void ConfigureJsonOptions()
        {
            Configure<AbpSystemTextJsonSerializerOptions>(options =>
            {
                //options.JsonSerializerOptions.Converters.Add(new SystemTextJsonConvert.DatetimeJsonConverter());          //格式化日期时间格式
                //options.JsonSerializerOptions.Converters.Add(new SystemTextJsonConvert.DateTimeNullableConverter());
                //options.JsonSerializerOptions.Converters.Add(new EnumDisplayConverter());
                //options.JsonSerializerOptions.Converters.Add(new SystemTextJsonConvert.LongToStringConverter());
                options.JsonSerializerOptions.WriteIndented = false;                                //格式化json字符串
                options.JsonSerializerOptions.AllowTrailingCommas = true;                           //可以结尾有逗号
                options.JsonSerializerOptions.IgnoreNullValues = true;                              //可以有空值,转换json去除空值属性
                options.JsonSerializerOptions.IgnoreReadOnlyProperties = true;                      //忽略只读属性 
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;                   //反序列化过程中属性名称是否使用忽略大小写
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;    //数据格式首字母小写
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);//取消Unicode编码
            });
            Configure<AbpJsonOptions>(options =>
            {
                options.DefaultDateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            });
        }
        private static void ConfigureSwaggerServices(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddAbpSwaggerGenWithOAuth(
                configuration["AuthServer:Authority"],
                new Dictionary<string, string>
                {
                    {"Design", "Design API"}
                },
                options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Design API", Version = "v1" });
                    options.DocInclusionPredicate((docName, description) => true);
                    options.CustomSchemaIds(type => type.FullName);
                    #region 小绿锁，JWT身份认证配置
                    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                            {
                                {
                                    new OpenApiSecurityScheme()
                                    {
                                        Reference = new OpenApiReference()
                                        {
                                            Id = JwtBearerDefaults.AuthenticationScheme,
                                            Type = ReferenceType.SecurityScheme
                                        }
                                    },
                                    Array.Empty<string>()
                                }
                            });
                    //options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme()
                    //{
                    //    Description = "JWT模式授权，请输入 Bearer {Token} 进行身份验证",
                    //    Name = "Authorization",
                    //    In = ParameterLocation.Header,
                    //    Type = SecuritySchemeType.ApiKey
                    //});
                    #endregion
                });
        }

        private void ConfigureLocalization()
        {
            Configure<AbpLocalizationOptions>(options =>
            {
                options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
            });
        }
        private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
               {
                   builder.AllowAnyOrigin()  //支持所有跨域
                       //.WithOrigins(
                       //    configuration["App:CorsOrigins"]
                       //        .Split(",", StringSplitOptions.RemoveEmptyEntries)
                       //        .Select(o => o.RemovePostFix("/"))
                       //        .ToArray()
                       //)
                       .WithAbpExposedHeaders()
                       .SetIsOriginAllowedToAllowWildcardSubdomains()
                       .AllowAnyHeader()
                       .AllowAnyMethod();
                           //.AllowCredentials();
                   });
            });
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAbpRequestLocalization();

            if (!env.IsDevelopment())
            {
                app.UseErrorPage();
            }

            app.UseCorrelationId();
            //DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
            //defaultFilesOptions.DefaultFileNames.Clear();
            //defaultFilesOptions.DefaultFileNames.Add("index.html");
            //app.UseDefaultFiles(defaultFilesOptions);
            //app.UseHostFiltering();

            app.UseStaticFiles();
            #region 通过url访问uploads文件夹资源
            string path = AppContext.BaseDirectory;
            path = Path.Combine(path, "uploads");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            app.UseStaticFiles(new StaticFileOptions()//自定义自己的文件路径
            {
                RequestPath = new PathString("/uploads"),//对外的访问路径
                FileProvider = new PhysicalFileProvider(path)//指定实际物理路径
            });
            #endregion
            app.UseRouting();
            app.UseCors();
            app.UseAuthentication();
            app.UseJwtTokenMiddleware();

            app.UseUnitOfWork();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseAbpSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Design API");

                var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();
                c.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
                c.OAuthClientSecret(configuration["AuthServer:SwaggerClientSecret"]);
                c.OAuthScopes("Design");
                //模型的默认扩展深度，设置为 -1 完全隐藏模型
                c.DefaultModelsExpandDepth(-1);
                //API文档仅展开标记
                c.DocExpansion(DocExpansion.None);
                ////API前缀设置为空
                //options.RoutePrefix = string.Empty;
                //API页面Title
                c.DocumentTitle = "Design API";
            });

            app.UseAuditing();
            app.UseAbpSerilogEnrichers();
            app.UseConfiguredEndpoints();
        }
    }
}

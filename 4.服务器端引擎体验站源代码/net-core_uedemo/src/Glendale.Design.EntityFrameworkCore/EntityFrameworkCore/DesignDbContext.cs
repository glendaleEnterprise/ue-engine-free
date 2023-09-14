using Glendale.Design.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShardingCore.Core.VirtualRoutes.TableRoutes.RouteTails.Abstractions;
using ShardingCore.Sharding.Abstractions;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.IdentityServer.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.Users.EntityFrameworkCore;

namespace Glendale.Design.EntityFrameworkCore
{
    [ReplaceDbContext(typeof(IIdentityDbContext))]
    [ConnectionStringName("Default")]
    public class DesignDbContext : AbpDbContext<DesignDbContext>,IIdentityDbContext
    {
        /* Add DbSet properties for your Aggregate Roots / Entities here. */

        #region Entities from the modules

        /* Notice: We only implemented IIdentityDbContext 
         * and replaced them for this DbContext. This allows you to perform JOIN
         * queries for the entities of these modules over the repositories easily. You
         * typically don't need that for other modules. But, if you need, you can
         * implement the DbContext interface of the needed module and use ReplaceDbContext
         * attribute just like IIdentityDbContext.
         *
         * More info: Replacing a DbContext of a module ensures that the related module
         * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
         */

        //Identity
        public DbSet<IdentityUser> Users { get; set; }        
        public DbSet<IdentityRole> Roles { get; set; }
        public DbSet<IdentityClaimType> ClaimTypes { get; set; }
        public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
        public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
        public DbSet<IdentityLinkUser> LinkUsers { get; set; }
        public DbSet<RoleOrgJoin> RoleOrgJoins { get; set; }
        #endregion

        #region 项目
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectUser> ProjectUsers { get; set; }
        public DbSet<ProjectImage> ProjectImages { get; set; }
        public DbSet<ProjectFolder> ProjectFolders { get; set; }
        public DbSet<ProjectFolderUser> ProjectFolderUsers { get; set; }       
        #endregion

        #region 文档
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentConfig> DocumentConfigs { get; set; }
        public DbSet<DocumentVersion> DocumentVersion { get; set; }
        public DbSet<DocumentLog> DocumentLogs { get; set; }
        public DbSet<ProjectFolderUser> DocumentUsers { get; set; }       
        public DbSet<DocumentVerThan> DocumentVerThan { get; set; }
        #endregion

        #region 漫游
        public DbSet<Roaming> Roamings { get; set; }
        public DbSet<RoamingPoint> RoamingPoints { get; set; }
        #endregion

        #region Models   
        public DbSet<Label> Label { get; set; }
        public DbSet<ViewPoint> ViewPoints { get; set; }
        public DbSet<ActionLog> ActionLogs { get; set; }
        public DbSet<Postil> Postil { get; set; } 
        #endregion

        #region 合模
        public DbSet<Combine> Combines { get; set; }
        public DbSet<CombineDetail> CombineDetails { get; set; }
        public DbSet<CombineFlatten> CombineFlatten { get; set; }
        public DbSet<CombineLog> CombineLog { get; set; }

        #endregion

        #region  分享模型
        public DbSet<ShareRecord> ShareRecords { get; set; }        
        #endregion

        public DbSet<File> Files { get; }
        public DbSet<Dictionary> Dictionarys { get; set; }


        public DbSet<Message> Message{get; set;}

        public DbSet<MessageReceive> MessageReceive { get; set; }

        #region sqltable
        public DbSet<SqlModelTreeGLID> SqlModelTreeGLIDs { get; set; }
        #endregion sqltable

        public DesignDbContext(DbContextOptions<DesignDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            /* Include modules to your migration db context */

            builder.ConfigurePermissionManagement(o => o.TablePrefix = "Sys_");
            builder.ConfigureSettingManagement(o => o.TablePrefix = "Sys_");
            builder.ConfigureBackgroundJobs(o => o.TablePrefix = "Sys_");
            builder.ConfigureAuditLogging(o => o.TablePrefix = "Sys_");
            builder.ConfigureIdentity(o => o.TablePrefix = "Sys_");
            builder.ConfigureFeatureManagement(o => o.TablePrefix = "Sys_");
            builder.ConfigureIdentityServer(o => o.TablePrefix = "Ids_");

            /* Configure your own tables/entities inside here */
            #region 系统            

            builder.Entity<File>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.File + DesignConsts.DbSchema);
                b.Property(q => q.FileName).IsRequired().HasMaxLength(FileConsts.MaxFileNameLength);
                b.Property(q => q.BlobName).IsRequired().HasColumnType("char(32)").HasMaxLength(FileConsts.MaxBlobNameLength);
                b.Property(q => q.ByteSize).IsRequired();
                b.ConfigureFullAudited();
            });

            builder.Entity<Dictionary>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.Dictionary + DesignConsts.DbSchema);
                b.HasIndex(x => new { x.Id, x.ParentId });
                b.HasOne(x => x.Parent).WithMany(x => x.Children).HasForeignKey(x => x.ParentId).HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Cascade);
                b.ConfigureFullAudited();
            });

            builder.Entity<ActionLog>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.ActionLog + DesignConsts.DbSchema);
                //b.HasOne(o => o.User).WithOne().HasForeignKey<Logs>(o => o.CreatorId);
            });

            builder.Entity<Message>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.Message + DesignConsts.DbSchema);
                b.HasIndex(o => new { o.Id });
                b.ConfigureFullAudited();
            });

            builder.Entity<RoleOrgJoin>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.RoleOrgJoin + DesignConsts.DbSchema);
                b.HasIndex(o => new { o.Id });
                b.HasOne(o => o.RoleInfo).WithMany().HasForeignKey(o => o.RoleId).OnDelete(DeleteBehavior.Cascade);
                b.HasOne(o => o.OrgInfo).WithMany().HasForeignKey(o => o.OrgId).OnDelete(DeleteBehavior.Cascade);
                b.ConfigureFullAudited();
            });

            builder.Entity<MessageReceive>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.MessageReceive + DesignConsts.DbSchema);
                b.HasIndex(o => new { o.Id });
                b.HasOne(o => o.Message).WithMany(o => o.MessageReceives).HasForeignKey(o => o.MessageId).OnDelete(DeleteBehavior.Cascade);
                b.ConfigureCreationAudited();
            });
            #endregion

            #region 项目

            //项目
            builder.Entity<Project>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.Project + DesignConsts.DbSchema);
                b.HasIndex(o => new { o.Id });               
                b.HasOne(o => o.OrgInfo).WithMany().HasForeignKey(o => o.OrgId).OnDelete(DeleteBehavior.Cascade);
                b.HasOne(o => o.ManageInfo).WithMany().HasForeignKey(o => o.ManageId).OnDelete(DeleteBehavior.Cascade);
                b.ConfigureFullAudited();
            });
            //项目图片
            builder.Entity<ProjectImage>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.ProjectImage + DesignConsts.DbSchema);
                b.HasIndex(o => new { o.Id });                 
                b.HasOne(o => o.Project).WithMany(o => o.ProjectImages).HasForeignKey(o => o.ProjectId).OnDelete(DeleteBehavior.Cascade);
                b.ConfigureCreationAudited();
            });

            //项目参与人员
            builder.Entity<ProjectUser>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.ProjectUser + DesignConsts.DbSchema);
                b.HasIndex(o => new { o.Id });
                b.HasOne(o => o.User).WithMany().HasForeignKey(o => o.UserId);
                b.HasOne(o => o.Project).WithMany(o => o.ProjectUsers).HasForeignKey(o => o.ProjectId).OnDelete(DeleteBehavior.Cascade);
                b.ConfigureCreationAudited();
            });

            //项目目录
            builder.Entity<ProjectFolder>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.ProjectFolder + DesignConsts.DbSchema);
                b.HasIndex(o => new { o.Id });
                b.HasOne(o => o.Parent).WithMany(o => o.Children).HasForeignKey(o => o.ParentId).HasPrincipalKey(o => o.Id).OnDelete(DeleteBehavior.Cascade);
                b.HasOne(o => o.Project).WithMany(o => o.ProjectFolders).HasForeignKey(o => o.ProjectId).OnDelete(DeleteBehavior.Cascade);
                b.ConfigureCreationAudited();
            });

            //项目目录用户
            builder.Entity<ProjectFolderUser>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.ProjectFolderUser + DesignConsts.DbSchema);
                b.HasIndex(o => new { o.Id });
                b.HasOne(o => o.User).WithMany().HasForeignKey(o => o.UserId);
                b.HasOne(o => o.ProjectFolder).WithMany(o => o.ProjectFolderUsers).HasForeignKey(o => o.ProjectFolderId).OnDelete(DeleteBehavior.Cascade);                
                b.ConfigureCreationAudited();
            });
            #endregion

            #region 文档
            //文档
            builder.Entity<Document>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.Document + DesignConsts.DbSchema);
                b.HasIndex(o => new { o.Id });  
                b.HasOne(o=>o.Project).WithMany().HasForeignKey(o => o.ProjectId).OnDelete(DeleteBehavior.Cascade);
                b.HasOne(o=>o.ProjectFolder).WithMany().HasForeignKey(o => o.ProjectFolderId).OnDelete(DeleteBehavior.Cascade);
                b.HasMany(o => o.DocumentVersion).WithOne(o => o.Document).HasForeignKey(o => o.DocumentId).OnDelete(DeleteBehavior.Cascade);
                b.HasMany(o => o.DocumentLog).WithOne(o => o.Document).HasForeignKey(o => o.DocumentId).OnDelete(DeleteBehavior.Cascade);
                b.HasMany(o => o.CombineDetail).WithOne(o => o.Document).HasForeignKey(o => o.DocumentId).OnDelete(DeleteBehavior.Cascade);
                b.ConfigureFullAudited();
            });
            //模型轻量化配置参数
            builder.Entity<DocumentConfig>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.DocumentConfig + DesignConsts.DbSchema);
                b.HasOne(o=>o.DocumentVersion).WithMany().HasForeignKey(o => o.DocumentVerId).OnDelete(DeleteBehavior.Cascade);
                b.HasIndex(o => new { o.Id });
                b.ConfigureFullAudited();
            });
            //文档版本
            builder.Entity<DocumentVersion>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.DocumentVersion + DesignConsts.DbSchema);
                b.HasIndex(o => new { o.Id });
                b.ConfigureFullAudited();
            });
            //文档日志
            builder.Entity<DocumentLog>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.DocumentLog + DesignConsts.DbSchema);
                b.HasIndex(o => new { o.Id });
                b.ConfigureCreationAudited();
            });
            //版本比对
            builder.Entity<DocumentVerThan>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.DocumentVerThan + DesignConsts.DbSchema);
                b.HasOne(o => o.SourceDocVer).WithMany().HasForeignKey(o => o.SourceDocVerId).OnDelete(DeleteBehavior.NoAction);
                b.HasOne(o => o.DestinationDocVer).WithMany().HasForeignKey(o => o.DestinationDocVerId).OnDelete(DeleteBehavior.NoAction);
                b.HasIndex(o => new { o.Id });
                b.ConfigureCreationAudited();
            });
            #endregion

            #region 模型操作
            //漫游
            builder.Entity<Roaming>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.Roaming, DesignConsts.DbSchema);
                b.HasIndex(x => new { x.Id });
                b.HasMany(o => o.RoamingPoints).WithOne(o => o.Roaming).HasForeignKey(o => o.RoamingId).OnDelete(DeleteBehavior.Cascade);
                b.ConfigureFullAudited();
            });
            //漫游记录
            builder.Entity<RoamingPoint>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.RoamingPoint, DesignConsts.DbSchema);
                b.HasIndex(x => new { x.Id });
            });            
            //视点
            builder.Entity<ViewPoint>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.ViewPoint, DesignConsts.DbSchema);
                b.ConfigureAudited();
            });  

            //模型标签
            builder.Entity<Label>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.Label + DesignConsts.DbSchema);
                b.HasIndex(o => new { o.Id });
            });

            //批注
            builder.Entity<Postil>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.Postil + DesignConsts.DbSchema);
                b.HasIndex(o => new { o.Id });
                b.ConfigureCreationAudited();
            });
            #endregion

            #region 合模
            //合模
            builder.Entity<Combine>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.Combine + DesignConsts.DbSchema);
                b.HasIndex(o => new { o.Id });
                b.HasMany(o => o.CombineDetails).WithOne(o => o.Combine).HasForeignKey(o => o.CombineId).OnDelete(DeleteBehavior.Cascade);
                b.HasMany(o => o.CombineFlattens).WithOne(o => o.Combine).HasForeignKey(o => o.CombineId).OnDelete(DeleteBehavior.Cascade);
                b.HasMany(o => o.CombineLogs).WithOne(o => o.Combine).HasForeignKey(o => o.CombineId).OnDelete(DeleteBehavior.Cascade);
                b.ConfigureFullAudited();
            });
            //合模明细
            builder.Entity<CombineDetail>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.CombineDetail + DesignConsts.DbSchema);
                b.HasIndex(o => new { o.Id });
                b.ConfigureFullAudited();
            });
            //合模压平
            builder.Entity<CombineFlatten>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.CombineFlatten + DesignConsts.DbSchema);
                b.HasIndex(o => new { o.Id });
                b.ConfigureCreationAudited();
            });
            //合模日志
            builder.Entity<CombineLog>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.CombineLog + DesignConsts.DbSchema);
                b.HasIndex(o => new { o.Id });
                b.ConfigureCreationAudited();
            });
            #endregion

            #region 分享
            //分享功能
            builder.Entity<ShareRecord>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.ShareRecord + DesignConsts.DbSchema);
                b.HasIndex(o => new { o.Id });
                
                b.ConfigureFullAudited();
            });
            #endregion

            #region sqltable
            //批注
            builder.Entity<SqlModelTreeGLID>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.SqlModelTreeGLID + DesignConsts.DbSchema);
            });
            #endregion sqltable

        }
    }
}

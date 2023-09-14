using Glendale.Design.Enums;
using Glendale.Design.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

namespace Glendale.Design.Data
{
    /// <summary>
    /// 系统默认种子数据
    /// </summary>
    public class DefaultDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IGuidGenerator GuidGenerator;
        private readonly IUnitOfWork UnitOfWork;

        private readonly IRepository<Document, Guid> DocumentRepository;
        private readonly IRepository<DocumentVersion, Guid> DocumentVersionRepository;
        private readonly IRepository<File, Guid> FileRepository;
        private readonly IRepository<Project, Guid> ProjectRepository;
        private readonly IRepository<ProjectFolder, Guid> ProjectFolderRepository;
        private readonly OrganizationUnitManager OrganizationUnitManager;
        private readonly IRepository<IdentityRole, Guid> RoleRepository;
        private readonly IPermissionGrantRepository PermissionAppService;
        private readonly IIdentityUserRepository UserRepository;
        private readonly IdentityUserManager UserManager;

        public DefaultDataSeedContributor(IGuidGenerator guidGenerator,
            IUnitOfWork _UnitOfWork,
            OrganizationUnitManager organizationUnitManager,
            IRepository<IdentityRole, Guid> _RoleRepository,
            IPermissionGrantRepository _PermissionAppService,
            IIdentityUserRepository _UserRepository,
            IdentityUserManager _UserManager,
            IRepository<Document, Guid> _DocumentRepository,
            IRepository<DocumentVersion, Guid> _DocumentVersionRepository,
            IRepository<File, Guid> _FileRepository,
            IRepository<Project, Guid> _ProjectRepository,
            IRepository<ProjectFolder, Guid> projectFolderRepository)
        {
            GuidGenerator = guidGenerator;
            UnitOfWork = _UnitOfWork;
            OrganizationUnitManager = organizationUnitManager;
            RoleRepository = _RoleRepository;
            PermissionAppService = _PermissionAppService;
            UserRepository = _UserRepository;
            ProjectRepository = _ProjectRepository;
            ProjectFolderRepository = projectFolderRepository;
            DocumentRepository = _DocumentRepository;
            DocumentVersionRepository = _DocumentVersionRepository;
            FileRepository = _FileRepository;
            UserManager = _UserManager;
            UserManager.Options.Password.RequireLowercase = false;
            UserManager.Options.Password.RequireDigit = false;
            UserManager.Options.Password.RequireUppercase = false;
            UserManager.Options.Password.RequireNonAlphanumeric = false;
            UserManager.Options.Password.RequiredUniqueChars = 0;
            UserManager.Options.Password.RequiredLength = 6;
        }
        [UnitOfWork]
        public virtual async Task SeedAsync(DataSeedContext context)
        {
            await CreateOrganizationUnitAsync();
            await CreateRoleAsync();
            await CreateDataAsync();
        }
        /// <summary>
        /// 组织机构
        /// </summary>
        /// <param name="TenantId"></param>
        /// <returns></returns>
        private async Task CreateOrganizationUnitAsync()
        {
            var org = new OrganizationUnit(CommonOrgId, displayName: "外部单位");
            await OrganizationUnitManager.CreateAsync(org);//添加并没有立即执行
            await UnitOfWork.SaveChangesAsync();
        }

        private static Guid CommonRoleId = Guid.NewGuid();
        private static Guid GuestRoleId = Guid.NewGuid();
        private static Guid CommonOrgId = Guid.NewGuid();
        /// <summary>
        /// 角色
        /// </summary> 
        /// <returns></returns>
        private async Task CreateRoleAsync()
        {
            var roleList = new List<IdentityRole>();
            #region system
            var entity = new IdentityRole(CommonRoleId, name: "system");
            roleList.Add(entity);
            var permission = new List<PermissionGrant>();

            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "AbpIdentity.Roles", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "AbpIdentity.Roles.ManagePermissions", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "AbpIdentity.Users", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "AbpIdentity.Users.ManagePermissions", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "FeatureManagement.ManageHostFeatures", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "SettingManagement.Emailing", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Combine.SetGIS", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Postil.Export", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Base.Projects", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Base.Projects.Search", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Base.Projects.Add", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Base.Projects.Update", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Base.Projects.Delete", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Base.Postil", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Base.Postil.Close", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Base.Postil.Handle", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Base.Postil.Delete", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Documents", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Documents.Search", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Documents.Upload", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Documents.UploadNewVersion", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Documents.Move", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Documents.Delete", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Documents.Share", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Documents.Version", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Documents.VersionThan", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Documents.VersionDelete", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Documents.Download", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Viewpoint.Add", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Label.Add", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Folder", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Folder.Search", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Folder.Add", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Folder.Update", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Folder.Delete", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.FolderUser.Add", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.FolderUser.Delete", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Combine", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Combine.Search", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Combine.Add", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Combine.Share", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Combine.Move", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Combine.Delete", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Share", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Share.Search", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Share.Add", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Share.Delete", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Share.Info", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Postil", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Postil.Search", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Postil.Add", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Postil.Info", providerKey: "system", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Home", providerKey: "system", providerName: "R"));
            #endregion system

            #region manager
            var manager = new IdentityRole(GuidGenerator.Create(), name: "manager");
            manager.IsDefault = true;
            manager.IsPublic = true;
            roleList.Add(manager);

            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Base.Projects", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Base.Projects.Search", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Base.Projects.Add", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Base.Projects.Update", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Base.Projects.Delete", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Home", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Folder", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Folder.Search", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Folder.Add", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Folder.Update", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Folder.Delete", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.FolderUser.Add", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.FolderUser.Delete", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Documents", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Documents.Search", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Documents.Upload", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Documents.UploadNewVersion", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Documents.Move", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Documents.Delete", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Documents.Share", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Documents.Version", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Documents.VersionThan", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Documents.VersionDelete", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Documents.Download", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Viewpoint.Add", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Label.Add", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Combine", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Combine.Search", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Combine.Add", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Combine.Share", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Combine.Move", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Combine.Delete", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Share", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Share.Search", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Share.Add", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Share.Delete", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Share.Info", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Postil", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Postil.Search", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Postil.Add", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "Design.Business.Postil.Info", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "AbpIdentity.Roles", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "AbpIdentity.Roles.Create", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "AbpIdentity.Roles.Update", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "AbpIdentity.Roles.Delete", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "AbpIdentity.Roles.ManagePermissions", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "AbpIdentity.Users", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "AbpIdentity.Users.Create", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "AbpIdentity.Users.Update", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "AbpIdentity.Users.Delete", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "AbpIdentity.Users.ManagePermissions", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "FeatureManagement.ManageHostFeatures", providerKey: "manager", providerName: "R"));
            permission.Add(new PermissionGrant(GuidGenerator.Create(), name: "SettingManagement.Emailing", providerKey: "manager", providerName: "R"));
            #endregion manager

            await RoleRepository.InsertManyAsync(roleList, true);
            await PermissionAppService.InsertManyAsync(permission);
        }

        /// <summary>
        /// 用户数据
        /// </summary>
        /// <returns></returns>
        private async Task CreateDataAsync()
        {
            var admin = await UserManager.FindByNameAsync("admin");
            await UserManager.RemovePasswordAsync(admin);
            await UserManager.SetRolesAsync(admin, new string[] { "system" });
            await UserManager.AddPasswordAsync(admin, "123456");

            var user = new IdentityUser(Guid.NewGuid(), "user", "user@123.com");
            user.Name = "user";
            user = await UserRepository.InsertAsync(user, true);
            await UserManager.AddPasswordAsync(user, "123456");
            await UserManager.AddToRoleAsync(user, "system");

            var guest = new IdentityUser(Guid.NewGuid(), "guest", "guest@123.com");
            guest.Name = "guest";
            guest = await UserRepository.InsertAsync(guest, true);
            await UserManager.AddPasswordAsync(guest, "123456");
            await UserManager.AddToRoleAsync(guest, "system");


            var projectMod = new Project(Guid.Parse("3a0abfce-3b33-6958-4cb0-d6b14b8e3250"))
            {
                ProjectName = "默认公开项目",
                ShortName = "默认公开项目",
                ManageId = user.Id,
                IsPublic = true,
            };
            await ProjectRepository.InsertAsync(projectMod, true);
            var profolderMod = new ProjectFolder(GuidGenerator.Create())
            {
                ProjectId = projectMod.Id,
                FolderName = "默认目录"
            };
            await ProjectFolderRepository.InsertAsync(profolderMod, true);

            //var filelist = new List<File>()
            //{
            //    new File(GuidGenerator.Create(), "1693366072373.png", "3a56560148ac43a1a50e5ea5e248dd9d", 1837431),
            //    new File(GuidGenerator.Create(), "1693366115148.png", "293654292a1d4afaa888b6ec9452177b", 2118055),
            //    new File(GuidGenerator.Create(), "1693366326648.png", "a4e9b8f19e8947379ba618a4e06ab877", 2187264),
            //    new File(GuidGenerator.Create(), "1693366338937.png", "17474f92b1894f3390105f8204ef89fd", 1828293),
            //    new File(GuidGenerator.Create(), "1693366879586.png", "81a352ea06244e369594973aabe1fbd3", 2757017),
            //};
            //await FileRepository.InsertManyAsync(filelist);

            //var doclist = new List<Document>()
            //{
            //    new Document(GuidGenerator.Create()){ ProjectId = projectMod.Id, ProjectFolderId = profolderMod.Id, DocumentName="倾斜摄影", ModelName="202308261422300344", ModelFormat="osgb", Size=435071191, DocumentType=DocTypeEnum.GIS, Status=DocStatusEnum.Running, ModelConfig="{\"visibleDistance\":0.5,\"isLod\":0}", BlobName="3a56560148ac43a1a50e5ea5e248dd9d"},
            //    new Document(GuidGenerator.Create()){ ProjectId = projectMod.Id, ProjectFolderId = profolderMod.Id, DocumentName="桥梁模型", ModelName="202308261435066313", ModelFormat="glzip", Size=332238617, DocumentType=DocTypeEnum.Model, Status=DocStatusEnum.Running, ModelConfig="{\"visibleDistance\":2000,\"isLod\":0}", BlobName="293654292a1d4afaa888b6ec9452177b"},
            //    new Document(GuidGenerator.Create()){ ProjectId = projectMod.Id, ProjectFolderId = profolderMod.Id, DocumentName="房建模型", ModelName="202308261442552747", ModelFormat="revit", Size=265573168, DocumentType=DocTypeEnum.Model, Status=DocStatusEnum.Running, ModelConfig="{\"visibleDistance\":2000,\"isLod\":0}", SceneConfig="{\"scene\":\"0\",\"topography\":\"0\",\"sun\":\"1\",\"automaticRotation\":0,\"skyBox\":\"1\",\"lightIntensity\":1,\"ambientLight\":1,\"navigationCube\":\"0\",\"movementSpeed\":1,\"rotationSpeed\":2,\"zoomSpeed\":2,\"sharpening\":3,\"renderQuality\":100,\"coplanarScintillation\":\"1\",\"adjustment\":\"1\",\"contrastRatio\":0.1,\"saturationLevel\":0,\"exposure\":0,\"luminance\":0.1,\"colorCurve\":0.1,\"cloudAltitudeRatio\":0.6,\"fpsSetting\":0,\"ensureFps\":10}", BlobName="a4e9b8f19e8947379ba618a4e06ab877"},
            //    new Document(GuidGenerator.Create()){ ProjectId = projectMod.Id, ProjectFolderId = profolderMod.Id, DocumentName="化工厂一角", ModelName="202308261451052096", ModelFormat="navisworks", Size=37097748, DocumentType=DocTypeEnum.Model, Status=DocStatusEnum.Running, ModelConfig="{\"visibleDistance\":100,\"isLod\":1}", BlobName="17474f92b1894f3390105f8204ef89fd"},
            //    new Document(GuidGenerator.Create()){ ProjectId = projectMod.Id, ProjectFolderId = profolderMod.Id, DocumentName="地铁站", ModelName="GL_UEngine_zz-Windows", ModelFormat="UE工程模型", Size=418611020, DocumentType=DocTypeEnum.UE, Status=DocStatusEnum.Succeed, ModelConfig="{\"visibleDistance\":2000,\"isLod\":0}", BlobName="81a352ea06244e369594973aabe1fbd3", Camera="{\"world\":{\"location\":{\"x\":-714.1001586914062,\"y\":-1490.3529052734375,\"z\":115.55182647705078},\"rotation\":{\"x\":5.986898222015951e-16,\"y\":-5.062864303588867,\"z\":-24.99473762512207},\"origin\":{\"x\":108.87999725341797,\"y\":34.20000076293945,\"z\":0}}}", Remark="", Matrix="{\"rotate\":[0,0,0],\"offset\":[0,0,0]}"},
            //};
            //await DocumentRepository.InsertManyAsync(doclist, true);
            //var verlist = doclist.Select(s => new DocumentVersion()
            //{
            //    DocumentId = s.Id,
            //    ModelName = s.ModelName,
            //    ModelFormat = s.ModelFormat,
            //    Size = s.Size,
            //    DocumentType = s.DocumentType,
            //    Status = s.Status,
            //    IsCurrent = true,
            //}).ToList();
            //await DocumentVersionRepository.InsertManyAsync(verlist, true);

        }
    }
}
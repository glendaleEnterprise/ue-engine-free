using Glendale.Design.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Glendale.Design.Permissions
{
    public class DesignPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            //基础功能组
            var baseGroup = context.AddGroup("Design.Base", L("Permission:Design.Base"));

            //工作台
            var workplacePer = baseGroup.AddPermission("Design.Base.Postil", L("Permission:Design.Base.Postil"));
            workplacePer.AddChild("Design.Base.Postil.Close", L("Permission:Design.Base.Postil.Close"));
            workplacePer.AddChild("Design.Base.Postil.Handle", L("Permission:Design.Base.Postil.Handle"));
            workplacePer.AddChild("Design.Base.Postil.Delete", L("Permission:Design.Base.Postil.Delete"));

            //项目列表            
            var projectsPer = baseGroup.AddPermission("Design.Base.Projects", L("Permission:Design.Base.Projects"));
            projectsPer.AddChild("Design.Base.Projects.Search", L("Permission:Design.Base.Projects.Search"));
            projectsPer.AddChild("Design.Base.Projects.Add", L("Permission:Design.Base.Projects.Add"));
            projectsPer.AddChild("Design.Base.Projects.Update", L("Permission:Design.Base.Projects.Update"));
            projectsPer.AddChild("Design.Base.Projects.Delete", L("Permission:Design.Base.Projects.Delete"));


            #region 项目管理    
            var businessGroup = context.AddGroup("Design.Business", L($"Permission:Design.Business"));
            //项目首页
            businessGroup.AddPermission("Design.Business.Home", L($"Permission:Design.Business.Home"));
            //项目目录
            var businessFolder = businessGroup.AddPermission("Design.Business.Folder", L($"Permission:Design.Business.Folder"));
            businessFolder.AddChild("Design.Business.Folder.Search", L($"Permission:Design.Business.Folder.Search"));
            businessFolder.AddChild("Design.Business.Folder.Add", L($"Permission:Design.Business.Folder.Add"));
            businessFolder.AddChild("Design.Business.Folder.Update", L($"Permission:Design.Business.Folder.Update"));
            businessFolder.AddChild("Design.Business.Folder.Delete", L($"Permission:Design.Business.Folder.Delete"));
            businessFolder.AddChild("Design.Business.FolderUser.Add", L($"Permission:Design.Business.FolderUser.Add"));
            businessFolder.AddChild("Design.Business.FolderUser.Delete", L($"Permission:Design.Business.FolderUser.Delete"));

            //模型列表
            var businessDocuments = businessGroup.AddPermission("Design.Business.Documents", L($"Permission:Design.Business.Documents"));
            businessDocuments.AddChild("Design.Business.Documents.Search", L($"Permission:Design.Business.Documents.Search"));
            businessDocuments.AddChild("Design.Business.Documents.Upload", L($"Permission:Design.Business.Documents.Upload"));
            businessDocuments.AddChild("Design.Business.Documents.UploadNewVersion", L($"Permission:Design.Business.Documents.UploadNewVersion"));
            businessDocuments.AddChild("Design.Business.Documents.Move", L($"Permission:Design.Business.Documents.Move"));
            businessDocuments.AddChild("Design.Business.Documents.Delete", L($"Permission:Design.Business.Documents.Delete"));
            //王经理：我想了，只做公开项目设置，不做公开模型设置了。
            //心远(2023-1-17 16:10:31):确认的话，我们这边就着手调整了
            //王经理(2023-1-17 16:11:42)：确定了
            //心远(2023-1-17 16:11:53):OK
            //businessDocuments.AddChild("Design.Business.Documents.Public", L($"Permission:Design.Business.Documents.Public"));
            businessDocuments.AddChild("Design.Business.Documents.Share", L($"Permission:Design.Business.Documents.Share"));
            businessDocuments.AddChild("Design.Business.Documents.Version", L($"Permission:Design.Business.Documents.Version"));
            businessDocuments.AddChild("Design.Business.Documents.VersionThan", L($"Permission:Design.Business.Documents.VersionThan"));
            businessDocuments.AddChild("Design.Business.Documents.VersionDelete", L($"Permission:Design.Business.Documents.VersionDelete"));
            businessDocuments.AddChild("Design.Business.Documents.Download", L($"Permission:Design.Business.Documents.Download"));
            businessDocuments.AddChild("Design.Business.Viewpoint.Add", L($"Permission:Design.Business.Viewpoint.Add"));
            businessDocuments.AddChild("Design.Business.Label.Add", L($"Permission:Design.Business.Label.Add"));

            //合模
            var businessCombine = businessGroup.AddPermission("Design.Business.Combine", L($"Permission:Design.Business.Combine"));
            businessCombine.AddChild("Design.Business.Combine.Search", L($"Permission:Design.Business.Combine.Search"));
            businessCombine.AddChild("Design.Business.Combine.Add", L($"Permission:Design.Business.Combine.Add"));
            //businessCombine.AddChild("Design.Business.Combine.SetGIS", L($"Permission:Design.Business.Combine.SetGIS"));
            businessCombine.AddChild("Design.Business.Combine.Share", L($"Permission:Design.Business.Combine.Share"));
            businessCombine.AddChild("Design.Business.Combine.Move", L($"Permission:Design.Business.Combine.Move"));
            businessCombine.AddChild("Design.Business.Combine.Delete", L($"Permission:Design.Business.Combine.Delete"));
            //businessCombine.AddChild("Design.Business.Combine.Public", L($"Permission:Design.Business.Combine.Public"));

            //分享
            var businessShare = businessGroup.AddPermission("Design.Business.Share", L($"Permission:Design.Business.Share"));
            businessShare.AddChild("Design.Business.Share.Search", L($"Permission:Design.Business.Share.Search"));
            businessShare.AddChild("Design.Business.Share.Add", L($"Permission:Design.Business.Share.Add"));
            businessShare.AddChild("Design.Business.Share.Delete", L($"Permission:Design.Business.Share.Delete"));
            businessShare.AddChild("Design.Business.Share.Info", L($"Permission:Design.Business.Share.Info"));

            //批注
            var businessPostil = businessGroup.AddPermission("Design.Business.Postil", L($"Permission:Design.Business.Postil"));
            businessPostil.AddChild("Design.Business.Postil.Search", L($"Permission:Design.Business.Postil.Search"));
            businessPostil.AddChild("Design.Business.Postil.Add", L($"Permission:Design.Business.Postil.Add"));
            businessPostil.AddChild("Design.Business.Postil.Info", L($"Permission:Design.Business.Postil.Info"));
            //businessPostil.AddChild("Design.Business.Postil.Export", L($"Permission:Design.Business.Postil.Export"));
            
            #endregion


            #region 系统设置     
            //var systemGroup = context.AddGroup("Design.System", L($"Permission:Design.System"));

            var systemGrouptree = baseGroup.AddPermission("Design.System", L($"Permission:Design.System"));

            var systemUser = systemGrouptree.AddChild("Design.System.Users", L($"Permission:Design.System.Users"));
            //systemUser.AddChild("Design.System.User.Add");
            //systemUser.AddChild("Design.System.User.Update");
            //systemUser.AddChild("Design.System.User.Delete");
            //systemUser.AddChild("Design.System.User.ResetPwd");
            //systemUser.AddChild("Design.System.User.Search");

            var systemOrganization = systemGrouptree.AddChild("Design.System.OrganizationUnits", L($"Permission:Design.System.OrganizationUnits"));
            //systemOrganization.AddChild("Design.System.OrganizationUnits.Add");
            //systemOrganization.AddChild("Design.System.OrganizationUnits.Update");
            //systemOrganization.AddChild("Design.System.OrganizationUnits.Delete");
            //systemOrganization.AddChild("Design.System.OrganizationUnits.Search");

            var systemRoles = systemGrouptree.AddChild("Design.System.Roles", L($"Permission:Design.System.Roles"));
            //systemRoles.AddChild("Design.System.Roles.Add");
            //systemRoles.AddChild("Design.System.Roles.Update");
            //systemRoles.AddChild("Design.System.Roles.Delete");
            //systemRoles.AddChild("Design.System.Roles.Search");
            //systemRoles.AddChild("Design.System.Roles.SetPermission");

            systemGrouptree.AddChild("Design.System.Dictionary", L($"Permission:Design.System.Dictionary"));
            systemGrouptree.AddChild("Design.System.LogData", L($"Permission:Design.System.LogData"));

            #endregion

        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<DesignResource>(name);
        }
    }
}

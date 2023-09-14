using AutoMapper;
using Glendale.Design.Dtos.Dictionary;
using Glendale.Design.Dtos.ModelProperty;
using Glendale.Design.Dtos.ModelPropertySpace;
using Glendale.Design.Dtos.ModelTree;
using Glendale.Design.Dtos.ModelType;
using Glendale.Design.Dtos.ModelSpace;
using Glendale.Design.Dtos.OrganizationUnit;
using Glendale.Design.Dtos.Project;
using Glendale.Design.Dtos.ProjectUser;
using Glendale.Design.Dtos.Roaming;
using Glendale.Design.Dtos.ViewPoint;
using Glendale.Design.Models;
using Glendale.Design.Core;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity;
using Glendale.Design.Dtos.Document;
using System;
using Glendale.Design.Dtos.DocumentVer;
using Glendale.Design.Dtos.DocumentLog;
using Glendale.Design.Dtos.Combine;
using Glendale.Design.Dtos.ShareRecord;
using Glendale.Design.Dtos;
using System.Collections.Generic;
using Glendale.Design.Enums;
using Glendale.Design.Dtos.Label;
using System.Linq;
using Glendale.Design.Dtos.Postil;
using Glendale.Design.Dtos.ModelDrawing;
using Glendale.Design.Dtos.DocumentVerThan;
using Glendale.Design.Dtos.ProjectImage;
using Glendale.Design.Dtos.ProjectFolder;
using Glendale.Design.Dtos.ProjectFolderUser;
using Glendale.Design.Dtos.Message;
using Glendale.Design.Dtos.RoleOrgJoin;
using Glendale.Design.Dtos.DocumentConfig;

namespace Glendale.Design
{
    public class DesignApplicationAutoMapperProfile : Profile
    {
        public DesignApplicationAutoMapperProfile()
        {
            /* You can configure your AutoMapper mapping configuration here.
             * Alternatively, you can split your mapping configurations
             * into multiple profile classes for a better organization. */
            
            #region Sys
            CreateMap<Dtos.User.IdentityUserRegisterCreateDto, IdentityUser>()
                .MapExtraProperties();
            CreateMap<IdentityUser, IdentityUserDto>()
                .MapExtraProperties();
            //组织机构
            CreateMap<OrganizationUnit, OrganizationUnitDto>()
                .MapExtraProperties();
            CreateMap<OrganizationUnit, OrganizationUnitListDto>()
                .ForMember(x => x.System, opt => opt.MapFrom(o => o.ExtraProperties.GetValueOrDefault("System")))
                .ForMember(x=>x.Describe,opt=>opt.MapFrom(o=>o.ExtraProperties.GetValueOrDefault("Describe")));
            CreateMap<OrganizationUnitCreateDto, OrganizationUnit>()
                .MapExtraProperties()
                .Ignore(x => x.Id)
                .IgnoreFullAuditedObjectProperties();
            CreateMap<OrganizationUnitUpdateDto, OrganizationUnit>()
                .MapExtraProperties()
                .Ignore(x => x.Id)
                .IgnoreFullAuditedObjectProperties();

            //基础数据
            CreateMap<Dictionary, DictionaryDto>();
            CreateMap<Dictionary, DictionaryListDto>();
            CreateMap<Dictionary, DictionaryViewDto>();
            CreateMap<DictionaryCreateUpdateDto, Dictionary>()
                .Ignore(x => x.Id)
                .Ignore(x => x.Parent)
                .Ignore(x => x.Children)
                .IgnoreFullAuditedObjectProperties(); 

            //系统日志
            CreateMap<ActionLog, LogsDto>();
            CreateMap<ActionLog, LogsListDto>();
            CreateMap<LogsCreateUpdateDto, ActionLog>();

            //角色和组织机构关系表
            CreateMap<RoleOrgJoin, RoleOrgJoinDto>();
            CreateMap<RoleOrgJoin, RoleOrgJoinListDto>();
            CreateMap<RoleOrgJoinCreateUpdateDto, RoleOrgJoin>()
                .Ignore(x => x.Id)
                .IgnoreFullAuditedObjectProperties()
                .Ignore(x => x.Id);
            #endregion

            #region 模型同步
            //模型楼层信息
            CreateMap<ModelTree, ModelTreeDto>();
            CreateMap<ModelTree, ModelTreeListDto>();
            CreateMap<ModelTreeCreateUpdateDto, ModelTree>()
                .Ignore(x => x.Id);
            //.Ignore(o => o.Parent)
            // .Ignore(o => o.Children)
            //模型构建信息
            CreateMap<ModelType, ModelTypeDto>();
            CreateMap<ModelType, ModelTypeListDto>();
            CreateMap<ModelTypeCreateUpdateDto, ModelType>()
                .Ignore(x => x.Id);
            //.Ignore(o => o.Parent)
            // .Ignore(o => o.Children)
            //模型空间信息
            CreateMap<ModelSpace, ModelSpaceDto>();
            CreateMap<ModelSpace, ModelSpaceListDto>();
            CreateMap<ModelSpaceCreateUpdateDto, ModelSpace>()
                .Ignore(x => x.Id);

            //模型构建属性信息
            CreateMap<ModelProperty, ModelPropertyDto>();
            CreateMap<ModelProperty, ModelPropertyListDto>();
            CreateMap<ModelPropertyCreateUpdateDto, ModelProperty>()
                .Ignore(x => x.Id);
            //模型空间属性信息
            CreateMap<ModelPropertySpace, ModelPropertySpaceDto>();
            CreateMap<ModelPropertySpace, ModelPropertySpaceListDto>();
            CreateMap<ModelPropertySpaceCreateUpdateDto, ModelPropertySpace>()
                .Ignore(x => x.Id);
            //模型图纸
            CreateMap<ModelDrawing, ModelDrawingDto>();
            CreateMap<ModelDrawing, ModelDrawingListDto>();
            CreateMap<ModelDrawingCreateUpdateDto, ModelDrawing>()
                .Ignore(o => o.Id);

            //模型图纸关联表
            CreateMap<ModelDrawingRvtId, ModelDrawingRvtIdDto>();
            #endregion

            #region 项目
            //项目
            CreateMap<Project, ProjectDto>()
                .ForMember(x => x.CreationAccount, opt => opt.MapFrom(x => x.Creator.UserName))
                .ForMember(x => x.CreationName, opt => opt.MapFrom(x => x.Creator.Name));                
            CreateMap<Project, ProjectListDto>()
                .ForMember(x => x.CreationAccount, opt => opt.MapFrom(x => x.Creator.UserName))
                .ForMember(x=>x.CreationName,opt=>opt.MapFrom(x=>x.Creator.Name));
            CreateMap<ProjectCreateDto, Project>()
                .Ignore(x => x.Id)
                .ForMember(x => x.Tags, opt => opt.MapFrom(x => string.Join(';',x.Tags)))
                .Ignore(x => x.ProjectImages)
                .Ignore(x => x.ProjectUsers)
                .IgnoreFullAuditedObjectProperties();
            CreateMap<ProjectUpdateDto, Project>()
                .Ignore(x => x.Id)
                .ForMember(x => x.Tags, opt => opt.MapFrom(x => string.Join(';', x.Tags)))
                .Ignore(x=>x.ProjectImages)
                .Ignore(x=>x.ProjectUsers)
                .IgnoreFullAuditedObjectProperties();


            //项目图片
            CreateMap<ProjectImage, ProjectImageDto>();
            CreateMap<ProjectImage, ProjectImageListDto>();
            CreateMap<ProjectImageCreateDto, ProjectImage>()
                .Ignore(x => x.Id)
                .IgnoreCreationAuditedObjectProperties();
            CreateMap<ProjectImageUpdateDto, ProjectImage>()
                .Ignore(x => x.Id)
                .IgnoreCreationAuditedObjectProperties();


            //项目用户
            CreateMap<ProjectUser, ProjectUserDto>();
            CreateMap<ProjectUser, ProjectUserListDto>();
            CreateMap<ProjectUserCreateDto, ProjectUser>()
                .Ignore(x => x.Id)
                .IgnoreCreationAuditedObjectProperties();
            CreateMap<ProjectUserUpdateDto, ProjectUser>()
                .Ignore(x => x.Id)
                .IgnoreCreationAuditedObjectProperties();


            //项目目录
            CreateMap<ProjectFolder, ProjectFolderDto>();
            CreateMap<ProjectFolder, ProjectFolderListDto>();
            CreateMap<ProjectFolderCreateDto, ProjectFolder>()
                .Ignore(x => x.Id)
                .Ignore(x => x.ProjectFolderUsers)
                .IgnoreAuditedObjectProperties();
            CreateMap<ProjectFolderUpdateDto,ProjectFolder>()
                .Ignore(x => x.Id)
                .Ignore(x=>x.ProjectFolderUsers)
                .IgnoreAuditedObjectProperties();


            //目录人员
            CreateMap<ProjectFolderUser, ProjectFolderUserDto>()
                .ForMember(x => x.Name, opt => opt.MapFrom(x => x.User.Name))
                .ForMember(x => x.PhoneNumber, opt => opt.MapFrom(x => x.User.PhoneNumber));
            CreateMap<ProjectFolderUser, ProjectFolderUserListDto>()
                .ForMember(x => x.Name, opt => opt.MapFrom(x => x.User.Name))
                .ForMember(x => x.PhoneNumber, opt => opt.MapFrom(x => x.User.PhoneNumber));
            CreateMap<ProjectFolderUserCreateDto, ProjectFolderUser>()
                .Ignore(x=>x.Id)
                .IgnoreCreationAuditedObjectProperties();


            #endregion

            #region 模型操作相关
            //标签
            CreateMap<Label, LabelDto>();
            CreateMap<Label, LabelListDto>();
            CreateMap<LabelCreateUpdateDto, Label>();

            //视点
            CreateMap<ViewPoint, ViewPointDto>()
                .ForMember(x=>x.SceneTypeName, opt=>opt.MapFrom(x=>x.SceneType.GetDescription()));
            CreateMap<ViewPoint, ViewPointListDto>()
                .ForMember(x => x.SceneTypeName, opt => opt.MapFrom(x => x.SceneType.GetDescription()));          
            CreateMap<ViewPointCreateUpdateDto, ViewPoint>()
                .Ignore(x => x.Id)               
                .IgnoreFullAuditedObjectProperties();
            //漫游
            CreateMap<Roaming, RoamingDto>();
            CreateMap<Roaming, RoamingListDto>();
            CreateMap<RoamingFristCreateDto, Roaming>()
                .Ignore(x => x.Id)
                .Ignore(x => x.RoamingPoints)
                .IgnoreFullAuditedObjectProperties();
            CreateMap<RoamingViewPortCreateDto, Roaming>()
               .Ignore(x => x.Id)
               .Ignore(x => x.RoamingPoints)
               .IgnoreFullAuditedObjectProperties();
            #endregion

            #region 文档相关
            //文档
            CreateMap<Document, DocumentDto>();
            CreateMap<Document, DocumentListDto>()
                .ForMember(x=>x.CreationName,opt=>opt.MapFrom(x=>x.Creator.Name))
                .ForMember(x=>x.StatusName,opt=>opt.MapFrom(x=>x.Status == DocStatusEnum.Failure ? x.Exception : x.Status.GetDescription()));
            CreateMap<DocumentCreateUpdateDto, Document>()
                .Ignore(x => x.Id)                 
                .Ignore(x => x.VersionNo)                
                .IgnoreFullAuditedObjectProperties();
            CreateMap<DocumentCreateUpdateDto, DocumentVersion>()
                .Ignore(x => x.Id);

            //文档
            CreateMap<DocumentConfig, DocumentConfigDto>();
            CreateMap<DocumentConfig, DocumentConfigListDto>();
            CreateMap<DocumentConfigCreateUpdateDto, DocumentConfig>()
                .Ignore(x => x.Id)
                .Ignore(x => x.VersionNo)
                .IgnoreFullAuditedObjectProperties();

            //文档版本信息
            CreateMap<DocumentVersion, DocumentVersionDto>();
            CreateMap<DocumentVersion, DocumentVersionListDto>();
            CreateMap<DocumentVersionCreateUpdateDto, DocumentVersion>()                
                .Ignore(x => x.Id)
                .IgnoreFullAuditedObjectProperties();
            CreateMap<DocumentVersionCreateUpdateDto, DocumentCreateUpdateDto>();

            //文档版本比对
            CreateMap<DocumentVerThan, DocumentVerThanDto>();
            CreateMap<DocumentVerThan, DocumentVerThanListDto>()
                  .ForMember(x => x.CreationName, opt => opt.MapFrom(x => x.Creator.Name));
            CreateMap<DocumentVerThanCreateUpdateDto, DocumentVerThan>()
                .Ignore(x => x.Id)
                .IgnoreCreationAuditedObjectProperties();

            //文档日志
            CreateMap<DocumentLog, DocumentLogDto>();
            CreateMap<DocumentLog, DocumentLogListDto>();
            CreateMap<DocumentLogCreateUpdateDto, DocumentLog>()
                .Ignore(x => x.Id)
                .Ignore(x => x.Document)
                .IgnoreCreationAuditedObjectProperties();          


            //分享记录
            CreateMap<ShareRecord, ShareRecordDto>();
           // CreateMap<ShareRecord, ShareRecordShowDto>();
            CreateMap<ShareRecord, ShareRecordListDto>();
            CreateMap<ShareRecordCreateUpdateDto, ShareRecord>()
                .Ignore(x => x.Id)
                .IgnoreCreationAuditedObjectProperties();                   

            #endregion

            #region 合模
            //合模
            CreateMap<Combine, CombineDto>();
            CreateMap<Combine, CombineListDto>()
                    .ForMember(x => x.CreationName, opt => opt.MapFrom(x => x.Creator.Name)); ;
            CreateMap<CombineCreateUpdateDto, Combine>()
                .Ignore(x => x.Id)
                .IgnoreCreationAuditedObjectProperties();

            CreateMap<CombineDetail, ShowCombineDetailDto>();
            CreateMap<CreateCombineDetailDto, CombineDetail>();
            CreateMap<ShowCombineDetailDto, CombineDetail>()
                .Ignore(x => x.Id)
                .IgnoreFullAuditedObjectProperties();

            CreateMap<CombineLog, CombineLogListDto>()
               .ForMember(x => x.CreationName, opt => opt.MapFrom(x => x.Creator.Name));
            #endregion

            #region 合模压平
            CreateMap<CombineFlatten, CombineFlattenListDto>();
            CreateMap<CombineFlattenListDto, CombineFlatten>();

            CreateMap<CreateCombineFlattenDto, CombineFlatten>();
            CreateMap<CombineFlatten, CreateCombineFlattenDto>();

            CreateMap<CombineFlattenDto, CombineFlatten>();
            CreateMap<CombineFlatten, CombineFlattenDto>();

            CreateMap<CombineFlatten, ShowCombineFlattenDto>();
            CreateMap<ShowCombineFlattenDto, CombineFlatten>()
                .Ignore(x => x.Id)
                .IgnoreCreationAuditedObjectProperties();
            #endregion 合模压平

            #region 批注
            //批注
            CreateMap<Postil, PostilDto>()
                .ForMember(x=>x.PostilCategoryName, opt=>opt.MapFrom(x=>x.PostilCategory.GetDescription()));
            CreateMap<Postil, PostilListDto>()
                .ForMember(x => x.CreationName, opt => opt.MapFrom(x => x.Creator.Name))
                .ForMember(x => x.StatusName, opt => opt.MapFrom(x => x.Status.GetDescription()));                
            CreateMap<PostilCreateUpdateDto, Postil>()
                .Ignore(x => x.Id)
                .IgnoreCreationAuditedObjectProperties();

            #endregion

            #region 消息
            CreateMap<Message, MessageDto>();
            CreateMap<Message, MessageListDto>();
            CreateMap<MessageCreateUpdateDto, Message>()
                .Ignore(x => x.Id)
                .IgnoreFullAuditedObjectProperties();

            CreateMap<MessageReceive, MessageReceiveDto>();
            #endregion

            #region 对象转树结构
            //项目转树结构
            //CreateMap<Project, TreeDto>()
            //    .ForMember(o => o.Key, opt => opt.MapFrom(o => o.Id))
            //    .ForMember(o => o.Title, opt => opt.MapFrom(o => o.ProjectName));
            //文档转树结构
            CreateMap<ProjectFolder, TreeDto>()
                .ForMember(o => o.Id, opt => opt.MapFrom(o => o.Id))
                .ForMember(o => o.Key, opt => opt.MapFrom(o => o.Id))
                .ForMember(o => o.Title, opt => opt.MapFrom(o => o.FolderName));

            #endregion            

        }
    }
}

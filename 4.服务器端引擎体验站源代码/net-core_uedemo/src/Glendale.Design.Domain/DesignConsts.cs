namespace Glendale.Design
{
    public static class DesignConsts
    {
        public const string SysDbTablePrefix = "Sys_";
        public const string BimDbTablePrefix = "Bim_";
        public const string SqlDbTablePrefix = "Sql_";

        public const string DbSchema = null;
        public static class DbTableName
        {
            #region 基础
            public const string User = SysDbTablePrefix + "Users";

            /// <summary>
            /// 文件管理
            /// </summary>
            public const string File = SysDbTablePrefix + "Files";
            /// <summary>
            /// 基础数据
            /// </summary>
            public const string Dictionary = SysDbTablePrefix + "Dictionary";

            /// <summary>
            /// 消息
            /// </summary>
            public const string Message = SysDbTablePrefix + "Message";

            /// <summary>
            /// 接收消息表
            /// </summary>
            public const string MessageReceive = SysDbTablePrefix + "MessageReceive";

            /// <summary>
            /// 操作日志
            /// </summary>
            public const string ActionLog = SysDbTablePrefix + "ActionLog";

            /// <summary>
            /// 角色和组织机构关系表
            /// </summary>
            public const string RoleOrgJoin = SysDbTablePrefix + "RoleOrgJoin";
            #endregion

            #region 项目
            /// <summary>
            /// 项目
            /// </summary>
            public const string Project = BimDbTablePrefix + "Project";
            /// <summary>
            /// 项目图片
            /// </summary>
            public const string ProjectImage= BimDbTablePrefix + "ProjectImage";
            /// <summary>
            /// 项目参与人员
            /// </summary>
            public const string ProjectUser = BimDbTablePrefix + "ProjectUser";
            /// <summary>
            /// 项目目录
            /// </summary>
            public const string ProjectFolder = BimDbTablePrefix + "ProjectFolder";

            /// <summary>
            /// 项目目录人员
            /// </summary>
            public const string ProjectFolderUser= BimDbTablePrefix + "ProjectFolderUser";
            #endregion           

            #region 模型相关数据同步表
            /// <summary>
            /// 模型楼层表
            /// </summary>
            public const string ModelTree = "Model_Tree";
            /// <summary>
            /// 模型结构表
            /// </summary>
            public const string ModelType = "Model_Type";
            /// <summary>
            /// 模型空间表
            /// </summary>
            public const string ModelSpace = "Model_Space";
            /// <summary>
            /// 模型构建属性
            /// </summary>
            public const string ModelProperty = "Model_Property";
            /// <summary>
            /// 模型空间属性
            /// </summary>
            public const string ModelPropertySpace = "Model_Property_Space";
            /// <summary>
            /// 模型图纸表
            /// </summary>
            public const string ModelDrawing = "Model_Drawing";

            public const string ModelDrawingRvtId = "Model_Drawing_rvtid";
            #endregion

            #region 模型相关操作数据表
            /// <summary>
            /// 漫游
            /// </summary>
            public const string Roaming = BimDbTablePrefix + "Roaming";
            /// <summary>
            /// 漫游视角
            /// </summary>
            public const string RoamingPoint = BimDbTablePrefix + "RoamingPoint";
             
            /// <summary>
            /// 视点
            /// </summary>
            public const string ViewPoint = BimDbTablePrefix + "ViewPoint";

            /// <summary>
            /// 模型标签
            /// </summary>
            public const string Label = BimDbTablePrefix + "Label";


            /// <summary>
            /// 合模
            /// </summary>
            public const string Combine = BimDbTablePrefix + "Combine";
            /// <summary>
            /// 合模明细
            /// </summary>
            public const string CombineDetail = BimDbTablePrefix + "CombineDetail";
            /// <summary>
            /// 合模压平
            /// </summary>
            public const string CombineFlatten = BimDbTablePrefix + "CombineFlatten";
            /// <summary>
            /// 合模日志
            /// </summary>
            public const string CombineLog = BimDbTablePrefix + "CombineLog";

            #endregion

            #region 批注
            public const string Postil=BimDbTablePrefix + "Postil";
             
            #endregion

            #region 文档
            /// <summary>
            /// 文档
            /// </summary>
            public const string Document = BimDbTablePrefix + "Document";
            /// <summary>
            /// 模型轻量化配置参数
            /// </summary>
            public const string DocumentConfig = BimDbTablePrefix + "DocumentConfig";
            /// <summary>
            /// 文档版本
            /// </summary>
            public const string DocumentVersion = BimDbTablePrefix + "DocumentVersion";
            /// <summary>
            /// 文档日志
            /// </summary>
            public const string DocumentLog = BimDbTablePrefix + "DocumentLog";             
            /// <summary>
            /// 模型比对
            /// </summary>
            public const string DocumentVerThan = BimDbTablePrefix + "DocumentVerThan";
            
            #endregion             

            #region 模块分享功能
            /// <summary>
            /// 分享记录表
            /// </summary>
            public const string ShareRecord = BimDbTablePrefix + "ShareRecord";
            /// <summary>
            /// 分享记录明细表
            /// </summary>
            public const string ShareRecordDetail = BimDbTablePrefix + "ShareRecordDetail";
            #endregion

            #region sqltable
            public const string SqlModelTreeGLID = SqlDbTablePrefix + "SqlModelTreeGLID";
            #endregion sqltable
        }
    }
}

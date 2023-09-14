using System;
using System.Collections.Generic;
using System.Text;
using Glendale.Design.Dtos.DocumentVer;
using Glendale.Design.Enums;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.DocumentConfig
{
    public class DocumentConfigDto : FullAuditedEntityDto<Guid>
    {
        #region 属性 
        /// <summary>
        /// 模型版本Id
        /// </summary>
        public virtual Guid DocumentVerId { get; set; }

        /// <summary>
        /// 模型Id
        /// </summary>
        public virtual Guid DocumentId { get; set; }


        /// <summary>
        /// 模型轻量化名称
        /// </summary>
        public virtual string ModelName { get; set; }
        /// <summary>
        /// 版本号(0.1)
        /// </summary>         
        public virtual double VersionNo { get; set; } = 0.1;
        /// <summary>
        /// 模型类型
        /// </summary>
        public virtual string ModelSuffix { get; set; }
        /// <summary>
        /// 应用场景
        /// </summary>
        public virtual string ModelScene { get; set; }
        /// <summary>
        /// 定位方式
        /// </summary>
        public virtual string PositionMethod { get; set; }

        /// <summary>
        /// 轻量化模式 0:着色模式 1:真实模式
        /// </summary>
        public int Style { get; set; }
        /// <summary>
        /// 轻量化精度 值越大转出模型越精细，范围1~10； 默认值为5； 仅适用于revit 和 bentley格式的模型
        /// </summary>
        public int Accuracy { get; set; }
        /// <summary>
        /// 圆柱类别参数化设置 0:不启用参数化 1:启用参数化
        /// </summary>
        public int Parametric { get; set; }
        /// <summary>
        /// 是否实例化轻量化 0:非实例化 1:实例化 默认值:1
        /// </summary>
        public int IsInstance { get; set; }
        /// <summary>
        /// 单位比例 米:1,分米:0.1,厘米:0.01,毫米:0.001 默认值:0.001
        /// </summary>
        public double UnitRatio { get; set; }
        /// <summary>
        /// 坐标系对应EPSG代号 例如：EPSG:32649 （参考）
        /// </summary>
        public string Srs { get; set; }
        /// <summary>
        /// 项目基点 格式为: [535450.001,2976084.866] （参考）
        /// </summary>
        public string SrsOrigin { get; set; }
        /// <summary>
        /// wgs84经度(弧度制)，用户输入角度，需要转换成弧度 默认值: 1.9003144895714261
        /// </summary>
        public double Longitude { get; set; }
        /// <summary>
        /// wgs84纬度(弧度制)，用户输入角度，需要转换成弧度 默认值: 0.5969026041820608
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// 海拔高度 单位：米
        /// </summary>
        public double TransHeight { get; set; }
        #endregion

        #region 导航属性

        /// <summary>
        /// 文档版本
        /// </summary>
        public virtual ICollection<DocumentVersionDto> DocumentVersion { get; set; }
        #endregion
    }
}

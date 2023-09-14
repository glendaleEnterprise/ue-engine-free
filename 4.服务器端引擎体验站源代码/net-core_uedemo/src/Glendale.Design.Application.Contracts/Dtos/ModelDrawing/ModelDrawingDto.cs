using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.ModelDrawing
{
    public class ModelDrawingDto : EntityDto<int>
    {
        /// <summary>
        /// 视图名称
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Dwg文件名称
        /// </summary>
        public virtual string FileName { get; set; }

        /// <summary>
        /// 视图类型
        /// </summary>
        public virtual string Type { get; set; }

        /// <summary>
        /// 关联ID
        /// </summary>
        public virtual string Guid { get; set; }

        /// <summary>
        /// 模型名称
        /// </summary>       
        public virtual string ModelName { get; set; }


        #region 导航属性
        /// <summary>
        ///  
        /// </summary>
        public virtual ICollection<ModelDrawingRvtIdDto> ModelDrawingRvtIds { get; set; }

        #endregion
    }
}

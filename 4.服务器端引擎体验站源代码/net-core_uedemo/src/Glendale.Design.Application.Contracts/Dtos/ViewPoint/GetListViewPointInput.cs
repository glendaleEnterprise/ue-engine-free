using Glendale.Design.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
namespace Glendale.Design.Dtos.ViewPoint
{
    /// <summary>
    /// 视点设置
    /// </summary>
    public class GetListViewPointInput : PagedAndSortedResultRequestDto
    {
         
        /// <summary>
        /// 名称
        /// </summary>
        [MaxLength(100)]
        public virtual string ViewName { get; set; }

        /// <summary>
        /// 模型Id
        /// </summary>
        [MaxLength(50)]
        public virtual string ModelId { get; set; }

        /// <summary>
        /// 项目Id
        /// </summary>
        public virtual Guid? ProjectId { get; set; }

        /// <summary>
        /// 场景类型
        /// </summary>
        public virtual SceneTypeEnum SceneType { get; set; }


        public GetListViewPointInput()
        {
            Sorting = "CreationTime";
        }

    }
}

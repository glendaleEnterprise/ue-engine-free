using Glendale.Design.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.Roaming
{
    /// <summary>
    /// 自定义漫游设置
    /// </summary>
    public class GetListRoamingInput : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// 场景类型
        /// </summary>
        public virtual SceneTypeEnum SceneType { get; set; }

        /// <summary>
        /// 漫游类型
        /// </summary>
        public virtual RoamingTypeEnum RoamingType { get; set; }
        /// <summary>
        /// 方案名称
        /// </summary>
        [MaxLength(100)]
        public virtual string Name { get; set; }
        /// <summary>
        /// 模型Id
        /// </summary>
        [MaxLength(50)]
        public virtual string ModelId { get; set; }

        /// <summary>
        /// 项目Id
        /// </summary>
        public virtual Guid? ProjectId { get; set; }



        public GetListRoamingInput()
        {
            Sorting = "creationTime";
        }
    }
}

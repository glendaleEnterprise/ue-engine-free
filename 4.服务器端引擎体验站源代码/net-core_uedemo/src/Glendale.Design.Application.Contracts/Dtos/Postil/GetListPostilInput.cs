using Glendale.Design.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.Postil
{
    public class GetListPostilInput : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// 所属项目
        /// </summary>
        public virtual Guid? ProjectId { get; set; }

        /// <summary>
        /// 模型文件Id/合模Id
        /// </summary>
        public virtual Guid? DocumentId { get; set; }

        /// <summary>
        /// 标题
        /// </summary>        
        public virtual string Title { get; set; }        

        /// <summary>
        /// 状态
        /// </summary>
        public virtual PostilStatusEnum? Status { get; set; }

        /// <summary>
        /// 问题类型
        /// </summary>
        public virtual PostilCategoryEnum? Category { get; set; }

        /// <summary>
        /// 发起人
        /// </summary>
        public virtual Guid? CreatorId { get; set; }


        /// <summary>
        /// 处理者Id
        /// </summary>
        public virtual Guid? HandlerUserId { get; set; }


        public GetListPostilInput()
        {
            Sorting = "CreationTime desc";
        }
    }
}

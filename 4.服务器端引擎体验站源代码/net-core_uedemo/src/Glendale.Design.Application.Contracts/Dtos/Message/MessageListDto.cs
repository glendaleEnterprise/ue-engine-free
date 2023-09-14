using Glendale.Design.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;

namespace Glendale.Design.Dtos.Message
{
    public class MessageListDto : FullAuditedEntityDto<Guid>
    {
        /// <summary>
        /// 项目Id
        /// </summary>
        public virtual Guid ProjectId { get; set; }

        /// <summary>
        /// 标题
        /// </summary>  
        public string Title { get; set; }

        /// <summary>
        /// 正文
        /// </summary>       
        public string BodyText { get; set; }

        /// <summary>
        /// 描述
        /// </summary>       
        public string Description { get; set; }  




        public bool IsRead { get; set; }

        public DateTime? ReadTime { get; set; }

        public int ReadCount { get; set; }

        /// <summary>
        /// 关联类型
        /// </summary>
        public MessJoinTypeEnum JoinType { get; set; }

        /// <summary>
        /// 关联标识
        /// </summary>
        public Guid? JoinId { get; set; }


        public virtual ICollection<MessageReceiveDto> MessageReceives { get; set; }


        public virtual ICollection<IdentityUserDto> IdentityUsers { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.Message
{
    public class MessageCreateUpdateDto 
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

        /// <summary>
        /// 消息接收人ids
        /// </summary>
        public Guid[] ReceiveUserIds { get; set; }
    }
}

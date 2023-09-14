using Glendale.Design.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Glendale.Design.Models
{
    /// <summary>
    /// 系统消息
    /// </summary>    
    public class Message : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 项目Id
        /// </summary>
        [Required]
        public virtual Guid ProjectId { get; set; }

        /// <summary>
        /// 标题
        /// </summary>       
        [MaxLength(200)]
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
        /// 关联类型
        /// </summary>
        public MessJoinTypeEnum JoinType { get; set; }

        /// <summary>
        /// 关联标识
        /// </summary>
        public Guid? JoinId { get; set; }


        [NotMapped]
        public virtual ICollection<MessageReceive> MessageReceives { get; set; }

        public Message()
        {

        }

        public Message(Guid id)
            : base(id)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace Glendale.Design.Models
{
    public class ModelTree : Entity<int>
    {
        #region 属性

        [MaxLength(50)]
        public virtual string Glid { get; set; }

        [MaxLength(50)]
        public virtual string PGlid { get; set; }
        public int? Level { get; set; }

        [MaxLength(100)]
        public virtual string Name { get; set; }
        [MaxLength(50)]
        public virtual string ExternalId { get; set; }

        [MaxLength(50)]
        public virtual string GroupName { get; set; }

        [MaxLength(36)]
        public virtual string ModelName { get; set; }
        #endregion
        #region 导航属性
        /// <summary>
        /// 父数据
        /// </summary>
        [NotMapped]
        public virtual ModelTree Parent { get; set; }
        /// <summary>
        /// 子数据
        /// </summary>
        [NotMapped]
        public virtual ICollection<ModelTree> Children { get; set; }
        public virtual string LightweightName => ModelName;
        #endregion
        protected ModelTree()
        {
        }
        public ModelTree(int id, string glid , string pGlid, int level, string externalId, string groupName, ICollection<ModelTree> children)
            : base(id)
        {
            Glid = glid;
            PGlid = pGlid;
            Level = level;
            ExternalId = externalId;
            GroupName = groupName;
        }
        public ModelTree(int id, string glid, string pGlid, int level, string externalId, string groupName, ModelTree parent, ICollection<ModelTree> children)
            : base(id)
        {
            Glid = glid;
            PGlid = pGlid;
            Level = level;
            ExternalId = externalId;
            GroupName = groupName;
            Parent = parent;
            Children = children;
        }
    }
}

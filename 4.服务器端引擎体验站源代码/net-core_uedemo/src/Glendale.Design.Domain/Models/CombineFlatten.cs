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
    /// 合模压平记录
    /// </summary>
    public class CombineFlatten : CreationAuditedEntity<Guid>
    {
        /// <summary>
        /// 合模Id
        /// </summary>
        public virtual Guid CombineId { get; set; }

        /// <summary>
        /// 类型 0：压平； 1：裁剪；
        /// </summary>
        public virtual int FlattenType { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [MaxLength(50)]
        public virtual string FlattenName { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        public virtual double FlattenHeight { get; set; }

        /// <summary>
        /// 范围
        /// </summary>
        public virtual string FlattenScope { get; set; }

        #region 导航
        /// <summary>
        /// 合模
        /// </summary>
        [NotMapped]
        public virtual Combine Combine { get; set; }

        #endregion

        public CombineFlatten()
        {
        }
        public CombineFlatten(Guid id, Guid combineId, int flattenType, string flattenName, double flattenHeight,string flattenScope) : base(id)
        {
            CombineId = combineId;
            FlattenType = flattenType;
            FlattenName = flattenName;
            FlattenHeight = flattenHeight;
            FlattenScope = flattenScope;
        }

    }
}

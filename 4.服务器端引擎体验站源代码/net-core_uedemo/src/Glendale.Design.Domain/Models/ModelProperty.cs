using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace Glendale.Design.Models
{
    /// <summary>
    /// 构建属性查询
    /// </summary>
    public class ModelProperty : Entity<int>
    {
        #region 属性
        /// <summary>
        /// 外键Id
        /// </summary>
        [MaxLength(50)]
        public virtual string Glid { get; set; }
        /// <summary>
        /// 构件id，对应结构表externalId
        /// </summary>
        [MaxLength(50)]
        public virtual string ExternalId { get; set; }
        /// <summary>
        /// 属性大值
        /// </summary>
        [MaxLength(100)]
        public virtual string PropertyTypeName { get; set; }
        /// <summary>
        /// 属性小值
        /// </summary>
        [MaxLength(100)]
        public virtual string PropertySetName { get; set; }
        /// <summary>
        /// 属性名称
        /// </summary>
        [MaxLength(100)]
        public virtual string PropertyName { get; set; }
        /// <summary>
        /// 当前属性ifc文件路径
        /// </summary>
        [MaxLength(200)]
        public virtual string Ifcurl { get; set; }
        /// <summary>
        /// 属性值
        /// </summary>
        [MaxLength(100)]
        public virtual string Value { get; set; }

        [MaxLength(36)]
        public virtual string ModelName { get; set; }
        
        //[MaxLength(50)]
        //public virtual string GroupName { get; set; }

        #endregion


        #region 导航属性
        /// <summary>
        /// 父数据
        /// </summary>
        [NotMapped]
        public virtual ModelProperty Parent { get; set; }
        /// <summary>
        /// 子数据
        /// </summary>
        [NotMapped]
        public virtual ICollection<ModelProperty> Children { get; set; }
        #endregion


        protected ModelProperty()
        {
        }
        public ModelProperty(int id, string glid, string externalId, string propertyTypeName, string propertySetName, string propertyName, string ifcurl, string value, string modelName, string groupName, ModelProperty parent, ICollection<ModelProperty> children)
            : base(id)
        {
            Glid = glid;
            ExternalId = externalId;
            PropertyTypeName = propertyTypeName;
            PropertySetName = propertySetName;
            PropertyName = propertyName;
            Ifcurl = ifcurl;
            Value = value;
            ModelName = modelName;
            //GroupName = groupName;
            Parent = parent;
            Children = children;
        }
    }
}

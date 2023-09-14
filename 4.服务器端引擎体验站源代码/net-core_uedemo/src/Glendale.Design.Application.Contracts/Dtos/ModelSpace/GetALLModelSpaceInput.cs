using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.ModelSpace
{
    /// <summary>
    /// 模型楼层数据
    /// </summary>
    public class GetALLModelSpaceInput
    {
        public virtual string Glid { get; set; }
        public virtual string PGlid { get; set; }
        public int? Level { get; set; }
        /// <summary>
		/// 机机构名称
		/// </summary>
		[MaxLength(50)]
        public string Name { get; set; }
        public string ExternalId { get; set; }
        public  string GroupName { get; set; }
        public virtual string ModelName { get; set; }
    }
}

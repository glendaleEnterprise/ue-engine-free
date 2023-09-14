using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.ModelTree
{
	/// <summary>
	/// 模型楼层数据
	/// </summary>
	public class ModelTreeDto : EntityDto<int>
    {
		public virtual string Glid { get; set; }
		public virtual string PGlid { get; set; }
		public  int Level { get; set; }
		public string Name { get; set; }
		public string ExternalId { get; set; }
		public virtual string GroupName { get; set; }
		public virtual string LightweightName { get; set; }
		/// <summary>
		/// 子数据
		/// </summary>
		public virtual ICollection<ModelTreeListDto> Children { get; set; }
	}
}

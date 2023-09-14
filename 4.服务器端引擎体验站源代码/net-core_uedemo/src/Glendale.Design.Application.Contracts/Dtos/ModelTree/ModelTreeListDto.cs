using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.ModelTree
{
	/// <summary>
	/// 模型楼层数据
	/// </summary>
	public class ModelTreeListDto : EntityDto<int>
	{
		public virtual string Glid { get; set; }
		public virtual string PGlid { get; set; }
		public virtual int Level { get; set; }
		public virtual string Name { get; set; }
		public virtual string ExternalId { get; set; }
		public virtual string GroupName { get; set; }
		public virtual string LightweightName { get; set; }
		/// <summary>
		/// 子数据
		/// </summary>
		public virtual ICollection<ModelTreeListDto> Children { get; set; }



	}
}

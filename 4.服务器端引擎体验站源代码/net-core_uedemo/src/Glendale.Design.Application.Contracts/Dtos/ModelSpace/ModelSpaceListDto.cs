using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.ModelSpace
{
	/// <summary>
	/// 模型楼层数据
	/// </summary>
	public class ModelSpaceListDto : EntityDto<int>
    {
		public virtual string Glid { get; set; }
		public virtual string PGlid { get; set; }
		public virtual int Level { get; set; }
		public virtual string Name { get; set; }
		public virtual string ExternalId { get; set; }
		public virtual string GroupName { get; set; }
		public virtual string ModelName { get; set; }
		/// <summary>
		/// 子数据
		/// </summary>
		public virtual ICollection<ModelSpaceListDto> Children { get; set; }



	}
}

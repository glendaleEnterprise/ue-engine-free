using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.ModelType
{
	/// <summary>
	/// 模型楼层数据
	/// </summary>
	public class ModelTypeDto:EntityDto<int>
	{
		public virtual string Glid { get; set; }
		public virtual string PGlid { get; set; }
		public  int Level { get; set; }
		public string Name { get; set; }
		public string ExternalId { get; set; }
		public virtual string GroupName { get; set; }
		public virtual string ModelName { get; set; }
	}


	public class ModelTypeChar
	{
		/// <summary>
		/// 父id
		/// </summary>
		public string ParentId { get; set; }
		/// <summary>
		/// 组件id
		/// </summary>
		public string Char { get; set; }
	} 
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.Dictionary
{
	/// <summary>
	/// 基础数据
	/// </summary>
	public class DictionaryDto : EntityDto<Guid>
    {
		/// <summary>
		/// 名称
		/// </summary>
		public virtual string Name { get; set; }
		/// <summary>
		/// 图标
		/// </summary>
		public virtual string Value { get; set; }
		/// <summary>
		/// 排序值
		/// </summary>
		public int OrderIndex { get; set; }
		/// <summary>
		/// 是否启用
		/// </summary>
		public virtual bool Enable { get; set; }
		/// <summary>
		/// 是否系统内置
		/// </summary>
		public virtual bool System { get; set; }
	}
}

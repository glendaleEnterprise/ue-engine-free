using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.Dictionary
{
	/// <summary>
	/// 基础数据
	/// </summary>
	public class DictionaryListDto : EntityDto<Guid>
    {
		/// <summary>
		/// 父Id
		/// </summary>6
		public virtual Guid? ParentId { get; set; }
		/// <summary>
		/// 名称
		/// </summary>
		public virtual string Name { get; set; }
		/// <summary>
		/// 排序值
		/// </summary>
		public virtual int OrderIndex { get; set; }
		/// <summary>
		/// 是否启用
		/// </summary>
		public virtual bool Enable { get; set; }
		/// <summary>
		/// 值
		/// </summary>
		public virtual string Value { get; set; }
		/// <summary>
		/// 是否系统内置
		/// </summary>
		public virtual bool System { get; set; }
		/// <summary>
		/// 子数据
		/// </summary>
		public virtual ICollection<DictionaryListDto> Children { get; set; }



	}
}

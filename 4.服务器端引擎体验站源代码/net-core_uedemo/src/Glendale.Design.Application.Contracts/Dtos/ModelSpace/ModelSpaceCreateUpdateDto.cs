using System;
using System.ComponentModel.DataAnnotations;

namespace Glendale.Design.Dtos.ModelSpace
{
    /// <summary>
    /// 模型楼层数据
    /// </summary>
    public class ModelSpaceCreateUpdateDto
	{
		/// <summary>
		/// 主键
		/// </summary>
		public virtual Guid Glid { get; set; }
		/// <summary>
		/// 父id
		/// </summary>
		public virtual string PGlid { get; set; }
		/// <summary>
		/// 层级深度
		/// </summary>
		public virtual int Level { get; set; }

		/// <summary>
		/// 机机构名称
		/// </summary>
		[MaxLength(50)]
		public virtual string Name { get; set; }
		/// <summary>
		/// 构件id
		/// </summary>
		public virtual string ExternalId { get; set; }
		/// <summary>
		/// 类别
		/// </summary>
		public virtual string GroupName { get; set; }
		/// <summary>
		/// 模型名称
		/// </summary>
		[Required]
		public virtual string ModelName { get; set; }
	}
}
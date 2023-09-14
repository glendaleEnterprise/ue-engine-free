using System;
using System.ComponentModel.DataAnnotations;

namespace Glendale.Design.Dtos.ModelTree
{
    /// <summary>
    /// 模型楼层数据
    /// </summary>
    public class ModelTreeCreateUpdateDto
	{
		/// <summary>
		/// 主键
		/// </summary>
		[Required]
		public virtual Guid Glid { get; set; }
		/// <summary>
		/// 父id
		/// </summary>
		[MaxLength(150)]
		public virtual string PGlid { get; set; }
		/// <summary>
		/// 层级深度
		/// </summary>
		[MaxLength(150)]
		public virtual int Level { get; set; }

		/// <summary>
		/// 机机构名称
		/// </summary>
		[MaxLength(150)]
		public virtual string Name { get; set; }
		/// <summary>
		/// 构件id
		/// </summary>
		[MaxLength(150)]
		public virtual string ExternalId { get; set; }
		/// <summary>
		/// 类别
		/// </summary>
		[MaxLength(150)]
		public virtual string GroupName { get; set; }
	}
}
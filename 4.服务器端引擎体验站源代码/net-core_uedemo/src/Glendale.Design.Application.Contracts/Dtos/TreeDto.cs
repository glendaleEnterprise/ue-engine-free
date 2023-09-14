using Glendale.Design.Dtos.ProjectFolderUser;
using Glendale.Design.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Glendale.Design.Dtos
{
    /// <summary>
    /// 树结构
    /// </summary>
    public class TreeDto
    {
        public string Key { get; set; }
        public string Title { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? Id { get; set; }
        public ICollection<TreeDto> Children { get; set; } 
        public bool IsLeaf => Children == null || Children.Count == 0;

        public string Remark { get; set; }     
        
        /// <summary>
        /// 开放给人员状态
        /// </summary>
        public string UserStatus { get; set; }

        /// <summary>
        /// 目录用户
        /// </summary>
        public virtual ICollection<ProjectFolderUserDto> ProjectFolderUsers { get; set; }

    }


}

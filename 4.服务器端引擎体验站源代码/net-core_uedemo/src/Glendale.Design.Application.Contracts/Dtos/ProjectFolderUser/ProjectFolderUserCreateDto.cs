using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Glendale.Design.Dtos.ProjectFolderUser
{
    public class ProjectFolderUserCreateDto
    {
        public virtual Guid ProjectId { get; set; }
        public virtual Guid ProjectFolderId { get; set; }
        public virtual Guid UserId { get; set; }
        public virtual string UserRoleNames { get; set; }
    }
}

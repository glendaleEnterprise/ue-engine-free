using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Glendale.Design.Dtos.ProjectImage
{
    public class ProjectImageCreateDto
    {
        public virtual string BlobName { get;set; }
        public virtual bool IsCover { get; set; } = false;
    }
}

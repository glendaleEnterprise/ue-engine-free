using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Glendale.Design.Dtos.File
{
    public class FileUploadInputDto
    {
        [Required]
        public byte[] Bytes { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string FileType { get; set; }
    }
}

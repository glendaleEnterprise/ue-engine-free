using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Glendale.Design.Dtos.File
{
    public class FileDto
    {
        public byte[] Bytes { get; set; }
        public string FileName { get; set; }
    }
}

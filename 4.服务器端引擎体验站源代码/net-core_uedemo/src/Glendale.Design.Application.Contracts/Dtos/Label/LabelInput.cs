using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.Label
{
    public class LabelInput: PagedResultRequestDto
    {    
        public string modelId { get; set; }        
         
    }
}

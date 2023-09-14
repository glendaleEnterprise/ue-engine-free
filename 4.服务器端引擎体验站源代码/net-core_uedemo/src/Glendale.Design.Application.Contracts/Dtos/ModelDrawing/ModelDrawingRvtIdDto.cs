using System;
using System.Collections.Generic;
using System.Text;

namespace Glendale.Design.Dtos.ModelDrawing
{
    public class ModelDrawingRvtIdDto 
    {
        /// <summary>
        /// 
        /// </summary>        
        public virtual string DrawGuid { get; set; }
        
        public virtual string RevitId { get; set; }
         
        public virtual string ModelName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Glendale.Design.Models
{
    public class ModelDrawingRvtId : Entity<int>
    {
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(36)]
        public virtual string DrawGuid{get;set;}

        [MaxLength(36)]
        public virtual string RevitId { get; set; }

        [MaxLength(36)]
        public virtual string ModelName { get; set; }
        
    }
}

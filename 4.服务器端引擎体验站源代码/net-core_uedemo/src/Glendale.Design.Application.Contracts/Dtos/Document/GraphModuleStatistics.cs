using System;
using System.Collections.Generic;
using System.Text;

namespace Glendale.Design.Dtos.Document
{
    public class GraphModuleStatistics
    {
        public List<GroupByUserDto> GroupByUserDto{ get; set; }
        public List<GroupByFileTypeDto> GroupByFileTypeDto { get; set; }
        public int TotalNum { get; set; }
    }
}

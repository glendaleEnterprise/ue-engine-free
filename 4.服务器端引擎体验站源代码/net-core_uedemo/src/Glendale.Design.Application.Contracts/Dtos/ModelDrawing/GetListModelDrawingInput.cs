using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.ModelDrawing
{
    public class GetListModelDrawingInput : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// 模型名称
        /// </summary>     
        public string ModelName { get; set; }

        /// <summary>
        /// 视图类型
        /// </summary>
        public string Type { get; set; }


        public GetListModelDrawingInput()
        {
            Sorting = "";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.DocumentVerThan
{
    public class GetListDocumentVerThanInput : PagedAndSortedResultRequestDto
    {

        public GetListDocumentVerThanInput()
        {
            Sorting = "CreationTime desc";
        }
    }
}

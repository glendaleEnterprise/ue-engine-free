using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.ProjectFolderUser
{
    public class GetListProjectFolderUserInput : PagedAndSortedResultRequestDto
    {
        public Guid? ProjectId { get; set; }
        public Guid? UserId { get; set; }

        public GetListProjectFolderUserInput()
        {
            Sorting = "CreationTime Desc";
        }
    }
}

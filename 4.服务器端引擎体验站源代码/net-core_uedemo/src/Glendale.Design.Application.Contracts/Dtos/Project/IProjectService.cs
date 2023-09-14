using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Glendale.Design.Dtos.Project
{
    /// <summary>
    /// 项目
    /// </summary>
    public interface IProjectService : ICrudAppService<ProjectDto,ProjectListDto,Guid,GetListProjectInput,ProjectCreateDto,ProjectUpdateDto>
    {          
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Glendale.Design.Dtos.Roaming
{
    /// <summary>
    /// 漫游设置
    /// </summary>
    public interface IRoamingService :ICrudAppService<RoamingDto,RoamingListDto,Guid,GetListRoamingInput,RoamingFristCreateDto,RoamingCreateDto>  
    {
    }
}
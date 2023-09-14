using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Services;

namespace Glendale.Design.Dtos.Postil
{
    public interface IPostilAppService :ICrudAppService<PostilDto,PostilListDto,Guid,GetListPostilInput,PostilCreateUpdateDto,PostilCreateUpdateDto>  
    {
    }
}

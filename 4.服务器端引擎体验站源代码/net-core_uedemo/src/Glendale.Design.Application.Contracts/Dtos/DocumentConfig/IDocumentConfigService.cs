using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Glendale.Design.Enums;
using Volo.Abp.Application.Services;

namespace Glendale.Design.Dtos.DocumentConfig
{
    public interface IDocumentConfigService : ICrudAppService<
        DocumentConfigDto,
        DocumentConfigListDto,
        Guid,
        DocumentConfigInput,
        DocumentConfigCreateUpdateDto,
        DocumentConfigCreateUpdateDto> 
    {
    }
}

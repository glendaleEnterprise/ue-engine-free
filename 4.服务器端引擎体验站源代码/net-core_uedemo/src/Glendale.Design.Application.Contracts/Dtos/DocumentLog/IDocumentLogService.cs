using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Glendale.Design.Dtos.DocumentLog
{
    public interface IDocumentLogService : ICrudAppService<DocumentLogDto,DocumentLogListDto,Guid,GetListDocumentLogInput, DocumentLogCreateUpdateDto,
            DocumentLogCreateUpdateDto> 
    {
        
    }
}

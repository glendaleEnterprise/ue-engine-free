using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Services;

namespace Glendale.Design.Dtos.DocumentVerThan
{
    public interface IDocumentVerThanService :
           ICrudAppService<DocumentVerThanDto, DocumentVerThanListDto, Guid, GetListDocumentVerThanInput, DocumentVerThanCreateUpdateDto, DocumentVerThanCreateUpdateDto>
    {
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Services;

namespace Glendale.Design.Dtos.Label
{
    public interface ILabelService : ICrudAppService< 
            LabelDto, 
            LabelListDto,
            Guid, 
            LabelInput,
            LabelCreateUpdateDto, 
            LabelCreateUpdateDto> 
    {
    }
}

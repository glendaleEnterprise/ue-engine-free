using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Glendale.Design.Dtos.Dictionary
{
    /// <summary>
    /// 基础数据
    /// </summary>
    public interface IDictionaryService :ICrudAppService<DictionaryDto,DictionaryListDto,Guid,GetListDictionaryInput,DictionaryCreateUpdateDto,DictionaryCreateUpdateDto> 
    {         
        Task<DictionaryDto> GetByValueAsync(string value);         
    }
}
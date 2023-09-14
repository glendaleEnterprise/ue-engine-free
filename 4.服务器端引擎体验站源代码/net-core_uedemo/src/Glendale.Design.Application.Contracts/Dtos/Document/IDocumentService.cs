using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Glendale.Design.Enums;
using Volo.Abp.Application.Services;

namespace Glendale.Design.Dtos.Document
{
    public interface IDocumentService : ICrudAppService<DocumentDto,DocumentListDto,Guid,GetListDocumentInput,DocumentCreateUpdateDto,DocumentCreateUpdateDto> 
    {   
        /// <summary>
        /// 查询每种图模格式的数量以及上传人的统计
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //Task<Dictionary<int, GraphModuleStatistics>> GetNumOfEachFormat(Guid id);
    }
}

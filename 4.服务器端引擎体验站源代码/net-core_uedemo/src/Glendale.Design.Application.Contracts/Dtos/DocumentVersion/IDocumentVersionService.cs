using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Glendale.Design.Enums;
using Volo.Abp.Application.Services;

namespace Glendale.Design.Dtos.DocumentVer
{
    public interface IDocumentVersionService : ICrudAppService<DocumentVersionDto,DocumentVersionListDto,Guid,GetListDocumentVersionInput,DocumentVersionCreateUpdateDto, DocumentVersionCreateUpdateDto> 
    {
        /// <summary>
        /// 获取全部模型名称
        /// </summary>
        /// <returns></returns>
        Task<List<string>> GetModelNamesAsync();
        Task SetCurrent(Guid id);

        //Task UpdateSync(Guid id, SyncStatusEnum syncStatus);
        /// <summary>
        /// 同步轻量化名称
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modelName"></param>
        /// <returns></returns>
        Task SyncModelName(Guid id, string modelName);
        /// <summary>
        /// 同步轻量化状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="statusDescription"></param>
        /// <returns></returns>
        Task SyncModelStatus(Guid id, DocStatusEnum status,string statusDescription);
        /// <summary>
        /// 获取待轻量化模型文件(获取三十天内的数据)
        /// </summary>
        /// <returns></returns>
        Task<List<DocumentVersionDto>> GetLightweightModelAsync();
        /// <summary>
        /// 获取待轻量化CAD文件(获取三十天内的数据)
        /// </summary>
        /// <returns></returns>
        Task<List<DocumentVersionDto>> GetLightweightCADAsync();
    }
}

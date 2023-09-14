using Glendale.Design.Dtos.ModelDrawing;
using Glendale.Design.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;

namespace Glendale.Design.Services
{
    [AllowAnonymous]
    public class ModelDrawingService : CrudAppService<ModelDrawing, ModelDrawingDto, ModelDrawingListDto, int, GetListModelDrawingInput,
                       ModelDrawingCreateUpdateDto, ModelDrawingCreateUpdateDto>, IModelDrawingAppService
    {
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        private readonly IRepository<ModelDrawingRvtId, int> _repositoryModelDrawingRvtId;
        public ModelDrawingService(IRepository<ModelDrawing, int> repository, IRepository<ModelDrawingRvtId, int> repositoryModelDrawingRvtId)
            : base(repository)
        {
            _repositoryModelDrawingRvtId = repositoryModelDrawingRvtId; 
        }
        [RemoteService(false)]
        public override Task<ModelDrawingDto> CreateAsync(ModelDrawingCreateUpdateDto input)
        {
            return base.CreateAsync(input);
        }
        [RemoteService(false)]
        public override Task<ModelDrawingDto> UpdateAsync(int id, ModelDrawingCreateUpdateDto input)
        {
            return base.UpdateAsync(id, input);
        }
        [RemoteService(false)]
        public override Task DeleteAsync(int id)
        {
            return base.DeleteAsync(id);
        }
        [RemoteService(false)]
        public override Task<ModelDrawingDto> GetAsync(int id)
        {
            return base.GetAsync(id);
        }
        [RemoteService(false)]
        public override Task<PagedResultDto<ModelDrawingListDto>> GetListAsync(GetListModelDrawingInput input)
        {
            return base.GetListAsync(input);
        }


        /// <summary>
        /// 获取图纸类型
        /// </summary>
        /// <param name="modelName">模型名称</param>
        /// <returns></returns>
        public List<string> GetListType(string modelName)
        {
            try
            {
                return Repository.WhereIf(true, o => o.ModelName == modelName).Select(o => o.Type).Distinct().ToList();
            }
            catch
            {
                return null;                     
            }
            
        }
        /// <summary>
        /// 获取图纸列表
        /// </summary>
        /// <param name="modelName">模型名称</param>
        /// <param name="type">视图类型</param>
        /// <returns></returns>
        public Task<List<ModelDrawingListDto>> GetListDrawing(string modelName,string type)
        {
            try
            {
                var data = Repository.WhereIf(true, o => o.ModelName == modelName)
                .WhereIf(true, o => o.Type == type).ToList();
                return MapToGetListOutputDtosAsync(data);
            }
            catch
            {
                return null;
            }             
        }
        /// <summary>
        /// 获取图纸对应的构件
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="drawGuid"></param>
        /// <returns></returns>
        public async Task<List<ModelDrawingRvtIdDto>> GetListDrawingRvtId(string modelName, string drawGuid)
        {
            var data = _repositoryModelDrawingRvtId.Where(p => p.ModelName == modelName && p.DrawGuid == drawGuid).ToList();
            return  ObjectMapper.Map<List<ModelDrawingRvtId>, List<ModelDrawingRvtIdDto>>(data);
        }
    }
}

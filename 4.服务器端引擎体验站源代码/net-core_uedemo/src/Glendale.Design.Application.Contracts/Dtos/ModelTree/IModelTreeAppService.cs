using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Glendale.Design.Dtos.ModelTree
{
    /// <summary>
    /// 模型楼层数据
    /// </summary>
    public interface IModelTreeAppService :
            ICrudAppService< //定义了CRUD方法
            ModelTreeDto, //用来展示
            ModelTreeListDto,
            int, //主键
            GetListModelTreeInput, //获取的时候用于分页和排序
            ModelTreeCreateUpdateDto, //用于创建
            ModelTreeCreateUpdateDto> //用于更新
    {
        //Task<IEnumerable<ModelTreeListDto>> GetByParentAsync(string parent);
        //Task<ModelTreeDto> GetByValueAsync(string value);
        //Task<bool> EnableAsync(Guid id);
        //Task<IEnumerable<ModelTreeViewDto>> GetTreeAsync();
        //Task<IEnumerable<ModelTreeListDto>> GetAllListAsync();

        public Task<List<string>> GetTreeChildId(string modelName, string glId);
    }
}
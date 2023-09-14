using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Glendale.Design.Dtos.ModelSpace
{
    /// <summary>
    /// 模型楼层数据
    /// </summary>
    public interface IModelSpaceAppService :
            ICrudAppService< //定义了CRUD方法
            ModelSpaceDto, //用来展示
            ModelSpaceListDto,
            int, //主键
            GetListModelSpaceInput, //获取的时候用于分页和排序
            ModelSpaceCreateUpdateDto, //用于创建
            ModelSpaceCreateUpdateDto> //用于更新
    {
        //Task<IEnumerable<ModelSpaceListDto>> GetByParentAsync(string parent);
        //Task<ModelSpaceDto> GetByValueAsync(string value);
        //Task<bool> EnableAsync(Guid id);
        //Task<IEnumerable<ModelSpaceViewDto>> GetTreeAsync();
        //Task<IEnumerable<ModelSpaceListDto>> GetAllListAsync();
    }
}
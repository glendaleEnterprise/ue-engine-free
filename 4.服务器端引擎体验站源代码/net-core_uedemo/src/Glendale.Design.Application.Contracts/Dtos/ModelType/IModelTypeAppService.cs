using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Glendale.Design.Dtos.ModelType
{
    /// <summary>
    /// 模型楼层数据
    /// </summary>
    public interface IModelTypeAppService :
            ICrudAppService< //定义了CRUD方法
            ModelTypeDto, //用来展示
            ModelTypeListDto,
            int, //主键
            GetListModelTypeInput, //获取的时候用于分页和排序
            ModelTypeCreateUpdateDto, //用于创建
            ModelTypeCreateUpdateDto> //用于更新
    {
        //Task<IEnumerable<ModelTypeListDto>> GetByParentAsync(string parent);
        //Task<ModelTypeDto> GetByValueAsync(string value);
        //Task<bool> EnableAsync(Guid id);
        //Task<IEnumerable<ModelTypeViewDto>> GetTreeAsync();
        //Task<IEnumerable<ModelTypeListDto>> GetAllListAsync();
    }
}
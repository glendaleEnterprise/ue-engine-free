using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Glendale.Design.Dtos.ModelProperty
{
    /// <summary>
    /// 模型楼层数据
    /// </summary>
    public interface IModelPropertyAppService :
            ICrudAppService< //定义了CRUD方法
            ModelPropertyDto, //用来展示
            ModelPropertyListDto,
            int, //主键
            GetListModelPropertyInput, //获取的时候用于分页和排序
            ModelPropertyCreateUpdateDto, //用于创建
            ModelPropertyCreateUpdateDto> //用于更新
    {
        //Task<IEnumerable<ModelPropertyListDto>> GetByParentAsync(string parent);
        //Task<ModelPropertyDto> GetByValueAsync(string value);
        //Task<bool> EnableAsync(Guid id);
        //Task<IEnumerable<ModelPropertyViewDto>> GetTreeAsync();
        //Task<IEnumerable<ModelPropertyListDto>> GetAllListAsync();
    }
}
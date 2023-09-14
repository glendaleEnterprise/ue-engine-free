using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Glendale.Design.Dtos.ModelPropertySpace
{
    /// <summary>
    /// 模型楼层数据
    /// </summary>
    public interface IModelPropertySpaceAppService :
            ICrudAppService< //定义了CRUD方法
            ModelPropertySpaceDto, //用来展示
            ModelPropertySpaceListDto,
            int, //主键
            GetListModelPropertySpaceInput, //获取的时候用于分页和排序
            ModelPropertySpaceCreateUpdateDto, //用于创建
            ModelPropertySpaceCreateUpdateDto> //用于更新
    {
        //Task<IEnumerable<ModelPropertySpaceListDto>> GetByParentAsync(string parent);
        //Task<ModelPropertySpaceDto> GetByValueAsync(string value);
        //Task<bool> EnableAsync(Guid id);
        //Task<IEnumerable<ModelPropertySpaceViewDto>> GetTreeAsync();
        //Task<IEnumerable<ModelPropertySpaceListDto>> GetAllListAsync();
    }
}
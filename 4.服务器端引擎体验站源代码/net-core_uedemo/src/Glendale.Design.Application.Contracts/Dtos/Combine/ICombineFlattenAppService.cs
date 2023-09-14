using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Services;

namespace Glendale.Design.Dtos.Combine
{
    public interface ICombineFlattenAppService :
           ICrudAppService< //定义了CRUD方法
           CombineFlattenDto, //用来展示
           CombineFlattenListDto,
           Guid, //主键
           GetListCombineFlattenInput, //获取的时候用于分页和排序
           CreateCombineFlattenDto, //用于创建
           CreateCombineFlattenDto> //用于更新
    {
    }
}

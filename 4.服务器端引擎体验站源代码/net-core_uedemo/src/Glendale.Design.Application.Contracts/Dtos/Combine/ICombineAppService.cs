using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Services;

namespace Glendale.Design.Dtos.Combine
{
    public interface ICombineAppService :
           ICrudAppService< //定义了CRUD方法
           CombineDto, //用来展示
           CombineListDto,
           Guid, //主键
           GetListCombineInput, //获取的时候用于分页和排序
           CombineCreateUpdateDto, //用于创建
           CombineCreateUpdateDto> //用于更新
    {
    }
}

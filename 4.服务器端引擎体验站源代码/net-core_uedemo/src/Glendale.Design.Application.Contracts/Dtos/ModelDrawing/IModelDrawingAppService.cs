using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Services;

namespace Glendale.Design.Dtos.ModelDrawing
{   
    public interface IModelDrawingAppService :
            ICrudAppService< //定义了CRUD方法
            ModelDrawingDto, //用来展示
            ModelDrawingListDto,
            int, //主键
            GetListModelDrawingInput, //获取的时候用于分页和排序
            ModelDrawingCreateUpdateDto, //用于创建
            ModelDrawingCreateUpdateDto> //用于更新
    {
         
    }
}

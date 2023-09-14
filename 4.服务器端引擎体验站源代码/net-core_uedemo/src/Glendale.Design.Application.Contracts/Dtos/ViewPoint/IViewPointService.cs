using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Glendale.Design.Dtos.ViewPoint
{
    /// <summary>
    /// 视点设置
    /// </summary>
    public interface IViewPointService :
            ICrudAppService< //定义了CRUD方法
            ViewPointDto, //用来展示
            ViewPointListDto,
            Guid, //主键
            GetListViewPointInput, //获取的时候用于分页和排序
            ViewPointCreateUpdateDto, //用于创建
            ViewPointCreateUpdateDto> //用于更新
    {
    }
}
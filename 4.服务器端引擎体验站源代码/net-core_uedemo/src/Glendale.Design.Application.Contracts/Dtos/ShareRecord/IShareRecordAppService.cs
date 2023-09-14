using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Services;

namespace Glendale.Design.Dtos.ShareRecord
{
    public interface IShareRecordAppService :
           ICrudAppService< //定义了CRUD方法
           ShareRecordDto, //用来展示
           ShareRecordListDto,
           Guid, //主键
           GetListShareRecordInput, //获取的时候用于分页和排序
           ShareRecordCreateUpdateDto, //用于创建
           ShareRecordCreateUpdateDto> //用于更新
    {
    }
}

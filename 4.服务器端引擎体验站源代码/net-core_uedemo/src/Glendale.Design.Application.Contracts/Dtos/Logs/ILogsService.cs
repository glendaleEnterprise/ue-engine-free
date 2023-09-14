using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Glendale.Design.Dtos
{
    public interface ILogsService :
            ICrudAppService<
            LogsDto,
            LogsListDto,
            Guid,
            GetListLogsInput,
            LogsCreateUpdateDto,
            LogsCreateUpdateDto>
    {
    }
}

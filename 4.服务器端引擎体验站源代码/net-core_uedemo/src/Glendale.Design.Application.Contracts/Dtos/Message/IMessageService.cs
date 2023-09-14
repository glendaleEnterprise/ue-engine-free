using Glendale.Design.Dtos.Message;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Glendale.Design.Dtos.Message
{
    public interface IMessageService :
            ICrudAppService<
            MessageDto,
            MessageListDto,
            Guid,
            GetListMessageInput,
            MessageCreateUpdateDto,
            MessageCreateUpdateDto>
    {
        Task<MessageDto> CreateAsync(MessageCreateUpdateDto input);
    }
}
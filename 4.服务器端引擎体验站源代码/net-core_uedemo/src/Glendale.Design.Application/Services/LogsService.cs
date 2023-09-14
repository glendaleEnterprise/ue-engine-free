using Glendale.Design.Dtos;
using Glendale.Design.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace Glendale.Design.Services
{
    public class LogsService : CrudAppService<ActionLog, LogsDto, LogsListDto, Guid, GetListLogsInput, LogsCreateUpdateDto, LogsCreateUpdateDto>, ILogsService
    {

        private readonly IIdentityUserRepository UserRepository;
        public LogsService(IRepository<ActionLog, Guid> repository, IIdentityUserRepository userRepository) : base(repository)
        {
            UserRepository = userRepository;
        }

        protected override async Task<IQueryable<ActionLog>> CreateFilteredQueryAsync(GetListLogsInput input)
        {   
            var Query = await base.CreateFilteredQueryAsync(input);
            Query = Query 
                .WhereIf(input.Keyword.IsNotNullOrEmpty(), o => o.Name.Contains(input.Keyword)
                || o.ModuleName.Contains(input.Keyword)|| o.Address.Contains(input.Keyword))
                .WhereIf(input.UserId!=null,o=>o.CreatorId==input.UserId);
            if (input.DateType >-1)
            {
                var now = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
                var date = now.AddDays(-input.DateType);
                Query = Query.Where( o => o.CreationTime >= date);
            }
            return Query;
        }

        public override async Task<PagedResultDto<LogsListDto>> GetListAsync(GetListLogsInput input)
        {
            var cu = CurrentUser;

            var dtos = await base.GetListAsync(input);
            var entitys = await UserRepository.GetListAsync();
            foreach (var dto in dtos.Items)
            {
                dto.UserName = entitys.Where(p => p.Id == dto.CreatorId).FirstOrDefault()?.Name;
            }
            return dtos;
        }

        public override async Task<LogsDto> CreateAsync(LogsCreateUpdateDto input)
        {
            var logs = new ActionLog();
            await MapToEntityAsync(input,logs);             
            var logsResult = await base.Repository.InsertAsync(logs);
            return await MapToGetOutputDtoAsync(logsResult);
        }
    }
}

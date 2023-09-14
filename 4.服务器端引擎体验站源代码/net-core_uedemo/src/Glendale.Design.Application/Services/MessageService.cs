using Glendale.Design.Dtos.Message;
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
using Volo.Abp.Linq;

namespace Glendale.Design.Services
{ 
    public class MessageService : CrudAppService<Message, MessageDto, MessageListDto, Guid, GetListMessageInput, MessageCreateUpdateDto, MessageCreateUpdateDto>, IMessageService
    {
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }
        private readonly IRepository<Message, Guid> _messageRepository;
        private readonly IRepository<MessageReceive, Guid> _messageReceiveRepository;
        private readonly IRepository<IdentityUser, Guid> _userRepository;
        private readonly IRepository<Project, Guid> _projectRepository;
        public MessageService(IRepository<Message, Guid> repository,IRepository<Message, Guid> messageRepostory,IRepository<MessageReceive, Guid> messageReceiveRepository, IRepository<IdentityUser, Guid> userRepository,IRepository<Project, Guid> projectRepository)
            : base(repository)
        {
            _messageRepository = messageRepostory;
            _messageReceiveRepository = messageReceiveRepository;
            _userRepository = userRepository;
            _projectRepository = projectRepository;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override Task<MessageDto> CreateAsync(MessageCreateUpdateDto input)
        {
            var result = base.CreateAsync(input);
            if (input.ReceiveUserIds.Length > 0)
            {
                for (int i = 0; i < input.ReceiveUserIds.Length; i++)
                {
                    _messageReceiveRepository.InsertAsync(new MessageReceive()
                    {
                         ProjectId=input.ProjectId,
                        IsRead = false,
                        MessageId = result.Result.Id,
                        ReadCount = 0,
                        ReadTime = null,
                        UserId = input.ReceiveUserIds[i],
                    });
                }
            }
            return result;
        }

        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async override Task DeleteAsync(Guid id)
        {
            var messageEntity = await _messageRepository.GetAsync(id);
            if (messageEntity != null)
            {
                await base.DeleteByIdAsync(id);
                await _messageReceiveRepository.DeleteAsync(p => p.MessageId == id);
            }
        }

        /// <summary>
        /// 用户全部阅读
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UserAllRead()
        {
            var query = from t in await _messageRepository.GetListAsync()
                        join r in await _messageReceiveRepository.GetListAsync(p => p.UserId == Guid.Parse(CurrentUser.Id.ToString()))
                        on t.Id equals r.MessageId
                        where 1 == 1
                        orderby t.CreationTime descending
                        select new MessageListDto
                        {
                            Id = t.Id,                           
                            CreationTime = t.CreationTime,
                            Title = t.Title,
                            BodyText = t.BodyText,
                            Description = t.Description,                             
                            ProjectId = t.ProjectId
                        };

            var receiveEntityList = _messageReceiveRepository.Where(p=>query.Select(o=>o.Id).Contains(p.MessageId)&&p.IsRead==false).ToList();
            receiveEntityList.ForEach(o => {
                o.ReadCount += 1; o.IsRead = true; o.ReadTime = DateTime.Now;
            });
            foreach (var item in receiveEntityList)
            {
                //更新消息阅读记录
                item.ReadCount += 1; 
                item.IsRead = true; 
                item.ReadTime = DateTime.Now;
                var result = await _messageRepository.GetAsync(p => p.Id ==item.MessageId);                
            }
            await _messageReceiveRepository.UpdateManyAsync(receiveEntityList);
            return true;
        }

        /// <summary>
        /// 阅读消息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<MessageDto> Read(Guid id)
        {
            var result = await _messageRepository.GetAsync(id);
            var receiveEntity = await _messageReceiveRepository.GetAsync(p => p.MessageId == id && p.UserId == Guid.Parse(CurrentUser.Id.ToString()));
            if (receiveEntity != null)
            {
                //更新消息阅读记录
                receiveEntity.IsRead = true;
                receiveEntity.ReadTime = DateTime.Now;
                receiveEntity.ReadCount += 1;
                await _messageReceiveRepository.UpdateAsync(receiveEntity);                
            }
            return MapToGetOutputDto(result);
        }

        protected override async Task<IQueryable<Message>> CreateFilteredQueryAsync(GetListMessageInput input)
        {
            var Query = await base.CreateFilteredQueryAsync(input);
            return Query.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), p => p.Title.Contains(input.Keyword) || p.BodyText.Contains(input.Keyword))  
                .WhereIf(!string.IsNullOrWhiteSpace(input.StartDate), p => p.CreationTime >= Convert.ToDateTime(input.StartDate))
                .WhereIf(!string.IsNullOrWhiteSpace(input.EndDate), p => p.CreationTime <= Convert.ToDateTime(input.EndDate));
        }

        /// <summary>
        /// 所有消息列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>        
        public async override Task<PagedResultDto<MessageListDto>> GetListAsync(GetListMessageInput input)
        {
            var result= await base.GetListAsync(input);             
            return result;
        }

        /// <summary>
        /// 我的消息列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<MessageListDto>> GetMineList(GetListMessageInput input)
        {
            var Query = _messageRepository.Include(s => s.MessageReceives)
                .Where(s => s.MessageReceives.Any(r => r.UserId == CurrentUser.Id))
                .WhereIf(input.ProjectId.HasValue, s => input.ProjectId == s.ProjectId)
                .WhereIf(!string.IsNullOrEmpty(input.Keyword), s => (s.Title.Contains(input.Keyword) || s.BodyText.Contains(input.Keyword)))
                .WhereIf(!string.IsNullOrEmpty(input.StartDate), s => s.CreationTime >= Convert.ToDateTime(input.StartDate))
                .WhereIf(!string.IsNullOrEmpty(input.EndDate), s => s.CreationTime <= Convert.ToDateTime(input.EndDate))
                .WhereIf(input.IsRead >= 0, s => s.MessageReceives.Any(r => r.UserId == CurrentUser.Id && r.IsRead == Convert.ToBoolean(input.IsRead)));

            var totalCount = await AsyncQueryableExecuter.CountAsync(Query);
            Query = ApplySorting(Query, input);
            Query = ApplyPaging(Query, input);
            var entitys = await AsyncQueryableExecuter.ToListAsync(Query);
            var dtos = ObjectMapper.Map<List<Message>, List<MessageListDto>>(entitys);

            var userids = new List<Guid>();
            foreach (var item in dtos)
            {
                userids.AddRange(item.MessageReceives.Select(s => s.UserId).ToList());
            }
            var userlist = await _userRepository.Where(s => userids.Contains(s.Id)).ToListAsync();

            foreach (var item in dtos)
            {
                item.IdentityUsers = userlist.Where(s => item.MessageReceives.Any(r => r.UserId == s.Id)).Select(s => new IdentityUserDto() { Id = s.Id, UserName = s.UserName, Name = s.Name, PhoneNumber = s.PhoneNumber}).ToList();
                item.IsRead = item.MessageReceives.Where(s => s.UserId == CurrentUser.Id).First().IsRead;
            }

            return new PagedResultDto<MessageListDto>(totalCount, dtos);
        }

        /// <summary>
        /// 我的消息列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<int> GetMineCount(GetListMessageInput input)
        {
            var Query = _messageRepository.Include(s => s.MessageReceives)
                .Where(s => s.MessageReceives.Any(r => r.UserId == CurrentUser.Id))
                .WhereIf(input.ProjectId.HasValue, s => input.ProjectId == s.ProjectId)
                .WhereIf(!string.IsNullOrEmpty(input.Keyword), s => (s.Title.Contains(input.Keyword) || s.BodyText.Contains(input.Keyword)))
                .WhereIf(!string.IsNullOrEmpty(input.StartDate), s => s.CreationTime >= Convert.ToDateTime(input.StartDate))
                .WhereIf(!string.IsNullOrEmpty(input.EndDate), s => s.CreationTime <= Convert.ToDateTime(input.EndDate))
                .WhereIf(input.IsRead >= 0, s => s.MessageReceives.Any(r => r.UserId == CurrentUser.Id && r.IsRead == Convert.ToBoolean(input.IsRead)));

            return await AsyncQueryableExecuter.CountAsync(Query);
        }

        /// <summary>
        /// 未读消息数量
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetNoReadCount()
        {
            var total = 0;
            var query = (from r in await _messageReceiveRepository.GetListAsync()
                         where !r.IsRead && CurrentUser.Id.HasValue ? r.UserId == CurrentUser.Id.Value : false
                        select new MessageListDto()
                        {
                            Id = r.Id,
                            CreationTime = r.CreationTime,
                            ProjectId = r.ProjectId
                        }).ToList();
            total = query.Count();
            return total;
            //return _messageReceiveRepository.Where(s => s.UserId == CurrentUser.Id && !s.IsRead).Count();
        }
    }
}

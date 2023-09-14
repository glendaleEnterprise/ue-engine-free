using Glendale.Design.Dtos;
using Glendale.Design.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;

namespace Glendale.Design.Services
{
    [RemoteService(true)]
    public class UserRegisterAppService : DesignAppService
    {
        protected readonly IdentityUserManager _userManager;
        private readonly IIdentityUserRepository _userRepository;
        private readonly IIdentityRoleRepository _roleRepository;
        private readonly ILogsService _logsService;
        private readonly IMemoryCache _memoryCache;

        public UserRegisterAppService(IdentityUserManager userManager , IIdentityUserRepository userRepository, IIdentityRoleRepository roleRepository
            , ILogsService logsService , IMemoryCache memoryCache)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _logsService = logsService;
            _memoryCache = memoryCache;
            _userManager.Options.Password.RequireLowercase = false;
            _userManager.Options.Password.RequireDigit = false;
            _userManager.Options.Password.RequireUppercase = false;
            _userManager.Options.Password.RequireNonAlphanumeric = false;
            _userManager.Options.Password.RequiredUniqueChars = 0;
            _userManager.Options.Password.RequiredLength = 6;
        }

        /// <summary>
        /// 注册账号
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<IdentityUserDto> PostRegisterAsync(IdentityUserRegisterCreateDto input)
        {
#if RELEASE
            var memory = _memoryCache.Get(input.PhoneNumber);
            if (memory == null || !memory.Equals(input.Code))
            {
                throw new UserFriendlyException("验证码不正确");
            }
#endif
            var listUser = await _userRepository.GetListAsync(p => p.UserName == input.PhoneNumber);
            if (listUser.Count > 0)
            {
                throw new UserFriendlyException(input.PhoneNumber + "已经存在，不能重复注册");
            }
            var userDto = new IdentityUser(Guid.NewGuid(), input.PhoneNumber, input.Email);
            userDto.Name = input.Name;
            userDto.SetPhoneNumber(input.PhoneNumber, true);
            foreach (var item in input.ExtraProperties)
            {
                userDto.ExtraProperties.Add(item.Key, item.Value);
            }
            userDto = await _userRepository.InsertAsync(userDto, true);
            await _userManager.AddPasswordAsync(userDto, input.Password);
            await _userManager.AddToRolesAsync(userDto, input.RoleNames);

            await _logsService.CreateAsync(new Dtos.LogsCreateUpdateDto { ModuleName = "用户管理", Name = "更新", Content = $"更新了用户：[{input.PhoneNumber}]" });
            return ObjectMapper.Map<IdentityUser, IdentityUserDto>(userDto); ;
        }
    }
}

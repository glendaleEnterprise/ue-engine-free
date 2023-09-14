using Glendale.Design.Dtos;
using Glendale.Design.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    [Authorize]
    public class UserProfileAppService : DesignAppService, IUserProfileAppService
    {
        protected readonly IdentityUserManager _userManager;
        private readonly IIdentityUserRepository _userRepository;
        private readonly IIdentityRoleRepository _roleRepository;
        protected readonly IOptions<IdentityOptions> IdentityOptions;
        private readonly ILogsService _logsService;
        private readonly IMemoryCache _memoryCache;

        public UserProfileAppService(IdentityUserManager userManager , IIdentityUserRepository userRepository, IIdentityRoleRepository roleRepository
            , IOptions<IdentityOptions> identityOptions , ILogsService logsService , IMemoryCache memoryCache)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            IdentityOptions = identityOptions;
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
        /// 获取系统所有用户
        /// </summary>
        /// <returns></returns>
        public async Task<List<IdentityUserDto>> GetAll()
        {
            //.GetListAsync(p=>p.UserName!="guest");  //需要设置用户项目角色，所以 guest 用户也需要在项目中选择
            var result = await _userRepository.GetListAsync(); 
            var data = ObjectMapper.Map<IEnumerable<Volo.Abp.Identity.IdentityUser>, IReadOnlyList<IdentityUserDto>>(result);
            return new List<IdentityUserDto>(data);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public virtual async Task ResetPasswordAsync(Guid id, [Required] string password)
        {
            await IdentityOptions.SetAsync();

            var user = await _userManager.GetByIdAsync(id);

            if (user.IsExternal)
            {
                throw new BusinessException(code: IdentityErrorCodes.ExternalUserPasswordChange);
            }
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            (await _userManager.ResetPasswordAsync(user, resetToken, password)).CheckErrors();

            await _logsService.CreateAsync(new Dtos.LogsCreateUpdateDto { ModuleName = "用户管理", Name = "重置密码", Content = $"重置密码，账号：{user.PhoneNumber}" });
        }

        /// <summary>
        /// 修改手机号码
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<string> UpdatePhoneNumber(string userName, string phoneNumber, string code, string name)
        {
            var currentUser = await _userManager.FindByNameAsync(userName);
            if (currentUser == null)
            {
                return "账号不存在";
            }
            var memory = _memoryCache.Get(phoneNumber);
            if (memory == null || !memory.Equals(code))
            {
                return "验证码不正确";
            }           
            var data = await _userRepository.GetListAsync(p => p.PhoneNumber == phoneNumber && p.UserName != userName);
            if (data != null && data.Count > 0)
            {
                return "手机号已绑定";
            }
            await _logsService.CreateAsync(new Dtos.LogsCreateUpdateDto { ModuleName = "用户管理", Name = "修改手机号", Content = $"新手机号：{phoneNumber}" });
            await _userManager.SetPhoneNumberAsync(currentUser, phoneNumber);

            currentUser= await _userManager.FindByNameAsync(userName);
            currentUser.Name = name;            
            await _userManager.SetUserNameAsync(currentUser, phoneNumber);
            await _userManager.UpdateNormalizedUserNameAsync(currentUser);
            return "ok";
        }

        /// <summary>
        /// 更新用户锁定状态
        /// </summary>
        /// <returns></returns>
        public async Task<IdentityResult> UpdateLock(string userName)
        {
            if(CurrentUser.UserName != "admin" || CurrentUser.UserName != "sysadmin")
            {
                throw new Volo.Abp.UserFriendlyException("无权修改");
            }
            var currentUser = await _userManager.FindByNameAsync(userName);
            if (currentUser == null)
            {
                throw new Volo.Abp.UserFriendlyException("用户不存在");
            }
            currentUser = await _userManager.FindByNameAsync(userName);
            DateTime? dt = null;
            if (!currentUser.LockoutEnd.HasValue) dt = DateTime.Now.AddMinutes(1);
            if (!currentUser.LockoutEnabled) await _userManager.SetLockoutEnabledAsync(currentUser, !currentUser.LockoutEnabled);
            return await _userManager.SetLockoutEndDateAsync(currentUser, dt);
        }

        /// <summary>
        /// 验证密码是否试初始值123456
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<bool> IsInitPwd(string userName)
        {
            var currentUser = await _userManager.FindByNameAsync(userName);
            if (currentUser != null)
            {
              return await  _userManager.CheckPasswordAsync(currentUser, "123456");
            }
            return false;
        }

        /// <summary>
        /// 修改当前密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<IdentityResult> ChangePassword(SystemChangePasswordInput input)
        {
            if(!string.IsNullOrEmpty(input.Code)) input.Code = input.Code.Trim();
            if(!string.IsNullOrEmpty(input.CurrentPassword)) input.CurrentPassword = input.CurrentPassword.Trim();
            input.NewPassword = input.NewPassword.Trim();
            //判断旧密码是否正确
            if (string.IsNullOrWhiteSpace(input.CurrentPassword) && string.IsNullOrWhiteSpace(input.Code))
            {
                throw new UserFriendlyException("旧密码和短信验证码不能同时为空");
            }
            //获取abp用户
            var user = await _userManager.GetByIdAsync((Guid)CurrentUser.Id);
            if (!string.IsNullOrWhiteSpace(input.CurrentPassword))
            {
                if (input.CurrentPassword == input.NewPassword)
                {
                    throw new UserFriendlyException("当前密码与新密码不能相同");
                }
                //判断当前密码是否正确
                var result = await _userManager.CheckPasswordAsync(user, input.CurrentPassword);
                if (!result)
                {
                    throw new UserFriendlyException("当前密码错误");
                }
                //var resetToken = _userManager.GeneratePasswordResetTokenAsync(user).Result;
                //(await _userManager.ResetPasswordAsync(user, resetToken, input.NewPassword)).CheckErrors();
                //return true;
                return await _userManager.ChangePasswordAsync(user, input.CurrentPassword, input.NewPassword);
            }

            var memory = _memoryCache.Get(user.PhoneNumber);
            if (memory == null || !memory.Equals(input.Code))
            {
                throw new UserFriendlyException("验证码不正确");
            }
            await _userManager.RemovePasswordAsync(user);
            return await _userManager.AddPasswordAsync(user, input.NewPassword);
        }
    }
}

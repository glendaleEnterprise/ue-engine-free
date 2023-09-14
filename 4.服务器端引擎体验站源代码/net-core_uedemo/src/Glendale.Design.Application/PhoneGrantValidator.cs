using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Identity;
using static IdentityModel.OidcConstants;

namespace Glendale.Design
{
    /// <summary>
    /// 扩展手机验证码登录
    /// </summary>
    public class PhoneGrantValidator : IExtensionGrantValidator
    {
        public string GrantType => ExtensionGrantTypes.SMSGrantType;
        protected readonly IdentityUserManager _userManager;
        protected readonly IIdentityUserRepository _userRepository;
        private readonly IMemoryCache _memoryCache;
        public PhoneGrantValidator(IdentityUserManager userManager, IIdentityUserRepository userRepository, IMemoryCache memoryCache)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _memoryCache = memoryCache;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var phone = context.Request.Raw.Get("phone");
            var code = context.Request.Raw.Get("code");

            if (string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(code))
            {
                context.Result = new GrantValidationResult(
                        error: TokenRequestErrors.UnauthorizedClient,
                        errorDescription: "手机号和验证码不能为空");
                return;
            }
            var userList = await _userRepository.GetListAsync(p => p.PhoneNumber == phone);
            if (userList == null || userList.Count == 0)
            {
                context.Result = new GrantValidationResult(
                   error: TokenRequestErrors.UnauthorizedClient,
                   errorDescription: "账户不存在");
                return;
            }
            var user = userList[0];

            //判断账户有效期
            var validityDate= user.ExtraProperties.GetOrDefault("ValidityDate");
            if(validityDate!=null && Convert.ToDateTime(validityDate) < DateTime.Now)
            {
                context.Result = new GrantValidationResult(
                  error: TokenRequestErrors.UnauthorizedClient,
                  errorDescription: "账户已经过期，请联系管理员");
                return ;
            }

            if (code != null && code.Equals(_memoryCache.Get(phone)))
            {
                context.Result = new GrantValidationResult(
                    subject: user.Id.ToString(),
                    authenticationMethod: AuthenticationMethods.ConfirmationByTelephone,
                    claims: new List<Claim>());
            }
            else
            {
                context.Result = new GrantValidationResult(
                    error: TokenRequestErrors.UnauthorizedClient,
                    errorDescription: "验证码不正确");
            }             
        }

    }

    public class ExtensionGrantTypes
    {
        public const string SMSGrantType = "PhoneCodeGrantType";
    }
}

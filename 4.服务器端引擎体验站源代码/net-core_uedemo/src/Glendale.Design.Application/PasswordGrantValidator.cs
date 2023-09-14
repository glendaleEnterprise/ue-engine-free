using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Identity;
using static IdentityModel.OidcConstants;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace Glendale.Design
{
    /// <summary>
    /// 重写密码登录，补充登录前校验
    /// </summary>
    public class PasswordGrantValidator : DesignAppService, IResourceOwnerPasswordValidator
    {         
        protected readonly IdentityUserManager _userManager;
        protected readonly IIdentityUserRepository _userRepository;
        private readonly SignInManager<IdentityUser> _signInManager;
        public PasswordGrantValidator(IdentityUserManager userManager, IIdentityUserRepository userRepository, SignInManager<IdentityUser> signInManager)
        {          
            _userManager = userManager;
            _userRepository = userRepository;
            _signInManager = signInManager;
        }
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var username = context.UserName;
            var password = context.Password;
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                context.Result = new GrantValidationResult(
                        error: TokenRequestErrors.UnauthorizedClient,
                        errorDescription: "账号和密码不能为空");
                return;
            }

            var userList = await _userRepository.GetListAsync(p => p.UserName == username);
            if (userList == null || userList.Count == 0)
            {
                context.Result = new GrantValidationResult(
                   error: TokenRequestErrors.UnauthorizedClient,
                   errorDescription: "账户不存在");
                return ;
            }
            var user = userList[0];
            //判断账户有效期
            var validityDate = user.ExtraProperties.GetOrDefault("ValidityDate");
            if (validityDate != null && Convert.ToDateTime(validityDate) < DateTime.Now)
            {
                context.Result = new GrantValidationResult(
                  error: TokenRequestErrors.UnauthorizedClient,
                  errorDescription: "账户已经过期，请联系管理员");
                return;
            }

            //判断密码是否正确
            var signInResult = await _signInManager.PasswordSignInAsync(username, password, false, false);
            if (!signInResult.Succeeded)
            {
                context.Result = new GrantValidationResult(
                 error: TokenRequestErrors.UnauthorizedClient,
                 errorDescription: "密码不正确，请重新输入");
            }
            else
            {
                context.Result = new GrantValidationResult(
                    subject: user.Id.ToString(),
                    authenticationMethod: AuthenticationMethods.Password,
                    claims: new List<Claim>());
            }

        }
        //public async Task ValidateAsync(ExtensionGrantValidationContext context)
        //{
        //    var username = context.Request.Raw.Get("username");
        //    var password = context.Request.Raw.Get("password");

        //    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        //    {
        //        context.Result = new GrantValidationResult(
        //                error: TokenRequestErrors.UnauthorizedClient,
        //                errorDescription: "账号和密码不能为空");
        //    }

        //     var userList = await _userRepository.GetListAsync(p => p.UserName == username);
        //    if (userList == null || userList.Count == 0)
        //    {
        //        context.Result = new GrantValidationResult(
        //           error: TokenRequestErrors.UnauthorizedClient,
        //           errorDescription: "账户不存在");
        //    }
        //    var user = userList[0];
        //    //判断账户有效期
        //    var validityDate = user.ExtraProperties.GetOrDefault("ValidityDate");
        //    if (validityDate != null && Convert.ToDateTime(validityDate) < DateTime.Now)
        //    {
        //        context.Result = new GrantValidationResult(
        //          error: TokenRequestErrors.UnauthorizedClient,
        //          errorDescription: "账户已经过期，请联系管理员");
        //    }

        //    //判断密码是否正确
        //    var signInResult = await _signInManager.PasswordSignInAsync(username, password, false, false);
        //    if (!signInResult.Succeeded)
        //    {
        //        context.Result = new GrantValidationResult(
        //         error: TokenRequestErrors.UnauthorizedClient,
        //         errorDescription: "密码不正确，请重新输入");
        //    }
        //    //if (user.AccessFailedCount != null && code.Equals(_memoryCache.Get(phone)))
        //    //{
        //    //    context.Result = new GrantValidationResult(
        //    //        subject: user.Id.ToString(),
        //    //        authenticationMethod: AuthenticationMethods.ConfirmationByTelephone,
        //    //        claims: new List<Claim>());
        //    //}
        //    //else
        //    //{
        //    //    context.Result = new GrantValidationResult(
        //    //        error: TokenRequestErrors.UnauthorizedClient,
        //    //        errorDescription: "验证码不正确");
        //    //}
        //}
    }
}

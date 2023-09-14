using Glendale.Design.Dtos.OrganizationUnit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Auditing;
using Volo.Abp.Identity;
using Volo.Abp.Validation;

namespace Glendale.Design.Dtos.User
{
    public class SystemChangePasswordInput : ChangePasswordInput
    {
        /// <summary>
        /// 短信编码
        /// </summary>
        public string Code { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Threading;

namespace Glendale.Design
{
    public static class DesignDtoExtensions
    {
        private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

        public static void Configure()
        {
            OneTimeRunner.Run(() =>
            {
                /* You can add extension properties to DTOs
                 * defined in the depended modules.
                 *
                 * Example:
                 *
                 * ObjectExtensionManager.Instance
                 *   .AddOrUpdateProperty<IdentityRoleDto, string>("Title");
                 *
                 * See the documentation for more:
                 * https://docs.abp.io/en/abp/latest/Object-Extensions
                 */
                ObjectExtensionManager.Instance
                    .AddOrUpdateProperty<DateTime?>(new[] { typeof(IdentityUserDto), typeof(IdentityUserCreateDto), typeof(IdentityUserUpdateDto), typeof(ProfileDto), typeof(UpdateProfileDto), }, "ValidityDate")
                    .AddOrUpdateProperty<string>(new[] { typeof(IdentityUserDto), typeof(IdentityUserCreateDto), typeof(IdentityUserUpdateDto), typeof(ProfileDto), typeof(UpdateProfileDto), }, "Position")
                    .AddOrUpdateProperty<List<Guid>>(new[] { typeof(IdentityUserDto), typeof(IdentityUserCreateDto), typeof(IdentityUserUpdateDto), typeof(ProfileDto), }, "OrgIds")
                    .AddOrUpdateProperty<string>(new[] { typeof(IdentityUserDto), typeof(IdentityUserCreateDto), typeof(IdentityUserUpdateDto), }, "Describe")
                    .AddOrUpdateProperty<string>(new[] { typeof(IdentityUserDto), typeof(IdentityUserCreateDto), typeof(IdentityUserUpdateDto), }, "CorpName")
                    .AddOrUpdateProperty<string>(new[] { typeof(IdentityRoleDto), typeof(IdentityRoleCreateDto), typeof(IdentityRoleUpdateDto), }, "Describe")
                    .AddOrUpdateProperty<string>(new[] { typeof(IdentityRoleDto), typeof(IdentityRoleCreateDto), typeof(IdentityRoleUpdateDto), }, "RoleType");
            });
        }
    }
}
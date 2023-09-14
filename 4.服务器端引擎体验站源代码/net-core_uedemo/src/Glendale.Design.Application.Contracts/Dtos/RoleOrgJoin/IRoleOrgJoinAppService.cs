using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Glendale.Design.Dtos.RoleOrgJoin
{
    /// <summary>
    /// 组织机构
    /// </summary>
    public interface IRoleOrgJoinAppService :
              ICrudAppService< //定义了CRUD方法
              RoleOrgJoinDto, //用来展示
              RoleOrgJoinListDto,
              Guid, //主键
              GetListRoleOrgJoinInput, //获取的时候用于分页和排序
              RoleOrgJoinCreateUpdateDto, //用于创建
              RoleOrgJoinCreateUpdateDto> //用于更新
    {
    }
}

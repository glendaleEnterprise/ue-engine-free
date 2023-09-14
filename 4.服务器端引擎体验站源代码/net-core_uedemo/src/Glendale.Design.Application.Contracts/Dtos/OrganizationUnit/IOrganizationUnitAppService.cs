using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Glendale.Design.Dtos.OrganizationUnit
{
    /// <summary>
    /// 组织机构
    /// </summary>
    public interface IOrganizationUnitAppService :
              ICrudAppService< //定义了CRUD方法
              OrganizationUnitDto, //用来展示
              OrganizationUnitListDto,
              Guid, //主键
              GetListOrganizationUnitInput, //获取的时候用于分页和排序
              OrganizationUnitCreateDto, //用于创建
              OrganizationUnitUpdateDto> //用于更新
    {
        Task<string> GetNextChildCodeAsync();
        Task<string> GetNextChildCodeAsync(Guid parentId);
        Task<ListResultDto<OrganizationUnitDto>> GetListByUseridAsync(Guid userId);
    }
}

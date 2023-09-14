using Glendale.Design.Dtos.ViewPoint;
using Glendale.Design.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;

namespace Glendale.Design.Services
{
    /// <summary>
    /// 视点设置
    /// </summary>
    [Authorize]
    public class ViewPointService : CrudAppService<ViewPoint, ViewPointDto, ViewPointListDto, Guid, GetListViewPointInput,ViewPointCreateUpdateDto, ViewPointCreateUpdateDto>, IViewPointService
    {
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }
        public ViewPointService(IRepository<ViewPoint, Guid> repository)
            : base(repository)
        {
        }

        /// <summary>
        /// 创建视点
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<ViewPointDto> CreateAsync(ViewPointCreateUpdateDto input)
        {
            await CheckCreatePolicyAsync();
            var entity = ObjectMapper.Map<ViewPointCreateUpdateDto, ViewPoint>(input);
            entity = await Repository.InsertAsync(entity, true);//设置true返回自增id
            return ObjectMapper.Map<ViewPoint, ViewPointDto>(entity);
        }
        public override Task DeleteAsync(Guid id)
        {
            return base.DeleteAsync(id);
        }

        /// <summary>
        /// 视点结构树【带分页】
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public override async Task<PagedResultDto<ViewPointListDto>> GetListAsync(GetListViewPointInput input)
        {
            var Query = await base.CreateFilteredQueryAsync(input);
            Query = Query.WhereIf(!input.ViewName.IsNullOrEmpty(), o => o.ViewName == input.ViewName)
                .WhereIf(input.ModelId != null, o => o.ModelId == input.ModelId)
                .WhereIf(input.ProjectId != null, o => o.ProjectId == input.ProjectId)
                .WhereIf(!input.ViewName.IsNullOrEmpty(), o => o.ViewName.Contains(input.ViewName));                
            var totalCount = await AsyncQueryableExecuter.CountAsync(Query);
            Query = ApplySorting(Query, input);
            Query = ApplyPaging(Query, input);
            var entitys = await AsyncQueryableExecuter.ToListAsync(Query);
            var list = ObjectMapper.Map<IEnumerable<ViewPoint>, IReadOnlyList<ViewPointListDto>>(entitys);
            return new PagedResultDto<ViewPointListDto>(totalCount, list);
        }         

        /// <summary>
        /// 更新视点状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Status"></param>
        /// <param name="OutStatus"></param>
        /// <returns></returns>
        public async Task<int> UpdateStatusAsync(Guid id, int Status, int? OutStatus)
        {
            var cont = 0;
            try
            {
                var entity = await Repository.GetAsync(id);
                var context = await Repository.GetDbContextAsync();
                if (OutStatus.HasValue)
                {
                    cont += await context.Database.ExecuteSqlRawAsync(
                        string.Format("update bim_viewpoint set `Status` = {0} where `ModelId` = '{1}';" +
                        "update bim_viewpoint set `Status` = {2} where `Id` = '{3}';",
                        OutStatus, entity.ModelId, Status, id));
                }
                return cont == 0 ? 1 : cont;
            }
            catch (Exception)
            {
                return cont;
            }
        }
    }
}
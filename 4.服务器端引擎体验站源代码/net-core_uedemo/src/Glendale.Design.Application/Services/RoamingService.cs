using Glendale.Design.Dtos.Roaming;
using Glendale.Design.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;

namespace Glendale.Design.Services
{
    /// <summary>
    /// 漫游功能
    /// </summary>
    [Authorize]
    public class RoamingService : CrudAppService<Roaming, RoamingDto, RoamingListDto, Guid, GetListRoamingInput,RoamingFristCreateDto,RoamingCreateDto>, IRoamingService
    {
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }       
        public RoamingService(IRepository<Roaming, Guid> repository) : base(repository)
        {
            
        }

        /// <summary>
        /// 创建第一人称漫游
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<RoamingDto> CreateAsync(RoamingFristCreateDto input)
        {             
            await CheckCreatePolicyAsync();
            var entity = await MapToEntityAsync(input);
            entity.RoamingType = Enums.RoamingTypeEnum.Frist;
            entity.RoamingPoints = new List<RoamingPoint>();
            int serial = 0;

            input.RoamingPoints.ToList().ForEach(point =>
            {
                entity.RoamingPoints.Add(new RoamingPoint(GuidGenerator.Create(), entity.Id, point.ToJson(), serial++));
            });
            SetIdForGuids(entity);
            entity = await Repository.InsertAsync(entity);
            return await MapToGetOutputDtoAsync(entity);
        }

        /// <summary>
        /// 创建自定义视点漫游
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<RoamingDto> CreateViewPort(RoamingViewPortCreateDto input)
        {
            await CheckCreatePolicyAsync();
            var entity = ObjectMapper.Map<RoamingViewPortCreateDto, Roaming>(input);
            entity.RoamingType = Enums.RoamingTypeEnum.ViewPortRoam;
            entity.RoamingPoints = new List<RoamingPoint>();
            int serial = 0;
            input.ViewPortPoints.ToList().ForEach(point =>
            {
                entity.RoamingPoints.Add(new RoamingPoint(GuidGenerator.Create(), entity.Id, point.ToJson(), serial++));
            });

            SetIdForGuids(entity);
            entity = await Repository.InsertAsync(entity);
            return await MapToGetOutputDtoAsync(entity);
        }

        /// <summary>
        /// 修改漫游名称和备注
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<RoamingDto> UpdateAsync(Guid id, RoamingCreateDto input)
        {
            var entity =await Repository.GetAsync(id);
            entity.Name = input.Name;
            entity.Remark = input.Remark;
            return MapToGetOutputDto(await Repository.UpdateAsync(entity));            
        }


        /// <summary>
        ///漫游模型查询[带分页]
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public override async Task<PagedResultDto<RoamingListDto>> GetListAsync(GetListRoamingInput input)
        {
            var Query = await base.CreateFilteredQueryAsync(input);
            Query = Query.Include(x=>x.RoamingPoints).Where(o=>o.RoamingType==input.RoamingType)
                .WhereIf(!input.Name.IsNullOrEmpty(), o => o.Name == input.Name)
                .WhereIf(!input.ModelId.IsNullOrEmpty(), o => o.ModelId == input.ModelId)
                .WhereIf(input.ProjectId != null, o => o.ProjectId == input.ProjectId);
            var totalCount = await AsyncQueryableExecuter.CountAsync(Query);
            Query = ApplySorting(Query, input);
            Query = ApplyPaging(Query, input);
            var entitys = await AsyncQueryableExecuter.ToListAsync(Query);
            var list = ObjectMapper.Map<IEnumerable<Roaming>, IReadOnlyList<RoamingListDto>>(entitys);
            return new PagedResultDto<RoamingListDto>(totalCount, list);
        }



        /// <summary>
        /// 漫游模型查询[不带分页]
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[HttpGet("/api/app/roaming/GetALLAsync")]
        //public async Task<List<RoamingListDto>> GetALLAsync(GetListRoamingInput input)
        //{
        //    var Query = await base.CreateFilteredQueryAsync(input);
        //    Query = Query.Include(x => x.RoamingPoints)
        //        .WhereIf(!input.Name.IsNullOrEmpty(), o => o.Name == input.Name)
        //        .WhereIf(!input.ModelId.IsNullOrEmpty(), o => o.ModelId == input.ModelId)
        //        .WhereIf(input.ProjectId != null, o => o.ProjectId == input.ProjectId);
        //    var totalCount = await AsyncQueryableExecuter.CountAsync(Query);
        //    Query = ApplySorting(Query, input);
        //    Query = ApplyPaging(Query, input);
        //    var entitys = await AsyncQueryableExecuter.ToListAsync(Query);
        //    var list = ObjectMapper.Map<IEnumerable<Roaming>, List<RoamingListDto>>(entitys);
        //    return list;
        //}

    }
}
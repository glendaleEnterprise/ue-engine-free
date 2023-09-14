using System;
using System.Linq;
using System.Threading.Tasks;
using Glendale.Design.Dtos.Document;
using Glendale.Design.Dtos.DocumentLog;
using Glendale.Design.Dtos.DocumentVer;
using Glendale.Design.Models;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;
using Glendale.Design.Enums;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.ObjectMapping;
using System.IO;
using Volo.Abp.Identity;
using Volo.Abp;
using Glendale.Design.Dtos;
using Glendale.Design.Dtos.File;
using Microsoft.AspNetCore.Http;
using Glendale.Design.Jobs;
using Microsoft.Extensions.Options;
using Masuit.Tools;
using System.Linq.Dynamic.Core;

namespace Glendale.Design.Services
{
    public class SqlModelTreeGLIDService : CrudAppService<SqlModelTreeGLID, SqlModelTreeGLIDDto, string>, ISqlModelTreeGLIDService
    {
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }
        private readonly ModelDBOptions ModelDBOptions;

        public SqlModelTreeGLIDService(IRepository<SqlModelTreeGLID, string> repository,
            IOptionsSnapshot<ModelDBOptions> _ModelDBOptions
            ) : base(repository)
        {
            ModelDBOptions = _ModelDBOptions.Value;
        }

        /// <summary>
        /// 获取模型楼层所有子id
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="glId">模型id</param>
        /// <returns></returns>
        [HttpGet("/api/app/model-tree/child-id-list")]
        public async Task<List<string>> GetTreeChildId(string modelName, string glId)
        {
            //这里的 Repository 如果是多租户的 model 库，首次调用会根据规则modelname生成多租户DbContext的链接而报错
            var db = await Repository.GetDbSetAsync();
            var list = await db.FromSqlRaw(string.Format(@"with recursive tp1 as(
	select glid as Id, glid, pGlid,externalId from {2}.model_tree_{0} where glid = '{1}'
	union all
	select tb2.glid as Id, tb2.glid, tb2.pGlid, tb2.externalId from {2}.model_tree_{0} tb2, tp1 WHERE tb2.pGlid = tp1.glid
)select * from tp1 where externalId <> '0'", modelName, glId, ModelDBOptions.Database)).IgnoreQueryFilters().ToListAsync();
            return list.Select(s => s.glid).ToList();
        }
    }
}
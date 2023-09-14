using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Glendale.Design.Dtos.DocumentVer;
using Glendale.Design.Models;
using Microsoft.Extensions.DependencyInjection;
using ShardingCore.Core.EntityMetadatas;
using ShardingCore.Core.VirtualRoutes;
using ShardingCore.Core.VirtualRoutes.TableRoutes.Abstractions;

namespace Glendale.Design.VirtualRoutes
{
    public class ModelPropertySpaceVirtualTableRoute : AbstractShardingOperatorVirtualTableRoute<ModelPropertySpace, string>
    {
        private readonly IServiceProvider _serviceProvider;

        public ModelPropertySpaceVirtualTableRoute(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public override void Configure(EntityMetadataTableBuilder<ModelPropertySpace> builder)
        {
            builder.ShardingProperty(o => o.ModelName);
        }

        public override string ShardingKeyToTail(object shardingKey)
        {
            return shardingKey?.ToString() ?? string.Empty;
        }

        public override List<string> GetAllTails()
        {           
            return _serviceProvider.GetService<IDocumentVersionService>()?.GetModelNamesAsync().Result;
        }

        //如何过滤后缀(已经实现了condition on right)用户无需关心条件位置和如何解析条件逻辑判断，也不需要用户考虑and 还是or
        public override Expression<Func<string, bool>> GetRouteToFilter(string shardingKey, ShardingOperatorEnum shardingOperator)
        {
            //因为hash路由仅支持等于所以仅仅只需要写等于的情况
            var t = ShardingKeyToTail(shardingKey);
            switch (shardingOperator)
            {
                case ShardingOperatorEnum.Equal: return tail => tail == t;
                default:
                    {
                        return tail => true;
                    }
            }
        }
    }
}
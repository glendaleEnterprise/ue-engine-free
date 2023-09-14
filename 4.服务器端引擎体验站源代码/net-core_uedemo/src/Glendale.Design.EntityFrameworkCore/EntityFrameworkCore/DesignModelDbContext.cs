using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glendale.Design.Models;
using Microsoft.EntityFrameworkCore;
using ShardingCore.Core.VirtualRoutes.TableRoutes.RouteTails.Abstractions;
using ShardingCore.Sharding.Abstractions;
using Volo.Abp.Data;

namespace Glendale.Design.EntityFrameworkCore
{
    [ConnectionStringName("Model")]
    public class DesignModelDbContext :
        AbstractShardingAbpDbContext<DesignModelDbContext>,
        IShardingTableDbContext
    {
        /* Add DbSet properties for your Aggregate Roots / Entities here. */

        #region 模型属性
        public DbSet<ModelTree> ModelTrees { get; set; }
        public DbSet<ModelType> ModelTypes { get; set; }
        public DbSet<ModelSpace> ModelSpaces { get; set; }

        public DbSet<ModelProperty> ModelPropertys { get; set; }
        public DbSet<ModelPropertySpace> ModelPropertySpaces { get; set; }

        public DbSet<ModelDrawing> ModelDrawings { get; set; }
        public DbSet<ModelDrawingRvtId> ModelDrawingRvtIds { get; set; }
        #endregion

        public IRouteTail RouteTail { get; set; }
        public DesignModelDbContext(DbContextOptions<DesignModelDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region 模型结构表
            //楼层结构
            builder.Entity<ModelTree>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.ModelTree + DesignConsts.DbSchema);
                b.HasIndex(x => new { x.PGlid, x.ModelName });
                // b.HasOne(x => x.Parent).WithMany(x => x.Children).HasForeignKey(x => x.PGlid).HasPrincipalKey(x => x.Glid).OnDelete(DeleteBehavior.Cascade);

            });
            //模型结构
            builder.Entity<ModelType>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.ModelType + DesignConsts.DbSchema);
                b.HasIndex(x => new { x.PGlid, x.ModelName });
                //b.HasOne(x => x.Parent).WithMany(x => x.Children).HasForeignKey(x => x.PGlid).HasPrincipalKey(x => x.Glid).OnDelete(DeleteBehavior.Cascade);
            });
            //模型结构
            builder.Entity<ModelSpace>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.ModelSpace + DesignConsts.DbSchema);
                b.HasIndex(x => new { x.PGlid, x.ModelName });
                //b.HasOne(x => x.Parent).WithMany(x => x.Children).HasForeignKey(x => x.PGlid).HasPrincipalKey(x => x.Glid).OnDelete(DeleteBehavior.Cascade);
            });
            //模型构建属性
            builder.Entity<ModelProperty>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.ModelProperty + DesignConsts.DbSchema);
                b.HasIndex(x => new { x.ExternalId, x.ModelName });
            });
            //模型空间属性
            builder.Entity<ModelPropertySpace>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.ModelPropertySpace + DesignConsts.DbSchema);
                b.HasIndex(x => new { x.ExternalId, x.ModelName });
            });
            //模型图纸表
            builder.Entity<ModelDrawing>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.ModelDrawing + DesignConsts.DbSchema);
                b.HasIndex(x => new { x.Name, x.ModelName });
            });
            builder.Entity<ModelDrawingRvtId>(b =>
            {
                b.ToTable(DesignConsts.DbTableName.ModelDrawingRvtId + DesignConsts.DbSchema);
               // b.HasOne(o => o.ModelDrawing).WithMany(o => o.ModelDrawingRvtIds).HasForeignKey(o => o.DrawGuid);
                b.HasIndex(x => new { x.ModelName });
            });
            #endregion
        }
    }
}

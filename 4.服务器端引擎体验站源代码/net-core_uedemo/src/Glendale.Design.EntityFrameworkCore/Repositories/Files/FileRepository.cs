using System;
using System.Threading.Tasks;
using Glendale.Design.EntityFrameworkCore;
using Glendale.Design.Models;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Glendale.Design.Repositories
{
    public class FileRepository : EfCoreRepository<DesignDbContext, File, Guid>, IFileRepository
    {
        public FileRepository(IDbContextProvider<DesignDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<File> FindByBlobNameAsync(string blobName)
        {
            Check.NotNullOrWhiteSpace(blobName, nameof(blobName));
            var DbSet = await this.GetDbSetAsync();
            return await DbSet.FirstOrDefaultAsync(p => p.BlobName == blobName);
        }
    }
}
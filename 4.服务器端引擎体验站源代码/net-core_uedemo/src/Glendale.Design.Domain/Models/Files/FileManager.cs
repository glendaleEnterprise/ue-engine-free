using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Services;

namespace Glendale.Design.Models
{
    public class FileManager : DomainService, IFileManager
    {
        protected IFileRepository FileRepository { get; }
        protected IBlobContainer BlobContainer { get; }

        public FileManager(IFileRepository fileRepository, IBlobContainer blobContainer)
        {
            FileRepository = fileRepository;
            BlobContainer = blobContainer;
        }

        public virtual async Task<File> FindByBlobNameAsync(string blobName)
        {
            Check.NotNullOrWhiteSpace(blobName, nameof(blobName));

            return await FileRepository.FindByBlobNameAsync(blobName);
        }

        public virtual async Task<File> CreateAsync(string fileName, byte[] bytes)
        {
            var blobName = Guid.NewGuid().ToString("N");
            var file = await FileRepository.InsertAsync(new File(GuidGenerator.Create(), fileName, blobName, bytes.Length));
            await BlobContainer.SaveAsync(blobName, bytes);
            return file;
        }



        public virtual async Task<byte[]> GetBlobAsync(string blobName)
        {
            Check.NotNullOrWhiteSpace(blobName, nameof(blobName));

            return await BlobContainer.GetAllBytesOrNullAsync(blobName);
        }
    }
}
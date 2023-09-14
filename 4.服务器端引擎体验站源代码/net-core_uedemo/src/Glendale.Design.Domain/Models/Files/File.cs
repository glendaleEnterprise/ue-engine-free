using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Glendale.Design.Models
{
    public class File : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 文件名
        /// </summary>
        [NotNull]
        public virtual string FileName { get; protected set; }
        
        /// <summary>
        /// 扩展名
        /// </summary>
        [NotNull]
        [MaxLength(6)]        
        public virtual string Extension { get; protected set; }

        [NotNull]
        public virtual string BlobName { get; protected set; }

        public virtual long ByteSize { get; protected set; }

        protected File() { }

        public File(Guid id, [NotNull] string fileName, [NotNull] string blobName, long byteSize) : base(id)
        {
            FileName = Check.NotNullOrWhiteSpace(fileName, nameof(fileName));
            BlobName = Check.NotNullOrWhiteSpace(blobName, nameof(blobName));
            ByteSize = byteSize;
            Extension = Path.GetExtension(fileName);
        }
    }
}

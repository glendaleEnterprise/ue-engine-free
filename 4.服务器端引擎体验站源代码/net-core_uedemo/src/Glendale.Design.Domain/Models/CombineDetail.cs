using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace Glendale.Design.Models
{
    /// <summary>
    /// 合模明细
    /// </summary>
    public class CombineDetail : FullAuditedEntity<Guid>
    {
        #region 导航属性
        /// <summary>
        /// 合模Id
        /// </summary>
        public virtual Guid CombineId { get; set; }
        /// <summary>
        /// 模型文件Id
        /// </summary>
        public virtual Guid DocumentId { get; set; }
        /// <summary>
        /// 模型矩阵
        /// </summary>
        [MaxLength(500)]
        public virtual string Matrix { get; set; }

        /// <summary>
        /// 模型配置
        /// </summary>
        public virtual string ModelConfig { get; set; }
        /// <summary>
        /// 影像矩阵
        /// </summary>
        [MaxLength(2000)]
        public virtual string MatrixGis { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public virtual int? Status { get; set; }

        #endregion

        #region 导航
        /// <summary>
        /// 合模
        /// </summary>
        [NotMapped]
        public virtual Combine Combine { get; set; }
        /// <summary>
        /// 模型文件
        /// </summary>
        [NotMapped]
        public virtual Document Document { get; set; }

        #endregion

        public CombineDetail()
        {
        }
        public CombineDetail(Guid id, Guid combineId, Guid documentId, string matrix,string matrixGis, string modelConfig) : base(id)
        {
            CombineId = combineId;
            DocumentId = documentId;
            Matrix = matrix;
            MatrixGis = matrixGis;
            ModelConfig = modelConfig;
        }
        public CombineDetail(Guid id,Guid combineId, Guid documentId, string matrix, string matrixGis, string modelConfig, Combine combine, Document document):base(id)
        {
            CombineId = combineId;
            DocumentId = documentId;
            Matrix = matrix;
            MatrixGis = matrixGis;
            ModelConfig = modelConfig;
            Combine = combine;
            Document = document;
        }

    }
}

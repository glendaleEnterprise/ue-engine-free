﻿using Glendale.Design.Dtos.Document;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.Combine
{
    /// <summary>
    /// 合摸细节
    /// </summary>
    public class CreateCombineDetailDto
    {
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

    }
}

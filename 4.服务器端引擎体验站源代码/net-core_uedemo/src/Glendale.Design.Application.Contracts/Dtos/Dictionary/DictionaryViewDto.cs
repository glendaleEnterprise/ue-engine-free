﻿using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.Dictionary
{
    public class DictionaryViewDto : EntityDto<Guid>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// 子数据
        /// </summary>
        public virtual ICollection<DictionaryViewDto> Children { get; set; }
    }
}

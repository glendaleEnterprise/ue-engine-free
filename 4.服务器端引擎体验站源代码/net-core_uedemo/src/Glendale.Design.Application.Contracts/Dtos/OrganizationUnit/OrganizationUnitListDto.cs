using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Glendale.Design.Dtos.OrganizationUnit
{
    public class OrganizationUnitListDto : CreationAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public Guid? ParentId { get; set; }
        public string Code { get; set; }
        public string DisplayName { get; set; }
        public string ConcurrencyStamp { get; set; }
        public int Level => GetLevel(Code);
        public virtual bool IsLeaf { get; set; }
        /// <summary>
        /// 是否系统内置数据
        /// </summary>
        public virtual bool System { get; set; }

        public virtual string Describe { get; set; }

        public List<OrganizationUnitListDto> Children { get; set; } = new List<OrganizationUnitListDto>();

        private int GetLevel(string code)
        {
            return Code.Split('.').Length;
        }

        public void SetLeaf()
        {
            IsLeaf = true;
        }
    }
}

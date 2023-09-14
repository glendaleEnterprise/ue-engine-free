using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.ModelType
{
    /// <summary>
    /// 模型楼层数据
    /// </summary>
    public class GetListModelTypeInput //: PagedAndSortedResultRequestDto
    {
        public virtual string Glid { get; set; }
        public virtual string PGlid { get; set; }
        public int? Level { get; set; }
        public string Name { get; set; }
        public string ExternalId { get; set; }
        public  string GroupName { get; set; }
        public virtual string ModelName { get; set; }
        //[Required]
        //public override int SkipCount { get; set; }
        //[Required]
        //public override int MaxResultCount { get; set; }

        //public GetListModelTypeInput()
        //{
        //    Sorting = "";
        //}
    }
}

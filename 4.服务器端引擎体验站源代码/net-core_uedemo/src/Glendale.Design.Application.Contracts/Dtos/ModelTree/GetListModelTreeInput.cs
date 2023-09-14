using System;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.ModelTree
{
    /// <summary>
    /// 模型楼层数据
    /// </summary>
    public class GetListModelTreeInput //: PagedAndSortedResultRequestDto
    {
        public virtual string Glid { get; set; }
        public virtual string PGlid { get; set; }
        public int? Level { get; set; }
        public string Name { get; set; }
        public string ExternalId { get; set; }
        public  string GroupName { get; set; }

        public string ModelName { get; set; }

        //public GetListModelTreeInput()
        //{
        //    Sorting = "";
        //}
    }
}

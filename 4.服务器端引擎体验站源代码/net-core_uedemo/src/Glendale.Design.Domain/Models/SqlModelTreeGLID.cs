using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Glendale.Design.Models
{
    /// <summary>
    /// modeltree表是通过租户模式创建，调用sql脚本会根据规则创建所有db，若存在modelname没有对应租户时，会报错
    /// 放在业务库中，现有表都存在标识，添加人，添加时间等无关字段，使脚本写起来不那么方便
    /// </summary>
    public class SqlModelTreeGLID : Entity<string>
    {
        public string glid { get; set; }
        public string pglid { get; set; }
        public string externalId { get; set; }
    }
}

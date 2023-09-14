using System;
using System.Collections.Generic;
using System.Text;
using Glendale.Design.Dtos.DocumentLog;
using Glendale.Design.Dtos.DocumentVer;
using Glendale.Design.Enums;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos
{
    public class SqlModelTreeGLIDDto : EntityDto<string>
    {
        public string glid { get; set; }
        public string pglid { get; set; }
        public string externalId { get; set; }
    }
}

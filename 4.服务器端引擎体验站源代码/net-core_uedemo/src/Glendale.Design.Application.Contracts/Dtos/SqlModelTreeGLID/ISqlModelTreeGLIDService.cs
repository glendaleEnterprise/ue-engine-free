using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Glendale.Design.Enums;
using Volo.Abp.Application.Services;

namespace Glendale.Design.Dtos
{
    public interface ISqlModelTreeGLIDService : ICrudAppService< //定义了CRUD方法
            SqlModelTreeGLIDDto, //用来展示
            string> //用于更新
    {
    }
}

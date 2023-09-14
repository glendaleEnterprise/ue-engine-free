using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Glendale.Design.Dtos.File
{
    public interface IFileAppService : IApplicationService
    {
        //Task<FileResult> GetAsync(string blobName);        

        //Task<string> CreateAsync(IFormFile file);
    }
}

using Glendale.Design.Dtos;
using Glendale.Design.Json;
using Glendale.Design.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;

namespace Glendale.Design.Services
{
    public class SystemSettingAppService : DesignAppService
    {
        protected ISettingManager SettingManager { get; }
        protected IConfiguration _configuration;
        protected IHostEnvironment _hostingEnvironment;
        private readonly ILogsService _logsService;


        public SystemSettingAppService(ISettingManager settingManager,IConfiguration configuration,IHostEnvironment hostingEnvironment, ILogsService logsService)
        {
            SettingManager = settingManager;
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _logsService = logsService;
        }


        public virtual async Task<List<SettingValue>> GetAsync()
        {            
            return await SettingManager.GetAllGlobalAsync();
        }        

        public virtual async Task UpdateBatchAsync(Dictionary<string,string> dic)
        {
            foreach (var item in dic)
            {
                await SettingManager.SetGlobalAsync(item.Key, item.Value);
            }            
        }        
    }    
}

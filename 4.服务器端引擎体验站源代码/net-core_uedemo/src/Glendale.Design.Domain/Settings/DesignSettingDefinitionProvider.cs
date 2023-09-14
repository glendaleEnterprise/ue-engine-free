using Glendale.Design.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Settings;

namespace Glendale.Design.Settings
{
    public class DesignSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            //文件上传设置
            context.Add(new SettingDefinition(DesignSettings.AllowedMaxFileSize, DesignSettings.AllowedMaxFileSize, L($"DisplayName:{DesignSettings.AllowedMaxFileSize}"), L($"Description:{DesignSettings.AllowedMaxFileSize}")),
                new SettingDefinition(DesignSettings.AllowedUploadFormats, DesignSettings.AllowedUploadFormats, L($"DisplayName:{DesignSettings.AllowedUploadFormats}"), L($"Description:{DesignSettings.AllowedUploadFormats}")),
                new SettingDefinition(DesignSettings.AllowedModelUploadFormats, DesignSettings.AllowedModelUploadFormats, L($"DisplayName:{DesignSettings.AllowedUploadFormats}"), L($"Description:{DesignSettings.AllowedUploadFormats}")),
                new SettingDefinition(DesignSettings.AllowedCADUploadFormats, DesignSettings.AllowedCADUploadFormats, L($"DisplayName:{DesignSettings.AllowedUploadFormats}"), L($"Description:{DesignSettings.AllowedUploadFormats}"))
            );            
        }
        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<DesignResource>(name);
        }
    }
}

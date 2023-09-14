namespace Glendale.Design.Settings
{
    public static class DesignSettings
    {
        private const string Prefix = "Design";

        /// <summary>
        /// 上传文件大小限制
        /// </summary>
        public const string AllowedMaxFileSize = Prefix + ".File.AllowedMaxFileSize";
        /// <summary>
        /// 上传资料文件类型限制
        /// </summary>
        public const string AllowedUploadFormats = Prefix + ".File.AllowedUploadFormats";

        /// <summary>
        /// 模型文件上传类型限制
        /// </summary>
        public const string AllowedModelUploadFormats = Prefix + ".ModelFile.AllowedUploadFormats";
        /// <summary>
        /// CAD文件上传类型限制
        /// </summary>
        public const string AllowedCADUploadFormats = Prefix + ".CADFile.AllowedUploadFormats";
         
    }
}
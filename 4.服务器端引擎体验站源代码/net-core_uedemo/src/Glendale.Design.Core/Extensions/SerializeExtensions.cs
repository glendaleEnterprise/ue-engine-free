using System;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Glendale.Design.Core.Extensions
{
    public static class SerializeExtensions
    {
        /// <summary>
        /// 实体对象转JSON字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ignoreNull"></param>
        /// <returns></returns>
        public static string ToJson(this object obj, bool ignoreNull = true)
        {
            return JsonSerializer.Serialize(obj, new JsonSerializerOptions { IgnoreNullValues = ignoreNull, PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        /// <summary>
        /// JSON字符串转实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static T FromJson<T>(this string jsonStr)
        {
            return jsonStr.IsNullOrEmpty() ? default : JsonSerializer.Deserialize<T>(jsonStr, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new Json.DynamicJsonConverter() } });
        }

        /// <summary>
        /// 字符串序列化成字节序列
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] SerializeUtf8(this string str)
        {
            return str == null ? null : Encoding.UTF8.GetBytes(str);
        }

        /// <summary>
        /// 字节序列序列化成字符串
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string DeserializeUtf8(this byte[] stream)
        {
            return stream == null ? null : Encoding.UTF8.GetString(stream);
        }

        /// <summary>
        /// 根据key将json文件内容转换为指定对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<T> FromJsonFile<T>(this string filePath, string key = "") where T : new()
        {
            if (!File.Exists(filePath)) return new T();

            using StreamReader reader = new StreamReader(filePath, Encoding.Default);
            var json = await reader.ReadToEndAsync();
            if (string.IsNullOrEmpty(key)) return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new Json.DynamicJsonConverter() } });
            return JsonSerializer.Deserialize<object>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new Json.DynamicJsonConverter() } }) is not JsonDocument obj ? new T() : JsonSerializer.Deserialize<T>(obj.RootElement.GetProperty(key).ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new Json.DynamicJsonConverter() } });
        }
    }
}
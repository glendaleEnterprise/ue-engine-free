using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Glendale.Design.Core
{
    public class HttpHelp
    {
        /// <summary>
        /// 注入http请求
        /// </summary>
        private readonly IHttpClientFactory httpClientFactory;
        public HttpHelp(IHttpClientFactory _httpClientFactory)
        {
            httpClientFactory = _httpClientFactory;
        }

        // <summary>
        // Get请求数据
        // <para>最终以url参数的方式提交</para>
        // </summary>
        // <param name="parameters">参数字典,可为空</param>
        // <param name="requestUri">例如/api/Files/UploadFile</param>
        // <returns></returns>
        public async Task<string> HttpGetAsync(string requestUri, Dictionary<string, string> parameters, string token)
        {
            //从工厂获取请求对象
            using var client = httpClientFactory.CreateClient("IgnoreSSL");

            //拼接地址
            if (parameters != null)
            {
                var strParam = string.Join("&", parameters.Select(o => o.Key + "=" + o.Value));
                requestUri = string.Concat(requestUri, '?', strParam);
            }

            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(requestUri));
            //添加请求头
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Add("Token", token);
            }

            var response = await client.SendAsync(request).ConfigureAwait(false);


            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            return null;
        }

        // <summary>
        // Post请求数据
        // </summary>
        // <param name="parameters">参数字典,可为空</param>
        // <param name="requestUri">例如/api/Files/UploadFile</param>
        // <returns></returns>
        public async Task<string> HttpPostAsync(string requestUri, Dictionary<string, string> parameters, string token)
        {
            //从工厂获取请求对象
            var client = httpClientFactory.CreateClient("IgnoreSSL");

            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(requestUri));
            //添加请求头
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Add("Token", token);
            }
            var httpContent = new StringContent(parameters.ToJson(), Encoding.UTF8);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            
            request.Content = httpContent;

            var response = await client.SendAsync(request).ConfigureAwait(false);
             
             
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            return null;
        }


        // <summary>
        // Post请求FormData数据
        // </summary>
        // <param name="parameters">参数字典,可为空</param>
        // <param name="requestUri">例如/api/Files/UploadFile</param>
        // <returns></returns>
        public async Task<string> HttpPostFormDataAsync(string requestUri, Dictionary<string, string> parameters, string token)
        {
            //从工厂获取请求对象
            var client = httpClientFactory.CreateClient("IgnoreSSL");

            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(requestUri));
            //添加请求头
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Add("Token", token);
            }
            var httpContent = new MultipartFormDataContent();
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    httpContent.Add(new StringContent(parameter.Value, Encoding.UTF8), parameter.Key);
                }
            }

            request.Content = httpContent;

            var response = await client.SendAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            return null;
        }

        // <summary>
        // Post请求数据
        // </summary>
        // <param name="parameters">参数字典,可为空</param>
        // <param name="requestUri">例如/api/Files/UploadFile</param>
        // <returns></returns>
        public async Task<string> UploadFileAsync(string requestUri, string filePath, Dictionary<string, string> parameters, string token)
        {
            //从工厂获取请求对象
            var client = httpClientFactory.CreateClient("IgnoreSSL");

            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(requestUri));
            //添加请求头
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Add("Token", token);
            }
            var httpContent = new MultipartFormDataContent();
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    httpContent.Add(new StringContent(parameter.Value, Encoding.UTF8), parameter.Key);
                }
            }
            string path = Path.Combine(Environment.CurrentDirectory, filePath);
            httpContent.Add(new ByteArrayContent(File.ReadAllBytes(path)), "file", Path.GetFileName(path));
            request.Content = httpContent;
            var response = await client.SendAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                File.Delete(path);
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            return null;
        }
        // <summary>
        // Get请求数据
        // <para>最终以url参数的方式提交</para>
        // </summary>
        // <param name="parameters">参数字典,可为空</param>
        // <param name="requestUri">例如/api/Files/UploadFile</param>
        // <returns></returns>
        public async Task<byte[]> DownloadFileAsync(string requestUri, Dictionary<string, string> parameters, string token)
        {
            //从工厂获取请求对象
            using var client = httpClientFactory.CreateClient("IgnoreSSL");

            //拼接地址
            if (parameters != null)
            {
                var strParam = string.Join("&", parameters.Select(o => o.Key + "=" + o.Value));
                requestUri = string.Concat(requestUri, '?', strParam);
            }

            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(requestUri));
            //添加请求头
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Add("Token", token);
            }

            var response = await client.SendAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            }
            return null;
        }
    }
}

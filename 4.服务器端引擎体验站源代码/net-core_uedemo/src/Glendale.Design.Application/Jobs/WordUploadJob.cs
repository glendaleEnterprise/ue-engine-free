using Glendale.Design.Core;
using Glendale.Design.Dtos.Document;
using Glendale.Design.Dtos.DocumentVer;
using Glendale.Design.Dtos.File;
using Glendale.Design.Dtos.ModelType;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Spire.Doc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.IO;

namespace Glendale.Design.Jobs
{
    public class WordUploadJob : IAsyncBackgroundJob<WordUploadAags>, ITransientDependency
    {
        
        public WordUploadJob()
        {
            
        }

        public async Task ExecuteAsync(WordUploadAags args)
        {
            var dirPath = args.DirPath;
            //实例化一个Document类加载文档
            Spire.Doc.Document original = new Spire.Doc.Document();
            original.LoadFromFile(args.FilePath);

            //实例化Document类对象并添加section
            Spire.Doc.Document newWord = new Spire.Doc.Document();
            Section section = newWord.AddSection();
            //遍历文档所有section
            int index = 0;
            int count = 0;
            foreach (Section sec in original.Sections)
            {
                //遍历文档所有子对象
                foreach (DocumentObject obj in sec.Body.ChildObjects)
                {
                    section.Body.ChildObjects.Add(obj.Clone());
                    count++;
                    if (count % 5 == 0)
                    {
                        var path = String.Format("{0}/page/{1}.docx", dirPath, index);
                        //保存新文档到文件夹
                        newWord.SaveToFile(path, FileFormat.Docx);

                        FileInfo fileInfo = new FileInfo(path);
                        var size = System.Math.Ceiling(fileInfo.Length / 1024.0 / 1024.0);
                        if (size > 10)//大于10M 新建对象
                        {
                            index++;
                            //实例化Document类对象&#xff0c;添加section&#xff0c;将原文档段落的子对象复制到新文档
                            newWord = new Spire.Doc.Document();
                            section = newWord.AddSection();
                        }
                    }

                }
            }
            //拆分后的新文档保存至指定文档
            newWord.SaveToFile(String.Format("{0}/page/{1}.docx", dirPath, index), FileFormat.Docx);
            await Task.CompletedTask;
        }
    }
}

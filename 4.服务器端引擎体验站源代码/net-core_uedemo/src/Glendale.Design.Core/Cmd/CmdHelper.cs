using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace Glendale.Design.Cmd
{
    public class CmdHelper
    {       
        #region   基础参数

        //异步执行信息
        private static string _strInfo = null;

        #endregion

        /// <summary>
        /// 运行cmd命令（会显示命令窗口）
        /// </summary>
        /// <param name="cmdExe">指定应用程序的完整路径(比如：D:\GetHostInfo.exe)</param>
        /// <param name="cmdStr">执行命令行参数（比如：dir、ipconfig、ping 等命令）</param>
        /// <returns>返回结果内容</returns>
        public static string ShowRunCmdCommand(string cmdExe, string cmdStr)
        {
            string result = null;
            try
            {
                using (Process myPro = new Process())
                {
                    //指定启动进程是调用的应用程序和命令行参数
                    ProcessStartInfo psi = new ProcessStartInfo(cmdExe, cmdStr);
                    myPro.StartInfo = psi;
                    myPro.Start();
                    myPro.WaitForExit();
                    myPro.Close();
                    result = $"执行：{cmdStr} 命令成功";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 运行cmd命令（不显示命令窗口）
        /// </summary>
        /// <param name="cmdExe">指定应用程序的完整路径(比如：D:\GetHostInfo.exe)</param>
        /// <param name="cmdStr">执行命令行参数（比如：dir、ipconfig、ping 等命令）</param>
        /// <returns>返回结果内容</returns>
        public static string HideRunCmdCommand(string cmdExe, string cmdStr)
        {
            string result = null;
            try
            {
                using (Process myPro = new Process())
                {
                    myPro.StartInfo.FileName = "cmd.exe";
                    myPro.StartInfo.UseShellExecute = false;
                    myPro.StartInfo.RedirectStandardInput = true;
                    myPro.StartInfo.RedirectStandardOutput = true;
                    myPro.StartInfo.RedirectStandardError = true;
                    myPro.StartInfo.CreateNoWindow = true;
                    myPro.Start();
                    //如果调用程序路径中有空格时，cmd命令执行失败，可以用双引号括起来 ，在这里两个引号表示一个引号（转义）
                    string str = string.Format(@"""{0}"" {1} {2}", cmdExe, cmdStr, "&exit");

                    myPro.StandardInput.WriteLine(str);
                    myPro.StandardInput.AutoFlush = true;
                    myPro.StandardInput.WriteLine("exit");
                    result = myPro.StandardOutput.ReadToEnd();
                    myPro.WaitForExit();


                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 运行批处理文件（不显示命令窗口）
        /// </summary>
        /// <param name="cmdStr">执行命令（比如：dir、ipconfig、ping 等命令）</param>
        /// <returns>返回结果内容</returns>
        public static string HideRunCmdCommand(string cmdStr)
        {
            string result = null;
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
                p.StartInfo.CreateNoWindow = false;//不显示程序窗口
                p.Start();//启动程序
                //向cmd窗口发送输入信息
                p.StandardInput.WriteLine(cmdStr + "&exit");
                p.StandardInput.AutoFlush = true;
                p.StandardInput.WriteLine("exit");
                //向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
                //同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令
                //获取cmd窗口的输出信息
                result = p.StandardOutput.ReadToEnd();
                //StreamReader reader = p.StandardOutput;
                //string line=reader.ReadLine();
                //while (!reader.EndOfStream)
                //{
                //    str += line + "  ";
                //    line = reader.ReadLine();
                //}
                p.WaitForExit();//等待程序执行完退出进程
                p.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 异步执行执行CMD命令
        /// </summary>
        /// <param name="cmdStr">执行命令参数（比如：dir、ipconfig、ping 等命令）</param>
        public static void HideAsyncRunCmdCommand(string cmdStr)
        {
            _strInfo = null;
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = "cmd";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.OutputDataReceived += new DataReceivedEventHandler(HideRunCmdCommand2_OutputDataReceived);
                p.Start();
                StreamWriter cmdWriter = p.StandardInput;
                p.BeginOutputReadLine();
                if (!String.IsNullOrEmpty(cmdStr))
                {
                    cmdWriter.WriteLine(cmdStr);
                }
                cmdWriter.Close();

                p.WaitForExit();
                p.Close();


            }
            catch (Exception ex)
            {
            }

        }

        //接收输出信息
        private static void HideRunCmdCommand2_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            _strInfo += e.Data + "\r\n";

        }

        //获取到异步执行CMD命令的信息
        public static string GetAsyncRunCmdCommandInfo()
        {
            return _strInfo;
        }


        /// <summary>
        /// 执行bat(批处理)文件
        /// </summary>
        /// <param name="filePathAndName">指定应用程序的完整路径(比如：D:\GetHostInfo.bat)</param>
        /// <param name="isShowCMDWindow">是否显示执行的命令行窗体(注意：false时是隐式执行，需要在bat文件中添加退出命令exit)</param>
        /// <returns>返回执行结果（true:表示成功）</returns>
        public static bool RunBatFile(string filePathAndName, bool isShowCMDWindow)
        {
            bool success = false;
            try
            {
                Process pro = new Process();
                FileInfo file = new FileInfo(filePathAndName);
                pro.StartInfo.WorkingDirectory = file.Directory.FullName;
                pro.StartInfo.FileName = filePathAndName;
                if (isShowCMDWindow)
                {
                    pro.StartInfo.UseShellExecute = true;
                    pro.StartInfo.CreateNoWindow = false;
                }
                else
                {
                    pro.StartInfo.UseShellExecute = false;
                    pro.StartInfo.CreateNoWindow = true;
                }

                pro.Start();
                pro.WaitForExit();
                success = true;

            }
            catch (Exception ex)
            {
            }

            return success;
        }

    }
}


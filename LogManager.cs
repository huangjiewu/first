using System;
using System.IO;
using System.Text;

/// <summary>
/// LogManager 的摘要说明
/// </summary>
public class LogManager
{

    private static string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format(@"Log\{0}\", DateTime.Now.ToString("yyyyMM")));
    private static string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format(@"config.txt"));
    ///<summary>

    /// 保存日志的文件夹

    ///</summary>

    public static string LogPath
    {
        get
        {
            if (!Directory.Exists(logPath))
            {

                    Directory.CreateDirectory(logPath);
                    

            }
            return logPath;
        }
        set { logPath = value; }
    }

    ///<summary>
    /// 写日志
    ///</summary>
    public static void WriteLog(string logFile, string msg)
    {
        try
        {
            System.IO.StreamWriter sw = System.IO.File.AppendText(
                LogPath + logFile + " " +
                DateTime.Now.ToString("yyyyMMdd") + ".Log"
                );
            sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss: ") + msg);
            sw.Close();
        }
        catch
        { }
    }

    ///<summary>
    /// 读配置文件
    ///</summary>
    public static string ReadTXT()
    {
        StreamReader sr = new StreamReader(configPath, Encoding.Default);
        String line;
        string txt="";
        while ((line = sr.ReadLine()) != null)
        {
            txt += line.ToString();
        }
        sr.Close();
        return txt;
    }
    ///<summary>
    /// 写配置文件
    ///</summary>
    public static void writeTXT(string msg)
    {
        System.IO.File.WriteAllText(configPath, msg);
    }
    ///<summary>
    /// 写日志
    ///</summary>
    public static void WriteLog(LogFile logFile, string msg)
    {
        WriteLog(logFile.ToString(), msg);
    }
}

/// <summary>
/// 日志类型
/// </summary>
public enum LogFile
{
    Info,
    Warning,
    Error
}
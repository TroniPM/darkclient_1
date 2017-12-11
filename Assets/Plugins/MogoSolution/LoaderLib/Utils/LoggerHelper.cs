#region ģ����Ϣ
/*----------------------------------------------------------------
// Copyright (C) 2013 ���ݣ�����
//
// ģ������LoggerHelper
// �����ߣ�Ash Tang
// �޸����б�
// �������ڣ�2013.1.17
// ģ����������־����ࡣ
//----------------------------------------------------------------*/
#endregion

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;

namespace Mogo.Util
{
    /// <summary>
    /// ��־�ȼ�������
    /// </summary>
    [Flags]
    public enum LogLevel
    {
        /// <summary>
        /// ȱʡ
        /// </summary>
        NONE = 0,
        /// <summary>
        /// ����
        /// </summary>
        DEBUG = 1,
        /// <summary>
        /// ��Ϣ
        /// </summary>
        INFO = 2,
        /// <summary>
        /// ����
        /// </summary>
        WARNING = 4,
        /// <summary>
        /// ����
        /// </summary>
        ERROR = 8,
        /// <summary>
        /// �쳣
        /// </summary>
        EXCEPT = 16,
        /// <summary>
        /// �ؼ�����
        /// </summary>
        CRITICAL = 32,
    }

    /// <summary>
    /// ��־�����ࡣ
    /// </summary>
    /// 
    public class LoggerHelper
    {
        /// <summary>
        /// ��ǰ��־��¼�ȼ���
        /// </summary>
        public static LogLevel CurrentLogLevels = LogLevel.DEBUG | LogLevel.INFO | LogLevel.WARNING | LogLevel.ERROR | LogLevel.CRITICAL | LogLevel.EXCEPT;
        private const Boolean SHOW_STACK = true;
        private static LogWriter m_logWriter;
        public static string DebugFilterStr = string.Empty;

        static LoggerHelper()
        {
            m_logWriter = new LogWriter();
            Application.RegisterLogCallback(new Application.LogCallback(ProcessExceptionReport));
        }

        public static void Release()
        {
            m_logWriter.Release();
        }

        public static void UploadLogFile()
        {
            m_logWriter.UploadTodayLog();
        }

        static ulong index = 0;

        /// <summary>
        /// ������־��
        /// </summary>
        /// <param name="message">��־����</param>
        /// <param name="isShowStack">�Ƿ���ʾ����ջ��Ϣ</param>
        public static void Debug(object message, Boolean isShowStack = SHOW_STACK, int user = 0)
        {
            //if (user != 11)
            //    return;
            if (DebugFilterStr != "") return;

            if (LogLevel.DEBUG == (CurrentLogLevels & LogLevel.DEBUG))
                Log(string.Concat(" [DEBUG]: ", isShowStack ? GetStackInfo() : "", message, " Index = ", index++), LogLevel.DEBUG);
        }

        /// <summary>
        /// ��չdebug
        /// </summary>
        /// <param name="message"></param>
        /// <param name="filter">ֻ�������DebugMsg->filter�������õ�filterһ����debug</param>
        public static void Debug(string filter, object message, Boolean isShowStack = SHOW_STACK)
        {

            if (DebugFilterStr != "" && DebugFilterStr != filter) return;
            if (LogLevel.DEBUG == (CurrentLogLevels & LogLevel.DEBUG))
            {
                Log(string.Concat(" [DEBUG]: ", isShowStack ? GetStackInfo() : "", message), LogLevel.DEBUG);
            }

        }

        /// <summary>
        /// ��Ϣ��־��
        /// </summary>
        /// <param name="message">��־����</param>
        public static void Info(object message, Boolean isShowStack = SHOW_STACK)
        {
            if (LogLevel.INFO == (CurrentLogLevels & LogLevel.INFO))
                Log(string.Concat(" [INFO]: ", isShowStack ? GetStackInfo() : "", message), LogLevel.INFO);
        }

        /// <summary>
        /// ������־��
        /// </summary>
        /// <param name="message">��־����</param>
        public static void Warning(object message, Boolean isShowStack = SHOW_STACK)
        {
            if (LogLevel.WARNING == (CurrentLogLevels & LogLevel.WARNING))
                Log(string.Concat(" [WARNING]: ", isShowStack ? GetStackInfo() : "", message), LogLevel.WARNING);
        }

        /// <summary>
        /// �쳣��־��
        /// </summary>
        /// <param name="message">��־����</param>
        public static void Error(object message, Boolean isShowStack = SHOW_STACK)
        {
            if (LogLevel.ERROR == (CurrentLogLevels & LogLevel.ERROR))
                Log(string.Concat(" [ERROR]: ", message, '\n', isShowStack ? GetStacksInfo() : ""), LogLevel.ERROR);
        }

        /// <summary>
        /// �ؼ���־��
        /// </summary>
        /// <param name="message">��־����</param>
        public static void Critical(object message, Boolean isShowStack = SHOW_STACK)
        {
            if (LogLevel.CRITICAL == (CurrentLogLevels & LogLevel.CRITICAL))
                Log(string.Concat(" [CRITICAL]: ", message, '\n', isShowStack ? GetStacksInfo() : ""), LogLevel.CRITICAL);
        }

        /// <summary>
        /// �쳣��־��
        /// </summary>
        /// <param name="ex">�쳣ʵ����</param>
        public static void Except(Exception ex, object message = null)
        {
            if (LogLevel.EXCEPT == (CurrentLogLevels & LogLevel.EXCEPT))
            {
                Exception innerException = ex;
                while (innerException.InnerException != null)
                {
                    innerException = innerException.InnerException;
                }
                Log(string.Concat(" [EXCEPT]: ", message == null ? "" : message + "\n", ex.Message, innerException.StackTrace), LogLevel.CRITICAL);
            }
        }

        /// <summary>
        /// ��ȡ��ջ��Ϣ��
        /// </summary>
        /// <returns></returns>
        private static String GetStacksInfo()
        {
            StringBuilder sb = new StringBuilder();
            StackTrace st = new StackTrace();
            var sf = st.GetFrames();
            for (int i = 2; i < sf.Length; i++)
            {
                sb.AppendLine(sf[i].ToString());
            }

            return sb.ToString();
        }

        /// <summary>
        /// д��־��
        /// </summary>
        /// <param name="message">��־����</param>
        private static void Log(string message, LogLevel level, bool writeEditorLog = true)
        {
            var msg = string.Concat(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff"), message);
            m_logWriter.WriteLog(msg, level, writeEditorLog);
            //Debugger.Log(0, "TestRPC", message);
        }

        /// <summary>
        /// ��ȡ����ջ��Ϣ��
        /// </summary>
        /// <returns>����ջ��Ϣ</returns>
        private static String GetStackInfo()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(2);//[0]Ϊ����ķ��� [1]Ϊ���÷���
            var method = sf.GetMethod();
            return String.Format("{0}.{1}(): ", method.ReflectedType.Name, method.Name);
        }

        private static void ProcessExceptionReport(string message, string stackTrace, LogType type)
        {
            var logLevel = LogLevel.DEBUG;
            switch (type)
            {
                case LogType.Assert:
                    logLevel = LogLevel.DEBUG;
                    break;
                case LogType.Error:
                    logLevel = LogLevel.ERROR;
                    break;
                case LogType.Exception:
                    logLevel = LogLevel.EXCEPT;
                    break;
                case LogType.Log:
                    logLevel = LogLevel.DEBUG;
                    break;
                case LogType.Warning:
                    logLevel = LogLevel.WARNING;
                    break;
                default:
                    break;
            }

            if (logLevel == (CurrentLogLevels & logLevel))
                Log(string.Concat(" [SYS_", logLevel, "]: ", message, '\n', stackTrace), logLevel, false);
        }
    }

    /// <summary>
    /// ��־д���ļ������ࡣ
    /// </summary>
    public class LogWriter
    {
        private string m_logPath = UnityEngine.Application.persistentDataPath + "/log/";
        private string m_logFileName = "log_{0}.txt";
        private string m_logFilePath;
        private FileStream m_fs;
        private StreamWriter m_sw;
        private Action<String, LogLevel, bool> m_logWriter;
        private readonly static object m_locker = new object();

        /// <summary>
        /// Ĭ�Ϲ��캯����
        /// </summary>
        public LogWriter()
        {
            if (!Directory.Exists(m_logPath))
                Directory.CreateDirectory(m_logPath);
            m_logFilePath = String.Concat(m_logPath, String.Format(m_logFileName, DateTime.Today.ToString("yyyyMMdd")));
            try
            {
                m_logWriter = Write;
                m_fs = new FileStream(m_logFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                m_sw = new StreamWriter(m_fs);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError(ex.Message);
            }
        }

        /// <summary>
        /// �ͷ���Դ��
        /// </summary>
        public void Release()
        {
            lock (m_locker)
            {
                if (m_sw != null)
                {
                    m_sw.Close();
                    m_sw.Dispose();
                }
                if (m_fs != null)
                {
                    m_fs.Close();
                    m_fs.Dispose();
                }
            }
        }

        public void UploadTodayLog()
        {
            //lock (m_locker)
            //{
            //    using (var fs = new FileStream(m_logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            //    {
            //        using (StreamReader sr = new StreamReader(fs))
            //        {
            //            var content = sr.ReadToEnd();
            //            var fn = Utils.GetFileName(m_logFilePath);//.Replace('/', '\\')
            //            if (MogoWorld.theAccount != null)
            //            {
            //                fn = string.Concat(MogoWorld.theAccount.name, "_", fn);
            //            }
            //            DownloadMgr.Instance.UploadLogFile(fn, content);
            //        }
            //    }
            //}
        }

        /// <summary>
        /// д��־��
        /// </summary>
        /// <param name="msg">��־����</param>
        public void WriteLog(string msg, LogLevel level, bool writeEditorLog)
        {
#if UNITY_IPHONE
            m_logWriter(msg, level, writeEditorLog);
#else
            m_logWriter.BeginInvoke(msg, level, writeEditorLog, null, null);
#endif
        }

        private void Write(string msg, LogLevel level, bool writeEditorLog)
        {
            lock (m_locker)
                try
                {
                    if (writeEditorLog)
                    {
                        switch (level)
                        {
                            case LogLevel.DEBUG:
                            case LogLevel.INFO:
                                UnityEngine.Debug.Log(msg);
                                break;
                            case LogLevel.WARNING:
                                UnityEngine.Debug.LogWarning(msg);
                                break;
                            case LogLevel.ERROR:
                            case LogLevel.EXCEPT:
                            case LogLevel.CRITICAL:
                                UnityEngine.Debug.LogError(msg);
                                break;
                            default:
                                break;
                        }
                    }
                    if (m_sw != null)
                    {
                        m_sw.WriteLine(msg);
                        m_sw.Flush();
                    }
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError(ex.Message);
                }
        }
    }
}
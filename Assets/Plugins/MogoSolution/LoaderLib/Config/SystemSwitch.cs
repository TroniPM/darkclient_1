using Mogo.Util;
using System;
using System.IO;

public class SystemSwitch
{
    private static Boolean m_releaseMode = true;
    private static Boolean m_destroyResource = false;
    private static Boolean m_useFileSystem = true;
    private static Boolean m_useHmf = true;
    private static Boolean m_destroyAllUI = false;
    private static Boolean m_usePlatformSDK = true;

    /// <summary>
    /// 是否为发布模式。
    /// </summary>
    public static Boolean ReleaseMode
    {
        get { return m_releaseMode; }
        private set { m_releaseMode = value; }
    }

    public static Boolean DestroyResource
    {
        get { return m_destroyResource; }
        private set { m_destroyResource = value; }
    }

    public static Boolean UseFileSystem
    {
        get { return m_useFileSystem; }
        private set { m_useFileSystem = value; }
    }

    public static Boolean UseHmf
    {
        get { return m_useHmf; }
        private set { m_useHmf = value; }
    }

    /// <summary>
    /// 控制是否消耗UI资源。（使用MFUIResourceManager框架的GameObject由框架控制，原有UI会受此变量控制是否Destroy GameObject）
    /// </summary>
    public static Boolean DestroyAllUI
    {
        get { return m_destroyAllUI; }
        private set { m_destroyAllUI = value; }
    }

    public static Boolean UsePlatformSDK
    {
        get { return m_usePlatformSDK; }
        private set { m_usePlatformSDK = value; }
    }
    //public readonly static Boolean ReleaseMode = false;
    //public readonly static Boolean DestroyResource = false;
    //public readonly static Boolean UseFileSystem = false;
    //public readonly static Boolean UseHmf = false;
    ////public readonly static Boolean UseStreamingAssets = false;
    //public readonly static Boolean DestroyAllUI = false;
    //public readonly static Boolean UsePlatformSDK = true;

    public static void InitSystemSwitch()
    {
        String content;
        if (File.Exists(SystemConfig.SystemSwitchPath))
            content = Utils.LoadFile(SystemConfig.SystemSwitchPath);
        else
            content = Utils.LoadResource(Utils.GetFileNameWithoutExtention(SystemConfig.SystemSwitchPath));

        if (!String.IsNullOrEmpty(content))
        {
            try
            {
                //LoggerHelper.Debug(content);
                var xml = XMLParser.LoadXML(content);
                var props = typeof(SystemSwitch).GetProperties();
                foreach (System.Security.SecurityElement item in xml.Children)
                {
                    //LoggerHelper.Debug(item.Tag + " " + item.Text);
                    foreach (var prop in props)
                    {
                        if (item.Tag == prop.Name)
                        {
                            //LoggerHelper.Debug(" prop.Name " + prop.Name);
                            prop.SetValue(null, Utils.GetValue(item.Text, typeof(bool)), null);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Except(ex);
            }
        }
    }
}
#if !UNITY_IPHONE
using Mogo.Util;
using UnityEngine;

namespace Assets.Plugins.Init
{
    public class PlatformUpdateCallback : MonoBehaviour
    {
        //没有新版本时
        public void OnNotNewVersion(string s)
        {
            LoggerHelper.Debug("---------------NotNewVersion-----------------");
            gameObject.GetComponent<Driver>().CheckVersionFinish();
        }
        //没有sd卡时
        public void OnNotSDCard(string s)
        {
            LoggerHelper.Error("--------------NotSDCard-----------------");
        }
        //取消普通更新
        public void OnCancelNormalUpdate(string s)
        {
            Application.Quit();
        }
        //核对版本失败
        public void OnCheckVersionFailure(string s)
        {
            
        }
        //强制更新中
        public void OnForceUpdateLoading(string s)
        {
            
        }
        //普通更新中
        public void OnNormalUpdateLoading(string s)
        {
            
        }
        //网络错误
        public void OnNetWorkError(string s)
        {
            
        }
        //更新异常
        public void OnUpdateException(string s)
        {
            
        }
        //取消强制更新
        public void OnCancelForceUpdate(string s)
        {
            Application.Quit();
        }
    }
}
#endif
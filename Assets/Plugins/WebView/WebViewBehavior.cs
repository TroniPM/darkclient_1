using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Kogarasi.WebView;

public class WebViewBehavior : MonoBehaviour
{

    IWebView webView;
    IWebViewCallback callback;

    #region Method

    public void Awake()
    {

        if (Application.platform == RuntimePlatform.Android)
        {
#if UNITY_ANDROID
            webView = new WebViewAndroid();
#endif
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
#if UNITY_IPHONE
            webView = new WebViewIOS();
#endif
        }
        else
        {
            webView = new WebViewNull();

        }


        webView.Init(name);

        callback = null;
    }

    public void Close()
    {
        webView.Term();
    }

    public void SetMargins(int left, int top, int right, int bottom)
    {
        webView.SetMargins(left, top, right, bottom);
    }
    public void SetVisibility(bool state)
    {
        webView.SetVisibility(state);
    }

    public void LoadURL(string url)
    {
        webView.LoadURL(url);
    }

    public void EvaluateJS(string js)
    {
        webView.EvaluateJS(js);
    }

    /*
    public void CallFromJS( string message )
    {
        Debug.Log( "CallFromJS : " + message );
    }
    */

    public void setCallback(IWebViewCallback _callback)
    {
        callback = _callback;
    }

    public void onLoadStart(string url)
    {
        if (callback != null)
        {
            callback.onLoadStart(url);
        }
    }

    public void onLoadFinish(string url)
    {
        if (callback != null)
        {
            callback.onLoadFinish(url);
        }
    }

    public void onLoadFail(string url)
    {
        if (callback != null)
        {
            callback.onLoadFail(url);
        }
    }

    #endregion

}

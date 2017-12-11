using UnityEngine;
using System.Collections;
using Mogo.Util;

#if UNITY_IPHONE
public class IOSSDKManager : PlatformSdkManager
{
	virtual public int RecommendedServerID
	{
		get
		{
			return 0;
		}
	}

	void Start()
	{
		Instance = this;
		IOSUtils.InitSSJJEngine();
	}
	
	override public void Login()
	{
		SendBeforeLoginLog();
		IOSUtils.LoginSSJJEngine();
	}
	
	public void Charge(int amount)
	{

	}
	
	override public void GotoGameCenter()
	{

	}
	
	override public void CheckUpdate()
	{
		IOSUtils.UpdateSSJJEngine();
	}
	
	override public void CleanLocalData()
	{
		IOSUtils.ClearLocalAcount();
	}
	
	public override void RoleLevelLog(string roleName, string serverId)
	{
		IOSUtils.RoleLevelLog(roleName.ToCharArray(),serverId.ToCharArray());
	}
	
	public override void CreateRoleLog(string roleName, string serverId)
	{
		IOSUtils.CreateRoleLog(roleName.ToCharArray(),serverId.ToCharArray());
	}

	override public void SendLoginLog()
	{
		IOSUtils.OpenLogBeforeLogin();
	}
	
	override public void SendStartGameLog()
	{
		IOSUtils.OpenSSJJEngineAppLog();
	}
	
	override public void SendBeforeLoginLog()
	{
		IOSUtils.OpenLogBeforeLogin();
	}
	
	override public void OpenForum()
	{

	}
	
	override public void OnSwitchAccount()
	{
		CleanLocalData();
		Login();
	}
	
	public override void RestartGame()
	{

	}
	
	public override void Shake(long milliseconds)
	{

	}
	public override void StopShake()
	{

	}
	
	public override void SetNetwork()
	{

	}

	public void OnCancel()
	{
		
	}
	
	public void OnError()
	{
		
	}
	
	public void OnComplete()
	{

	}
	
	private void OnLoginDoneUnion()
	{

	}
	
	private void OnLoginDoneSSJJ()
	{
        if (LoginCallBack!=null)
        {
            LoginCallBack();
        }
	}
	
	public void OnException()
	{
		
	}

	public override void SetupNotificationData()
	{

	}
	
	override public void AddNotificationRecord(int tag)
	{

	}

	override public void OnSetupNotification()
	{

	}

	public override void ShowAssistant()
	{

	}

	public void OnStart()
	{
		LoggerHelper.Debug("OnStart");
	}
	
	public void OnPause()
	{
		LoggerHelper.Debug("OnPause");
	}
	
	public void OnStop()
	{
		LoggerHelper.Debug("OnStop");
	}
	
	public void OnRestart()
	{
		LoggerHelper.Debug("OnRestart");
	}
	
	public void OnDestory()
	{
		LoggerHelper.Debug("OnDestory");
	}
	
	public void OnResume()
	{
		LoggerHelper.Debug("OnResume");
	}

}
#endif
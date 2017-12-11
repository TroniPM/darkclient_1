using UnityEngine;
using System.Collections;
using System;
using Mogo.Util;
using System.Threading;
using Mogo.GameData;
using System.Collections.Generic;

public class GlobleLoadingUIData
{
    public string tip;
    public string logoImg;
    public int status;
    public string animImg;
}

public class PassRewardGridData
{
    public int id;
    public int num = 0;
    public string imgName = "";
    public string iconName = "";
}

public enum ButtonBgType
{
    Green,
    Blue,
    Yellow,
    Brown,
}

public class MogoGlobleUIManager : MonoBehaviour
{
    private static MogoGlobleUIManager m_instance;
    public static MogoGlobleUIManager Instance { get { return MogoGlobleUIManager.m_instance; } }

    Transform m_myTransform;

    public OKCancelBox m_goOKCancelBox;
    private GameObject m_goModelController;

    private GameObject m_goUICursorSprite;
    private GameObject m_goUICursorUVAnim;

    private GameObject m_goGlobleLoadingUI;
    private GameObject m_goReconnectServerTip;

    private GameObject m_goPassRewardUI;
    private UITexture m_texComposeSucessSign;
    private UITexture m_texFinishedTaskSign;
    private MogoGlobleLoadingUI m_mgl;

    private UILabel m_lblGlobleStaticText;

    private Camera m_globleCamera;
    private GameObject m_msgBoxCamera;

    private GameObject m_goTaskRewardTipOKBtn;
    private GameObject m_goTaskRewardTip;
    private GameObject m_goTaskRewardItem;
    private GameObject[] m_arrTaskRewardItem = new GameObject[3];

    // task reward pos
    private GameObject m_goTaskRewardCloseBtnPos1;
    private GameObject m_goTaskRewardCloseBtnPos2;
    private GameObject m_goTaskRewardInfoPos1;
    private GameObject m_goTaskRewardInfoPos2;
    private GameObject m_goTaskRewardTitlePos1;
    private GameObject m_goTaskRewardTitlePos2;

    private GameObject m_goGOTaskRewardBG1;
    private GameObject m_goGOTaskRewardBG2;
    private GameObject m_goTaskRewardOKBtn;
    private GameObject m_goGOTaskRewardInfo;
    private GameObject m_goTaskRewardTitle;

    private GameObject m_goWaitingTip; // 菊花等待界面
    private GameObject m_goBattlePlayerBloodTipPanel;
    private UISprite m_spBattlePlayerBloodTip;

    void Awake()
    {
        m_instance = this;
        m_myTransform = transform;

        m_goOKCancelBox = m_myTransform.Find("OKCancel").GetComponentsInChildren<OKCancelBox>(true)[0];
        m_goModelController = m_myTransform.Find("MogoGlobleUIBG").gameObject;
        m_goUICursorSprite = m_myTransform.Find("UICursor/UICursorSprite").gameObject;
        m_goUICursorUVAnim = m_myTransform.Find("UICursor/UICursorUVAnim").gameObject;
        m_goGlobleLoadingUI = m_myTransform.Find("MogoGlobleLoadingUI").gameObject;
        m_goReconnectServerTip = m_myTransform.Find("ReconnectServerTip").gameObject;
        m_goWaitingTip = m_myTransform.Find("WaitingTip").gameObject;
        m_goBattlePlayerBloodTipPanel = m_myTransform.parent.Find("BattlePlayerBloodTipPanel").gameObject;
        m_goBattlePlayerBloodTipPanel.SetActive(false);
        m_spBattlePlayerBloodTip = m_myTransform.parent.Find("BattlePlayerBloodTipPanel/BattlePlayerBloodTipSprite").GetComponentsInChildren<UISprite>(true)[0];
        m_spBattlePlayerBloodTip.transform.localScale = new Vector3(1280, 1280.0f / Screen.width * Screen.height, -10);
        m_goPassRewardUI = m_myTransform.Find("PassRewardUI").gameObject;

        m_texComposeSucessSign = m_myTransform.Find("ComposeUIComposeSucessSign").GetComponentsInChildren<UITexture>(true)[0];
        m_texFinishedTaskSign = m_myTransform.Find("FinshedTaskSign").GetComponentsInChildren<UITexture>(true)[0];

        m_goTaskRewardTip = m_myTransform.Find("TaskReward").gameObject;
        m_goTaskRewardTipOKBtn = m_myTransform.Find("TaskReward/TaskRewardOKBtn").gameObject;
        m_goTaskRewardItem = m_myTransform.Find("TaskReward/GOTaskRewardInfo/TaskRewardItem").gameObject;

        m_goTaskRewardCloseBtnPos1 = m_myTransform.Find("TaskReward/GOTaskRewardPosManager/TaskRewardOKBtnPos1").gameObject;
        m_goTaskRewardCloseBtnPos2 = m_myTransform.Find("TaskReward/GOTaskRewardPosManager/TaskRewardOKBtnPos2").gameObject;
        m_goTaskRewardInfoPos1 = m_myTransform.Find("TaskReward/GOTaskRewardPosManager/TaskRewardInfoPos1").gameObject;
        m_goTaskRewardInfoPos2 = m_myTransform.Find("TaskReward/GOTaskRewardPosManager/TaskRewardInfoPos2").gameObject;
        m_goTaskRewardTitlePos1 = m_myTransform.Find("TaskReward/GOTaskRewardPosManager/TaskRewardTitlePos1").gameObject;
        m_goTaskRewardTitlePos2 = m_myTransform.Find("TaskReward/GOTaskRewardPosManager/TaskRewardTitlePos2").gameObject;
        m_goGOTaskRewardBG1 = m_myTransform.Find("TaskReward/GOTaskRewardBG/GOTaskRewardBG1").gameObject;
        m_goGOTaskRewardBG2 = m_myTransform.Find("TaskReward/GOTaskRewardBG/GOTaskRewardBG2").gameObject;
        m_goTaskRewardOKBtn = m_myTransform.Find("TaskReward/TaskRewardOKBtn").gameObject;
        m_goGOTaskRewardInfo = m_myTransform.Find("TaskReward/GOTaskRewardInfo").gameObject;
        m_goTaskRewardTitle = m_myTransform.Find("TaskReward/TaskRewardTitle").gameObject;

        m_lblGlobleStaticText = m_myTransform.Find("MogoGlobleStaticText").GetComponentsInChildren<UILabel>(true)[0];

        LoggerHelper.Debug(m_goOKCancelBox.name + " Awake");


        for (int i = 0; i < 3; ++i)
        {
            m_arrTaskRewardItem[i] = m_myTransform.Find("TaskReward/TaskRewardItemList/TaskRewardItem" + i).gameObject;
        }

        m_globleCamera = transform.parent.parent.GetComponent<Camera>();
        m_msgBoxCamera = GameObject.Find("MessageBoxCamera");
        Initialize();
    }

    private void Initialize()
    {
        m_goTaskRewardTipOKBtn.transform.GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnTaskRewardTipOKUp;


        m_arrTaskRewardItem[0].GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnTaskRewardItem0Up;
        m_arrTaskRewardItem[1].GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnTaskRewardItem1Up;
        m_arrTaskRewardItem[2].GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnTaskRewardItem2Up;

        EventDispatcher.AddEventListener<bool>(Events.MogoGlobleUIManagerEvent.ShowWaitingTip, m_instance.ShowWaitingTip);
    }

    public void Release()
    {
        m_goTaskRewardTipOKBtn.transform.GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnTaskRewardTipOKUp;

        m_arrTaskRewardItem[0].GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnTaskRewardItem0Up;
        m_arrTaskRewardItem[1].GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnTaskRewardItem1Up;
        m_arrTaskRewardItem[2].GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnTaskRewardItem2Up;

    }

    public void OnTaskRewardTipOKUp()
    {
        TruelyShowTaskRewardTip(false);
        //MogoUIManager.Instance.ShowMogoNormalMainUI();

        MogoUIQueue.Instance.Locked = false;
        MogoUIQueue.Instance.CheckQueue();
        EventDispatcher.TriggerEvent(Events.TaskEvent.ShowRewardEnd);
    }

    private void TruelyShowTaskRewardTip(bool isShow)
    {
        m_goTaskRewardTip.SetActive(isShow);
        MogoUIQueue.Instance.Locked = true;
    }

    public void ShowTaskRewardTip(bool isShow)
    {
        Mogo.Util.LoggerHelper.Debug("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!ShowTaskRewardTip: " + isShow);
        MogoUIQueue.Instance.PushOne(() => { TruelyShowTaskRewardTip(isShow); }, MogoUIManager.Instance.m_NormalMainUI, "ShowTaskRewardTip", 15);
    }

    public void SetTaskRewardTipExp(string exp)
    {
        m_goTaskRewardTip.transform.Find("GOTaskRewardInfo/TaskRewardExp/TaskRewardExpNum").GetComponentsInChildren<UILabel>(true)[0].text = exp;
    }

    public void SetTaskRewardTipGold(string gold)
    {
        m_goTaskRewardTip.transform.Find("GOTaskRewardInfo/TaskRewardGold/TaskRewardGoldNum").GetComponentsInChildren<UILabel>(true)[0].text = gold;
    }


    private void OnTaskRewardItem0Up()
    {
        Mogo.Util.LoggerHelper.Debug("Item0");
    }

    private void OnTaskRewardItem1Up()
    {
        Mogo.Util.LoggerHelper.Debug("Item1");
    }

    private void OnTaskRewardItem2Up()
    {
        Mogo.Util.LoggerHelper.Debug("Item2");
    }

    public void SetTaskRewardTipItem(List<int> listItemID)
    {
        for (int i = 0; i < m_arrTaskRewardItem.Length; ++i)
        {
            if (i < listItemID.Count)
            {
                m_arrTaskRewardItem[i].SetActive(true);

                InventoryManager.SetIcon(listItemID[i], m_arrTaskRewardItem[i].transform.Find("TaskRewardItem" + i + "FG").GetComponentsInChildren<UISprite>(true)[0],
                     0, null, m_arrTaskRewardItem[i].transform.Find("TaskRewardItem" + i + "BG").GetComponentsInChildren<UISprite>(true)[0]);

                //m_arrTaskRewardItem[i].transform.FindChild("TaskRewardItem" + i + "FG").GetComponentsInChildren<UISprite>(true)[0].atlas
                //    = MogoUIManager.Instance.GetAtlasByIconName(listItem[i]);

                //m_arrTaskRewardItem[i].transform.FindChild("TaskRewardItem" + i + "FG").GetComponentsInChildren<UISprite>(true)[0].spriteName = listItem[i];
            }
            else
            {
                m_arrTaskRewardItem[i].SetActive(false);
            }
        }

        if (listItemID.Count == 0)
            ShowTaskRewardPanelDiversity(false);
        else
            ShowTaskRewardPanelDiversity(true);
    }

    void ShowTaskRewardPanelDiversity(bool isContainItem)
    {
        if (isContainItem)
        {
            m_goTaskRewardItem.SetActive(true);

            m_goGOTaskRewardBG1.SetActive(true);
            m_goGOTaskRewardBG2.SetActive(false);

            m_goTaskRewardOKBtn.transform.localPosition = new Vector3(
                m_goTaskRewardOKBtn.transform.localPosition.x,
                m_goTaskRewardCloseBtnPos1.transform.localPosition.y,
                m_goTaskRewardOKBtn.transform.localPosition.z);

            m_goGOTaskRewardInfo.transform.localPosition = new Vector3(
                m_goGOTaskRewardInfo.transform.localPosition.x,
                m_goTaskRewardInfoPos1.transform.localPosition.y,
                m_goGOTaskRewardInfo.transform.localPosition.z);

            m_goTaskRewardTitle.transform.localPosition = new Vector3(
                m_goTaskRewardTitle.transform.localPosition.x,
                m_goTaskRewardTitlePos1.transform.localPosition.y,
                m_goTaskRewardTitle.transform.localPosition.z);
        }
        else
        {
            m_goTaskRewardItem.SetActive(false);

            m_goGOTaskRewardBG2.SetActive(true);
            m_goGOTaskRewardBG1.SetActive(false);

            m_goTaskRewardOKBtn.transform.localPosition = new Vector3(
                m_goTaskRewardOKBtn.transform.localPosition.x,
                m_goTaskRewardCloseBtnPos2.transform.localPosition.y,
                m_goTaskRewardOKBtn.transform.localPosition.z);

            m_goGOTaskRewardInfo.transform.localPosition = new Vector3(
               m_goGOTaskRewardInfo.transform.localPosition.x,
               m_goTaskRewardInfoPos2.transform.localPosition.y,
               m_goGOTaskRewardInfo.transform.localPosition.z);

            m_goTaskRewardTitle.transform.localPosition = new Vector3(
                m_goTaskRewardTitle.transform.localPosition.x,
                m_goTaskRewardTitlePos2.transform.localPosition.y,
                m_goTaskRewardTitle.transform.localPosition.z);
        }
    }

    public void ClearTaskRewardTipItemList()
    {
        for (int i = 0; i < m_arrTaskRewardItem.Length; ++i)
        {
            m_arrTaskRewardItem[i].SetActive(false);
        }
    }

    #region 等待界面

    bool m_bShowWaitingTip = false;
    public void ShowWaitingTip(bool isShow)
    {
        //Debug.LogError("WaitingTip " + isShow);
        m_bShowWaitingTip = isShow;
        MogoUIManager.Instance.LockMainCamera(isShow);
        MogoUIManager.Instance.GetSwitchUICamera().gameObject.SetActive(isShow);
        m_goWaitingTip.SetActive(isShow);
    }

    const float MAXWAITINGTIME = 10.0f;
    private float m_fCurrentTime = 0f;
    void Update()
    {
        if (m_bShowWaitingTip)
        {
            m_fCurrentTime += Time.deltaTime;
            if (m_fCurrentTime >= MAXWAITINGTIME)
            {
                MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(605));
                if (MogoWorld.thePlayer.sceneId == MogoWorld.globalSetting.homeScene)
                {
                    MogoUIManager.Instance.ShowMogoNormalMainUI();
                }
                else
                {
                    MogoUIManager.Instance.ShowMogoBattleMainUI();
                }
                ShowWaitingTip(false);
                m_fCurrentTime = 0.0f;
            }
        }
        else
        {
            m_fCurrentTime = 0.0f;
        }
    }

    #endregion

    /// <summary>
    /// 显示血量不足提示
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowBattlePlayerBloodTipPanel(bool isShow)
    {
        if (MogoUIManager.Instance.CurrentUI == MogoUIManager.Instance.m_MainUI
            && MogoUIManager.Instance.m_MainUI.activeSelf)
        {
            m_goBattlePlayerBloodTipPanel.SetActive(isShow);
        }
        else
        {
            m_goBattlePlayerBloodTipPanel.SetActive(false);
        }
    }

    public void TruelyInfo(string text, string okText = "OK", string cancelText = "CANCEL", float countDownTime = -1,
        ButtonBgType okBgType = ButtonBgType.Blue, ButtonBgType cancelBgType = ButtonBgType.Brown, Action callback = null)
    {
        if (okText.Equals("OK"))
            okText = LanguageData.GetContent(25561);
        if (cancelText.Equals("CANCEL"))
            cancelText = LanguageData.GetContent(25562);

        m_goOKCancelBox.SetBoxText(text);
        m_goOKCancelBox.SetOKBtnText(okText);
        m_goOKCancelBox.SetCancelBtnText(cancelText);
        SetButtonBg(m_goOKCancelBox.m_spOKBgUp, m_goOKCancelBox.m_spOKBgDown, okBgType);
        SetButtonBg(m_goOKCancelBox.m_spCancelBgUp, m_goOKCancelBox.m_spCancelBgDown, cancelBgType);
        m_goOKCancelBox.SetCountDown(countDownTime);

        m_goOKCancelBox.ShowAsOK();

        m_goOKCancelBox.gameObject.SetActive(true);

        m_goOKCancelBox.SetCallback((rst) =>
        {
            ConfirmHide();
            if (callback != null)
            {
                callback();
            }
        });
        //m_goModelController.gameObject.SetActive(true);

        Mogo.Util.LoggerHelper.Debug("TruleyInfo");
    }

    public void Info(string text, string okText = "OK", string cancelText = "CANCEL", float countDownTime = -1,
        ButtonBgType okBgType = ButtonBgType.Blue, ButtonBgType cancelBgType = ButtonBgType.Brown, Action callback = null)
    {
        //m_goOKCancelBox.SetBoxText(text);
        //m_goOKCancelBox.SetOKBtnText(okText);
        //m_goOKCancelBox.SetCancelBtnText(cancelText);
        //m_goOKCancelBox.SetCountDown(countDownTime);

        //m_goOKCancelBox.ShowAsOK();

        //m_goOKCancelBox.gameObject.SetActive(true);
        //m_goOKCancelBox.SetCallback((rst) => { ConfirmHide(); });
        //m_goModelController.gameObject.SetActive(true);

        MogoOKCancelBoxQueue.Instance.PushOne(text, okText, cancelText, null, okBgType, cancelBgType, callback);
    }

    public void TruelyConfirm(string text, Action<bool> Callback, string okText = "OK", string cancelText = "CANCEL", float countDownTime = -1,
        ButtonBgType okBgType = ButtonBgType.Blue, ButtonBgType cancelBgType = ButtonBgType.Brown)
    {
        if (okText.Equals("OK"))
            okText = LanguageData.GetContent(25561);
        if (cancelText.Equals("CANCEL"))
            cancelText = LanguageData.GetContent(25562);

        m_goOKCancelBox.SetBoxText(text);
        m_goOKCancelBox.SetOKBtnText(okText);
        m_goOKCancelBox.SetCancelBtnText(cancelText);
        SetButtonBg(m_goOKCancelBox.m_spOKBgUp, m_goOKCancelBox.m_spOKBgDown, okBgType);
        SetButtonBg(m_goOKCancelBox.m_spCancelBgUp, m_goOKCancelBox.m_spCancelBgDown, cancelBgType);
        m_goOKCancelBox.SetCountDown(countDownTime);

        m_goOKCancelBox.gameObject.SetActive(true);
        m_goOKCancelBox.SetCallback(Callback);
        //m_goModelController.gameObject.SetActive(true);

        m_goOKCancelBox.ShowAsOKCancel();

        Mogo.Util.LoggerHelper.Debug("TruelyConfirm");
    }

    public void Confirm(string text, Action<bool> Callback, string okText = "OK", string cancelText = "CANCEL", float countDownTime = -1,
        ButtonBgType okBgType = ButtonBgType.Blue, ButtonBgType cancelBgType = ButtonBgType.Brown)
    {
        //m_goOKCancelBox.SetBoxText(text);
        //m_goOKCancelBox.SetOKBtnText(okText);
        //m_goOKCancelBox.SetCancelBtnText(cancelText);
        //m_goOKCancelBox.SetCountDown(countDownTime);

        //m_goOKCancelBox.gameObject.SetActive(true);
        //m_goOKCancelBox.SetCallback(Callback);
        //m_goModelController.gameObject.SetActive(true);

        //m_goOKCancelBox.ShowAsOKCancel();   

        MogoOKCancelBoxQueue.Instance.PushOne(text, okText, cancelText, Callback, okBgType, cancelBgType);
    }

    static public void SetButtonBg(UISprite buttonBgUp, UISprite buttonBgDown = null, ButtonBgType type = ButtonBgType.Blue)
    {
        switch (type)
        {
            case ButtonBgType.Blue:
                {
                    if (buttonBgUp != null)
                        buttonBgUp.spriteName = "btn_03up";
                    if (buttonBgDown != null)
                        buttonBgDown.spriteName = "btn_03down";
                } break;
            case ButtonBgType.Green:
                {
                    if (buttonBgUp != null)
                        buttonBgUp.spriteName = "tongyong_butten_green_up";
                    if (buttonBgDown != null)
                        buttonBgDown.spriteName = "tongyong_butten_green_down";
                } break;
            case ButtonBgType.Yellow:
                {
                    if (buttonBgUp != null)
                        buttonBgUp.spriteName = "tongyong_butten_yellow_up";
                    if (buttonBgDown != null)
                        buttonBgDown.spriteName = "tongyong_butten_yellow_down";
                } break;
            case ButtonBgType.Brown:
                {
                    if (buttonBgUp != null)
                        buttonBgUp.spriteName = "btn_04up";
                    if (buttonBgDown != null)
                        buttonBgDown.spriteName = "btn_04down";
                } break;
            default:
                {
                    if (buttonBgUp != null)
                        buttonBgUp.spriteName = "btn_03up";
                    if (buttonBgDown != null)
                        buttonBgDown.spriteName = "btn_03down";
                } break;

        }
    }

    public void ShowGlobleStaticText(bool isShow = true, string text = "")
    {
        m_lblGlobleStaticText.text = text;
        m_lblGlobleStaticText.gameObject.SetActive(isShow);
        m_goModelController.SetActive(isShow);
    }

    public void ConfirmHide()
    {
        if (m_goOKCancelBox != null)
        {
            m_goOKCancelBox.gameObject.SetActive(false);
            //m_goModelController.gameObject.SetActive(false);
        }
    }

    public void ShowUICursorSprite(string imgName)
    {
        if (imgName == null)
        {
            m_goUICursorSprite.transform.parent.gameObject.SetActive(false);
            m_goUICursorSprite.SetActive(false);
        }
        else
        {
            m_goUICursorSprite.transform.parent.GetComponentsInChildren<UISprite>(true)[0].spriteName = imgName;
            m_goUICursorSprite.transform.parent.gameObject.SetActive(true);
            m_goUICursorSprite.SetActive(true);
        }
    }

    public void ShowUICursorUIAnim(bool isShow)
    {
        m_goUICursorUVAnim.SetActive(isShow);
    }

    public void ShowGlobleLoadingUI(bool isShow, int tipId = 0, int textureId = 0)
    {
        //Mogo.Util.LoggerHelper.Debug("ShowGlobleLoadingUI:" + isShow);
        m_goGlobleLoadingUI.SetActive(isShow);
        //if (m_msgBoxCamera == null)
        //    m_msgBoxCamera = GameObject.Find("MessageBoxCamera");
        //if (m_msgBoxCamera != null)
        //    m_msgBoxCamera.SetActive(isShow);
        if (isShow)
        {
            m_globleCamera.clearFlags = CameraClearFlags.SolidColor;
            m_globleCamera.backgroundColor = Color.black;
            MogoGlobleLoadingUI loadingView = m_goGlobleLoadingUI.GetComponent<MogoGlobleLoadingUI>();
            loadingView.LoadingTip = LoadingTipsData.GetTip(tipId);
            loadingView.LoadingImgBg = LoadingTexturesData.GetTexture(textureId);
            loadingView.LoadingStatus = 0;


            //uint tId = 0;
            //tId = TimerHeap.AddTimer(0, 5, () =>
            //{
            //    if (loadingView.LoadingStatus >= 100) return;
            //    loadingView.LoadingStatus += 1;
            //});
            //TimerHeap.AddTimer(800, 0, () =>
            //{
            //    TimerHeap.DelTimer(tId);
            //});

            if (MFUIManager.CurrentUI == MFUIManager.MFUIID.EnterWaittingMessageBox)
            {

            }
        }
        else
        {
            m_globleCamera.clearFlags = CameraClearFlags.Depth;
        }
    }

    public void ShowReconnectServerTip(bool isShow)
    {
        m_goReconnectServerTip.SetActive(isShow);
    }

    public void ShowPassRewardUI(bool isShow)
    {
        m_goPassRewardUI.SetActive(isShow);
        if (isShow)
        {
            // 关闭MainUI,并重置摇杆
            MogoUIManager.Instance.m_MainUI.SetActive(false);
            if (ControlStick.instance != null)
                ControlStick.instance.Reset();
        }
    }

    public void FillPassRewardItemData(List<PassRewardGridData> list, Action cb, bool bWin)
    {
        PassRewardUI passRewardUI = m_goPassRewardUI.GetComponentsInChildren<PassRewardUI>(true)[0];
        passRewardUI.IsWin = bWin;
        passRewardUI.FillItemGridData(list, cb);
    }

    public void FillGlobalLoadingUIData(GlobleLoadingUIData gd)
    {
        if (m_mgl == null)
            m_mgl = m_goGlobleLoadingUI.GetComponentsInChildren<MogoGlobleLoadingUI>(true)[0];
        m_mgl.LoadingTip = gd.tip;
        m_mgl.LoadingStatus = gd.status;
    }

    public void SetLoadingStatus(int progress)
    {
        if (m_mgl == null)
            m_mgl = m_goGlobleLoadingUI.GetComponentsInChildren<MogoGlobleLoadingUI>(true)[0];
        m_mgl.LoadingStatus = progress;
    }

    public void ShowComposeSucessSign(bool isShow)
    {
        if (m_texComposeSucessSign.mainTexture == null)
        {
            AssetCacheMgr.GetResourceAutoRelease("zb_hechengchenggong.png", (obj) =>
            {
                m_texComposeSucessSign.mainTexture = (Texture)obj;
            });
        }

        m_texComposeSucessSign.gameObject.SetActive(isShow);

        TweenAlpha ta = m_texComposeSucessSign.GetComponentsInChildren<TweenAlpha>(true)[0];
        ta.Reset();
        ta.enabled = true;
        ta.Play(true);

        m_texComposeSucessSign.gameObject.AddComponent<MogoUIAnimPowerup>();

    }

    public void ShowFinishedTaskSign(bool isShow)
    {
        //if (m_texFinishedTaskSign.mainTexture == null)
        //{
        //    AssetCacheMgr.GetUIResource("rwwc.png", (obj) => { m_texFinishedTaskSign.mainTexture = (Texture)obj; });
        //}

        //if (m_texFinishedTaskSign.mainTexture.name != "rwwc.png")
        //{
        //    AssetCacheMgr.GetUIResource("rwwc.png", (obj) => { m_texFinishedTaskSign.mainTexture = (Texture)obj; });
        //}

        //m_texFinishedTaskSign.gameObject.SetActive(isShow);

        //TweenAlpha ta = m_texFinishedTaskSign.GetComponentsInChildren<TweenAlpha>(true)[0];
        //ta.Reset();
        //ta.enabled = true;
        //ta.Play(true);
    }

    public void ShowUpgreateSign(bool isShow)
    {
        //if (m_texFinishedTaskSign.mainTexture == null)
        //{
        //    AssetCacheMgr.GetUIResource("sj.png", (obj) => { m_texFinishedTaskSign.mainTexture = (Texture)obj; });
        //}

        //if (m_texFinishedTaskSign.mainTexture.name != "sj.png")
        //{
        //    AssetCacheMgr.GetUIResource("sj.png", (obj) => { m_texFinishedTaskSign.mainTexture = (Texture)obj; });
        //}

        //m_texFinishedTaskSign.gameObject.SetActive(isShow);

        //TweenAlpha ta = m_texFinishedTaskSign.GetComponentsInChildren<TweenAlpha>(true)[0];
        //ta.Reset();
        //ta.enabled = true;
        //ta.Play(true);
    }
}

public class MogoMessageBox
{
    private const string ERROR_CODE_DEFAULT = "Unkown error. code: {0}";
    public const int LANGUAGE_CONFORM = 717;
    public const int LANGUAGE_CANCEL = 716;
    public static readonly String OKText;
    public static readonly String CancelText;

    static MogoMessageBox()
    {
        OKText = LanguageData.GetContent(LANGUAGE_CONFORM);
        CancelText = LanguageData.GetContent(LANGUAGE_CANCEL);
    }

    /// <summary>
    /// 显示信息提示框。
    /// </summary>
    /// <param name="text"></param>
    /// <param name="countDownTime"></param>
    public static void Info(string text, float countDownTime = -1)
    {
        MogoMsgBox.Instance.ShowMsgBox(text, OKText);
    }

    /// <summary>
    /// 显示信息提示框。
    /// </summary>
    /// <param name="text"></param>
    /// <param name="okText"></param>
    /// <param name="countDownTime"></param>
    public static void Info(string text, string okText, float countDownTime = -1)
    {
        MogoMsgBox.Instance.ShowMsgBox(text, okText);
    }

    /// <summary>
    /// 显示确认提示框。（选择后自动关闭）
    /// </summary>
    /// <param name="text"></param>
    /// <param name="Callback"></param>
    /// <param name="countDownTime"></param>
    public static void Confirm(string text, Action<bool> Callback = null, float countDownTime = -1)
    {
        Confirm(text, OKText, CancelText, Callback, countDownTime);
    }

    /// <summary>
    /// 显示确认提示框。（选择后自动关闭）
    /// </summary>
    /// <param name="text"></param>
    /// <param name="okText"></param>
    /// <param name="Callback"></param>
    /// <param name="countDownTime"></param>
    public static void Confirm(string text, string okText, Action<bool> Callback = null, float countDownTime = -1)
    {
        Confirm(text, okText, CancelText, Callback, countDownTime);
    }

    /// <summary>
    /// 显示确认提示框。（选择后自动关闭）
    /// </summary>
    /// <param name="text"></param>
    /// <param name="okText"></param>
    /// <param name="cancelText"></param>
    /// <param name="Callback"></param>
    /// <param name="countDownTime"></param>
    public static void Confirm(string text, string okText, string cancelText, Action<bool> Callback = null, float countDownTime = -1,
        ButtonBgType okBgType = ButtonBgType.Blue, ButtonBgType cancelBgType = ButtonBgType.Brown)
    {
        MogoGlobleUIManager.Instance.Confirm(text, (rst) =>
        {
            try
            {
                if (Callback != null)
                    Callback(rst);
            }
            catch (Exception ex)
            {
                LoggerHelper.Except(ex);
            }
            MogoGlobleUIManager.Instance.ConfirmHide();
        }, okText, cancelText, countDownTime, okBgType, cancelBgType);
    }

    /// <summary>
    /// 显示服务回调错误信息。
    /// </summary>
    /// <param name="offset">语言表分段偏移量。</param>
    /// <param name="errorId">错误码。</param>
    public static void RespError(Mogo.Game.LangOffset offset, int errorId)
    {
        RespError(offset, errorId, String.Empty);
    }

    /// <summary>
    /// 显示服务回调错误信息。
    /// </summary>
    /// <param name="offset">语言表分段偏移量。</param>
    /// <param name="errorId">错误码。</param>
    /// <param name="args">错误信息填充内容。</param>
    public static void RespError(Mogo.Game.LangOffset offset, int errorId, params object[] args)
    {
        var errorCode = (int)offset + errorId;
        var content = LanguageData.dataMap.GetValueOrDefault(errorCode, new LanguageData() { content = String.Format(ERROR_CODE_DEFAULT, errorCode) });
        MogoMessageBox.Info(content.Format(args));
    }

    public static void ShowLoading(int tipId = 0, int textureId = 0)
    {
        try
        {
            if (MogoGlobleUIManager.Instance)
                MogoGlobleUIManager.Instance.ShowGlobleLoadingUI(true, tipId, textureId);
        }
        catch (Exception ex)
        {
            LoggerHelper.Except(ex);
        }
    }


    public static void Loading(int progress)
    {
        try
        {
            if (MogoGlobleUIManager.Instance)
                MogoGlobleUIManager.Instance.SetLoadingStatus(progress);
        }
        catch (Exception ex)
        {
            LoggerHelper.Except(ex);
        }
    }

    public static void HideLoading()
    {
        try
        {
            if (MogoGlobleUIManager.Instance)
                MogoGlobleUIManager.Instance.ShowGlobleLoadingUI(false);
        }
        catch (Exception ex)
        {
            LoggerHelper.Except(ex);
        }
    }
}
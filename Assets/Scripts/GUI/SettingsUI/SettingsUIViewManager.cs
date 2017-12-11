using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mogo.Util;

public class SettingsUIViewManager : MonoBehaviour
{

    private static SettingsUIViewManager m_instance;

    public static SettingsUIViewManager Instance
    {
        get
        {
            return SettingsUIViewManager.m_instance;
        }
    }

    public enum SettingsAdviceType
    {
        Type_Bug = 11,
        Type_Advice = 13,
        Type_Complain = 12,
        Type_Others = 10
    }

    private Dictionary<string, string> m_widgetToFullName = new Dictionary<string, string>();
    private Transform m_myTransform;

    MogoSlider m_sliderSound;
    MogoSlider m_sliderMusic;

    GameObject m_goAdviceDialog;
    GameObject m_goSettingsDialog;

    UIInput m_inputAdvices;

    SettingsAdviceType m_adviceType = SettingsAdviceType.Type_Bug;

    UILabel[] m_arrGraphicQualityDesripe = new UILabel[3];

    public Action<int> OnPeopleInScreenChanged;
    public Action<int> OnGraphicQualityChanged;
    private MogoSingleButtonList m_settingsUIPeopleInScreeBtnList;
    private MogoSingleButtonList m_settingsUIGraphicQualityBtnList;

    public void AddWigetToFullNameData(string widgetName, string fullName)
    {
        m_widgetToFullName.Add(widgetName, fullName);
    }

    private string GetFullName(Transform currentTransform)
    {
        string fullName = "";

        while (currentTransform != m_myTransform)
        {
            fullName = currentTransform.name + fullName;

            if (currentTransform.parent != m_myTransform)
            {
                fullName = "/" + fullName;
            }

            currentTransform = currentTransform.parent;
        }

        return fullName;
    }

    private void FillFullNameData(Transform rootTransform)
    {
        for (int i = 0; i < rootTransform.GetChildCount(); ++i)
        {
            AddWigetToFullNameData(rootTransform.GetChild(i).name, GetFullName(rootTransform.GetChild(i)));
            FillFullNameData(rootTransform.GetChild(i));
        }
    }


    //public static Dictionary<string, Action<int>> ButtonTypeToEventUp = new Dictionary<string, Action<int>>();

    void Awake()
    {
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_sliderMusic = m_myTransform.Find(m_widgetToFullName["SettingsUIMusicLoundProgressBar"]).GetComponentsInChildren<MogoSlider>(true)[0];
        m_sliderSound = m_myTransform.Find(m_widgetToFullName["SettingsUISoundLoundProgressBar"]).GetComponentsInChildren<MogoSlider>(true)[0];

        m_sliderMusic.RelatedCamera = GameObject.Find("MogoMainUI").transform.Find("Camera").GetComponentsInChildren<Camera>(true)[0];
        m_sliderSound.RelatedCamera = GameObject.Find("MogoMainUI").transform.Find("Camera").GetComponentsInChildren<Camera>(true)[0];

        m_instance = m_myTransform.GetComponentsInChildren<SettingsUIViewManager>(true)[0];

        m_goAdviceDialog = m_myTransform.Find(m_widgetToFullName["SettingsUIAdvices"]).gameObject;
        m_goSettingsDialog = m_myTransform.Find(m_widgetToFullName["SettingsUISettings"]).gameObject;

        m_inputAdvices = m_myTransform.Find(m_widgetToFullName["SettingsUIInput"]).GetComponentsInChildren<UIInput>(true)[0];

        m_arrGraphicQualityDesripe[2] = m_myTransform.Find(m_widgetToFullName["SettingsUIGraphicQualityHighDescripe"]).GetComponentsInChildren<UILabel>(true)[0];
        m_arrGraphicQualityDesripe[1] = m_myTransform.Find(m_widgetToFullName["SettingsUIGraphicQualityMediumDescripe"]).GetComponentsInChildren<UILabel>(true)[0];
        m_arrGraphicQualityDesripe[0] = m_myTransform.Find(m_widgetToFullName["SettingsUIGraphicQualityLowDescripe"]).GetComponentsInChildren<UILabel>(true)[0];


        m_settingsUIPeopleInScreeBtnList = m_myTransform.Find(m_widgetToFullName["SettingsUIPeopleInScreen"]).GetComponentsInChildren<MogoSingleButtonList>(true)[0];
        m_settingsUIGraphicQualityBtnList = m_myTransform.Find(m_widgetToFullName["SettingsUIGraphicQuality"]).GetComponentsInChildren<MogoSingleButtonList>(true)[0];
        Initialize();

        m_sliderSound.SetCurrentStatus(SoundManager.SoundVolume);
        m_sliderMusic.SetCurrentStatus(SoundManager.MusicVolume / 0.7f > 1 ? 1 : SoundManager.MusicVolume / 0.7f);



    }


    void OnDragMoveModeBtnUp(int i)
    {
        MogoUIManager.Instance.ChangeSettingToControlStick();
        SystemConfig.Instance.IsDragMove = true;
        SystemConfig.SaveConfig();
        LoggerHelper.Debug("Drag");
    }

    void OnTouchMoveModeBtnUp(int i)
    {

        MogoUIManager.Instance.ChangeSettingToTouch();
        SystemConfig.Instance.IsDragMove = false;
        SystemConfig.SaveConfig();
        LoggerHelper.Debug("Touch");
    }

    void OnUploadLogBtnUp(int i)
    {
        Mogo.Util.LoggerHelper.UploadLogFile();
    }

    void OnBackToChooseCharacterBtnUp(int i)
    {
        Mogo.Util.LoggerHelper.Debug("BackToChooseCharacter");
        MogoWorld.Logout(1);
    }

    void OnHighGraphicQualityUp(int i)
    {
        Mogo.Util.LoggerHelper.Debug("HighGraphicQuality");
        if (OnGraphicQualityChanged != null)
            OnGraphicQualityChanged(2);
        //Shader.globalMaximumLOD = 211;
        //ShowGraphicQualityDesripe(2);
    }
    void OnLowGraphicQualityUp(int i)
    {
        Mogo.Util.LoggerHelper.Debug("LowGraphicQuality");
        if (OnGraphicQualityChanged != null)
            OnGraphicQualityChanged(0);
        //Shader.globalMaximumLOD = 201;
        //ShowGraphicQualityDesripe(0);
    }
    void OnMediumGraphicQualityUp(int i)
    {
        Mogo.Util.LoggerHelper.Debug("MediumGraphicQuality");
        if (OnGraphicQualityChanged != null)
            OnGraphicQualityChanged(1);
        //Shader.globalMaximumLOD = 201;
        //ShowGraphicQualityDesripe(1);
    }
    void OnPeople5InScreenUp(int i)
    {
        if (OnPeopleInScreenChanged != null)
            OnPeopleInScreenChanged(5);
        Mogo.Util.LoggerHelper.Debug("People5InScreen");
    }
    void OnPeople10InScreenUp(int i)
    {
        if (OnPeopleInScreenChanged != null)
            OnPeopleInScreenChanged(10);
        Mogo.Util.LoggerHelper.Debug("People10InScreen");
    }
    void OnPeople15InScreenUp(int i)
    {
        if (OnPeopleInScreenChanged != null)
            OnPeopleInScreenChanged(15);
        Mogo.Util.LoggerHelper.Debug("People15InScreen");
    }
    void OnAddviceBtnUp(int i)
    {
        Mogo.Util.LoggerHelper.Debug("Addvice");
        ShowAdvicesDialog(true);
        ShowSettingsDialog(false);
    }

    void OnAdviceUp(int i)
    {
        m_adviceType = SettingsAdviceType.Type_Advice;
    }

    void OnBugUp(int i)
    {
        m_adviceType = SettingsAdviceType.Type_Bug;
    }

    void OnComplainUp(int i)
    {
        m_adviceType = SettingsAdviceType.Type_Complain;
    }

    void OnOthersUp(int i)
    {
        m_adviceType = SettingsAdviceType.Type_Others;
    }

    void OnAdvicesCancelBtnUp(int i)
    {
        Mogo.Util.LoggerHelper.Debug("AdvicesCancel");

        ShowSettingsDialog(true);
        ShowAdvicesDialog(false);
    }

    void OnAdvicesSendBtnUp(int i)
    {
        Mogo.Util.LoggerHelper.Debug("AdvicesSend Type = " + m_adviceType);

        if (m_inputAdvices.text == "")
            return;

        MogoWorld.thePlayer.RpcCall("bug_report", (byte)m_adviceType, "Title", m_inputAdvices.text);
        ClearAdvicesInput();
    }
    void Initialize()
    {
        SettingsUILogicManager.Instance.Initialize();

        SettingsUIDict.ButtonTypeToEventUp.Add("DragMoveModeBtn", OnDragMoveModeBtnUp);
        SettingsUIDict.ButtonTypeToEventUp.Add("TouchMoveModeBtn", OnTouchMoveModeBtnUp);
        SettingsUIDict.ButtonTypeToEventUp.Add("UploadLogBtn", OnUploadLogBtnUp);
        SettingsUIDict.ButtonTypeToEventUp.Add("BackToChooseCharacterBtn", OnBackToChooseCharacterBtnUp);
        SettingsUIDict.ButtonTypeToEventUp.Add("SettingsUIGraphicQualityHigh", OnHighGraphicQualityUp);
        SettingsUIDict.ButtonTypeToEventUp.Add("SettingsUIGraphicQualityLow", OnLowGraphicQualityUp);
        SettingsUIDict.ButtonTypeToEventUp.Add("SettingsUIGraphicQualityMedium", OnMediumGraphicQualityUp);
        SettingsUIDict.ButtonTypeToEventUp.Add("SettingsUIPeople5InScreen", OnPeople5InScreenUp);
        SettingsUIDict.ButtonTypeToEventUp.Add("SettingsUIPeople10InScreen", OnPeople10InScreenUp);
        SettingsUIDict.ButtonTypeToEventUp.Add("SettingsUIPeople15InScreen", OnPeople15InScreenUp);
        SettingsUIDict.ButtonTypeToEventUp.Add("SettingsUIAddviceBtn", OnAddviceBtnUp);
        SettingsUIDict.ButtonTypeToEventUp.Add("SettingsUIAdvice", OnAdviceUp);
        SettingsUIDict.ButtonTypeToEventUp.Add("SettingsUIBug", OnBugUp);
        SettingsUIDict.ButtonTypeToEventUp.Add("SettingsUIComplain", OnComplainUp);
        SettingsUIDict.ButtonTypeToEventUp.Add("SettingsUIOthers", OnOthersUp);
        SettingsUIDict.ButtonTypeToEventUp.Add("SettingsUIAdvicesCancel", OnAdvicesCancelBtnUp);
        SettingsUIDict.ButtonTypeToEventUp.Add("SettingsUIAdvicesSend", OnAdvicesSendBtnUp);
    }

    public void Release()
    {
        SettingsUILogicManager.Instance.Release();



        SettingsUIDict.ButtonTypeToEventUp.Clear();
    }

    public void CallWhenMusicSliderSlidering(Action<float> act)
    {
        m_sliderMusic.OnSliding = act;
    }

    public void CallWhenSoundSliderSlidering(Action<float> act)
    {
        m_sliderSound.OnSliding = act;
    }


    public void ShowSettingsDialog(bool isShow)
    {
        m_goSettingsDialog.SetActive(isShow);
    }

    public void ShowAdvicesDialog(bool isShow)
    {
        m_goAdviceDialog.SetActive(isShow);
        //m_settingsUIPeopleInScreeBtnList.SetAllButtonUp();

        m_goAdviceDialog.GetComponentsInChildren<MogoSingleButtonList>(true)[0].SetAllButtonUp();
    }

    public void ClearAdvicesInput()
    {
        m_inputAdvices.text = "";
    }

    public void ShowGraphicQualityDesripe(int id)
    {
        switch (id)
        {
            case 0:
                Shader.globalMaximumLOD = 201;
                break;
            case 1:
                Shader.globalMaximumLOD = 201;
                break;
            case 2:
                Shader.globalMaximumLOD = 211;
                break;
            default:
                break;
        }
        for (int i = 0; i < 3; ++i)
        {
            m_arrGraphicQualityDesripe[i].GetComponent<TweenAlpha>().Reset();
            m_arrGraphicQualityDesripe[i].GetComponent<TweenAlpha>().enabled = false;
            m_arrGraphicQualityDesripe[i].gameObject.SetActive(false);
        }

        m_arrGraphicQualityDesripe[id].gameObject.SetActive(true);
        m_arrGraphicQualityDesripe[id].GetComponent<TweenAlpha>().Play(true);
    }

    public void SetCurrentUIPeopleInScreeBtnDown(int index)
    {

        m_settingsUIPeopleInScreeBtnList.SetCurrentDownButton(index);
    }

    public void SetCurrentUIGraphicQualityBtnDown(int index)
    {
        m_settingsUIGraphicQualityBtnList.SetCurrentDownButton(index);
    }

    public void DestroyUIAndResources()
    {
        if (SystemSwitch.DestroyAllUI)
        {
            MogoUIManager.Instance.DestroySettingsUI();
        }
    }

    void OnDisable()
    {
        DestroyUIAndResources();
    }
}

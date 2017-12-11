/*----------------------------------------------------------------
// Copyright (C) 2013 ���ݣ�����
//
// ģ������VIPInfoUIViewManager
// �����ߣ�Charles
// �޸����б��
// �������ڣ�
// ģ��������
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mogo.Util;
using Mogo.GameData;
public class VIPInfoUIViewManager : MogoParentUI
{
    #region ��������

    #endregion

    #region ˽�б���

    private Transform m_transform;
    private GameObject m_btnClose;

    private GameObject m_vipInfo;
    private GameObject m_vipInfoNext;
    private MogoListImproved m_listImproveVIPInfo;
    private MogoListImproved m_listImproveVIPInfoNext;

    private UILabel m_lblProgress;
    private UISlider m_progress;
    private UILabel m_lblVIPLevel;
    private UILabel m_lblCharge;
    private GameObject m_goVIPUINextLevel;
    private UISprite m_spNextLevel1;
    private UISprite m_spNextLevel2;
    private UISprite m_spLevel;
    private UISprite m_spLevel2;
    private Transform m_tranBtnCharge; // ��ֵ��ť

    #endregion

    UISprite m_spResCtrl;

    void Awake()
    {
        m_transform = transform;

        m_vipInfo = m_transform.Find("VIPInfo").gameObject;
        m_vipInfoNext = m_transform.Find("VIPInfoNext").gameObject;
        m_listImproveVIPInfo = m_vipInfo.GetComponentsInChildren<MogoListImproved>(true)[0];
        m_listImproveVIPInfo.LeftArrow = m_transform.Find("VIPInfoUIArrow/VIPInfoUIArrowL").gameObject;
        m_listImproveVIPInfo.RightArrow = m_transform.Find("VIPInfoUIArrow/VIPInfoUIArrowR").gameObject;
        m_listImproveVIPInfoNext = m_vipInfoNext.GetComponentsInChildren<MogoListImproved>(true)[0];
        m_listImproveVIPInfoNext.LeftArrow = m_transform.Find("VIPInfoUIArrow/VIPInfoUIArrowL").gameObject;
        m_listImproveVIPInfoNext.RightArrow = m_transform.Find("VIPInfoUIArrow/VIPInfoUIArrowR").gameObject;

        m_btnClose = m_transform.Find("btnClose").gameObject;
        m_lblVIPLevel = m_transform.Find("VIPHead/lblVIPlevel").GetComponentsInChildren<UILabel>(true)[0];
        m_lblCharge = m_transform.Find("lblCharge").GetComponentsInChildren<UILabel>(true)[0];
        m_lblProgress = m_transform.Find("ProgressBar/txtProgress").GetComponentsInChildren<UILabel>(true)[0];
        m_progress = m_transform.Find("ProgressBar").GetComponentsInChildren<UISlider>(true)[0];
        m_goVIPUINextLevel = m_transform.Find("VIPUINextLevel").gameObject;
        m_spNextLevel1 = m_transform.Find("VIPUINextLevel/VIPUINextLevel1").GetComponentsInChildren<UISprite>(true)[0];
        m_spNextLevel2 = m_transform.Find("VIPUINextLevel/VIPUINextLevel2").GetComponentsInChildren<UISprite>(true)[0];
        m_spLevel = m_transform.Find("VIPHead/imgVIPLevel").GetComponentsInChildren<UISprite>(true)[0];
        m_spLevel2 = m_transform.Find("VIPHead/imgVIPLevel2").GetComponentsInChildren<UISprite>(true)[0];
        m_tranBtnCharge = m_transform.Find("btnCharge");

        m_spResCtrl = m_transform.Find("vipUIRefreshCtrl").GetComponentsInChildren<UISprite>(true)[0];
        m_atlsCanRelease = m_spResCtrl.atlas;

        AddButtonListener("OnClicked", "btnClose", OnClose);
        AddButtonListener("OnClicked", "Tab/btnTabCurrent", OnCurrent);
        AddButtonListener("OnClicked", "Tab/btnTabNext", OnNext);
        AddButtonListener("OnClicked", "btnCharge", OnCharge);
        gameObject.SetActive(false);

        Initialize();
    }

    void Start()
    {
        m_btnClose.GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = GameObject.Find("MogoMainUI").transform.Find("Camera").GetComponentsInChildren<Camera>(true)[0];
    }
    
    public void Initialize()
    {
        VIPInfoUILogicManager.Instance.Initialize(this);
        //m_uiLoginManager = VIPInfoUILogicManager.Instance;
    }

    #region VIP�ȼ�

    /// <summary>
    /// ��ǰVIP�ȼ�
    /// </summary>
    /// <param name="level"></param>
    public void SetCurrentVipLevel(int level)
    {
        if (level < 10)
        {
            m_spLevel.spriteName = level.ToString();
            m_spLevel2.gameObject.SetActive(false);
        }
        else if (level >= 10)
        {
            m_spLevel.spriteName = "1";
            m_spLevel2.spriteName = (level - 10).ToString();
            m_spLevel2.gameObject.SetActive(true);
        }        
    }

    /// <summary>
    /// VIP��һ�ȼ�λ��
    /// </summary>
    /// <param name="labelWidth"></param>
    private void RefreshNextLevelPos(float labelWidth)
    {
        m_goVIPUINextLevel.transform.localPosition = new Vector3(m_lblCharge.transform.localPosition.x + labelWidth, 
            m_goVIPUINextLevel.transform.localPosition.y, 
            m_goVIPUINextLevel.transform.localPosition.z);
    }

    /// <summary>
    /// VIP��һ�ȼ�
    /// </summary>
    /// <param name="level"></param>
    public void SetNextVipLevel(int level)
    {        
        if (level < 10)
        {
            m_spNextLevel1.spriteName = level.ToString();
            m_spNextLevel2.gameObject.SetActive(false);
        }
        else if (level >= 10)
        {
            m_spNextLevel1.spriteName = "1";
            m_spNextLevel2.spriteName = (level - 10).ToString();
            m_spNextLevel2.gameObject.SetActive(true);
        }        
    }

    #endregion

    public void Release()
    {
        VIPInfoUILogicManager.Instance.Release();
    }

    public void SetViewData(byte vipLevel)
    {
        SetCurrentVipLevel(vipLevel);
    }

    void ResourceLoaded()
    {
        byte vipLevel = (byte)(MogoWorld.thePlayer.VipLevel);
        SetLogicData(vipLevel,m_vipInfo);
        SetViewData(vipLevel);
        SetPlayerChargeSum(MogoWorld.thePlayer.chargeSum);

        MogoGlobleUIManager.Instance.ShowWaitingTip(false);
    }

    void ResourceLoadedNext()
    {
        byte vipLevel = (byte)(MogoWorld.thePlayer.VipLevel+1);
        SetLogicData(vipLevel,m_vipInfoNext);
        OnCurrent();
    }

    public void SetLogicData(byte vipLevel,GameObject parent)
    {
        var icoList = PrivilegeData.dataMap.Get(vipLevel).iconList;
        var strList = PrivilegeData.dataMap.Get(vipLevel).stringList;
        int index = 0;
        foreach (var item in parent.GetComponentsInChildren<MogoListImproved>(true)[0].DataList)
        {
            VIPInfoGrid grid = (VIPInfoGrid)item.Value;
            grid.IconName = IconData.dataMap[icoList[index]].path;
            grid.Desc = LanguageData.GetContent(strList[index]);
            index++;
        }
    }

    public void SetPlayerChargeSum(uint chargeSum)
    {
        uint UpDiamond = uint.MaxValue;
        if (PrivilegeData.dataMap.ContainsKey(MogoWorld.thePlayer.VipLevel + 1))
            UpDiamond = PrivilegeData.dataMap[MogoWorld.thePlayer.VipLevel + 1].accumulatedAmount[0];
        SetNextVipLevel(MogoWorld.thePlayer.VipLevel + 1);
        m_lblCharge.text = LanguageData.GetContent(5000,(UpDiamond-chargeSum)*10,"" );
        m_lblProgress.text = chargeSum * 10 + " / " + UpDiamond * 10;
        m_progress.sliderValue = chargeSum / UpDiamond;

        // ����lblCharge�ı�����
        float labelWidth = m_lblCharge.font.CalculatePrintedSize(m_lblCharge.text, true, UIFont.SymbolStyle.None).x * m_lblCharge.transform.localScale.x;
        RefreshNextLevelPos(labelWidth);
    }

    void OnClose()
    {
        MogoUIManager.Instance.ShowMogoNormalMainUI();
    }
    void OnCurrent()
    {
        m_vipInfo.SetActive(true);
        m_vipInfoNext.SetActive(false);
        m_listImproveVIPInfo.TweenTo(0, true);
    }

    void OnNext()
    {
        m_vipInfo.SetActive(false);
        m_vipInfoNext.SetActive(true);
        m_listImproveVIPInfoNext.TweenTo(0, true);
    }   

    void OnCharge()
    {
#if UNITY_IPHONE
        EventDispatcher.TriggerEvent(IAPEvents.ShowIAPView);
#else
        //MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(25570));
        PlatformSdkManager.Instance.Charge(0);
#endif
    }

    UIAtlas m_atlsCanRelease;

    #region ��ֵ��ť��Ч

    private string m_fx1ChargeButton = "ChargeButtonFX1";

    /// <summary>
    /// �ڳ�ֵ��ť�ϸ�����Ч
    /// </summary>
    private void AttachChargeButtonAnimation()
    {
        if (m_tranBtnCharge == null)
        {
            LoggerHelper.Error("m_tranBtnCharge is null.");
            return;
        }

        MogoGlobleUIManager.Instance.ShowWaitingTip(true);

        MogoFXManager.Instance.AttachParticleAnim("fx_ui_baoxiangxingxing.prefab", m_fx1ChargeButton, m_tranBtnCharge.position,
            MogoUIManager.Instance.GetMainUICamera().GetComponent<Camera>(), 0, 0, 0, () =>
            {
                MogoGlobleUIManager.Instance.ShowWaitingTip(false);
            });
    }

    /// <summary>
    /// �ͷų�ֵ��ť��Ч
    /// </summary>
    private void ReleaseChargeButtonAnimation()
    {
        MogoFXManager.Instance.ReleaseParticleAnim(m_fx1ChargeButton);
    }

    #endregion

    #region ����򿪺͹ر�

    public void Show(bool IsShow)
    {
        if (IsShow)
        {
            byte vipLevel = (byte)(MogoWorld.thePlayer.VipLevel);
            int curCount = PrivilegeData.dataMap[vipLevel].iconList.Count;
            m_listImproveVIPInfo.SetGridLayout<VIPInfoGrid>(curCount, m_vipInfo.transform, ResourceLoaded);
            if (PrivilegeData.dataMap.ContainsKey(vipLevel + 1))
            {
                int nextCount = PrivilegeData.dataMap[vipLevel + 1].iconList.Count;
                m_listImproveVIPInfoNext.SetGridLayout<VIPInfoGrid>(nextCount, m_vipInfoNext.transform, ResourceLoadedNext);
            }
        }
    }

    void OnEnable()
    {
        AttachChargeButtonAnimation();

        return;
        //Ϊ���������ע�����´���
        //if (!SystemSwitch.DestroyResource)
        //{
        //    return;
        //}

        //if (m_atlsCanRelease != null)
        //{
        //    if (m_atlsCanRelease.spriteMaterial.mainTexture == null)
        //    {
        //        AssetCacheMgr.GetResourceAutoRelease("MogoOperatingUI.png", (obj) =>
        //        {

        //            m_atlsCanRelease.spriteMaterial.mainTexture = (Texture)obj;
        //            m_atlsCanRelease.MarkAsDirty();
        //        });
        //    }
        //}

    }

    void OnDisable()
    {
        ReleaseChargeButtonAnimation();

        return;
        //Ϊ���������ע�����´���
        //if (!SystemSwitch.DestroyResource)
        //{
        //    return;
        //}

        //m_atlsCanRelease.spriteMaterial.mainTexture = null;
        //AssetCacheMgr.ReleaseResourceImmediate("MogoOperatingUI.png");
    }

    #endregion
}
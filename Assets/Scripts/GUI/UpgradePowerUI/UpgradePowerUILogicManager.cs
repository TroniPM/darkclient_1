#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：
// 创建者：HongChengguo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/
#endregion

using UnityEngine;
using System.Collections;
using Mogo.Util;
using Mogo.Game;
using Mogo.GameData;

public class UpgradePowerUILogicManager : UILogicManager 
{
    private static UpgradePowerUILogicManager m_instance;
    public static UpgradePowerUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new UpgradePowerUILogicManager();                
            }

            return UpgradePowerUILogicManager.m_instance;
        }
    }

    #region 事件

    public void Initialize()
    {
        UpgradePowerUIViewManager.Instance.UPGRADEPOWERUICLOSEUP += OnCloseUp;
        UpgradePowerUIViewManager.Instance.UPGRADEPOWERUITIPENABLEUP += OnUpgradePowerTipEnable;
        UpgradePowerUIDict.SYSTEMGRIDUP += OnSystemGridUp;

        // 属性绑定
        ItemSource = MogoWorld.thePlayer; // 由于目前Logic可能会跟随UI关闭而Release，故在此重新设置ItemSource
        SetBinding<uint>(EntityParent.ATTR_FIGHT_FORCE, UpgradePowerUIViewManager.Instance.SetPlayerCurrentPower);
        SetBinding<byte>(EntityMyself.ATTR_LEVEL, UpgradePowerUIViewManager.Instance.SetUpgradePowerGridListDetail);
    }

    public override void Release()
    {
        base.Release();
        UpgradePowerUIViewManager.Instance.UPGRADEPOWERUICLOSEUP -= OnCloseUp;
        UpgradePowerUIViewManager.Instance.UPGRADEPOWERUITIPENABLEUP -= OnUpgradePowerTipEnable;
        UpgradePowerUIDict.SYSTEMGRIDUP -= OnSystemGridUp;
    }

    void OnCloseUp()
    {
        LoggerHelper.Debug("OnUpgradePowerUICloseUp");
        MogoUIManager.Instance.ShowMogoNormalMainUI();
    }

    void OnSystemGridUp(int xmlID)
    {        
        UpgradePowerData xmlData = UpgradePowerData.GetUpgradePowerDataByID(xmlID);
        if (xmlData != null)
        {
            int linkID = xmlData.hyper; // 跳转控件id

            switch((UpgradePowerSystem)xmlData.id)
            {
                case UpgradePowerSystem.Equipment:
                    MogoUIManager.Instance.ShowEquipRecommendUI(null);
                    break;
                case UpgradePowerSystem.JewelInset:
                    MogoUIManager.Instance.SwitchInsetUI(null);
                    break;
                case UpgradePowerSystem.Rune:
                    UpgradePowerUIViewManager.Instance.gameObject.SetActive(false);
                    MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.RuneUI);
                    break;
                case UpgradePowerSystem.BodyEnhance:
                    MogoUIManager.Instance.SwitchStrenthUI(null);
                    break;
                case UpgradePowerSystem.Tong:
                    MogoUIManager.Instance.SwitchTongUI();        
                    break;
                default:
                    break;
            }                  
        }        
    }

    void OnUpgradePowerTipEnable()
    {
        UpgradePowerUIViewManager.Instance.IsShowUpgradePowerTipDialog = !UpgradePowerUIViewManager.Instance.IsShowUpgradePowerTipDialog;
        UpgradePowerUIViewManager.SaveIsShowUpgradePowerTipDialog(UpgradePowerUIViewManager.Instance.IsShowUpgradePowerTipDialog);     
    }

    /// <summary>
    /// 如果选择提示，则显示战力提升界面
    /// </summary>
    public void TryShowUpgradePowerUI()
    {
        if (Mogo.Util.SystemConfig.Instance.IsShowUpgradePowerTipDialog)
        {
            MogoUIManager.Instance.ShowUpgradePowerUI(null);
        }
    }

    #endregion
}

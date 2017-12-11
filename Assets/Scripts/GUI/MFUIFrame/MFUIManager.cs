using System.Collections.Generic;
using UnityEngine;
public class MFUIManager
{
    public enum MFUIID
    {
        None = 0,
        BattleMainUI = 1,
        CityMainUI = 2,
        DragonUI = 3,
        RuneUI = 4,
        //NewBattleRewardUI = 5,
        PassCountDownUI = 6,
        BattlePassUI = 7,
        BattlePassCardListUI = 8,
        OgreMustDieTip = 9,
        InvitFriendMessageBox = 10,
        InvitFriendListUI = 11,
        EnterWaittingMessageBox = 12,
        BattlePassUINoCard = 13,
        NewChallengeUI = 14,
        WingUI = 15,
        WingPreviewInMarketUI = 16,
        ProtectGodressTip = 17,
        EquipFXUI = 18,
        WingUIUpgradeDialog=19,
        WingUIBuyDialog =20,
        NewLoginRewardUI = 21,
        ChargeReturnWingUI = 22,
        ElfDiamondUI = 23,
        NewChargeRewardUI =24,
        RewardUI=25
    }

    public static MFUIID CurrentUI = MFUIID.CityMainUI;

    bool m_bUICanChange = true;

    public Dictionary<MFUIID, GameObject> DictUIIDToOBj = new Dictionary<MFUIID, GameObject>();
    public static Dictionary<MFUIID, string> DictUIIDToPathName = new Dictionary<MFUIID, string>();

    public static MFUIManager m_singleton = null;

    public static MFUIManager GetSingleton()
    {
        if (m_singleton == null)
        {
            m_singleton = new MFUIManager();
            RegisterUIID();
        }

        return m_singleton;
    }

    static void RegisterUIID()
    {
        DictUIIDToPathName.Add(MFUIID.DragonUI, "DragonUI.prefab");
        DictUIIDToPathName.Add(MFUIID.RuneUI, "RuneUI.prefab");
        //DictUIIDToPathName.Add(MFUIID.NewBattleRewardUI, "NewBattleRewardUI.prefab");
        DictUIIDToPathName.Add(MFUIID.PassCountDownUI, "PassCountDownUI.prefab");
        DictUIIDToPathName.Add(MFUIID.BattlePassUI,"BattlePassUI.prefab");
        DictUIIDToPathName.Add(MFUIID.BattlePassCardListUI, "BattlePassCardListUI.prefab");
        DictUIIDToPathName.Add(MFUIID.OgreMustDieTip, "OgreMustDieOpenTip.prefab");
        DictUIIDToPathName.Add(MFUIID.InvitFriendMessageBox, "InvitFriendMessageBox.prefab");
        DictUIIDToPathName.Add(MFUIID.InvitFriendListUI, "InvitFriendListUI.prefab");
        DictUIIDToPathName.Add(MFUIID.EnterWaittingMessageBox, "EnterWaittingMessageBox.prefab");
        DictUIIDToPathName.Add(MFUIID.BattlePassUINoCard, "BattlePassUINoCard.prefab");
        DictUIIDToPathName.Add(MFUIID.NewChallengeUI, "NewChallengeUI.prefab");
        DictUIIDToPathName.Add(MFUIID.WingUI, "WingUI.prefab");
        DictUIIDToPathName.Add(MFUIID.WingPreviewInMarketUI, "WingUIReviewInMarket.prefab");
        DictUIIDToPathName.Add(MFUIID.ProtectGodressTip, "ProtectGodressTip.prefab");
        DictUIIDToPathName.Add(MFUIID.EquipFXUI, "EquipFXUI.prefab");
        DictUIIDToPathName.Add(MFUIID.WingUIUpgradeDialog, "WingUIUpgradeDialog.prefab");
        DictUIIDToPathName.Add(MFUIID.WingUIBuyDialog, "WingUIBuyDialog.prefab");
        DictUIIDToPathName.Add(MFUIID.RewardUI, "RewardUI.prefab");
        DictUIIDToPathName.Add(MFUIID.ChargeReturnWingUI, "ChargeReturnWingUI.prefab");
        DictUIIDToPathName.Add(MFUIID.ElfDiamondUI, "ElfDiamondUI.prefab");
        DictUIIDToPathName.Add(MFUIID.NewChargeRewardUI, "NewChargeRewardUI.prefab");
        DictUIIDToPathName.Add(MFUIID.NewLoginRewardUI, "NewLoginRewardUI.prefab");

        Mogo.Util.EventDispatcher.AddEventListener<int>(Mogo.Util.Events.MFUIManagerEvent.SwitchUIWithLoad, m_singleton.SwitchUIWithLoad);
    }

    void SwitchUIWithLoad(int uiid)
    {
        SwitchUIWithLoad((MFUIID)uiid);
    }

    public void SwitchUIWithLoad(MFUIID targetUIID, MFUIID baseUIID = MFUIID.None, uint pri = 0,bool isAttached = false,
        MFUISwitchAnim.MFUISwitchAnimType switchInType = MFUISwitchAnim.MFUISwitchAnimType.None,
        MFUISwitchAnim.MFUISwitchAnimType switchOutType = MFUISwitchAnim.MFUISwitchAnimType.None)
    {
        //Debug.LogError(targetUIID + " " + CurrentUI);
        if (m_bUICanChange == false)
            return;

        if (CurrentUI == targetUIID)
            return;

        if (DictUIIDToOBj.ContainsKey(CurrentUI) && !isAttached)
        { 
            DictUIIDToOBj[CurrentUI].GetComponentsInChildren<MFUIUnit>(true)[0].Hide();
            //Debug.LogError(CurrentUI + " Hide");
        }

        if (targetUIID == MFUIID.None)
        {
            MogoGlobleUIManager.Instance.ShowWaitingTip(false);
            CurrentUI = MFUIID.None;
            return;
        }

        MFUIQueueManager.Instance.PushOne(() =>
            {
                if (!DictUIIDToOBj.ContainsKey(targetUIID))
                {

                    List<MFUIResourceReqInfo> list = new List<MFUIResourceReqInfo>();

                    MFUIResourceReqInfo info = new MFUIResourceReqInfo();

                    info.path = DictUIIDToPathName[targetUIID];
                    info.goName = targetUIID.ToString();
                    info.id = targetUIID;

                    list.Add(info);

                    MFUIGameObjectPool.GetSingleton().RegisterGameObjectList(list, () =>
                    {
                        switch (targetUIID)
                        {
                            case MFUIID.DragonUI:
                                MFUIGameObjectPool.GetSingleton().GetGameObject(targetUIID.ToString()).AddComponent<DragonUIViewManager>();
                                break;

                            case MFUIID.RuneUI:
                                MFUIGameObjectPool.GetSingleton().GetGameObject(targetUIID.ToString()).AddComponent<RuneUIViewManager>();
                                break;

                            //case MFUIID.NewBattleRewardUI:
                            //    MFUIGameObjectPool.GetSingleton().GetGameObject("go~").AddComponent<NewBattleRewardUIViewManager>();
                            //    break;

                            case MFUIID.PassCountDownUI:
                                MFUIGameObjectPool.GetSingleton().GetGameObject(targetUIID.ToString()).AddComponent<PassCountDownUIViewManager>();
                                break;

                            case MFUIID.BattlePassUI:
                                MFUIGameObjectPool.GetSingleton().GetGameObject(targetUIID.ToString()).AddComponent<BattlePassUIViewManager>();
                                break;

                            case MFUIID.BattlePassCardListUI:
                                MFUIGameObjectPool.GetSingleton().GetGameObject(targetUIID.ToString()).AddComponent<BattlePassCardListUIViewManager>();
                                break;

                            case MFUIID.OgreMustDieTip:
                                MFUIGameObjectPool.GetSingleton().GetGameObject(targetUIID.ToString()).AddComponent<OgreMustDieTipViewManager>();
                                break;

                            case MFUIID.InvitFriendMessageBox:
                                MFUIGameObjectPool.GetSingleton().GetGameObject(targetUIID.ToString()).AddComponent<InvitFriendMessageBoxViewManager>();
                                break;

                            case MFUIID.InvitFriendListUI:
                                MFUIGameObjectPool.GetSingleton().GetGameObject(targetUIID.ToString()).AddComponent<InvitFriendListUIViewManager>();
                                break;

                            case MFUIID.EnterWaittingMessageBox:
                                MFUIGameObjectPool.GetSingleton().GetGameObject(targetUIID.ToString()).AddComponent<EnterWaittingMessageBoxViewManager>();
                                break;

                            case MFUIID.BattlePassUINoCard:
                                MFUIGameObjectPool.GetSingleton().GetGameObject(targetUIID.ToString()).AddComponent<BattlePassUINoCardUIViewManager>();
                                break;

                            case MFUIID.NewChallengeUI:
                                MFUIGameObjectPool.GetSingleton().GetGameObject(targetUIID.ToString()).AddComponent<NewChallengeUIViewManager>();
                                break;

                            case MFUIID.WingUI:
                                MFUIGameObjectPool.GetSingleton().GetGameObject(targetUIID.ToString()).AddComponent<WingUIViewManager>();
                                break;

                            case MFUIID.WingPreviewInMarketUI:
                                MFUIGameObjectPool.GetSingleton().GetGameObject(targetUIID.ToString()).AddComponent<WingUIPreviewInMarketUIViewManager>();
                                break;
                                
                            case MFUIID.ProtectGodressTip:
                                MFUIGameObjectPool.GetSingleton().GetGameObject(targetUIID.ToString()).AddComponent<ProtectGodressTipViewManager>();
                                break;

                            case MFUIID.EquipFXUI:
                                MFUIGameObjectPool.GetSingleton().GetGameObject(targetUIID.ToString()).AddComponent<EquipFXUIViewManager>();
                                break;

                            case MFUIID.WingUIUpgradeDialog:
                                MFUIGameObjectPool.GetSingleton().GetGameObject(targetUIID.ToString()).AddComponent<WingUIUpgradeDialogViewManager>();
                                break;

                            case MFUIID.WingUIBuyDialog:
                                MFUIGameObjectPool.GetSingleton().GetGameObject(targetUIID.ToString()).AddComponent<WingUIBuyDialogViewManager>();
                                break;

                            case MFUIID.RewardUI:
                                MFUIGameObjectPool.GetSingleton().GetGameObject(targetUIID.ToString()).AddComponent<RewardUIViewManager>();
                                break;

                            case MFUIID.ChargeReturnWingUI:
                                MFUIGameObjectPool.GetSingleton().GetGameObject(targetUIID.ToString()).AddComponent<ChargeReturnWingUIViewManager>();
                                break;

                            case MFUIID.ElfDiamondUI:
                                MFUIGameObjectPool.GetSingleton().GetGameObject(targetUIID.ToString()).AddComponent<ElfDiamondUIViewManager>();
                                break;

                            case MFUIID.NewChargeRewardUI:
                                MFUIGameObjectPool.GetSingleton().GetGameObject(targetUIID.ToString()).AddComponent<NewChargeRewardUIViewManager>();
                                break;

                            case MFUIID.NewLoginRewardUI:
                                MFUIGameObjectPool.GetSingleton().GetGameObject(targetUIID.ToString()).AddComponent<NewLoginRewardUIViewManager>();
                                break;

                        }
                        SwitchUI(targetUIID, baseUIID, pri, isAttached, switchInType, switchOutType);
                    });
                }
                else
                {

                    SwitchUI(targetUIID, baseUIID, pri, isAttached, switchInType, switchOutType);
                }
            }, baseUIID, pri, targetUIID.ToString());

    }

    private void SwitchUI(MFUIID targetUIID, MFUIID baseUIID = MFUIID.None, uint pri = 0,bool isAttached = false,
        MFUISwitchAnim.MFUISwitchAnimType switchInType = MFUISwitchAnim.MFUISwitchAnimType.None,
        MFUISwitchAnim.MFUISwitchAnimType switchOutType = MFUISwitchAnim.MFUISwitchAnimType.None)
    {
        if (CurrentUI == targetUIID)
            return;

        if (!DictUIIDToOBj.ContainsKey(targetUIID))
        {
            MFUIUtils.MFUIDebug("MFUIID Dictionary not contain Obj");
            return;
        }

        if (switchOutType != MFUISwitchAnim.MFUISwitchAnimType.None)
        {
            MFUISwitchAnimNode[] animNodeArr = DictUIIDToOBj[CurrentUI].GetComponentsInChildren<MFUISwitchAnimNode>(true);

            if (animNodeArr.Length > 0)
            {
                animNodeArr[0].Play(switchOutType);
            }
            else
            {
                DictUIIDToOBj[CurrentUI].AddComponent<MFUISwitchAnimNode>().Play(switchOutType);
            }
        }

        if (switchInType != MFUISwitchAnim.MFUISwitchAnimType.None)
        {
            MFUISwitchAnimNode[] animNodeArr = DictUIIDToOBj[targetUIID].GetComponentsInChildren<MFUISwitchAnimNode>(true);

            if (animNodeArr.Length > 0)
            {
                animNodeArr[0].Play(switchInType);
            }
            else
            {
                DictUIIDToOBj[targetUIID].AddComponent<MFUISwitchAnimNode>().Play(switchInType);
            }
        }

        //DictUIIDToOBj[CurrentUI].MFUIgameObject.GetComponentsInChildren<MFUISwitchAnimNode>(true)[0].Play(switchOutType);
        //DictUIIDToOBj[targetUIID].MFUIgameObject.GetComponentsInChildren<MFUISwitchAnimNode>(true)[0].Play(switchInType);
        //Debug.LogError(CurrentUI);
        //if (DictUIIDToOBj.ContainsKey(CurrentUI) && !isAttached)
        //{
        //    DictUIIDToOBj[CurrentUI].GetComponentsInChildren<MFUIUnit>(true)[0].Hide();
        //    //Debug.LogError(CurrentUI + " Hide");
        //}



        if (DictUIIDToOBj[targetUIID].GetComponentsInChildren<MFUIUnit>(true)[0].IsUIDirty())
        {
            DictUIIDToOBj[targetUIID].GetComponentsInChildren<MFUIUnit>(true)[0].Show();
        }
        //Debug.LogError(targetUIID + " Show");
    }

    public void RegisterUI(MFUIID UIID, GameObject obj)
    {
        if (obj == null)
        {
            MFUIUtils.MFUIDebug("Register Obj is null");
            return;
        }

        if (DictUIIDToOBj.ContainsKey(UIID))
        {
            MFUIUtils.MFUIDebug("UIID is AlReady In the Dict , now Will Replace it !");
        }

        DictUIIDToOBj[UIID] = obj;
    }

    public void ReleaseUI(MFUIID id)
    {
        DictUIIDToOBj.Remove(id);
    }

    public void IsUICanChange(bool canChange)
    {
        m_bUICanChange = canChange;
    }

    //public void DetachAllBasedUI(MFUIID id)
    //{
    //    Debug.LogError(DictUIBaseIDToTargetID.Count);

    //    foreach (var item in DictUIBaseIDToTargetID)
    //    {
    //        Debug.LogError(item.Key + " " + item.Value[0]);
    //    }

    //    if (DictUIBaseIDToTargetID.ContainsKey(id))
    //    {
    //        for (int i = 0; i < DictUIBaseIDToTargetID[id].Count; ++i)
    //        {
    //            Debug.LogError(DictUIIDToOBj[DictUIBaseIDToTargetID[id][i]].name+" @@@@@@@@@@@@@@@@@@@@");
    //            DictUIIDToOBj[DictUIBaseIDToTargetID[id][i]].GetComponentsInChildren<MFUIUnit>(true)[0].Hide();
    //        }

    //        DictUIBaseIDToTargetID.Remove(id);
    //    }
    //}

    //填坑函数

    public void ReleaseDragonUI(GameObject go)
    {
        if (MFUIGameObjectPool.GetSingleton().m_listResPathToPoolUnit.ContainsKey("DragonUI.prefab"))
        {
            MFUIGameObjectPool.GetSingleton().m_listResPathToPoolUnit.Remove("DragonUI.prefab");
        }
        MFUIResourceManager.GetSingleton().ReleasePreLoadResource(MFUIID.DragonUI);
        //AssetCacheMgr.ReleaseInstance(go);
        GameObject.Destroy(go);
        ReleaseUI(MFUIID.DragonUI);
    }

    public void ReleaseRuneUI(GameObject go)
    {
        if (MFUIGameObjectPool.GetSingleton().m_listResPathToPoolUnit.ContainsKey("RuneUI.prefab"))
        {
            MFUIGameObjectPool.GetSingleton().m_listResPathToPoolUnit.Remove("RuneUI.prefab");
        }

        MFUIResourceManager.GetSingleton().ReleasePreLoadResource(MFUIID.RuneUI);
        GameObject.Destroy(go);
        ReleaseUI(MFUIID.RuneUI);
    }

    public void ManualHideUI(MFUIID id)
    {
        if (DictUIIDToOBj.ContainsKey(id))
        {
            DictUIIDToOBj[id].GetComponentsInChildren<MFUIUnit>(true)[0].Hide();
        }
    }
}

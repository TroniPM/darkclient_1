// ģ����   :  PrefabScriptManager
// ������   :  Ī׿��
// �������� :  2013-8-1
// ��    �� :  PrefabScriptManager

using UnityEngine;
using System.Collections;

public class PrefabScriptManager : MonoBehaviour
{

    void Awake()
    {
        GameObject.Find("MogoNotice").AddComponent<MogoNotice>();
        GameObject.Find("MogoNotice2").AddComponent<MogoNotice2>();
        GameObject.Find("MsgBoxPanel").AddComponent<MogoMsgBox>();
        GameObject.Find("EquipTipRoot").AddComponent<EquipTipManager>();
        GameObject.Find("EquipUpgrade").AddComponent<EquipUpgradeViewManager>();
        GameObject.Find("EquipExchange").AddComponent<EquipExchangeUIViewManager>();
        GameObject.Find("BattleRecord").AddComponent<BattleRecordUIViewManager>();
        GameObject.Find("Enhant").AddComponent<FumoUIViewManager>();

        GameObject mogoMainUIPanel = GameObject.Find("MogoMainUIPanel");
        GameObject.Find("MogoMainUI").transform.Find("Camera").gameObject.AddComponent<BillboardViewManager>();
        mogoMainUIPanel.transform.Find("DebugUI").gameObject.AddComponent<DebugUIViewManager>();

        GameObject.Find("MogoGlobleUIPanel").AddComponent<MogoGlobleUIManager>();
        mogoMainUIPanel.AddComponent<MogoUIManager>();
        //mogoMainUIPanel.transform.FindChild("TeachUICamera/Anchor/TeachUIPanel").gameObject.AddComponent<TeachUIViewManager>();
        GameObject.Find("TeachUIPanel").AddComponent<TeachUIViewManager>();
        GameObject.Find("MogoGlobleUIPanel").transform.Find("MogoGlobleLoadingUI").gameObject.AddComponent<MogoGlobleLoadingUI>();
        GameObject.Find("MogoGlobleUIPanel").transform.Find("PassRewardUI").gameObject.AddComponent<PassRewardUI>();
        //GameObject.Find("BillboardPanel").transform.FindChild("SandFX").gameObject.AddComponent<SandFX>().scrollSpeed = 3f;

        mogoMainUIPanel.AddComponent<MogoUIQueue>();
        mogoMainUIPanel.AddComponent<MogoOKCancelBoxQueue>();

        mogoMainUIPanel.transform.Find("MainUI").gameObject.AddComponent<MainUIViewManager>();
        mogoMainUIPanel.transform.Find("NormalMainUI").gameObject.AddComponent<NormalMainUIViewManager>();
        mogoMainUIPanel.transform.Find("NormalMainUI").gameObject.SetActive(true);
        mogoMainUIPanel.transform.Find("NormalMainUI").gameObject.SetActive(false);
    }
}

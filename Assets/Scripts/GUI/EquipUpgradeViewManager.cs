/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：EquipUpgradeUIManager
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections.Generic;
using Mogo.Util;

public class EquipUpgradeViewManager : MogoUIParent
{
    static public EquipUpgradeViewManager Instance;

    public const string ON_UPGRADE = "EquipUpgradeViewManager.ON_UPGRADE";
    public const string ON_CLICK_MATERIAL = "EquipUpgradeViewManager.ON_CLICK_MATERIAL";

    private UISprite m_oldEquipIconFg;
    private UISprite m_oldEquipIconBg;
    private UISprite m_newEquipIconFg;
    private UISprite m_newEquipIconBg;
    private UILabel m_powerLbl;
    private UILabel m_goldLbl;
    private Transform m_materialListRoot;
    private Vector3 m_materialListRootPos;

    private List<GameObject> m_goList = new List<GameObject>();

    const string MATERIAL_ICON_PREFAB = "EquipUpgradeCostMaterialIcon.prefab";
    const int HORIZONTAL_GAP = 110;
    private EquipUpgradeViewData m_viewData;



    void Awake()
    {
        Instance = this;

        base.Init();

        m_oldEquipIconFg = GetUIChild("EquipUpgradeIconOldFG").GetComponent<UISprite>();
        m_oldEquipIconBg = GetUIChild("EquipUpgradeIconOldBG").GetComponent<UISprite>();
        m_newEquipIconBg = GetUIChild("EquipUpgradeIconNewBG").GetComponent<UISprite>();
        m_newEquipIconFg = GetUIChild("EquipUpgradeIconNewFG").GetComponent<UISprite>();
        m_powerLbl = GetUIChild("EquipUpgradePower").GetComponent<UILabel>();
        m_goldLbl = GetUIChild("EquipUpgradeCostGoldNumLbl").GetComponent<UILabel>();
        m_materialListRoot = GetUIChild("EquipUpgradeCostMaterialIconList");
        m_materialListRootPos = GetUIChild("EquipUpgradeCostMaterialIconPos").transform.localPosition;


        GetUIChild("EquipUpgradeBtn").gameObject.AddComponent<MogoUIListener>().MogoOnClick = OnUpgrade;
        GetUIChild("EquipUpgradeContainerCloseBtn").gameObject.AddComponent<MogoUIListener>().MogoOnClick = () =>
        { transform.gameObject.SetActive(false); };
        transform.gameObject.SetActive(false);
    }

    private void OnUpgrade()
    {
        EventDispatcher.TriggerEvent(ON_UPGRADE);
    }

    public class EquipUpgradeViewData
    {
        public int oldEquipId;
        public int newEquipId;
        public string power;
        public List<int> materialIdList;
        public List<string> materilNumStrList;//"20/20","10/20"
        public string needGold;
    }

    public void CloseUI()
    {
        transform.gameObject.SetActive(false);
    }

    public void SetViewData(EquipUpgradeViewData data)
    {
        m_viewData = data;
    }

    public void ShowMainUI(bool isShow = true)
    {
        transform.gameObject.SetActive(isShow);

        if (!isShow) return;
        ClearOld();
        InventoryManager.SetIcon(m_viewData.oldEquipId, m_oldEquipIconFg, 0, null, m_oldEquipIconBg);
        InventoryManager.SetIcon(m_viewData.newEquipId, m_newEquipIconFg, 0, null, m_newEquipIconBg);
        m_powerLbl.text = m_viewData.power;
        m_goldLbl.text = m_viewData.needGold;
        for (int i = 0; i < m_viewData.materialIdList.Count; i++)
        {
            int index = i;
            AssetCacheMgr.GetUIInstance(MATERIAL_ICON_PREFAB, (str, id, obj) =>
            {
                GameObject go = obj as GameObject;
                Utils.MountToSomeObjWithoutPosChange(go.transform, m_materialListRoot);

                go.transform.localPosition = new Vector3(go.transform.localPosition.x + index * HORIZONTAL_GAP, go.transform.localPosition.y, go.transform.localPosition.z);

                UISprite bg = go.transform.Find("EquipUpgradeCostMaterialIconBG").GetComponent<UISprite>();
                UISprite fg = go.transform.Find("EquipUpgradeCostMaterialIconFG").GetComponent<UISprite>();
                UILabel lbl = go.transform.Find("EquipUpgradeCostMaterialIconNum").GetComponent<UILabel>();
                InventoryManager.SetIcon(m_viewData.materialIdList[index], fg, 0, null, bg);
                lbl.text = m_viewData.materilNumStrList[index];

                go.AddComponent<MogoUIListener>().MogoOnClick = () => { OnClickMaterial(index); };
                m_goList.Add(go);

            });
        }

        //int offset = (m_viewData.materialIdList.Count - 1) * HORIZONTAL_GAP / 2;
        //m_materialListRoot.localPosition = new Vector3(m_materialListRootPos.x - offset, m_materialListRootPos.y, m_materialListRootPos.z);


    }

    private void ClearOld()
    {
        foreach (GameObject go in m_goList)
        {
            AssetCacheMgr.ReleaseInstance(go);
        }
        m_goList.Clear();

    }

    private void OnClickMaterial(int index)
    {
        EventDispatcher.TriggerEvent<int>(ON_CLICK_MATERIAL, index);
    }
}
using UnityEngine;
using System.Collections;
using Mogo.GameData;
using System.Linq;

public class EquipFXUILogicManager : MFUILogicUnit
{

    static EquipFXUILogicManager m_instance;

    public static EquipFXUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new EquipFXUILogicManager();

              
            }

            return EquipFXUILogicManager.m_instance;
        }
    }

    Shader m_shaderFlowLight;
    Shader m_playerShader;

    public EquipFXUILogicManager()
    {
        AssetCacheMgr.GetUIResource("FlowLightShaderWithTwirl.shader", (obj) => { m_shaderFlowLight = (Shader)obj; });
        AssetCacheMgr.GetUIResource("PlayerShader.shader", (obj) => { m_playerShader = (Shader)obj; });

        foreach (var item in EquipSpecialEffectData.dataMap)
        {
            if (item.Value.group == 1)
            {
                ++JewelFXNum;
            }
            else if (item.Value.group == 2)
            {
                ++EquipFXNum;
            }
            else if (item.Value.group == 3)
            {
                ++StrenthFXNum;
            }
        }

        Mogo.Util.EventDispatcher.AddEventListener<int, int>("EquipFXUIActiveBtnUp", OnActiveBtnUp);
        Mogo.Util.EventDispatcher.AddEventListener<byte,byte>("ActiveSepciaclEffectsResp", OnActiveFXResp);

    }

    ~EquipFXUILogicManager()
    {
        Mogo.Util.EventDispatcher.RemoveEventListener<int, int>("EquipFXUIActiveBtnUp", OnActiveBtnUp);
        Mogo.Util.EventDispatcher.RemoveEventListener<byte,byte>("ActiveSepciaclEffectsResp", OnActiveFXResp);
    }
    void OnActiveBtnUp(int id,int currentPage)
    {
        int fxId = -1;

        //switch (currentPage)
        //{
        //    case 0:
        //        fxId = id;
        //        break;

        //    case 1:
        //        fxId = id + JewelFXNum;
        //        break;

        //    case 2:
        //        fxId = id + JewelFXNum + EquipFXNum;
        //        break;

        //    default:
        //        fxId = -1;
        //        break;
        //}
        foreach (var item in EquipSpecialEffectData.dataMap)
        {
            if (item.Value.group == currentPage+1)
            {
                if (item.Value.level == id+1)
                {
                    fxId = item.Key;
                    break;
                }
            }
        }

        if (EquipFXUIViewManager.Instance.m_listCanActiveFXID.Contains(id))
        {
            MogoWorld.thePlayer.RpcCall("ActiveSepciaclEffectsReq", fxId);
        }
        else
        {
            MogoWorld.thePlayer.UpdateEquipFX((uint)(fxId));
        }
        //MogoWorld.thePlayer.UpdateEquipFX((uint)(fxId + 1));
        //System.Collections.Generic.List<GameObject> listGo = MogoWorld.thePlayer.GameObject.GetComponent<ActorParent>().WeaponObj;
        //for (int i = 0; i < listGo.Count; ++i)
        //{
        //    listGo[i].GetComponentsInChildren<MeshRenderer>(true)[0].material.shader = m_shaderFlowLight;
        //}
        //GameObject Weapon = MogoWorld.thePlayer.GameObject.GetComponent<ActorParent>().WeaponObj[0];

        //Weapon.GetComponentsInChildren<MeshRenderer>(true)[0].material.shader = m_shaderFlowLight;
    }

    void OnActiveFXResp(byte id,byte errorCode)
    {
        Debug.LogError(errorCode+" "+id);
        switch (errorCode)
        {
            case 0:
                MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(7534));
                Debug.LogError(LanguageData.GetContent(7534));
                MogoWorld.thePlayer.UpdateEquipFX(id);
                break;

            case 1:
                MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(7535));
                Debug.LogError(LanguageData.GetContent(7535));
                break;

            case 2:
                MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(7536));
                Debug.LogError(LanguageData.GetContent(7536));
                break;

            case 3:
                MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(7537));
                Debug.LogError(LanguageData.GetContent(7537));
                break;

            case 4:
                MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(7538));
                Debug.LogError(LanguageData.GetContent(7538));
                break;

            case 5:
                MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(7539));
                Debug.LogError(LanguageData.GetContent(7539));
                break;

            case 6:
                MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(7540));
                Debug.LogError(LanguageData.GetContent(7540));
                break;
        }
    }

    public int JewelFXNum;
    public int EquipFXNum;
    public int StrenthFXNum;

    System.Action actDirty;
    MFUIResult resultRefreshFXGrid = new MFUIResult();

    public void SetUIDirty()
    {
        actDirty = MFUIUtils.SafeDoAction(EquipFXUIViewManager.Instance, () =>
        {
            EquipFXUIViewManager.Instance.SetUIDirty();
        });
    }

    public override void FillBufferedData()
    {
        if (actDirty != null)
        {
            actDirty();
        }

        if (resultRefreshFXGrid.isFailed)
        {
            RefreshFXGrid((System.Collections.Generic.List<EquipFXUIGridData>)resultRefreshFXGrid.buffer);
        }
    }

    public void RefreshFXGrid(System.Collections.Generic.List<EquipFXUIGridData> list)
    {
        resultRefreshFXGrid = MFUIUtils.SafeSetValue_W(EquipFXUIViewManager.Instance,
            () => { EquipFXUIViewManager.Instance.RefreshFXGrid(list); }, list);
    }

    public int CalculateJewelMarks(int level)
    {
        int dataID = level;
        int totalMarks = 0;

        foreach (var item in EquipSpecialEffectData.dataMap)
        {
            if (item.Value.group == 1)
            {
                if (item.Value.level == level)
                {
                    dataID = item.Key;
                    break;
                }
            }
        }

        foreach (var item in InventoryManager.Instance.EquipOnDic)
        {
            for (int i = 0; i < item.Value.jewelSlots.Count; ++i)
            {
                if (item.Value.jewelSlots[i] != -1)
                {
                    Debug.LogError(dataID);
                    totalMarks += EquipSpecialEffectData.dataMap[dataID].scoreList[ItemParentData.GetItem(
                        item.Value.jewelSlots[i]).level];
                }
            }
        }

        return totalMarks;
    }

    public int CalculateEquipMarks(int level)
    {
        //InventoryManager.Instance.EquipOnDic[1-11].quality
        int dataID = level;
        int jewelFXNum = 0;

        //foreach (var item in EquipSpecialEffectData.dataMap)
        //{
        //    if (item.Value.group == 1)
        //    {
        //        ++jewelFXNum;
        //    }
        //}

        //dataID = level + jewelFXNum; //计算装备特效等级对应表ID

        foreach (var item in EquipSpecialEffectData.dataMap)
        {
            if (item.Value.group == 2)
            {
                if (item.Value.level == level)
                {
                    dataID = item.Key;
                    break;
                }
            }
        }

        int totalMarks = 0;

        foreach (var item in InventoryManager.Instance.EquipOnDic)
        {
            totalMarks += EquipSpecialEffectData.dataMap[dataID].scoreList[item.Value.quality];
        }

        return totalMarks;
    }

    public int CalculateStrenthMarks(int level)
    {
        //BodyEnhanceManager.Instance.myEnhance[1-10]

        int dataID = level;
        int jewelAndEquipNum = 0;

        //foreach (var item in EquipSpecialEffectData.dataMap)
        //{
        //    if (item.Value.group == 1 || item.Value.group == 2)
        //    {
        //        ++jewelAndEquipNum;
        //    }
        //}

        //dataID = level + jewelAndEquipNum;

        foreach (var item in EquipSpecialEffectData.dataMap)
        {
            if (item.Value.group == 3)
            {
                if (item.Value.level == level)
                {
                    dataID = item.Key;
                    break;
                }
            }
        }

        int totalMarks = 0;

        foreach (var item in BodyEnhanceManager.Instance.myEnhance)
        {
            if (item.Value != 0)
            {
                totalMarks += EquipSpecialEffectData.dataMap[dataID].scoreList[item.Value];
            }
        }

        return totalMarks;
    }



    public void FillJewelFXGridList(LuaTable FXData = null)
    {
        if (FXData == null)
            return;

        System.Collections.Generic.List<EquipFXUIGridData> listData = new System.Collections.Generic.List<EquipFXUIGridData>();
        var data = EquipSpecialEffectData.dataMap.OrderBy(x => x.Key);

        foreach (var item in data)
        {
            if (item.Value.group == 1)
            {
                int currentMark = CalculateJewelMarks(item.Value.level);
                int totalMark = item.Value.activeScore;
                EquipFXUIGridData gridData = new EquipFXUIGridData();
                gridData.gridIcon = "";//ItemParentData.GetItem(item.Value.icon).Icon;
                if (FXData.ContainsKey(item.Key.ToString()))
                {
                    gridData.isActive = true;
                }
                else
                {
                    gridData.isActive = false;
                }
                gridData.gridProgressText = string.Concat(currentMark, " / ", totalMark);
                gridData.gridProgressSize = (float)currentMark / (float)totalMark;
                gridData.gridText = LanguageData.GetContent(item.Value.activeDesp);
                listData.Add(gridData);
            }
        }

        RefreshFXGrid(listData);
    }

    public void FillEquipFXGridList(LuaTable FXData = null)
    {
        if (FXData == null)
            return;

        System.Collections.Generic.List<EquipFXUIGridData> listData = new System.Collections.Generic.List<EquipFXUIGridData>();
        var data = EquipSpecialEffectData.dataMap.OrderBy(x => x.Key);

        foreach (var item in data)
        {
            if (item.Value.group == 2)
            {
                int currentMark = CalculateEquipMarks(item.Value.level);
                int totalMark = item.Value.activeScore;
                EquipFXUIGridData gridData = new EquipFXUIGridData();
                gridData.gridIcon = "";// = ItemParentData.GetItem(item.Value.icon).Icon;

                if (FXData.ContainsKey(item.Key.ToString()))
                {
                    gridData.isActive = true;
                }
                else
                {
                    gridData.isActive = false;
                }
                gridData.gridProgressText = string.Concat(currentMark, " / ", totalMark);
                gridData.gridProgressSize = (float)currentMark / (float)totalMark;
                gridData.gridText = LanguageData.GetContent(item.Value.activeDesp);

                listData.Add(gridData);
            }
        }

        RefreshFXGrid(listData);
    }

    public void FillStrenthFXGridList(LuaTable FXData = null)
    {
        if (FXData == null)
            return;

        System.Collections.Generic.List<EquipFXUIGridData> listData = new System.Collections.Generic.List<EquipFXUIGridData>();
        var data = EquipSpecialEffectData.dataMap.OrderBy(x => x.Key);

        foreach (var item in data)
        {
            if (item.Value.group == 3)
            {
                int currentMark = CalculateStrenthMarks(item.Value.level);
                int totalMark = item.Value.activeScore;
                EquipFXUIGridData gridData = new EquipFXUIGridData();
                gridData.gridIcon = "";//ItemParentData.GetItem(item.Value.icon).Icon;
                gridData.isActive = false;
                gridData.gridProgressText = string.Concat(currentMark, " / ", totalMark);
                gridData.gridProgressSize = (float)currentMark / (float)totalMark;
                gridData.gridText = LanguageData.GetContent(item.Value.activeDesp);

                listData.Add(gridData);
            }
        }

        RefreshFXGrid(listData);
    }
}

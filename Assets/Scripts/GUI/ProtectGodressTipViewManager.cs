using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProtectGodressTipViewManager : MFUIUnit
{
    private static ProtectGodressTipViewManager m_instance;
    public static ProtectGodressTipViewManager Instance { get{ return ProtectGodressTipViewManager.m_instance;}}

    public static float GridHeight = 28f*3;
    System.Collections.Generic.List<GameObject> m_listReward = new System.Collections.Generic.List<GameObject>();

    public override void CallWhenLoadResources()
    {
        m_instance = this;
        ID = MFUIManager.MFUIID.ProtectGodressTip;
        MFUIManager.GetSingleton().RegisterUI(ID, m_myGameObject);
        MFUIUtils.AttachWidget(m_myTransform, GameObject.Find("MogoMainUIPanel").transform);
        m_myGameObject.name = "ProtectGodressTip";
        MFUIGameObjectPool.GetSingleton().NotRegisterGameObjectList(ID);
    }

    public override void CallWhenCreate()
    {
        AttachLogicUnit(ProtectGodressTipLogicManager.Instance);

        RegisterButtonHandler("ProtectGodressTipCloseBtn");
        SetButtonClickHandler("ProtectGodressTipCloseBtn", OnCloseBtnUp);

        GetTransform("ProtectGodressTipRewardListCam").GetComponent<UIViewport>().sourceCamera =
            MogoUIManager.Instance.GetMainUICamera();
    }

    public override void CallWhenUnloadResources()
    {
        m_instance = null;
    }

    public override void CallWhenShow()
    {
        MFUIUtils.ShowGameObject(true, m_myGameObject);
        MogoMainCamera.instance.SetActive(false);
    }

    public override void CallWhenHide()
    {
        MFUIUtils.ShowGameObject(false, m_myGameObject);
        MogoMainCamera.instance.SetActive(true);
        MFUIGameObjectPool.GetSingleton().DestroyGameObject(m_myGameObject);
    }

    void OnCloseBtnUp(int id)
    {
        MogoUIManager.Instance.ShowMogoNormalMainUI();
    }

    public void SetTipText(int id, string text)
    {
        SetLabelText(string.Concat("ProtectGodressTipText", id, "Content"), text);
    }

    #region 奖励列表

    private readonly static float OffsetY = -180.0f;

    public void SetTipRewardList(List<string> listReward)
    {
        if (listReward == null)
            return;

        for (int i = 0; i < listReward.Count; ++i)
        {
            int index = i;
            AssetCacheMgr.GetUIInstance("ProtectGodressTipReward.prefab", (name, id, go) =>
                {
                    GameObject gameObj = (GameObject)go;
                    gameObj.name = string.Concat("ProtectGodressTipReward", index);
                    MFUIUtils.AttachWidget(gameObj.transform, GetTransform("ProtectGodressTipRewardList"));
                    gameObj.transform.localPosition = new Vector3(-520, -GridHeight * index, 0);

                    UILabel lblText = gameObj.transform.Find("ProtectGodressTipRewardText").GetComponentsInChildren<UILabel>(true)[0];
                    lblText.text = listReward[index];
                    //GameObject goDOT1 = gameObj.transform.FindChild("ProtectGodressTipRewardDOT1").gameObject;
                    //GameObject goDOT2 = gameObj.transform.FindChild("ProtectGodressTipRewardDOT2").gameObject;              

                    if (listReward.Count > 5)
                    {
                        GetTransform("ProtectGodressTipRewardListCam").GetComponent<MyDragableCamera>().MINY =
                            OffsetY - (listReward.Count - 5) * GridHeight;
                    }
                    else
                    {
                        GetTransform("ProtectGodressTipRewardListCam").GetComponent<MyDragableCamera>().MINY = OffsetY;
                    }

                    m_listReward.Add(gameObj);
                });
        }

    }

    public void ClearRewardList()
    {
        for (int i = 0; i < m_listReward.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(m_listReward[i]);
        }

        m_listReward.Clear();
        GetTransform("ProtectGodressTipRewardListCam").transform.localPosition = new Vector3(0, OffsetY, 0);
    }

    #endregion
}

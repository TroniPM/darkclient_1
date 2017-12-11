using UnityEngine;
using System.Collections;
using Mogo.Util;
using System.Collections.Generic;
using System;

public class InstanceUIRebornViewManager : MonoBehaviour
{
    private static InstanceUIRebornViewManager m_instance;

    public static InstanceUIRebornViewManager Instance
    {
        get
        {
            return InstanceUIRebornViewManager.m_instance;
        }
    }

    public int i = 0;

    private Transform m_myTransform;
    GameObject m_tranInstanceRebornUI;
    private Transform m_instanceRebornItemList;
    private int m_RebornItemNum;
    private List<GameObject> m_listInstanceRebornItem = new List<GameObject>();

    private Dictionary<string, string> m_widgetToFullName = new Dictionary<string, string>();

    public Action INSTANCEREBORNLEFTUP;
    public Action INSTANCEREBORNUP;

    const float REWARDITEMSPACE = 0.153f;

    public void AddWigetToFullNameData(string widgetName, string fullName)
    {
        if (m_widgetToFullName.ContainsKey(widgetName))
            LoggerHelper.Debug(widgetName);
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

    void Awake()
    {
        m_myTransform = transform;

        FillFullNameData(m_myTransform);

        m_instanceRebornItemList = m_myTransform.Find(m_widgetToFullName["InstanceRebornItemList"]);

        m_tranInstanceRebornUI = gameObject;

        m_instance = gameObject.GetComponent<InstanceUIRebornViewManager>();

        Initialize();
    }

    public void Initialize()
    {
        InstanceUIDict.ButtonTypeToEventUp.Add("InstanceRebornUILeft", OnRebornUILeftUp);
        InstanceUIDict.ButtonTypeToEventUp.Add("InstanceRebornUIReborn", OnRebornUIRebornUp);

        InstanceUIRebornLogicManager.Instance.Initialize();
    }

    public void Release()
    {
        InstanceUIRebornLogicManager.Instance.Release();
    }

    void OnRebornUILeftUp(int i)
    {
        if (INSTANCEREBORNLEFTUP != null)
            INSTANCEREBORNLEFTUP();
        //m_tranInstanceRebornUI.SetActive(false);
        //m_tranInstanceChooseUI.SetActive(true);
        //m_tranInstanceUIMaskBG.SetActive(true);
    }

    void OnRebornUIRebornUp(int i)
    {
        if (INSTANCEREBORNUP != null)
        {
            INSTANCEREBORNUP();
        }

        //m_tranInstanceRebornUI.SetActive(false);
        //m_tranMainUI.SetActive(true);
        //m_tranInstanceUIMaskBG.SetActive(false);
    }
    //public void ShowInstanceRebornUI(bool show)
    //{
    //    m_tranInstanceRebornUI.SetActive(show);
    //}

    public void SetInstanceRebornListItem(List<ItemParent> list)
    {
        m_RebornItemNum = list.Count;

        AddRebornItem(m_RebornItemNum);
    }


    public void SetRebornItemData(int id, string imgName, string itemName)
    {
        m_listInstanceRebornItem[id].transform.Find("InstanceRewardItemFG").GetComponentsInChildren<UISprite>(true)[0].spriteName = imgName;
        m_listInstanceRebornItem[id].transform.Find("InstanceRewardItemText").GetComponentsInChildren<UILabel>(true)[0].text = itemName;
    }


    public void ClearRebornItemList()
    {
        for (int i = 0; i < m_listInstanceRebornItem.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(m_listInstanceRebornItem[i]);
        }

        m_listInstanceRebornItem.Clear();
    }



    void AddRebornItem(int num, Action act = null)
    {
        //GameObject obj;

        ClearRebornItemList();

        for (int i = 0; i < num; ++i)
        {
            //obj = (GameObject)Instantiate(Resources.Load("GUI/InstanceRewardItem"));
            //obj.transform.parent = m_instanceRebornItemList;
            //obj.transform.localPosition = new Vector3(REWARDITEMSPACE * i, 0, 0);
            //obj.transform.localScale = new Vector3(0.0008f, 0.0008f, 0.0008f);
            //m_listInstanceRebornItem.Add(obj);

            int index = i;

            AssetCacheMgr.GetUIInstance("InstanceRewardItem.prefab", (prefab, guid, go) =>
            {
                GameObject obj = (GameObject)go;
                obj.transform.parent = m_instanceRebornItemList;
                obj.transform.localPosition = new Vector3(REWARDITEMSPACE * index, 0, 0);
                obj.transform.localScale = new Vector3(0.0008f, 0.0008f, 1);
                m_listInstanceRebornItem.Add(obj);

                if (index == num - 1)
                {
                    if (m_listInstanceRebornItem.Count <= 5)
                    {

                        m_myTransform.Find(m_widgetToFullName["InstanceRebornItemListCamera"]).localPosition =
                            m_myTransform.Find(m_widgetToFullName["InstancePassReborn" + num + "ItemStartPos"]).localPosition;
                    }
                    else
                    {
                        m_myTransform.Find(m_widgetToFullName["InstanceRebornItemListCamera"]).localPosition =
                            m_myTransform.Find(m_widgetToFullName["InstancePassReborn5ItemStartPos"]).localPosition;
                    }

                    if (act != null)
                    {
                        act();
                    }
                }
            });
        }

        //if (num <= 5)
        //{

        //    m_myTransform.FindChild(m_widgetToFullName["InstanceRebornItemListCamera"]).localPosition =
        //        m_myTransform.FindChild(m_widgetToFullName["InstancePassReborn" + num + "ItemStartPos"]).localPosition;
        //}
        //else
        //{
        //    m_myTransform.FindChild(m_widgetToFullName["InstanceRebornItemListCamera"]).localPosition =
        //        m_myTransform.FindChild(m_widgetToFullName["InstancePassReborn5ItemStartPos"]).localPosition;
        //}
    }


    public void AddRebornItem<T, U>(int num, Action<T, U> act, T arg1, U arg2)
    {
        //GameObject obj;

        ClearRebornItemList();

        for (int i = 0; i < num; ++i)
        {
            //obj = (GameObject)Instantiate(Resources.Load("GUI/InstanceRewardItem"));
            //obj.transform.parent = m_instanceRebornItemList;
            //obj.transform.localPosition = new Vector3(REWARDITEMSPACE * i, 0, 0);
            //obj.transform.localScale = new Vector3(0.0008f, 0.0008f, 0.0008f);
            //m_listInstanceRebornItem.Add(obj);

            int index = i;

            AssetCacheMgr.GetUIInstance("InstanceRewardItem.prefab", (prefab, guid, go) =>
            {
                GameObject obj = (GameObject)go;
                obj.transform.parent = m_instanceRebornItemList;
                obj.transform.localPosition = new Vector3(REWARDITEMSPACE * index, 0, 0);
                obj.transform.localScale = new Vector3(0.0008f, 0.0008f, 1);
                m_listInstanceRebornItem.Add(obj);

                if (index == num - 1)
                {
                    if (m_listInstanceRebornItem.Count <= 5)
                    {

                        m_myTransform.Find(m_widgetToFullName["InstanceRebornItemListCamera"]).localPosition =
                            m_myTransform.Find(m_widgetToFullName["InstancePassReborn" + num + "ItemStartPos"]).localPosition;
                    }
                    else
                    {
                        m_myTransform.Find(m_widgetToFullName["InstanceRebornItemListCamera"]).localPosition =
                            m_myTransform.Find(m_widgetToFullName["InstancePassReborn5ItemStartPos"]).localPosition;
                    }

                    if (act != null)
                    {
                        act(arg1, arg2);
                    }
                }
            });
        }
    }

    public void AddRebornUIItemList(List<int> itemId, List<string> itemName, Action act = null)
    {
        for (int i = 0; i < itemId.Count; ++i)
        {
            int index = i;

            AssetCacheMgr.GetUIInstance("InstanceRewardItem.prefab", (prefab, guid, go) =>
            {
                GameObject obj = (GameObject)go;
                obj.transform.parent = m_instanceRebornItemList;
                obj.transform.localPosition = new Vector3(REWARDITEMSPACE * index, 0, 0);
                obj.transform.localScale = new Vector3(0.0008f, 0.0008f, 1);

                UISprite spFG = obj.transform.Find("InstanceRewardItemFG").GetComponentsInChildren<UISprite>(true)[0];
                UISprite spBG = obj.transform.Find("InstanceRewardItemBG").GetComponentsInChildren<UISprite>(true)[0];
                UILabel lblText = obj.transform.Find("InstanceRewardItemText").GetComponentsInChildren<UILabel>(true)[0];

                obj.GetComponent<MyDragCamera>().RelatedCamera = m_myTransform.Find("InstanceRebornItemList/InstanceRebornItemListCamera").GetComponent<Camera>();

                InventoryManager.SetIcon(itemId[index], spFG, 0, null, spBG);
                lblText.text = itemName[index];


                m_listInstanceRebornItem.Add(obj);

                if (index == itemId.Count - 1)
                {
                    if (m_listInstanceRebornItem.Count <= 5)
                    {

                        m_myTransform.Find(m_widgetToFullName["InstanceRebornItemListCamera"]).localPosition =
                            m_myTransform.Find(m_widgetToFullName["InstancePassReborn" + itemId.Count + "ItemStartPos"]).localPosition;
                    }
                    else
                    {
                        m_myTransform.Find(m_widgetToFullName["InstanceRebornItemListCamera"]).localPosition =
                            m_myTransform.Find(m_widgetToFullName["InstancePassReborn5ItemStartPos"]).localPosition;
                    }

                    if (act != null)
                    {
                        act();
                    }
                }
            });
        }
    }
}
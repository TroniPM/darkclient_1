using System.Collections.Generic;
using UnityEngine;

public class MFUIGameObjectPool
{

    Dictionary<MFUIManager.MFUIID, int> m_dictUIIDToUnInstanceObjCount = new Dictionary<MFUIManager.MFUIID, int>();

    public Dictionary<string, List<MFUIGameObjectPoolUnit>> m_listResPathToPoolUnit =
        new Dictionary<string, List<MFUIGameObjectPoolUnit>>(); //ResourceName -> GameObject (By MaiFeo)

    Dictionary<string, MFUIManager.MFUIID> m_dictGoNameToUIID = new Dictionary<string, MFUIManager.MFUIID>();   //GameObjectName -> MFUIID (By MaiFeo)
    Dictionary<MFUIManager.MFUIID, System.Action> m_dictUIIDToCallBack = new Dictionary<MFUIManager.MFUIID, System.Action>(); // MFUIID -> CallBack (By MaiFeo)

    public System.Action<MFUIManager.MFUIID> UIResourcesLoadedCB;

    public static MFUIGameObjectPool m_singleton = null;

    public static MFUIGameObjectPool GetSingleton()
    {
        if (m_singleton == null)
        {
            m_singleton = new MFUIGameObjectPool();
        }

        return m_singleton;
    }

    public void ResourceLoaded(string goName)
    {
        foreach (var item in m_dictGoNameToUIID)
        {
            if (item.Key == goName)
            {
                MFUIManager.MFUIID uiID = item.Value;

                if (m_dictUIIDToUnInstanceObjCount.ContainsKey(uiID))
                {
                    --m_dictUIIDToUnInstanceObjCount[uiID];

                    if (m_dictUIIDToUnInstanceObjCount[uiID] == 0)
                    {

                        if (m_dictUIIDToCallBack.ContainsKey(uiID))
                        {
                            if (m_dictUIIDToCallBack[uiID] != null)
                            {
                                m_dictUIIDToCallBack[uiID]();
                            }
                            else
                            {
                                if (UIResourcesLoadedCB != null)
                                {
                                    UIResourcesLoadedCB(uiID);
                                }
                            }

                            m_dictUIIDToCallBack.Remove(uiID);
                        }
                        else
                        {
                            if (UIResourcesLoadedCB != null)
                            {
                                UIResourcesLoadedCB(uiID);
                            }
                        }
                    }
                }

                m_dictGoNameToUIID.Remove(item.Key);

                break;
            }
        }
    }

    public void NotRegisterGameObjectList(MFUIManager.MFUIID id)
    {
        UIResourcesLoadedCB(id);
    }

    public void RegisterGameObjectList(List<MFUIResourceReqInfo> list, System.Action callBack = null,bool preLoad = false)
    {

        for (int i = 0; i < list.Count; ++i)
        {
            if (!m_dictUIIDToUnInstanceObjCount.ContainsKey(list[i].id))
            {
                m_dictUIIDToUnInstanceObjCount.Add(list[i].id, 1);
            }
            else
            {
                ++m_dictUIIDToUnInstanceObjCount[list[i].id];
            }
        }

        for (int i = 0; i < list.Count; ++i)
        {
            if (m_dictGoNameToUIID.ContainsKey(list[i].goName))
            {
                MFUIUtils.MFUIDebug("Same GameObject Name Registered now will Replace it !");
            }

            m_dictGoNameToUIID[list[i].goName] = list[i].id;
            m_dictUIIDToCallBack[list[i].id] = callBack;

            RegisterGameObject(list[i].id,list[i].path, list[i].goName,preLoad);
        }
    }

    private void RegisterGameObject(MFUIManager.MFUIID id ,string path, string goName,bool preLoad = false)
    {

        if (!m_listResPathToPoolUnit.ContainsKey(path))
        {

            List<MFUIGameObjectPoolUnit> listUnit = new List<MFUIGameObjectPoolUnit>();

            m_listResPathToPoolUnit.Add(path, listUnit);

            MFUIResourceManager.GetSingleton().LoadInstance(id,path, goName,preLoad);
        }
        else
        {
            //Debug.LogError("LoadResouce " + path);
            List<MFUIGameObjectPoolUnit> listUnit = m_listResPathToPoolUnit[path];

            for (int i = 0; i < listUnit.Count; ++i)
            {
                if (listUnit[i].isFree)
                {
                    listUnit[i].isFree = false;
                    listUnit[i].poolUnit.name = goName;
                    MFUIGameObjectPool.GetSingleton().ResourceLoaded(goName);
                    return;
                }
            }

            MFUIResourceManager.GetSingleton().LoadInstance(id,path, goName,preLoad);

        }

    }

    public GameObject GetGameObject(string goName)
    {
        foreach (var item in m_listResPathToPoolUnit)
        {
            for (int i = 0; i < item.Value.Count; ++i)
            {
                if (item.Value[i].poolUnit.name == goName)
                {
                    return item.Value[i].poolUnit;
                }
            }
        }
 
        return null;
    }

    public void DestroyGameObject(GameObject go)
    {
        foreach (var item in m_listResPathToPoolUnit)
        {
            for (int i = 0; i < item.Value.Count; ++i)
            {
                if (item.Value[i].poolUnit == go)
                {
                    GameObject.DestroyImmediate(item.Value[i].poolUnit);
                   // AssetCacheMgr.ReleaseInstance(item.Value[i].poolUnit);
                    item.Value.RemoveAt(i);

                    //if (item.Value.Count <= 0)
                    //{
                    //    Debug.LogError("ReleaseObj");
                    //    MFUIResourceManager.GetSingleton().ReleaseObject(item.Key);
                    //    m_listResPathToPoolUnit.Remove(item.Key);
                    //}
                    return;
                }
            }
        }
    }

    public void ReleaseGameObject(GameObject go)
    {
        foreach (var item in m_listResPathToPoolUnit)
        {
            for (int i = 0; i < item.Value.Count; ++i)
            {
                if (item.Value[i].poolUnit == go)
                {
                    item.Value[i].poolUnit.SetActive(false);
                    item.Value[i].isFree = true;
                }
            }
        }
    }

    public void ReleaseGameObject(string goName)
    {
        foreach (var item in m_listResPathToPoolUnit)
        {
            for (int i = 0; i < item.Value.Count; ++i)
            {
                if (item.Value[i].poolUnit.name == goName)
                {
                    item.Value[i].poolUnit.SetActive(false);
                    item.Value[i].isFree = true;
                }
            }
        }
    }
}

public class MFUIGameObjectPoolUnit
{
    public GameObject poolUnit;
    public bool isFree;
}

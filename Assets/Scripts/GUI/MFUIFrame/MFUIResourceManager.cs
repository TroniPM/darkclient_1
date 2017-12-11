using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MFUIResourceManager
{

    Dictionary<string, Object> m_dictResNameToObj = new Dictionary<string, Object>();
    Dictionary<MFUIManager.MFUIID, List<Object>> m_dictUIIDToObjList = new Dictionary<MFUIManager.MFUIID, List<Object>>();

    public static MFUIResourceManager m_singleton = null;

    public static MFUIResourceManager GetSingleton()
    {
        if (m_singleton == null)
        {
            m_singleton = new MFUIResourceManager();
        }

        return m_singleton;
    }

    //public void LoadResource(string path,MFUIManager.MFUIID id, GameObject outGO)
    //{
    //    if (m_dictResNameToObj.ContainsKey(path))
    //    {
    //        outGO = (GameObject)(GameObject.Instantiate(m_dictResNameToObj[path]));
    //        MFUIGameObjectPool.GetSingleton().ResourceLoaded(id);
    //    }
    //    else
    //    {
    //        Object obj = Resources.Load(path);

    //        if (obj == null)
    //        {
    //            MFUIUtils.MFUIDebug(string.Concat(path, " Not Found ! "));
    //            return;
    //        }
    //        m_dictResNameToObj.Add(path, obj);

    //        outGO = (GameObject)(GameObject.Instantiate(obj));
    //        MFUIGameObjectPool.GetSingleton().ResourceLoaded(id);
    //    }
    //}

    public void PreLoadResource(string path,MFUIManager.MFUIID id)      //预加载资源存放到UIID->Object Dict & resName->Object
    {
        if (!m_dictUIIDToObjList.ContainsKey(id))
        {
            m_dictUIIDToObjList.Add(id, new List<Object>());
        }

        if (!m_dictResNameToObj.ContainsKey(path))
        {
            AssetCacheMgr.GetUIResource(path, (obj) =>
            {
                m_dictResNameToObj.Add(path, obj);

                //if(!m_dictUIIDToObjList.ContainsKey(id))
                //{
                //    m_dictUIIDToObjList[id].Add(obj);
                //}

                if (!m_dictUIIDToObjList[id].Contains(obj))
                {
                    m_dictUIIDToObjList[id].Add(obj);
                }
                

            });
        }
        else
        {
            //if (!m_dictUIIDToObjList.ContainsKey(id))
            //{
            //    //m_dictUIIDToObjList[id].Add(m_dictResNameToObj[path]);
            //}

            if (!m_dictUIIDToObjList[id].Contains(m_dictResNameToObj[path]))
            {
                m_dictUIIDToObjList[id].Add(m_dictResNameToObj[path]);
            }
        }
    }

    public List<Object> GetPreLoadResource(MFUIManager.MFUIID id)       //获取预加载的资源 null为未加载
    {
        if (m_dictUIIDToObjList.ContainsKey(id))
        {
            return m_dictUIIDToObjList[id];
        }

        return null;
    }

    public void ReleasePreLoadResource(MFUIManager.MFUIID id)       //释放预加载的资源
    {
        if (!SystemSwitch.DestroyAllUI)
            return;
        if(!m_dictUIIDToObjList.ContainsKey(id))
            return;


        List<Object> objList = m_dictUIIDToObjList[id];

        foreach (var item in objList)
        {
            foreach (var item1 in m_dictResNameToObj)
            {
                if (item == item1.Value)
                {
                    AssetCacheMgr.ReleaseResourceImmediate(item1.Key);
                    m_dictResNameToObj.Remove(item1.Key);

                    break;
                }
            }
        }

        m_dictUIIDToObjList.Remove(id);
    }

    public void LoadResource(MFUIManager.MFUIID id, string path,System.Action<Object> callBack)
    {
        if (m_dictResNameToObj.ContainsKey(path))
        {
            if (callBack != null)
            {
                callBack(m_dictResNameToObj[path]);
            }
        }
        else
        {
            AssetCacheMgr.GetUIResource(path, (obj) => 
            {
                m_dictResNameToObj[path] = obj;

                if (!m_dictUIIDToObjList.ContainsKey(id))
                {
                    m_dictUIIDToObjList.Add(id, new List<Object>());
                }

                if (!m_dictUIIDToObjList[id].Contains(obj))
                {
                    m_dictUIIDToObjList[id].Add(obj);
                }

                callBack(obj);
            });
        }
    }

    public void LoadInstance(MFUIManager.MFUIID id,string path, string goName,bool preLoad) //加载资源并实例化
    {
        if (m_dictResNameToObj.ContainsKey(path))
        {
            MFUIGameObjectPoolUnit unit = new MFUIGameObjectPoolUnit();
            unit.isFree = false;
            unit.poolUnit = (GameObject)(GameObject.Instantiate(m_dictResNameToObj[path]));
            unit.poolUnit.name = goName;

            if (preLoad)
            {
                unit.isFree = true;
                MFUIUtils.ShowGameObject(false, unit.poolUnit);
            }

            MFUIGameObjectPool.GetSingleton().m_listResPathToPoolUnit[path].Add(unit);

            MFUIGameObjectPool.GetSingleton().ResourceLoaded(goName);
        }
        else
        {

            AssetCacheMgr.GetUIResource(path, (obj) =>
            {
                if (obj == null)
                {
                    MFUIUtils.MFUIDebug(string.Concat(path, " Not Found ! "));
                    return;
                }

                if (m_dictResNameToObj.ContainsKey(path))
                {
                    MFUIUtils.MFUIDebug("Same Key in ResNameToObj Dict , Now Replace It");
                }
                //m_dictResNameToObj.Add(path, obj);
                m_dictResNameToObj[path] = obj;

                if (!m_dictUIIDToObjList.ContainsKey(id))
                {
                    m_dictUIIDToObjList.Add(id, new List<Object>());
                }

                if (!m_dictUIIDToObjList[id].Contains(obj))
                {
                    m_dictUIIDToObjList[id].Add(obj);
                }

                MFUIGameObjectPoolUnit unit = new MFUIGameObjectPoolUnit();
                unit.isFree = false;
                unit.poolUnit = (GameObject)(GameObject.Instantiate(m_dictResNameToObj[path]));
                unit.poolUnit.name = goName;

                if (preLoad)
                {
                    unit.isFree = true;
                    MFUIUtils.ShowGameObject(false, unit.poolUnit);
                }

                MFUIGameObjectPool.GetSingleton().m_listResPathToPoolUnit[path].Add(unit);

                MFUIGameObjectPool.GetSingleton().ResourceLoaded(goName);
            });
            //Object obj = Resources.Load(path);

            //if (obj == null)
            //{
            //    MFUIUtils.MFUIDebug(string.Concat(path, " Not Found ! "));
            //    return;
            //}
            //m_dictResNameToObj.Add(path, obj);

            //MFUIGameObjectPoolUnit unit = new MFUIGameObjectPoolUnit();
            //unit.isFree = false;
            //unit.poolUnit = (GameObject)(GameObject.Instantiate(m_dictResNameToObj[path]));
            //unit.poolUnit.name = goName;

            //MFUIGameObjectPool.GetSingleton().m_listResPathToPoolUnit[path].Add(unit);

            //MFUIGameObjectPool.GetSingleton().ResourceLoaded(goName);
        }
    }

    public void ReleaseObject(Object obj)
    {
        foreach (var item in m_dictResNameToObj)
        {
            if (item.Value == obj)
            {
                AssetCacheMgr.ReleaseResourceImmediate(obj);
                obj = null;
                m_dictResNameToObj.Remove(item.Key);
                break;
            }
        }

    }

    public void ReleaseObject(string path)
    {
        foreach (var item in m_dictResNameToObj)
        {
            if (item.Key == path)
            {
                AssetCacheMgr.ReleaseResourceImmediate(item.Key);
                m_dictResNameToObj.Remove(item.Key);
                break;
            }
        }
    }
}

public class MFUIResourceReqInfo
{
    public string path;
    public MFUIManager.MFUIID id;
    public string goName;
}

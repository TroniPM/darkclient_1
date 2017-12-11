using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class MaiFeoMemoryPool
{

    protected List<GameObject> m_listPoolObj = new List<GameObject>();
    protected List<int> m_listPoolObjFree = new List<int>();
    protected Dictionary<int, GameObject> m_dictIDToObj = new Dictionary<int, GameObject>();


    public abstract void Initialize(int num);

    public abstract void AddOne();

    public GameObject GetOne()
    {
        //if (m_listPoolObjFree.Count < 5)
        //{
        //    Mogo.Util.LoggerHelper.Debug("Pool num less than 5!!!!!!!!!!!!!!!!!!!!!!");
        //    AddOne();
        //}

        if (m_listPoolObjFree.Count > 0)
        {
            int id = Random.Range(0, m_listPoolObjFree.Count);

            int i = m_listPoolObjFree[id];
            m_listPoolObjFree.RemoveAt(id);
            m_dictIDToObj[i].SetActive(true);
            return m_dictIDToObj[i];
        }
        else
        {
            Mogo.Util.LoggerHelper.Debug("MemoryPool has not free Obj !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            return m_dictIDToObj[0];
        }
    }

    public void ReturnOne(GameObject go)
    {
        if (m_dictIDToObj.ContainsValue(go))
        {
            for (int i = 0; i < m_dictIDToObj.Count; ++i)
            {
                if (m_dictIDToObj[i] == go)
                {
                    m_listPoolObjFree.Add(i);
                    m_dictIDToObj[i].transform.parent = GameObject.Find("Driver").transform;
                    m_dictIDToObj[i].transform.localPosition = Vector3.zero;
                    m_dictIDToObj[i].SetActive(false);

                    break;
                }
            }
        }
    }

    public void Release()
    {
        for (int i = 0; i < m_listPoolObj.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(m_listPoolObj[i]);
        }

        m_listPoolObj.Clear();
        m_listPoolObjFree.Clear();
        m_dictIDToObj.Clear();
    }
}

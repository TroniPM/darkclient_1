using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SplitBattleBillboardPool : MaiFeoMemoryPool
{
    public override void Initialize(int num)
    {

        Transform driverTrans = GameObject.Find("Driver").transform;

        for (int i = 0; i < num; ++i)
        {
            int index = i;

            AssetCacheMgr.GetUIInstance("SplitBattleBillboard.prefab", (prefab, guid, gameObject) =>
            {
                ((GameObject)gameObject).transform.parent = driverTrans;
                m_listPoolObj.Add((GameObject)(gameObject));
                ((GameObject)gameObject).AddComponent<BattleBillboardAnim>();
                m_listPoolObjFree.Add(index);
                m_dictIDToObj.Add(index, ((GameObject)gameObject));
            });

            if (index == num-1)
            {
                Mogo.Util.LoggerHelper.Debug("Pool Init Finished             ");
            }
        }
    }

    public override void AddOne()
    {
        Transform driverTrans = GameObject.Find("Driver").transform;

        AssetCacheMgr.GetUIInstance("SplitBattleBillboard.prefab", (prefab, guid, gameObject) =>
        {
            ((GameObject)gameObject).transform.parent = driverTrans;
            m_listPoolObj.Add((GameObject)(gameObject));
            ((GameObject)gameObject).AddComponent<BattleBillboardAnim>();
            m_listPoolObjFree.Add(m_listPoolObj.Count-1);
            m_dictIDToObj.Add(m_listPoolObj.Count - 1, ((GameObject)gameObject));
        });
    }
}

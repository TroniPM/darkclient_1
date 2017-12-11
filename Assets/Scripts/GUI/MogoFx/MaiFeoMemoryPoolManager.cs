using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MaiFeoMemoryPoolType
{
    PoolType_SplitBattleBillboard = 0,
    PoolType_AloneBattleBillobard = 1,
    PoolType_SuperBattleBillboard = 2,
    PoolType_Shadow = 3
 
}                                                         

public class MaiFeoMemoryPoolManager
{

    static MaiFeoMemoryPoolManager m_instance;

    int m_iPoolID = 0;

    Dictionary<int, MaiFeoMemoryPool> m_dictMemoryPool = new Dictionary<int, MaiFeoMemoryPool>();

    public static MaiFeoMemoryPoolManager GetSingleton()
    {
        if(m_instance == null)
        {
            m_instance = new MaiFeoMemoryPoolManager();
        }

        return m_instance;
    }

    public MaiFeoMemoryPoolManager()
    {
    }

    public int CreatePool(MaiFeoMemoryPoolType type,int num)
    {
        MaiFeoMemoryPool pool = null;

        switch (type)
        {
            case MaiFeoMemoryPoolType.PoolType_AloneBattleBillobard:
                pool = new AloneBattleBillboardPool();
                pool.Initialize(num);
                break;

            case MaiFeoMemoryPoolType.PoolType_Shadow:
                break;

            case MaiFeoMemoryPoolType.PoolType_SplitBattleBillboard:
                pool = new SplitBattleBillboardPool();
                pool.Initialize(num);
                break;

            case MaiFeoMemoryPoolType.PoolType_SuperBattleBillboard:
                pool = new SuperBattleBillboardPool();
                pool.Initialize(num);
                break;
        }

        m_dictMemoryPool.Add(m_iPoolID, pool);

        return m_iPoolID++;
    }

    public MaiFeoMemoryPool GetPoolByID(int id)
    {
        return m_dictMemoryPool[id];
    }
}

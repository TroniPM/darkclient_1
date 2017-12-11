using UnityEngine;
using System.Collections;

public class SuperBattleBillboardAnim : MonoBehaviour
{

    void FadeFinished()
	{
        
        MaiFeoMemoryPoolManager.GetSingleton().GetPoolByID(BillboardViewManager.Instance.DictPoolTypeToID[MaiFeoMemoryPoolType.PoolType_SuperBattleBillboard]).ReturnOne(gameObject);
	}
}

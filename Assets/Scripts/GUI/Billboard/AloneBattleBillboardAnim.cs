using UnityEngine;
using System.Collections;

public class AloneBattleBillboardAnim : MonoBehaviour
{

    void FadeFinished()
	{
        
        MaiFeoMemoryPoolManager.GetSingleton().GetPoolByID(BillboardViewManager.Instance.DictPoolTypeToID[MaiFeoMemoryPoolType.PoolType_AloneBattleBillobard]).ReturnOne(gameObject);
	}
}

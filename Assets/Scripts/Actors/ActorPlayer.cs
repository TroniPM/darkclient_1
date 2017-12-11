using UnityEngine;
using System.Collections;

using Mogo.Util;
using Mogo.Game;
using Mogo.GameData;

public class ActorPlayer : ActorPlayer<EntityPlayer> { }

public class ActorPlayer<T> : ActorParent<T> where T : EntityPlayer
{
	protected Transform m_billboardTrans;
    protected Transform m_wingBone;
    GameObject m_goWing;
	// 初始化
	void Start()
	{
        //gameObject.layer = 8;
		//theEntity.motor = this.gameObject.AddComponent<MogoMotorServer>();
		if (isNeedInitEquip) InitEquipment();

		m_billboardTrans = transform.Find("slot_billboard");
        
	}

	// 每帧调用
	void Update()
	{
        ActChange();
		if (m_billboardTrans != null && theEntity != null)
		{
            //EventDispatcher.TriggerEvent<Vector3, uint>(BillboardViewManager.BillboardViewEvent.UPDATEBILLBOARDPOS,

            //  m_billboardTrans.position, theEntity.ID);
            BillboardViewManager.Instance.UpdateBillboardPos(m_billboardTrans.position, theEntity.ID);
		}

        
	}

    private string currWing;
    public void AddWing(string wingName,System.Action callBack)
    {
        if (m_wingBone == null)
        {
            m_wingBone = transform.Find("Bip_master/Bip001 Pelvis/Bip001 Spine/bip_wing");
        }
        if (currWing == wingName)
        {
            return;
        }
        currWing = wingName;
        AssetCacheMgr.GetInstance(wingName, (name, id, obj) => 
        {
            m_goWing = (GameObject)obj;
            m_goWing.transform.parent = m_wingBone;
            m_goWing.transform.localPosition = Vector3.zero;
            m_goWing.transform.localEulerAngles = new Vector3(0, 90, 90);
            
            switch (theEntity.vocation)
            {
                case Vocation.Warrior:
                    m_goWing.transform.localScale = new Vector3(1.7f, 1.7f, 1.7f);
                    break;

                case Vocation.Mage:
                    m_goWing.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                    break;

                case Vocation.Assassin:
                    m_goWing.transform.localScale = new Vector3(1.15f, 1.15f, 1.15f);
                    break;
                case Vocation.Archer:
                    m_goWing.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                    break;
            }

            
            if (callBack!=null)
                callBack();
        });
    }

    public void RemoveWing()
    {
        if (m_goWing)
        {
            currWing = "";
            AssetCacheMgr.ReleaseInstance(m_goWing);
            m_goWing = null;
        }
    }

    public void SetLayer(int layer)
    {
        SetObjectLayer(layer, m_goWing);
    }

    public void SetObjectLayer(int layer, GameObject obj)
    {
        if (!obj)
            return;

        obj.layer = layer;

        foreach (Transform item in obj.transform)
        {
            SetObjectLayer(layer, item.gameObject);
        }
    }

}

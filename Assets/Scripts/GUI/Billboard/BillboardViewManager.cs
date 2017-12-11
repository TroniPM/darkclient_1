/*----------------------------------------------------------------
// Copyright (C) 2013 ���ݣ�����
//
// ģ������BillboardViewManager
// �����ߣ�MaiFeo
// �޸����б��
// �������ڣ�
// ģ��������Billboard��ͼ������
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;
using Mogo.Game;
using Mogo.View;
using Mogo.GameData;

public enum BILLBOARDTYPE
{
    BiggerRed = 1,
    NormalGreen = 2
};

public class BillboardViewManager : MogoUIBehaviour
{
    private static BillboardViewManager m_instance;
    public static BillboardViewManager Instance { get { return BillboardViewManager.m_instance; } }

    public Camera RelatedCamera;
    public Camera ViewCamera;
    GameObject goBillboard;
    GameObject goBillboardBlood;

    GameObject m_goInfoBillboard;

    float m_fDeltaTime = 0;

    UIFont m_font;
    UIAtlas m_atlas;

    Transform m_stage;
    // Ϊȥ��������ʱ�������´���
    //bool m_bIsFirst = true;

    UISlicedSprite[] m_ssIcon = new UISlicedSprite[3];

    Dictionary<uint, Head> m_heads = new Dictionary<uint, Head>();
    Dictionary<uint, Transform> m_trans = new Dictionary<uint, Transform>();


    public List<AloneBattleBillboard> AloneBattleBillboardBuffer = new List<AloneBattleBillboard>();

    public Dictionary<MaiFeoMemoryPoolType, int> DictPoolTypeToID = new Dictionary<MaiFeoMemoryPoolType, int>();

    GameObject billboardList;

    public static class BillboardViewEvent
    {
        public const string UPDATEBILLBOARDPOS = "UpdateBillboardPos";
        public const string REMOVEBILLBOARD = "RemoveBillboard";
        public const string ADDTASKBILLBOARD = "AddTaskBillboard";
        public const string ADDINFOBILLBOARD = "AddInfoBillboard";
        public const string ADDBATTLEBILLBOARD = "AddBattleBillboard";
    }

    void Start()
    {
       billboardList =  transform.Find("Anchor/MogoMainUIPanel/BillboardList").gameObject;
    }
    void OnUpdateBillboardPos(Vector3 position, uint playerid)
    {
        //����transform�����ڵ��õ�ʱ���Ѿ����٣���Ҫȷ�ϵ���������ʱ��
        //var t = m_myTransform.FindChild(playerid).transform.position = position;

        if (!m_heads.ContainsKey(playerid))
        {
            return;
        }
        if (!ViewCamera || !RelatedCamera)//�ڴ��������Ŀ���п��ܻ��
            return;
        position = ViewCamera.ScreenToWorldPoint(RelatedCamera.WorldToScreenPoint(position));
        m_heads[playerid].UpdatePosi(position);
    }

    public void UpdateBillboardPos(Vector3 position, uint playerid)
    {
        if (!m_heads.ContainsKey(playerid))
        {
            return;
        }
        if (!ViewCamera || !RelatedCamera)
            return;
        position = ViewCamera.ScreenToWorldPoint(RelatedCamera.WorldToScreenPoint(position));
        m_heads[playerid].UpdatePosi(position);
    }

    /// <summary>
    /// ����Ѫ��ֵ
    /// </summary>
    /// <param name="blood"></param>
    /// <param name="playerid"></param>
    void OnSetBillboardBlood(float blood, uint playerid)
    {
        if (m_heads.ContainsKey(playerid))
        {
            (m_heads[playerid] as PlayerHead).SetBillboardBlood(blood);
        }
    }

    /// <summary>
    /// ����ŭ��ֵ
    /// </summary>
    /// <param name="anger"></param>
    /// <param name="playerid"></param>
    void OnSetBillboardAnger(float anger, uint playerid)
    {
        if (m_heads.ContainsKey(playerid))
        {
            (m_heads[playerid] as PlayerHead).SetBillboardAnger(anger);
        }
    }

    /// <summary>
    /// ����Ѫ��ֵ
    /// </summary>
    /// <param name="blood"></param>
    /// <param name="playerid"></param>
    public void SetBillboardBlood(float blood, uint playerid)
    {
        if (m_heads.ContainsKey(playerid))
        {
            (m_heads[playerid] as PlayerHead).SetBillboardBlood(blood);
        }
    }

    /// <summary>
    /// ����ŭ��ֵ
    /// </summary>
    /// <param name="anger"></param>
    /// <param name="playerid"></param>
    public void SetBillboardAnger(float anger, uint playerid)
    {
        if (m_heads.ContainsKey(playerid))
        {
            (m_heads[playerid] as PlayerHead).SetBillboardAnger(anger);
        }
    }

    void OnSetBillboardName(string name, uint playerid)
    {
        //if (m_heads.ContainsKey(playerid))
        //{
        //    (m_heads[playerid] as PlayerHead).SetName(name);
        //}
    }

    void OnSetBillboardTong(string tong, uint playerid)
    {
        LoggerHelper.Debug("Hererererereerer");
        if (m_heads.ContainsKey(playerid))
        {
            (m_heads[playerid] as PlayerHead).SetTong(tong);
        }
    }

    public void ShowBillboardList(bool isShow)
    {
        TimerHeap.AddTimer(1, 0, () => { billboardList.SetActive(isShow); }); //��Щ�ط�����trigger����ֱ�ӵ��� u3d������������ Ϊ�˼��� �ӳٴ������ʱ�� by MaiFeo
    }
    public void RemoveBillboard(uint playerid)
    {
        if (!m_heads.ContainsKey(playerid))
        {
            return;
        }
        m_heads[playerid].Remove();
        m_heads.Remove(playerid);
        m_trans.Remove(playerid);
    }

    public void AddTaskBillboard(uint playerId)
    {
        var head = new NPCHead(playerId);

        Quaternion qu = new Quaternion();
        qu.eulerAngles = Vector3.zero;

        head.AddToParent(m_stage, qu);
        m_heads.Add(playerId, head);
    }

    void OnLoadPlayerHeadFinished(PlayerHead head, uint playerId, Transform trans, EntityParent obj)
    {
        Quaternion qu = new Quaternion();
        qu.eulerAngles = Vector3.zero;

        head.AddToParent(m_stage, qu);

        if (m_heads.ContainsKey(playerId) == false)
        {
            m_heads.Add(playerId, head);
        }

        if (m_trans.ContainsKey(playerId) == false)
        {
            m_trans.Add(playerId, trans);
        }

        SetHead(obj);
    }

    #region ͷ��Ѫ��

    public void AddInfoBillboard(uint playerId, Transform trans, EntityParent self, bool showBlood, HeadBloodColor bloodColor = HeadBloodColor.Red, bool showAnger = false)
    {
        //if (trans == null)
        //    return;

        if (m_heads.ContainsKey(playerId))
        {
            //LoggerHelper.Warning("Same player id: " + playerId);
            var head = m_heads[playerId] as PlayerHead;
            head.ShowBillboardBlood(showBlood);
            head.ShowBillboardAnger(showAnger);
            head.SetBillboardBloodColor(bloodColor);
        }
        else
        {
            var head = new PlayerHead(playerId, trans, OnLoadPlayerHeadFinished, self, showBlood, bloodColor, showAnger);
        }        
    }

    #endregion

    #region ͷ������

    public enum HeadStatus
    {
        Normal = 0, // ��ͨ���
        PVP = 1, // PVP
    }

    public enum PVPCamp
    {
        CampOwn = 1,
        CampEnemy = 2,
    }

    public class HeadNameColor
    {
        public const string Green = "[1FFF5A]";
        public const string Red = "[C10A01]";
    }

    /// <summary>
    /// ���û����ͷ������
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="headStatus"></param>
    /// <param name="pvpCamp">��Ӫ[PVPר��]</param>
    public void SetHead(EntityParent entity, HeadStatus headStatus = HeadStatus.Normal, PVPCamp pvpCamp = PVPCamp.CampOwn)
    {
        if (!m_heads.ContainsKey(entity.ID))
        {
            return;
        }
        var head = m_heads[entity.ID] as PlayerHead;

        // [����Լ�]
        if (entity is EntityMyself)
        {
            // ����Լ�����ǰ�ӵȼ�
            if (MogoUIManager.IsShowLevel)
            {
                switch (headStatus)
                {
                    case HeadStatus.Normal:
                        head.SetName(string.Concat("[5ef5ff]", "LV", entity.level, "[-]") + " " + string.Concat("[13C5D9]", entity.name, "[-]"));
                        break;
                    case HeadStatus.PVP:
                        break;
                    default:
                        break;
                }                
            }
            // ����ʾ�ȼ�
            else
            {
                switch (headStatus)
                {
                    case HeadStatus.Normal:
                        head.SetName(string.Concat("[13C5D9]", entity.name, "[-]"));
                        break;
                    case HeadStatus.PVP:
                        break;
                    default:
                        break;
                }                
            }

            head.SetTestInfo(entity.ID.ToString());

            if (IsShowTestInfo)
            {
                head.ShowTestInfo(true);
            }
        }
        // [�������]
        else if (entity is EntityPlayer)
        {
            // ����������ǰ�ӵȼ�
            if (MogoUIManager.IsShowLevel)
            {
                switch (headStatus)
                {
                    case HeadStatus.Normal:
                        head.SetName(string.Concat("[84e747]", "LV", entity.level, "[-]") + " " + string.Concat("[11AE21]", entity.name, "[-]"));
                        break;
                    case HeadStatus.PVP:
                        {
                            // �Լ���Ӫ���(��ɫ)
                            if (pvpCamp == PVPCamp.CampOwn)
                            {
                                head.SetName(string.Concat("[84e747]", "LV", entity.level, "[-]") + " " + string.Concat(HeadNameColor.Green, entity.name, "[-]"));
                            }
                            // ������Ӫ���(��ɫ)
                            else
                            {
                                head.SetName(string.Concat("[84e747]", "LV", entity.level, "[-]") + " " + string.Concat(HeadNameColor.Red, entity.name, "[-]"));
                            }
                        }
                        break;
                    default:
                        break;
                }               
            }
            // ����ʾ�ȼ�
            else
            {
                switch (headStatus)
                {
                    case HeadStatus.Normal:
                        head.SetName(string.Concat("[11AE21]", entity.name, "[-]"));
                        break;
                    case HeadStatus.PVP:
                        {
                            // �Լ���Ӫ���(��ɫ)
                            if (pvpCamp == PVPCamp.CampOwn)
                            {
                                head.SetName(string.Concat(HeadNameColor.Green, entity.name, "[-]"));
                            }
                            // ������Ӫ���(��ɫ)
                            else
                            {
                                head.SetName(string.Concat(HeadNameColor.Red, entity.name, "[-]"));
                            }
                        }
                        break;
                    default:
                        break;
                }                      
            }

            head.SetTestInfo(entity.ID.ToString());

            if (IsShowTestInfo)
            {
                head.ShowTestInfo(true);
            }
        }
        else if (entity is EntityDummy)
        {
            head.SetTestInfo(entity.ID.ToString());

            if (IsShowTestInfo)
            {
                head.ShowTestInfo(true);
            }
        }
        else if (entity is EntityMercenary)
        {
            // С���
            if (entity.ID == MogoWorld.theLittleGuyID)
            {
                head.SetName(HeadNameColor.Green + entity.name + "[-]");
                head.SetTestInfo(entity.ID.ToString());

                if (IsShowTestInfo)
                {
                    head.ShowTestInfo(true);
                }
            }
            else
            {
                MonsterData.MonsterType type = MonsterData.MonsterType.bigBoss;
                if ((entity as EntityMercenary).IsMonster())
                {
                    type = (MonsterData.MonsterType)(entity as EntityMercenary).MonsterData.monsterType;
                }
                else if ((entity as EntityMercenary).IsPVP())
                {
                    type = MonsterData.MonsterType.PVP;
                }
                switch (type)
                {
                    case MonsterData.MonsterType.bigBoss:
                        head.SetName("[FFD200]" + entity.name + "[-]");
                        head.SetTestInfo(entity.ID.ToString());

                        if (IsShowTestInfo)
                        {
                            head.ShowTestInfo(true);
                        }
                        break;
                    case MonsterData.MonsterType.smallBoss:
                        head.SetName("[FF7E00]" + entity.name + "[-]");
                        head.SetTestInfo(entity.ID.ToString());

                        if (IsShowTestInfo)
                        {
                            head.ShowTestInfo(true);
                        }
                        break;
                    case MonsterData.MonsterType.PVP:
                        if (MogoUIManager.IsShowLevel)
                        {
                            head.SetName(("[FF7E00]" + "LV" + entity.level + " " + entity.name + "[-]"));
                        }
                        else
                        {
                            head.SetName(("[FF7E00]" + entity.name + "[-]"));
                        }

                        head.SetTestInfo(entity.ID.ToString());

                        if (IsShowTestInfo)
                        {
                            head.ShowTestInfo(true);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        else if (entity is EntityNPC)
        {
            head.SetName("[FFD200]" + entity.name + "[-]");
        }

        //head.SetBillboardBlood((float)entity.hp / (float)entity.maxHp);
    }

    #endregion

    void Update()
    {
        //position = ViewCamera.ScreenToWorldPoint(RelatedCamera.WorldToScreenPoint(position));
        //m_heads[playerid].UpdatePosi(position);

        foreach (var item in m_trans)
        {
            if (item.Value != null)
            {
                Vector3 pos = ViewCamera.ScreenToWorldPoint(RelatedCamera.WorldToScreenPoint(item.Value.position));
                m_heads[item.Key].UpdatePosi(pos);
            }
        }

        if (AloneBattleBillboardBuffer.Count > 0)
        {

            if (m_fDeltaTime > 0.3f)
            {
                m_fDeltaTime = 0;

                if (AloneBattleBillboardBuffer[0] != null)
                {
                    AloneBattleBillboardBuffer[0].EnableBillboard();

                    AloneBattleBillboardBuffer.RemoveAt(0);
                }
            }

            m_fDeltaTime += Time.deltaTime;
        }
        else
        {
            m_fDeltaTime = 0f;
        }

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    SplitBattleBillboard sb;
        //    AloneBattleBillboard ab;
        //    SuperBattleBillboard sbb;

        //    switch (Random.Range(3, 5))
        //    {
        //        case 0:
        //            sbb = new CriticalMonster(999);
        //            break;

        //        case 1:
        //            sbb = new CriticalPlayer(888);
        //            break;

        //        case 2:
        //            sbb = new BrokenAttack(777);
        //            break;

        //        case 3:
        //            sb = new NormalPlayer(678);
        //            break;

        //        case 4:
        //            sb = new NormalMonster(555);
        //            break;

        //        case 5:
        //            sb = new Miss(999);
        //            break;

        //        case 6:
        //            new GetGold(888);
        //            break;

        //        case 7:
        //            new GetExp(9999);
        //            break;
        //    }
        //}
    }

    public void AddAloneBattleBillboard(Vector3 pos, int num, AloneBattleBillboardType type)
    {
        //return; //By MaiFeo
        AloneBattleBillboard ab = null;

        switch (type)
        {
            case AloneBattleBillboardType.Exp:
                LoggerHelper.Debug("---------Get Exp" + num);
                ab = new GetExp(num);
                break;

            case AloneBattleBillboardType.Gold:
            default:
                LoggerHelper.Debug("---------Get Gold" + num);
                ab = new GetGold(num);
                break;
        }

        ab.SetBillboardPos(pos);
    }

    public void AddSplitBattleBillboard(Vector3 pos, int blood, SplitBattleBillboardType type)
    {
        //return;
        //var head = new MonsterHead(text, type);

        //Quaternion qu = new Quaternion();
        //qu.eulerAngles = new Vector3(0, 0, 0);

        //head.AddToParent(m_stage, qu, pos);

        SplitBattleBillboard sb = null;
        SuperBattleBillboard ssb = null;

        switch (type)
        {

            case SplitBattleBillboardType.CriticalMonster:
                ssb = new CriticalMonster(blood);
                break;

            case SplitBattleBillboardType.CriticalPlayer:
                ssb = new CriticalPlayer(blood);
                break;

            case SplitBattleBillboardType.BrokenAttack:
                ssb = new BrokenAttack(blood);
                break;

            case SplitBattleBillboardType.NormalMonster:
                sb = new NormalMonster(blood);
                break;

            case SplitBattleBillboardType.NormalPlayer:
                sb = new NormalPlayer(blood);
                break;

            case SplitBattleBillboardType.Miss:
            default:
                sb = new Miss(blood);
                break;
        }

        if (sb != null)
        {
            sb.SetBillboardPos(pos);
        }
        else
        {
            ssb.SetBillboardPos(pos);
        }
    }

    public bool IsShowTestInfo = false;

    public void ShowTestInfo(bool isShow)
    {
        IsShowTestInfo = isShow;
        foreach (var item in m_heads)
        {
            ((PlayerHead)item.Value).ShowTestInfo(isShow);
        }
    }    

    public void ShowTaskIcon(uint playerId, uint idx)
    {
        if (!m_heads.ContainsKey(playerId))
        {
            return;
        }
        m_heads[playerId].ShowTaskIcon(idx);
    }

    public void ShowBillboard(uint playerId, bool isShow)
    {
        if (m_heads.ContainsKey(playerId) == false)
            return;

        ((PlayerHead)m_heads[playerId]).ShowBillboard(isShow);
    }

    public UIAtlas GetBattleBillboardAtlas()
    {
        if (m_atlas == null)
        {
            AssetCacheMgr.GetUIInstance("MogoBattleBillboardUI.prefab", (prefab, guid, gameObject) =>
            {
                GameObject go = (GameObject)gameObject;

                m_atlas = go.GetComponentInChildren<UIAtlas>();

                go.hideFlags = HideFlags.HideAndDontSave;

            });
        }

        return m_atlas;
    }

    public UIFont GetBattleBillboardFont()
    {
        if (m_font == null)
        {
            AssetCacheMgr.GetUIInstance("FontMsyh.prefab", (prefab, guid, gameObject) =>
            {
                GameObject go = (GameObject)gameObject;

                m_font = go.GetComponentInChildren<UIFont>();

                go.hideFlags = HideFlags.HideAndDontSave;

            });
        }

        return m_font;
    }

    public void Release()
    {
        BillboardLogicManager.Instance.Release();
        //  EventDispatcher.RemoveEventListener<Vector3, uint>(BillboardViewEvent.UPDATEBILLBOARDPOS, OnUpdateBillboardPos);
        //EventDispatcher.RemoveEventListener<float, uint>(BillboardLogicManager.BillboardLogicEvent.SETBILLBOARDBLOOD, OnSetBillboardBlood);
        //EventDispatcher.RemoveEventListener<string, uint>(BillboardLogicManager.BillboardLogicEvent.UPDATEBILLBOARDNAME, OnSetBillboardName);
        EventDispatcher.RemoveEventListener<string, uint>(BillboardLogicManager.BillboardLogicEvent.UPDATEBILLBOARDTONG, OnSetBillboardTong);

        foreach (var item in m_heads)
        {
            item.Value.Remove();
        }

        if (m_atlas != null)
        {
            AssetCacheMgr.ReleaseInstance(m_atlas.gameObject);
        }

        if (m_font != null)
        {
            AssetCacheMgr.ReleaseInstance(m_font.gameObject);
        }

        m_heads.Clear();
        m_trans.Clear();
    }

    public void Clear()
    {
        foreach (var item in m_heads)
        {
            item.Value.Remove();
        }

        m_heads.Clear();
        m_trans.Clear();
    }

    void Awake()
    {
        m_instance = transform.GetComponentsInChildren<BillboardViewManager>(true)[0];
        m_stage = transform.Find("Anchor/MogoMainUIPanel/BillboardList");
        //EventDispatcher.AddEventListener<Vector3, uint>(BillboardViewEvent.UPDATEBILLBOARDPOS, OnUpdateBillboardPos);
        //EventDispatcher.AddEventListener<float, uint>(BillboardLogicManager.BillboardLogicEvent.SETBILLBOARDBLOOD, OnSetBillboardBlood);
        //EventDispatcher.AddEventListener<string, uint>(BillboardLogicManager.BillboardLogicEvent.UPDATEBILLBOARDNAME, OnSetBillboardName);
        EventDispatcher.AddEventListener<string, uint>(BillboardLogicManager.BillboardLogicEvent.UPDATEBILLBOARDTONG, OnSetBillboardTong);

        GetBattleBillboardAtlas();
        GetBattleBillboardFont();

        DictPoolTypeToID.Add(MaiFeoMemoryPoolType.PoolType_SplitBattleBillboard,
            MaiFeoMemoryPoolManager.GetSingleton().CreatePool(MaiFeoMemoryPoolType.PoolType_SplitBattleBillboard, 5));

        DictPoolTypeToID.Add(MaiFeoMemoryPoolType.PoolType_SuperBattleBillboard,
            MaiFeoMemoryPoolManager.GetSingleton().CreatePool(MaiFeoMemoryPoolType.PoolType_SuperBattleBillboard, 5));

        DictPoolTypeToID.Add(MaiFeoMemoryPoolType.PoolType_AloneBattleBillobard,
            MaiFeoMemoryPoolManager.GetSingleton().CreatePool(MaiFeoMemoryPoolType.PoolType_AloneBattleBillobard,5));
    }
}
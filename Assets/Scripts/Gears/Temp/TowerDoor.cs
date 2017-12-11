using UnityEngine;
using System.Collections;
using Mogo.Util;
using Mogo.GameData;
using System.Collections.Generic;
using System.Linq;
public class TowerDoor : GearParent
{
    private Transform m_transform { get; set; }
    private BoxCollider m_collider { get; set; }
    private SfxHandler handler { get; set; }
    public int sfxID;
    private bool m_entered { get; set; }
    private GameObject text { get; set; }
    private Vector3 vecText { get; set; }
    private Transform trans;
    void Start()
    {
        handler = gameObject.AddComponent<SfxHandler>();
        m_transform = transform;
        trans = m_transform.Find("text");
        vecText = new Vector3(trans.position.x, trans.position.y, trans.position.z);
        m_collider = transform.GetComponentsInChildren<BoxCollider>(true)[0];
        ShowDoor(false, -1);
        AddListeners();


    }
    public override void AddListeners()
    {
        EventDispatcher.AddEventListener<ushort>(Events.OtherEvent.MapIdChanged, OnMapChanged);
        EventDispatcher.AddEventListener<bool, int>(Events.TowerEvent.CreateDoor, ShowDoor);
        EventDispatcher.AddEventListener<int, bool>(Events.InstanceEvent.InstanceUnLoaded, OnInstanceLeave);
    }

    public override void RemoveListeners()
    {
        EventDispatcher.RemoveEventListener<ushort>(Events.OtherEvent.MapIdChanged, OnMapChanged);
        EventDispatcher.RemoveEventListener<bool, int>(Events.TowerEvent.CreateDoor, ShowDoor);
        EventDispatcher.RemoveEventListener<int, bool>(Events.InstanceEvent.InstanceUnLoaded, OnInstanceLeave);
    }
    private void OnInstanceLeave(int sceneID, bool isInstance)
    {
        if (MapData.dataMap.Get(sceneID).type == MapType.ClimbTower)
        {
            //m_entered = false;
            //Debug.LogError("MissionReq start");
            //MogoWorld.thePlayer.RpcCall("MissionReq", (byte)3, 0, (ushort)1, "");
            ShowDoor(false, -1);
        }
    }
    void OnDestroy()
    {
        RemoveListeners();
    }
    void OnMapChanged(ushort lineID)
    {
        m_entered = false;
        MogoWorld.thePlayer.RpcCall("MissionReq", (byte)3, 0, (ushort)1, "");
        ShowDoor(false, -1);
    }
    void OnTriggerEnter()
    {
        if (m_entered)
        {
            m_entered = false;
            ClientEventData.TriggerGearEvent(20002);

            MainUIViewManager.Instance.ShowBossTarget(false);
            TimerHeap.AddTimer(1000, 0, () => { SubAssetCacheMgr.ReleaseCharacterResources(); EventDispatcher.TriggerEvent(Events.TowerEvent.EnterMap); });
        }
    }
    void OnTriggerExit()
    {
        m_entered = false;
    }
    void EnterDoor()
    {

    }
    void ShowDoor(bool show, int level)
    {
        m_entered = show;
        if (show)
        {
            TimerHeap.AddTimer(1000, 0, () =>
            {
                if (m_collider != null)
                {
                    m_collider.enabled = show;
                }
                handler.HandleFx(sfxID, m_transform);

                //showText(level, true);

                //var models = new List<int>();
                //var SpaceLevelID = (MogoUtils.GetSpaceLevelID((int)InstanceIdentity.TOWER))[level - 2];

                //models = MogoUtils.GetSpawnPointMonsterID(SpaceLevelID);

                //if (models.Count > 0)
                //{
                //    var ms = models.Select(x => AvatarModelData.dataMap.Get(x).prefabName).Distinct().ToArray();
                //    LoggerHelper.Warning("[Info] unload resource" + ms.PackArray());
                //    foreach (var item in ms)
                //    {
                //        AssetCacheMgr.ReleaseResourceImmediate(item);
                //    }

                //}
            });
        }
        else
        {
            if (m_collider != null)
            {
                m_collider.enabled = show;
            }
            handler.RemoveFXs(sfxID);
            //showText(level, false);
        }
    }
    //void Update()
    //{
    //    if (text)
    //    {
    //        if (trans == null)
    //        {
    //            trans = m_transform.FindChild("text");
    //        }
    //        vecText = new Vector3(trans.position.x, trans.position.y, trans.position.z);
    //        text.transform.position = MogoUtils.ConvertWorldPos(Camera.main, GameObject.Find("Camera").GetComponentsInChildren<Camera>(true)[0], vecText);
    //    }

    //}
    //void showText(int level, bool show)
    //{
    //    if (show)
    //    {
    //        if (text != null)
    //        {
    //            text.GetComponentsInChildren<UILabel>(true)[0].text = LanguageData.dataMap.Get(20017).Format(level);
    //            text.SetActive(true);
    //        }
    //        else
    //        {
    //            MogoUtils.AddBillboard("DoorText", vecText,
    //            (obj) =>
    //            {
    //                text = obj;
    //                text.transform.localScale = new Vector3(30, 30, 1);
    //                text.GetComponentsInChildren<UILabel>(true)[0].text = LanguageData.dataMap.Get(20017).Format(level);
    //            }
    //            );
    //        }
    //    }
    //    else
    //    {
    //        if (text != null)
    //        {
    //            text.SetActive(false);
    //        }
    //    }

    //}
}

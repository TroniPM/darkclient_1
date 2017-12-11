using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Mogo.Game;
using Mogo.Util;
using Mogo.GameData;
using Mogo.Task;
using HMF;
public class StoryManager : IEventManager
{
    private static StoryManager m_instance;
    private string config_xml = "xml/storyCG";

    private int m_currentStoryID = 0;

    private Dictionary<int, Dictionary<int, String>> m_mapOfStoryMap = new Dictionary<int, Dictionary<int, String>>();
    private Queue<String[]> m_commandQueue = new Queue<String[]>();
    private Dictionary<int, EntityParent> m_entityList = new Dictionary<int, EntityParent>();
    private Dictionary<int, GameObject> m_objectList = new Dictionary<int, GameObject>();
    private Action m_callback = null;
    public bool IsShake { get; set; }
    public bool IsOpen { get; set; }
    public StoryManager()
    {
        m_entityList.Add(0, MogoWorld.thePlayer);
        IsOpen = true;
        IsShake = false;
    }
    public static StoryManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new StoryManager();
            }
            return m_instance;
        }
    }
    public void AddListeners()
    {
        EventDispatcher.AddEventListener<SignalEvents>(Events.CommandEvent.CommandEnd, HandleWaitingEvent);
        EventDispatcher.AddEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, HandleEnterInstance);
        EventDispatcher.AddEventListener(TeachUILogicManager.TEACHUICRASHED, ClearGuide);
    }
    public void RemoveListeners()
    {
        EventDispatcher.RemoveEventListener<SignalEvents>(Events.CommandEvent.CommandEnd, HandleWaitingEvent);
        EventDispatcher.RemoveEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, HandleEnterInstance);
        EventDispatcher.RemoveEventListener(TeachUILogicManager.TEACHUICRASHED, ClearGuide);
    }
    void HandleEnterInstance(int sceneID, bool isInstance)
    {
        if (sceneID != MogoWorld.globalSetting.homeScene)
        {
            AllClear();
        }

    }
    public void Clear()
    {
        //m_mapOfStoryMap.Clear();
        m_commandQueue.Clear();
        //m_entityList.Clear();
        //m_objectList.Clear();
    }
    public void ClearGuide()
    {
        m_mapOfStoryMap.Clear();
        m_commandQueue.Clear();
        GuideSystem.Instance.dequeueGuide();
        MogoUIQueue.Instance.IsLocking = false;
        MogoUIQueue.Instance.Locked = false;
        m_callback = null;
        GuideSystem.Instance.IsGuideDialog = false;
        Debug.Log("@@@@@@@@@@@@@@@@@@@@@@@@@@ Unlock by ClearGuide");
        //m_entityList.Clear();
        //m_objectList.Clear();
    }
    public void AllClear()
    {
        m_mapOfStoryMap.Clear();
        m_entityList.Clear();
        m_entityList.Add(0, MogoWorld.thePlayer);
        m_objectList.Clear();
    }
    public void LoadConfig(int index)
    {
        Clear();
        if (!m_mapOfStoryMap.ContainsKey(index))
        {
            var path = string.Concat(SystemConfig.CONFIG_SUB_FOLDER, config_xml + index.ToString(), SystemConfig.CONFIG_FILE_EXTENSION);
            LoggerHelper.Warning("Load Story: " + path);
            if (SystemSwitch.UseHmf)
            {
                byte[] bs = XMLParser.LoadBytes(path);
                System.IO.MemoryStream stream = new MemoryStream(bs);
                stream.Seek(0, SeekOrigin.Begin);

                Hmf h = new Hmf();
                Dictionary<object, object> map = (Dictionary<object, object>)h.ReadObject(stream);
                Dictionary<int, String> m_storyMap = new Dictionary<int, String>();
                foreach (var node in map)
                {
                    Dictionary<object, object> m = (Dictionary<object, object>)node.Value;
                    m_storyMap.Add(int.Parse((string)node.Key), m[(object)"storyText"].ToString());
                }
#if UNITY_IPHONE
				m_mapOfStoryMap[index] = m_storyMap.SortByKey();
#else
                m_mapOfStoryMap[index] = m_storyMap.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
#endif
			}
            else
            {
                var xml = XMLParser.Load(path);
                if (xml != null)
                {
                    var map = XMLParser.LoadIntMap(xml, config_xml);
                    Dictionary<int, String> m_storyMap = new Dictionary<int, String>();
                    foreach (var node in map)
                    {
                        m_storyMap.Add(node.Key, node.Value["storyText"]);
                    }
                    m_mapOfStoryMap[index] = m_storyMap;
                }
            }
        }

    }

    private List<String> m_preloadStatus = new List<String>();
    public void Preload(List<int> cgIDs, Action callback = null)
    {
        m_preloadStatus.Clear();
        foreach (var cgID in cgIDs)
        {
            if (m_mapOfStoryMap.ContainsKey(cgID))
            {
                foreach (var command in m_mapOfStoryMap[cgID])
                {
                    String[] commands = Regex.Split(command.Value.Trim(), @"[ ]+");
                    if (commands[0].Equals("CreateModel"))
                    {
                        AvatarModelData data = AvatarModelData.dataMap.GetValueOrDefault(Int32.Parse(commands[2]), null);
                        if (data == null)
                        {
                            LoggerHelper.Error("Model not found: " + Int32.Parse(commands[2]));
                            return;
                        }
                        m_preloadStatus.Add(data.prefabName);
                    }
                }
            }

        }

        if (m_preloadStatus.Count > 0)
        {
            var ms = m_preloadStatus.ToArray();
            AssetCacheMgr.GetResourcesAutoRelease(ms, (obj) =>
            {
                //AssetCacheMgr.UnloadAssetbundles(ms);
                if (callback != null)
                {
                    callback();
                }
            });
        }
        else
        {
            if (callback != null)
            {
                callback();
            }
        }

    }

    public void ClearPreload()
    {
        foreach (var item in m_preloadStatus)
        {
            AssetCacheMgr.ReleaseResourceImmediate(item);
        }
        m_preloadStatus.Clear();
    }

    void HandleWaitingEvent(SignalEvents targetID)
    {
        if (m_commandQueue.Count > 0)
        {
            String[] commands = m_commandQueue.Peek();
            if (commands[0].Equals("Wait"))
            {
                if (Byte.Parse(commands[1]) != (Byte)targetID)
                {
                    return;
                }
                else
                {
                    m_commandQueue.Dequeue();
                    Execute();
                }
            }
        }

    }
    /// <summary>
    /// 调试打印执行队列
    /// </summary>
    public void PrintCommandQueue()
    {
        var arr = (from x in m_commandQueue select String.Join(" ", x)).ToArray();
        LoggerHelper.Warning(String.Join(" ; ", arr));
    }
    /// <summary>
    /// 供外界调用添加指令
    /// </summary>
    /// <param name="command"></param>
    public void AddCommand(String command)
    {
        String[] commands = Regex.Split(command.Trim(), @"[ ]+");
        switch (commands[0])
        {
            case "Sleep":
                m_commandQueue.Enqueue(commands);
                m_commandQueue.Enqueue(new String[] { "Wait", ((byte)SignalEvents.Sleep).ToString() });
                break;
            default:
                m_commandQueue.Enqueue(commands);
                break;
        }
    }
    public void AddCGCheckTimer()
    {
        TimerHeap.AddTimer(0, 5000, handleCheck);
    }
    void handleCheck()
    {
        //if(m_currentStoryID!=0 )
        //{}
    }
    /// <summary>
    /// 实际的功能函数执行过程
    /// </summary>
    public void Execute()
    {
        while (m_commandQueue.Count != 0)
        {
            string[] commands = m_commandQueue.Peek();
            if (commands[0].ToLower() == "wait")
            {
                return;
            }
            else
            {
                m_commandQueue.Dequeue();
                try
                {
                    executeCommand(commands);
                }
                catch (Exception e)
                {
                    m_commandQueue.Clear();
                    FinishCG();
                    AllClear();
                    LoggerHelper.Error(e.ToString(), false);
                }

            }
        }

    }
    private bool CheckCGCondition(int storyID)
    {
        if (!IsOpen)
        {
            return false;
        }
        if (MogoWorld.thePlayer.CurrentTask == null || (CGConfigData.dataMap.ContainsKey(storyID) && CGConfigData.dataMap.Get(storyID).quest != MogoWorld.thePlayer.CurrentTask.id))
        {
            if (MogoWorld.thePlayer.CurrentTask != null)
                LoggerHelper.Warning(storyID + "&" + CGConfigData.dataMap.Get(storyID).quest + "&" + MogoWorld.thePlayer.CurrentTask.id);
            return false;
        }
        return true;
    }
    public void SetCallBack(Action callback)
    {
        m_callback = callback;
    }
    private void _callback()
    {
        if (m_callback != null)
        {
            //清除callback要在回调之前，因为回调很可能就是设置callback，导致一设置完值就立刻被清掉了
            var cb = m_callback;
            m_callback = null;
            cb();
        }
    }
    public void PlayStory(int storyID)
    {
        if (!CheckCGCondition(storyID))
        {
            _callback();
            return;
        }
        if (m_currentStoryID != 0)
        {
            LoggerHelper.Debug("CG has already began!");
            _callback();
            return;
        }
        else
        {
            m_currentStoryID = storyID;
        }

        LoadConfig(m_currentStoryID);
        if (m_mapOfStoryMap.ContainsKey(storyID) && m_mapOfStoryMap[storyID].Count != 0)
        {
            //开始CG
            StartCG();
            //加入执行队列
            foreach (var single in m_mapOfStoryMap[storyID])
            {
                AddCommand(single.Value);
            }
            PrintCommandQueue();
            //开始执行
            Execute();
        }
        else
        {
            LoggerHelper.Warning("CG not exist: " + storyID);
            _callback();
            return;
        }


    }
    protected void HideUITip()
    {
        TeachUIViewManager.Instance.ShowFingerAnim(false);
        TeachUIViewManager.Instance.ShowTip(Vector3.zero, string.Empty, false);
        TeachUIViewManager.Instance.DestroyCloneObject();
    }
    /// <summary>
    /// 开始CG的通用处理操作
    /// </summary>
    void StartCG()
    {
        HideUITip();
        EventDispatcher.TriggerEvent(Events.StoryEvent.CGBegin);
        MogoUIManager.Instance.ShowUI(false);
        MogoUIManager.Instance.ShowBillboardList(false);
        MogoGlobleUIManager.Instance.ShowBattlePlayerBloodTipPanel(false);
    }
    /// <summary>
    /// 结束CG的通用处理操作
    /// </summary>
    void FinishCG()
    {
        ///代表是CG的执行序列
        if (m_currentStoryID != 0)
        {
            EventDispatcher.TriggerEvent(Events.StoryEvent.CGEnd);
            MogoUIManager.Instance.ShowUI(true);
            MogoUIManager.Instance.ShowBillboardList(true);
            m_currentStoryID = 0;
            MogoMainCamera.Instance.LockSight();
        }
        _callback();
    }
    public bool IsCurrentInCG()
    {
        return m_currentStoryID != 0;
    }
    public bool HasFinalCG()
    {
        var id = MissionData.GetCGByMission(MogoWorld.thePlayer.sceneId);
        if (id > 0)
        {
            return CheckCGCondition(id);
        }
        else
        {
            return false;
        }
    }
    #region 函数适配
    void CreateModel(int id, int model, int mapx, int mapy, Vector3 vec, bool playBornFX = true)
    {

        EntityMonster entity = new EntityMonster();
        entity.ID = (uint)id;
        entity.model = model;
        Vector3 Point = new Vector3();
        MogoUtils.GetPointInTerrain((float)mapx / 100, (float)mapy / 100, out Point);
        entity.PlayBornFX = playBornFX;
        entity.BillBoard = false;
        entity.position = Point;
        entity.rotation = vec;
        entity.CreateModel();
        EventDispatcher.AddEventListener<GameObject, Vector3>(MogoMotor.ON_MOVE_TO, entity.OnMoveTo);
        m_entityList[id] = entity;
    }
    void PlayAction(int id, int actionID)
    {
        if (!m_entityList.ContainsKey(id))
        {
            LoggerHelper.Debug("id not exist in list");
        }
        else
        {
            m_entityList[id].SetAction(actionID);
        }
    }
    void PlaySfx(int id, int sfxID)
    {
        if (!m_entityList.ContainsKey(id))
        {
            LoggerHelper.Debug("id not exist in list");
        }
        else
        {
            m_entityList[id].sfxHandler.HandleFx(sfxID);
        }
    }
    void StopSfx(int id, int sfxID)
    {
        if (!m_entityList.ContainsKey(id))
        {
            LoggerHelper.Debug("id not exist in list");
        }
        else
        {
            m_entityList[id].sfxHandler.RemoveFXs(sfxID);
        }
    }
    void PlaySfx(int id, int sfxID, int start, Vector3 vec)
    {
        GameObject sfx = new GameObject();
        Gadget gd = sfx.AddComponent<Gadget>();
        sfx.transform.localScale = new Vector3(1, 1, 1);
        sfx.transform.localPosition = vec;
        sfx.name = "SFX" + sfxID.ToString();
        m_objectList.Add(id, sfx);
        gd.EvolveToEndlessFX(sfxID, (uint)start);
    }
    void StopSfx(int id)
    {
        m_objectList[id].GetComponentsInChildren<Gadget>(true)[0].StopFX();
        AssetCacheMgr.ReleaseLocalInstance(m_objectList[id]);
        m_objectList.Remove(id);
    }
    void AddEventSender(int id, SignalEvents signal)
    {
        //GameObject sfx = new GameObject();
        //Gadget gd = sfx.AddComponent<Gadget>();
        //sfx.transform.localScale = new Vector3(1, 1, 1);
        //sfx.transform.localPosition = vec;
        //sfx.name = "SFX" + sfxID.ToString();
        //m_objectList.Add(id, sfx);
        //gd.EvolveToEndlessFX(sfxID, (uint)start);
    }
    void RemoveEventSender(int id)
    {

    }
    void MoveCamera(int id, float originalDis, float targetDis,
         float orginalRx, float targetRx,
         float orginalRy, float targetRy,
        float originalPx, float targetPx,
        float length)
    {
        MogoMainCamera.Instance.PlayCGAnim(originalDis, targetDis, orginalRx, targetRx, orginalRy, targetRy, originalPx, targetPx, length / 1000, m_entityList[id].Transform);
    }
    void MoveCamera(int id, float originalDis, float targetDis,
     float orginalRx, float targetRx,
     float orginalRy, float targetRy,
    float originalPx, float targetPx,
    float length, Vector3 vec)
    {
        MogoMainCamera.Instance.PlayCGAnim(originalDis, targetDis, orginalRx, targetRx, orginalRy, targetRy, originalPx, targetPx, length / 1000, vec);
    }
    void ShakeCamera(int animID, float length)
    {
        MogoMainCamera.Instance.Shake(animID, length);
        if (IsShake)
        {
            PlatformSdkManager.Instance.Shake((long)length);
        }
    }
    void ShakeCamera(Vector3 v1, int x, int y, int z, float length)
    {
        MogoMainCamera.Instance.Shake(v1.x, v1.y, v1.z, x, y, z, length);
        if (IsShake)
        {
            PlatformSdkManager.Instance.Shake((long)length);
        }
    }
    public string FormatTaskText(string taskText)
    {
        string text = taskText;

        Regex re = new Regex(@"\{\d+\}");

        foreach (Match matchData in re.Matches(text))
        {
            string value = matchData.Value;
            int flag = int.Parse(value.Replace("{", "").Replace("}", ""));

            if (flag == 0)
            {
                text = text.Replace(value, MogoWorld.thePlayer.name);
                continue;
            }
            else if (flag == 1)
            {
                text = text.Replace(value, MogoWorld.thePlayer.GetSexString(TaskManager.DialogueRelationship.You));
                continue;
            }
            else if (flag == 2)
            {
                text = text.Replace(value, MogoWorld.thePlayer.GetSexString(TaskManager.DialogueRelationship.OlderToYounger));
                continue;
            }
            else if (flag == 3)
            {
                text = text.Replace(value, MogoWorld.thePlayer.GetSexString(TaskManager.DialogueRelationship.YoungerToOlder));
                continue;
            }
            else
            {
                text = text.Replace(value, LanguageData.GetContent(flag));
            }
        }

        return text;
    }
    /// <summary>
    /// 私有的函数执行过程
    /// </summary>
    /// <param name="commands"></param>
    private void executeCommand(string[] commands)
    {
        LoggerHelper.Warning("executeCommand :" + commands.PackArray());
        switch (commands[0])
        {
            case "CreateModel":
                {
                    var list1 = commands[5].Split(new char[] { ',' });
                    Vector3 vec = new Vector3(float.Parse(list1[0]), float.Parse(list1[1]), float.Parse(list1[2]));
                    if (commands.Length > 6 && commands[6].Equals("false"))
                    {
                        CreateModel(Int32.Parse(commands[1]), Int32.Parse(commands[2]), Int32.Parse(commands[3]), Int32.Parse(commands[4]), vec, false);
                    }
                    else
                    {
                        CreateModel(Int32.Parse(commands[1]), Int32.Parse(commands[2]), Int32.Parse(commands[3]), Int32.Parse(commands[4]), vec);
                    }

                    break;
                }
            case "DestroyModel":
                {
                    int id = Int32.Parse(commands[1]);
                    AssetCacheMgr.ReleaseInstance(m_entityList[id].GameObject);
                    BillboardLogicManager.Instance.RemoveBillboard(m_entityList[Int32.Parse(commands[1])].ID);
                    EventDispatcher.RemoveEventListener<GameObject, Vector3>(MogoMotor.ON_MOVE_TO, m_entityList[Int32.Parse(commands[1])].OnMoveTo);
                    m_entityList.Remove(id);
                    break;
                }
            case "PlayAction":
                {
                    int id = Int32.Parse(commands[1]);
                    PlayAction(id, Int32.Parse(commands[2]));
                    break;
                }
            case "PlayOneAction":
                {
                    int id = Int32.Parse(commands[1]);
                    PlayAction(id, Int32.Parse(commands[2]));
                    m_entityList[id].AddCallbackInFrames<EntityParent>((ent) => { ent.SetAction(0); }, m_entityList[id]);
                    break;
                }
            case "StopAction":
                {
                    int id = Int32.Parse(commands[1]);
                    m_entityList[id].AddCallbackInFrames<EntityParent>((ent) => { ent.SetAction(0); }, m_entityList[id]);
                    break;
                }

            case "PlaySfx":
                {
                    if (commands.Length > 3)
                    {
                        var list1 = commands[4].Split(new char[] { ',' });
                        Vector3 vec = new Vector3(float.Parse(list1[0]), float.Parse(list1[1]), float.Parse(list1[2]));
                        PlaySfx(Int32.Parse(commands[1]), Int32.Parse(commands[2]), Int32.Parse(commands[3]), vec);
                    }
                    else
                    {
                        PlaySfx(Int32.Parse(commands[1]), Int32.Parse(commands[2]));
                    }

                    break;
                }
            case "StopSfx":
                if (commands.Length > 2)
                {
                    StopSfx(Int32.Parse(commands[1]), Int32.Parse(commands[2]));
                }
                else
                {
                    StopSfx(Int32.Parse(commands[1]));
                }
                break;
            case "MoveCamera":
                if (commands.Length == 5)
                {
                    var list1 = commands[2].Split(new char[] { ',' });
                    var list2 = commands[3].Split(new char[] { ',' });
                    MoveCamera(Int32.Parse(commands[1]), float.Parse(list1[0]), float.Parse(list2[0]), float.Parse(list1[1]), float.Parse(list2[1]),
                        float.Parse(list1[2]), float.Parse(list2[2]), float.Parse(list1[3]), float.Parse(list2[3]), float.Parse(commands[4]));
                }
                else if (commands.Length == 6)
                {
                    var list1 = commands[2].Split(new char[] { ',' });
                    var list2 = commands[3].Split(new char[] { ',' });
                    var list3 = commands[5].Split(new char[] { ',' });
                    Vector3 vec = new Vector3(float.Parse(list3[0]), float.Parse(list3[1]), float.Parse(list3[2]));
                    MoveCamera(Int32.Parse(commands[1]), float.Parse(list1[0]), float.Parse(list2[0]), float.Parse(list1[1]), float.Parse(list2[1]),
                        float.Parse(list1[2]), float.Parse(list2[2]), float.Parse(list1[3]), float.Parse(list2[3]), float.Parse(commands[4]), vec);
                }
                else
                {
                    MogoMainCamera.Instance.PlayCGAnim(commands[1]);
                }
                break;
            case "SetPosition":
                {
                    int id = Int32.Parse(commands[1]);
                    if (!m_entityList.ContainsKey(id))
                    {
                        LoggerHelper.Debug("id not exist in list");
                    }
                    else
                    {
                        var list1 = commands[2].Split(new char[] { ',' });
                        m_entityList[id].SetPositon(float.Parse(list1[0]), float.Parse(list1[1]), float.Parse(list1[2]));
                    }
                    break;
                }
            case "SetRotation":
                {
                    int id = Int32.Parse(commands[1]);
                    if (!m_entityList.ContainsKey(id))
                    {
                        LoggerHelper.Debug("id not exist in list");
                    }
                    else
                    {
                        var list1 = commands[2].Split(new char[] { ',' });
                        m_entityList[id].SetRotation(float.Parse(list1[0]), float.Parse(list1[1]), float.Parse(list1[2]));
                    }
                    break;
                }
            case "ShakeCamera":
                if (commands.Length > 3)
                {
                    var list1 = commands[1].Split(new char[] { ',' });
                    var vec = new Vector3(float.Parse(list1[0]), float.Parse(list1[1]), float.Parse(list1[2]));
                    var list2 = commands[2].Split(new char[] { ',' });
                    ShakeCamera(vec, Int32.Parse(list2[0]), Int32.Parse(list2[1]), Int32.Parse(list2[2]), float.Parse(commands[3]) / 1000);
                }
                else
                {
                    ShakeCamera(Int32.Parse(commands[1]), float.Parse(commands[2]) / 1000);
                }
                break;
            case "WhiteCamera":
                {
                    var list1 = commands[1].Split(new char[] { ',' });
                    Vector2 vec = new Vector2(float.Parse(list1[0]), float.Parse(list1[1]));
                    MogoMainCamera.Instance.FadeToAllWhite(vec, float.Parse(commands[2]) / 1000);
                    break;
                }
            case "NormalCamera":
                {
                    var list1 = commands[1].Split(new char[] { ',' });
                    Vector2 vec = new Vector2(float.Parse(list1[0]), float.Parse(list1[1]));
                    MogoMainCamera.Instance.FadeToNoneWhite(vec, float.Parse(commands[2]) / 1000);
                    break;
                }
            case "MoveTo":
                {
                    int id = Int32.Parse(commands[1]);
                    var list1 = commands[2].Split(new char[] { ',' });
                    m_entityList[id].MoveTo(float.Parse(list1[0]), float.Parse(list1[1]), float.Parse(list1[2]));
                    break;
                }
            case "ShowDialog":
                {
                    var list1 = (from x in commands[1].Split(new char[] { ',' }) select LanguageData.GetContent(Int32.Parse(x)).Replace("{0}", MogoWorld.thePlayer.name)).ToArray();
                    var list2 = (from x in commands[2].Replace("{0}", IconData.GetPortraitByVocation(MogoWorld.thePlayer.vocation)).Split(new char[] { ',' }) select IconData.dataMap.Get(Int32.Parse(x)).path).ToArray();
                    var list3 = (from x in commands[3].Split(new char[] { ',' }) select FormatTaskText(LanguageData.GetContent(Int32.Parse(x)))).ToArray();
                    GuideSystem.Instance.IsGuideDialog = true;
                    if (m_currentStoryID != 0)
                    {
                        //CG
                        MogoUIManager.Instance.ShowMogoTaskUI(TaskUILogicManager.Instance.SetTaskInfo, list1, list2, list3, MogoUIManager.Instance.m_MainUI);
                    }
                    else
                    {

                        MogoUIManager.Instance.ShowGuideTaskUI(DialogUILogicManager.Instance.SetDialogInfo, list1, list2, list3);
                    }
                    break;
                }
            case "Subtitle":
                {
                    MogoUIManager.Instance.ShowSubtitle(LanguageData.GetContent(Int32.Parse(commands[1])), float.Parse(commands[2]), float.Parse(commands[3]));
                    break;
                }
            case "ZoomCamera":
                {

                    int id = Int32.Parse(commands[1]);
                    var list = commands[3].Split(new char[] { ',' });
                    MogoMainCamera.Instance.CloseToTarget(m_entityList[id].Transform, float.Parse(commands[2]), float.Parse(list[1]), float.Parse(list[0]), float.Parse(commands[4]) / 1000, float.Parse(commands[5]) / 1000);
                    break;
                }
            case "LockSight":
                {
                    MogoMainCamera.Instance.LockSight();
                    break;
                }
            case "SetFocus":
                {
                    if (commands.Length > 4)
                    {
                        TeachUILogicManager.Instance.SetTeachUIFocus(Int32.Parse(commands[1]), LanguageData.GetContent(Int32.Parse(commands[2])), false, Int32.Parse(commands[3]), MogoUIManager.Instance.m_MainUI);
                        //TeachUILogicManager.Instance.ShowFingerAnim(true);
                    }
                    else
                    {
                        TeachUILogicManager.Instance.SetTeachUIFocus(Int32.Parse(commands[1]), LanguageData.GetContent(Int32.Parse(commands[2])), false, Int32.Parse(commands[3]));
                        //TeachUILogicManager.Instance.ShowFingerAnim(true);
                    }
                    break;
                }
            case "SetItemFocus":
                {
                    var itemIDs = commands[1].Split(new char[] { ',' }).Select(x => Int32.Parse(x));
                    foreach (var item in itemIDs)
                    {
                        if (InventoryManager.Instance.GetItemNumById(item) > 0)
                        {
                            LoggerHelper.Warning("SetItemFocus" + item);
                            TeachUILogicManager.Instance.SetItemFocus(item, LanguageData.GetContent(Int32.Parse(commands[2])), Int32.Parse(commands[3]));
                            break;
                        }
                    }
                    //TeachUILogicManager.Instance.ShowFingerAnim(true);
                    break;
                }
            case "SetNonFocus":
                {
                    if (commands.Length > 4)
                    {
                        TeachUILogicManager.Instance.SetTeachUIFocus(Int32.Parse(commands[1]), LanguageData.GetContent(Int32.Parse(commands[2])), true, Int32.Parse(commands[3]), MogoUIManager.Instance.m_MainUI);
                        TeachUILogicManager.Instance.ShowFingerAnim(true);
                    }
                    else
                    {
                        TeachUILogicManager.Instance.SetTeachUIFocus(Int32.Parse(commands[1]), LanguageData.GetContent(Int32.Parse(commands[2])), true, Int32.Parse(commands[3]));
                        TeachUILogicManager.Instance.ShowFingerAnim(true);
                    }

                    break;
                }
            case "OpenBillboard":
                {
                    MogoUIManager.Instance.ShowBillboardList(true);
                    break;
                }
            case "CloseBillboard":
                {
                    MogoUIManager.Instance.ShowBillboardList(false);
                    break;
                }
            case "OpenUI":
                {
                    if (commands.Length == 2)
                    {
                        var type = typeof(MainUIViewManager);
                        var method = type.GetMethod("Show" + commands[1]);
                        method.Invoke(MainUIViewManager.Instance, new object[] { true });
                    }
                    else if (commands.Length == 3)
                    {
                        var type = typeof(MainUIViewManager);
                        var method = type.GetMethod("Show" + commands[1]);
                        method.Invoke(MainUIViewManager.Instance, new object[] { true, Int32.Parse(commands[2]) });
                    }
                    else
                    {
                        MogoUIManager.Instance.ShowUI(true);
                    }

                    break;
                }
            case "CloseUI":
                {
                    if (commands.Length == 2)
                    {
                        var type = typeof(MainUIViewManager);
                        var method = type.GetMethod("Show" + commands[1]);
                        method.Invoke(MainUIViewManager.Instance, new object[] { false });
                    }
                    else if (commands.Length == 3)
                    {
                        var type = typeof(MainUIViewManager);
                        var method = type.GetMethod("Show" + commands[1]);
                        method.Invoke(MainUIViewManager.Instance, new object[] { false, Int32.Parse(commands[2]) });
                    }
                    else
                    {
                        MogoUIManager.Instance.ShowUI(false);
                        MogoUIManager.Instance.ShowBillboardList(false);
                    }

                    break;
                }
            case "AddPointer":
                {
                    LoggerHelper.Debug("AddPointerToTarget");
                    Vector3 point = new Vector3();
                    MogoUtils.GetPointInTerrain(float.Parse(commands[1]), float.Parse(commands[2]), out point);
                    MogoFXManager.Instance.AddPointerToTarget(MogoWorld.thePlayer.Transform.gameObject, MogoWorld.thePlayer.ID, point);
                    break;
                }
            case "Sleep":
                {
                    TimerHeap.AddTimer(UInt32.Parse(commands[1]), 0, () => { EventDispatcher.TriggerEvent<SignalEvents>(Events.CommandEvent.CommandEnd, SignalEvents.Sleep); });
                    break;
                }
            case "End":
                {
                    //结束CG
                    FinishCG();
                    break;
                }
            case "UnlockQueue":
                {
                    MogoUIQueue.Instance.IsLocking = false;
                    MogoUIQueue.Instance.Locked = false;
                    MogoUIQueue.Instance.CheckQueue();
                    Debug.Log("Unlocking by StoryManager...................................");
                    break;
                }
            case "StartGuideUI":
                {

                    Mogo.Util.LoggerHelper.Debug("Start GuideUI @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
                    TeachUIViewManager.Instance.PrepareShowTeachUI();
                    break;
                }
            case "ResetControlStick":
                {
                    if (ControlStick.instance != null)
                    {

                        MogoWorld.thePlayer.Idle();
                        MogoWorld.thePlayer.motor.StopNav();
                        ControlStick.instance.Reset();
                    }
                    else
                    {
                        MogoWorld.thePlayer.Idle();
                        MogoWorld.thePlayer.motor.StopNav();
                    }
                    break;
                }
            case "Gear":
                {
                    ClientEventData.TriggerGearEvent(Int32.Parse(commands[1]));
                    break;
                }
            case "RemoveEntities":
                {
                    MogoWorld.RemoveEntitiesPos();
                    break;
                }
            case "ResetEntities":
                {
                    MogoWorld.ResetEntitiesPos();
                    break;
                }
            default:
                break;
        }
    }
    #endregion
}

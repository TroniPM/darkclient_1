#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ScenesManager
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.3.18
// 模块描述：场景管理器
//----------------------------------------------------------------*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using UnityEngine;
using Mogo.GameData;
using Mogo.Util;
using System.Text.RegularExpressions;
using HMF;
using System.Collections;
namespace Mogo.Game
{
    public class ScenesManager : IEventManager
    {
        private Int32 m_lastScene = -1;
        private String m_lastSceneResourceName = String.Empty;
        private MapData m_currentMap;
        private List<UnityEngine.Object> m_sceneObjects;
        private UnityEngine.Object m_lightmap;
        private UnityEngine.Object m_lightProbes;
        private UnityEngine.Object m_globalUI;
        private UnityEngine.GameObject m_mainUI;

        public UnityEngine.GameObject MainUI
        {
            get { return m_mainUI; }
        }

        public ScenesManager()
        {
            m_sceneObjects = new List<UnityEngine.Object>();
        }

        public void AddListeners()
        {

        }

        public void RemoveListeners()
        {

        }

        public bool CheckSameScene(int id)
        {
            if (m_lastScene == id)
            {
                LoggerHelper.Debug("Same scene: " + id);
                return true;
            }
            else
                return false;
        }

        public void LoadHomeScene(Action<Boolean> loaded, Action<int> process = null)
        {
            LoadScene(MogoWorld.globalSetting.homeScene, (isLoadScene, data) =>
            {
                if (isLoadScene)
                {
                    LoadMainUI(() =>
                    {
                        LoadCamera(MogoWorld.globalSetting.homeScene, data, MogoUIManager.Instance.ShowMogoNormalMainUI);
                        if (loaded != null)
                            loaded(isLoadScene);
                    });
                }
                else
                {
                    LoadCamera(MogoWorld.globalSetting.homeScene, data, MogoUIManager.Instance.ShowMogoNormalMainUI);
                    if (loaded != null)
                        loaded(isLoadScene);
                }
            }, MogoMessageBox.Loading, true);
        }

        public void LoadCharacterScene(Action loaded, Action<int> process = null, bool CreateCharacter = false, bool hideReturnBtn = false)
        {
            Action action = () =>
            {
                MogoGlobleUIManager.Instance.ShowWaitingTip(true);
                var serverInfo = SystemConfig.GetSelectedServerInfo();
                if (CreateCharacter)
                {
                    //NewLoginUIViewManager.Instance.ShowCreateCharacterUI();
                    //NewLoginUIViewManager.Instance.ShowCreateCharacterUIBackBtn(!hideReturnBtn);
                    //if (serverInfo != null)
                    //    NewLoginUIViewManager.Instance.SetCreateCharacterServerBtnName(serverInfo.name);
                    NewLoginUILogicManager.Instance.LoadCreateCharacter(
                        () =>
                        {

                            if (loaded != null)
                                loaded();

                            NewLoginUIViewManager.Instance.ShowCreateCharacterUI();
                            NewLoginUIViewManager.Instance.ShowCreateCharacterUIBackBtn(!hideReturnBtn);
                            if (serverInfo != null)
                                NewLoginUIViewManager.Instance.SetCreateCharacterServerBtnName(serverInfo.name);

                            //MogoMessageBox.HideLoading();
                            MogoGlobleUIManager.Instance.ShowWaitingTip(false);
                        });

                }
                else
                {
                    NewLoginUIViewManager.Instance.ShowChooseCharacterUI();
                    //NewLoginUILogicManager.Instance.LoadChooseCharacterSceneAfterDelete();
                    if (serverInfo != null)
                        NewLoginUIViewManager.Instance.SetChooseCharacterServerBtnName(serverInfo.name);
                    if (loaded != null)
                        loaded();
                }

            };
            MogoMessageBox.ShowLoading();
            LoadScene(MogoWorld.globalSetting.chooseCharaterScene, (isLoadScene, data) =>
            {
                if (isLoadScene)
                {
                    if (LoginUIViewManager.Instance)
                    {
                        LoginUIViewManager.Instance.ReleaseUIAndResources();
                    }
                    LoadMainUI(() =>
                    {
                        MogoMessageBox.HideLoading();
                        MogoUIManager.Instance.ShowNewLoginUI(action);
                    });
                }
                else
                {
                    MogoMessageBox.HideLoading();
                    action();
                }
            }, MogoMessageBox.Loading, false);
        }

        public void LoadLoginScene(Action<Boolean> loaded, Action<int> process = null)
        {
            LoadScene(MogoWorld.globalSetting.loginScene, (isLoadScene, data) =>
            {
                //Debug.LogWarning("MogoWorld.globalSetting.loginScene");
                //Debug.LogWarning(GameObject.Find("long").name);
                GameObject longRoot = GameObject.Find("longRoot");
                if (longRoot != null && longRoot.GetComponent<LoginLong>() == null)
                {
                    GameObject.Find("longRoot").AddComponent<LoginLong>();
                }
                if (isLoadScene)
                {
                    LoadMainUI(() =>
                    {
                        MogoUIManager.Instance.ShowMogoLoginUI(() =>
                        {
                            if (Application.platform == RuntimePlatform.Android ||
                                Application.platform == RuntimePlatform.IPhonePlayer)
                            {
                                if (SystemSwitch.UsePlatformSDK)
                                {
                                    LoginUIViewManager.Instance.OnSwitchAccount = PlatformSdkManager.Instance.OnSwitchAccount;
                                    LoginUIViewManager.Instance.SwitchToAndroidMode();
                                }
                                  
                            }
                            if (loaded != null)
                                loaded(isLoadScene);
                        },
                        (progress) =>
                        {
                            process((int)(10 * progress + 90));
                        });
                    },
                    (progress) =>
                    {
                        process((int)(10 * progress + 80));
                    });
                }
                else
                    MogoUIManager.Instance.ShowMogoLoginUI(null);
            },
            (progress) =>
            {
                process((int)(30 * progress / 100 + 50));
            }, true);
        }

        public void LoadChooseServerScene(Action<Boolean> loaded, Action<int> process = null)
        {
            LoadScene(MogoWorld.globalSetting.chooseServerScene, (isLoadScene, data) =>
            {
                if (isLoadScene)
                {
                    LoadMainUI(() =>
                    {
                        MogoUIManager.Instance.ShowMogoChooseServerUI();
                    });
                    //AssetCacheMgr.GetUIInstance(MogoWorld.globalSetting.chooseServerUI, (prefab, guid, go) =>
                    //{
                    //    go.name = Mogo.Util.Utils.GetFileNameWithoutExtention(MogoWorld.globalSetting.chooseServerUI);
                    //});
                }
                else
                    MogoUIManager.Instance.ShowMogoChooseServerUI();
                if (loaded != null)
                    loaded(isLoadScene);
            }, MogoMessageBox.Loading, true);
        }

        public void LoadInstance(Int32 id, Action<Boolean> loaded, Action<int> process = null)
        {
            LoadScene(id, (isLoadScene, data) =>
            {
                if (isLoadScene)
                    LoadMainUI(() =>
                    {
                        if (loaded != null)
                            loaded(isLoadScene);
                        LoadCamera((int)id, data);
                        MogoUIManager.Instance.ShowMogoBattleMainUI();
                    });
                //AssetCacheMgr.GetUIInstance(MogoWorld.globalSetting.battleUI, (prefab, guid, go) =>
                //    {
                //        go.name = Mogo.Util.Utils.GetFileNameWithoutExtention(MogoWorld.globalSetting.battleUI);
                //    });
                //else
                //    MogoUIManager.Instance.ShowMogoBattleMainUI();
            }, MogoMessageBox.Loading, true);
        }

        /// <summary>
        /// 释放场景资源。
        /// </summary>
        public void UnloadScene(Action callBack)
        {
            if (m_currentMap != null)
            {
                try
                {
                    BillboardViewManager.Instance.Clear();
                    MogoFXManager.Instance.RemoveAllShadow();
                }
                catch (Exception ex)
                {
                    LoggerHelper.Except(ex);
                }
                foreach (var item in m_sceneObjects)
                {
                    //AssetCacheMgr.UnloadAsset(item);
                    AssetCacheMgr.ReleaseInstance(item);
                }
                m_sceneObjects.Clear();
                AssetCacheMgr.ReleaseResource(m_lightmap);
                m_lightmap = null;
                AssetCacheMgr.ReleaseResource(m_lightProbes);
                m_lightProbes = null;
                StoryManager.Instance.ClearPreload();
                SubAssetCacheMgr.ReleaseCharacterResources();
                SubAssetCacheMgr.ReleaseGearResources();
                SfxHandler.UnloadAllFXs();
                if (!String.IsNullOrEmpty(m_lastSceneResourceName))
                    AssetCacheMgr.ReleaseResource(m_lastSceneResourceName);
                //ResourceManager.UnloadUnusedAssets();
                //GC.Collect();
                if (callBack != null)
                    callBack();

                //LoggerHelper.Error("StartCoroutine UnloadUnusedAssets");
                //Driver.Instance.StartCoroutine(UnloadUnusedAssets(() =>
                //{
                //    //LoggerHelper.Error("UnloadUnusedAssets finish");
                //    GC.Collect();
                //    if (callBack != null)
                //        callBack();
                //}));
            }
            else
            {
                if (callBack != null)
                    callBack();
            }
        }

        private IEnumerator UnloadUnusedAssets(Action callBack)
        {
            //while (true)
            //{
            //LoggerHelper.Error("begin UnloadUnusedAssets");
            yield return Resources.UnloadUnusedAssets();
            //LoggerHelper.Error("end UnloadUnusedAssets");
            callBack();
            //    yield break;
            //}
        }

        private void LoadCharacterFX(Action callback, Action<int> process = null)
        {
            var fxs = new List<String>();
            //var sw = new System.Diagnostics.Stopwatch();
            //sw.Start();
            if (MogoWorld.thePlayer == null)
            {
                if (callback != null)
                    callback();
                return;
            }
            var playerName = MogoWorld.thePlayer.GameObject.name.ReplaceFirst("(Clone)", "");
            var weaponNumber = MogoWorld.thePlayer.GetEquipSubType();
            string weaponType;
            //Debug.LogError("weaponNumber" + weaponNumber);
            if (weaponNumber != 0)
            {
                weaponNumber = weaponNumber % 2 == 1 ? weaponNumber + 1 : weaponNumber - 1;
                weaponType = ((WeaponSubType)weaponNumber).ToString();
                //Debug.LogError(weaponType);
            }
            else
                weaponType = String.Empty;
            fxs.AddRange(FXData.dataMap.Values.Where(t => t.player.Contains(playerName) && !String.IsNullOrEmpty(t.resourcePath) && !t.resourcePath.Contains(weaponType)).Select(t => t.resourcePath));//加载主角特效
            LoadFX(fxs, callback, process);
        }

        private void LoadMonsterFx(IEnumerable<string> monstersList, Action callback, Action<int> process = null)
        {
            var fxs = (from t in FXData.dataMap.Values
                       where monstersList.Contains(t.player) && !String.IsNullOrEmpty(t.resourcePath)
                       select t.resourcePath).Distinct().ToList();//加载怪物特效
            LoadFX(fxs, callback, process);
        }

        private void LoadFX(List<String> fxs, Action callback, Action<int> process = null)
        {
            //Debug.LogError("preload avater fx: " + fxs.PackList('\n'));
            AssetCacheMgr.GetResourcesAutoRelease(fxs.ToArray(), (fxsObj) =>
            {
                foreach (var item in fxs)
                {
                    SfxHandler.AddloadedFX(item);
                }
                //foreach (var item in fxsObj)
                //{
                //    var go = GameObject.Instantiate(item);
                //    GameObject.Destroy(go);
                //}
                //sw.Stop();
                //LoggerHelper.Info("preload fx time: " + sw.ElapsedMilliseconds);
                if (callback != null)
                    callback();
            }, (progress) =>
            {
                if (process != null)
                    process((int)(10 * progress + 80));
            });
        }

        public void LoadMonster(int mapID, Action callback, Action<int> process = null)
        {
            var models = new List<int>();
            foreach (var item in MogoUtils.GetSpaceLevelID(mapID))
            {
                models.AddRange(MogoUtils.GetSpawnPointMonsterID(item));
            }
            //if (process != null)
            //    process(80);
            if (models.Count > 0)
            {
                var sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                var monstersList = models.Select(x => AvatarModelData.dataMap.Get(x).prefabName).Distinct();
                var ms = monstersList.ToArray();
                LoggerHelper.Warning("[Info] pre load resource" + ms.PackArray());
                SubAssetCacheMgr.GetCharacterResourcesAutoRelease(ms, (obj) =>
                {
                    sw.Stop();
                    LoggerHelper.Info("preload Monster time: " + sw.ElapsedMilliseconds);
                    //LoadFX(monstersList, callback, process);
                    if (callback != null)
                        callback();
                }, (progress) =>
                {
                    if (process != null)
                        process((int)(5 * progress + 90));
                });
            }
            else
            {
                if (callback != null)
                    callback();
            }

        }

        private void preloadResource(int id, Action<int> process = null, Action action = null)
        {
            if (id != MogoWorld.globalSetting.homeScene
                && MapData.dataMap.Get(id).type != MapType.ClimbTower)
            {
                if (MissionData.dataMap == null)//为登录界面做容错，当时没加载数据，但是实际上完全没用到
                {
                    if (action != null)
                        action();
                    return;
                }

                LoadCharacterFX(() =>
                {
                    if (MissionData.dataMap.Any(t => t.Value.mission == id))
                    {
                        //LoggerHelper.Error("LoadMonster.........");
                        LoadMonster(id,
                            () =>
                            {
                                var data = MissionData.dataMap.First(t => t.Value.mission == id).Value.preloadCG;
                                //load config
                                if (data != null)
                                {
                                    if (process != null)
                                        process(90);
                                    StoryManager.Instance.AllClear();
                                    foreach (var idx in data)
                                    {
                                        StoryManager.Instance.LoadConfig(idx);
                                    }
                                    if (process != null)
                                        process(95);
                                    StoryManager.Instance.Preload(data, () =>
                                    {
                                        if (process != null)
                                            process(97);
                                        if (action != null)
                                            action();
                                    });
                                }
                                else
                                {
                                    if (action != null)
                                        action();
                                }
                            }, process);
                    }
                    else
                    {
                        if (action != null)
                            action();
                    }

                }, process);
            }
            else
            {
                if (action != null)
                    action();
            }

        }
        /// <summary>
        /// 加载场景。
        /// </summary>
        /// <param name="id">场景标识</param>
        /// <param name="loaded">加载结束，参数为是否加载场景</param>
        /// <param name="needLoading">是否需要Loading界面</param>
        public void LoadScene(int id, Action<Boolean, MapData> loaded, Action<int> process = null, bool needLoading = true)
        {
            if (needLoading)
            {
                MogoMessageBox.ShowLoading();
            }

            if (m_lastScene == id)
            {
                LoggerHelper.Debug("Same scene in LoadScene: " + id);
                if (loaded != null)
                {
                    try
                    {
                        loaded(false, null);
                    }
                    catch (Exception ex)
                    {
                        LoggerHelper.Except(ex);
                    }
                    if (needLoading)
                    {
                        MogoMessageBox.HideLoading();
                    }
                }
                return;
            }
            //使用资源监测，在卸载前打印场景所使用的资源出来
            if (m_currentMap != null)
                ResourceWachter.Instance.SceneID = id;
            UnloadScene(() =>
            {
                MapData data;
                bool flag = MapData.dataMap.TryGetValue(id, out data);
                if (!flag)
                {
                    LoggerHelper.Error("map_setting id not exist: " + id);
                    if (loaded != null)
                    {
                        try
                        {
                            loaded(false, null);
                        }
                        catch (Exception ex)
                        {
                            LoggerHelper.Except(ex);
                        }
                        if (needLoading)
                        {
                            MogoMessageBox.HideLoading();
                        }
                    }
                    return;
                }
                m_lastScene = id;
                m_currentMap = data;

                if (id == MogoWorld.globalSetting.loginScene)
                    EventDispatcher.TriggerEvent(SettingEvent.BuildSoundEnvironment, id);

                Action loadScene = () =>
                {
                    LoadScene(data, (isLoadScene) =>
                    {
                        LoggerHelper.Debug("RenderSettings: " + id);

                        if (loaded != null)
                        {
                            try
                            {
                                if (process != null)
                                    process(80);
                                if (MogoWorld.thePlayer != null)
                                    MogoWorld.thePlayer.SetLightVisible(data.characherLight == GameData.LightType.Normal);
                                preloadResource(id, process, () =>
                                {
                                    if (process != null)
                                        process(99);
                                    loaded(isLoadScene, data);
                                    if (process != null)
                                        process(100);
                                });
                            }
                            catch (Exception ex)
                            {
                                LoggerHelper.Except(ex);
                            }


                        }
                    }, data.sceneName, process);
                };

                if (MogoWorld.thePlayer != null)
                {
                    MogoWorld.thePlayer.GotoPreparePosition();
                    //MogoWorld.thePlayer.AddCallbackInFrames(loadScene);
                }
                //else
                //    loadScene();
                loadScene();
                //TimerHeap.AddTimer(500, 0, loadScene);
            });
        }

        /// <summary>
        /// 切换光罩贴图与雾
        /// </summary>
        /// <param name="sceneId"></param>
        /// <param name="loaded"></param>
        public void SwitchLightMapFog(int sceneId, Action<Boolean> loaded)
        {
            var mapData = MapData.dataMap.GetValueOrDefault(sceneId, null);
            if (mapData != null)
            {
                SwitchLightMapFog(mapData, loaded);
            }
            else
            {
                LoggerHelper.Error("MapData not exist: " + sceneId);
                if (loaded != null)
                    loaded(false);
            }
        }

        private void SwitchLightMapFog(MapData data, Action<Boolean> loaded)
        {
            RenderSettings.fog = data.fog;
            RenderSettings.fogColor = data.fogColor;
            RenderSettings.fogMode = data.fogMode;
            RenderSettings.fogStartDistance = data.linearFogStart;
            RenderSettings.fogEndDistance = data.linearFogEnd;
            RenderSettings.ambientLight = data.ambientLight;

            if (String.IsNullOrEmpty(data.lightmap))
            {
                if (loaded != null)
                    loaded(true);
            }
            else
            {
                AssetCacheMgr.GetSceneResource(data.lightmap, (lm) =>
                {
                    AssetCacheMgr.UnloadAssetbundle(data.lightmap);
                    m_lightmap = lm;
                    LightmapData lmData = new LightmapData();
                    lmData.lightmapColor = lm as Texture2D;
                    LightmapSettings.lightmaps = new LightmapData[1] { lmData };
                    if (loaded != null)
                        loaded(true);
                });
                if (!String.IsNullOrEmpty(data.lightProbes))
                    AssetCacheMgr.GetSceneResource(data.lightProbes, (lp) =>
                    {
                        AssetCacheMgr.UnloadAssetbundle(data.lightProbes);
                        m_lightProbes = lp;
                        LightmapSettings.lightProbes = lp as LightProbes;
                    });
            }
        }

        /// <summary>
        /// 加载摄像机。
        /// </summary>
        private void LoadCamera(int missionID, MapData data, Action loaded = null)// = null
        {
            AssetCacheMgr.GetInstanceAutoRelease(MogoWorld.globalSetting.mainCamera, (prefab, id, go) =>
            {
                if (m_currentMap != null)
                {

                    go.name = Mogo.Util.Utils.GetFileNameWithoutExtention(MogoWorld.globalSetting.mainCamera);
                    var obj = go as GameObject;
                    obj.AddComponent<MogoMainCamera>();
                    //var fade = obj.AddComponent<FadeCamera>();
                    //fade.enabled = false;
                    //var light = obj.AddComponent<MogoCameraLighting>();
                    //light.enabled = false;

                    var camera = obj.GetComponent<Camera>();

                    UnityEngine.Object.Destroy(camera.GetComponent<AudioListener>());

                    camera.backgroundColor = m_currentMap.cameraColor;
                    camera.far = m_currentMap.cameraFar;

                    if (data != null
                        && data.layerList != null && data.layerList.Count != 0
                        && data.distanceList != null && data.distanceList.Count != 0
                        && data.layerList.Count == data.distanceList.Count)
                    {
                        var cull = (go as GameObject).AddComponent<MogoCameraCullByLayer>();
                        cull.LayerList = data.layerList;
                        cull.DistanceList = data.distanceList;
                    }

                    EventDispatcher.TriggerEvent(SettingEvent.BuildSoundEnvironment, missionID);
                }
                if (loaded != null)
                    loaded();
                TimerHeap.AddTimer(500, 0, MogoMessageBox.HideLoading);
            });
        }

        private void LoadGlobalUI(Action loaded)
        {
            //todo: 等待GlobalUI拆分。
        }

        public void UnloadMainUI()
        {
            if (m_mainUI)
            {
                try
                {
                    MogoUIManager.Instance.ReleaseMogoUI();
                    GameObject.Destroy(m_mainUI);
                    m_mainUI = null;
                }
                catch (Exception ex)
                {
                    LoggerHelper.Except(ex);
                }
            }
        }

        private void LoadMainUI(Action loaded, Action<float> progress = null)
        {
            if (m_mainUI)
            {
                if (loaded != null)
                    loaded();
            }
            else
                AssetCacheMgr.GetUIInstance(MogoWorld.globalSetting.homeUI, (prefab, guid, go) =>
                {
                    go.name = Mogo.Util.Utils.GetFileNameWithoutExtention(MogoWorld.globalSetting.homeUI);
                    GameObject.DontDestroyOnLoad(go);
                    var ui = go as GameObject;
                    ui.transform.localPosition = new Vector3(5000, 5000, 0);
                    if (ui)
                        ui.AddComponent<PrefabScriptManager>();
                    m_mainUI = ui;
                    if (loaded != null)
                        loaded();
                }, progress);
        }

        /// <summary>
        /// 加载场景。
        /// </summary>
        /// <param name="lightmap">场景名称</param>
        /// <param name="loaded">加载结束，参数为是否加载场景</param>
        private void LoadScene(MapData data, Action<Boolean> loaded, string sceneName, Action<int> process = null)
        {
            try
            {
                //场景文件读取完毕后处理
                Action sceneWasLoaded = () =>
                {
                    //if (MogoUIManager.Instance != null)
                    //    MogoUIManager.Instance.ReleaseMogoUI();
                    //场景文件加载完毕后处理
                    Action LevelWasLoaded = () =>
                    {
                        LoggerHelper.Debug("LevelWasLoaded: " + sceneName);
                        //if (!String.IsNullOrEmpty(m_lastSceneResourceName))
                        //    AssetCacheMgr.ReleaseResource(m_lastSceneResourceName);
                        if (data.modelName.Count == 0)
                            if (loaded != null)
                                loaded(true);

                        LoggerHelper.Debug("modelName Count: " + data.modelName.Count);
                        if (process != null)
                            process(20);
                        var models = data.modelName.Keys.ToList();
                        for (int i = 0; i < models.Count; i++)
                        {
                            var currentOrder = i;
                            LoggerHelper.Debug("modelName order: " + currentOrder);
                            //加载场景模型
                            AssetCacheMgr.GetSceneInstance(models[currentOrder],
                            (pref, id, go) =>
                            {
                                go.name = Utils.GetFileNameWithoutExtention(models[currentOrder]);
                                if (data.modelName[models[currentOrder]])// && SystemSwitch.ReleaseMode
                                    StaticBatchingUtility.Combine(go as GameObject);
                                LoggerHelper.Debug("sceneLoaded: " + go.name);
                                m_sceneObjects.Add(go);
                                if (currentOrder == data.modelName.Count - 1)
                                {
                                    AssetCacheMgr.UnloadAssetbundles(models.ToArray());
                                    SwitchLightMapFog(data, loaded);
                                }
                            }, (progress) =>
                            {
                                float cur = 60 * ((progress + currentOrder) / models.Count) + 20;
                                if (process != null)
                                    process((int)(cur));
                            });
                        }
                        Driver.Instance.LevelWasLoaded = null;
                        //if (process != null)
                        //    process(80);
                    };
                    if (sceneName != "10002_Login")
                    {

                        Driver.Instance.LevelWasLoaded = () =>
                        {
                            Driver.Instance.StartCoroutine(UnloadUnusedAssets(() =>
                            {
                                //LoggerHelper.Error("UnloadUnusedAssets finish");
                                GC.Collect();
                                LevelWasLoaded();
                            }));
                        };

                        //Driver.Instance.LevelWasLoaded = LevelWasLoaded;
                        Application.LoadLevel(sceneName);
                    }
                    else
                    {
                        LevelWasLoaded();
                    }
                    LoggerHelper.Debug("LoadLevel: " + sceneName);
                };

                if (SystemSwitch.ReleaseMode)
                {
                    if (process != null)
                        process(5);
                    //LoggerHelper.Info("sceneName: " + m_lastSceneResourceName);
                    m_lastSceneResourceName = string.Concat(sceneName, ".unity");
                    AssetCacheMgr.GetResource(m_lastSceneResourceName,//加载场景
                        (scene) =>
                        {
                            sceneWasLoaded();
                        },
                        (progress) =>
                        {
                            float cur = 15 * progress + 5;
                            if (process != null)
                                process((int)(cur));
                        }
                    );
                    //if (process != null)
                    //    process(40);
                }
                else
                    sceneWasLoaded();

            }
            catch (Exception ex)
            {
                LoggerHelper.Except(ex);
            }
        }
    }
}
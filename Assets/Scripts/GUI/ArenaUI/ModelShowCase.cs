using UnityEngine;
using System.Collections.Generic;
using Mogo.GameData;
using Mogo.Util;
using Mogo.Game;
using System;

public class ModelShowCase : MonoBehaviour
{
    public Transform left;
    public Transform right;

    private Vector3 m_vecThisInit = Vector3.zero;
    void Awake()
    {
        //LoadCreateCharacter();
        m_vecThisInit = transform.localPosition;
        TimerHeap.AddTimer(500, 0, ResetPos);
    }

    #region ��ɫģ��

    /// <summary>
    /// ��ǰģ�͵�ְҵ
    /// </summary>
    private int m_currentVocation;
    public int CurrentVocation
    {
        get { return m_currentVocation; }
        set
        {
            m_currentVocation = value;
        }
    }

    /// <summary>
    /// һ����Load��4����ɫģ��
    /// </summary>
    /// <param name="action"></param>
    private void LoadCharacters(Action action)
    {
        LoadCharacter((int)Vocation.Warrior, () =>
        {
            LoadCharacter((int)Vocation.Assassin, () =>
            {
                LoadCharacter((int)Vocation.Archer, () =>
                {
                    LoadCharacter((int)Vocation.Mage, () =>
                    {
                        if (action != null)
                        {

                            action();
                        }
                    });
                });
            });
        });
    }   

    private void LoadCharacter(int vocation, Action loaded)
    {
        var ci = CharacterInfoData.dataMap.GetValueOrDefault(vocation, new CharacterInfoData());
        var ai = AvatarModelData.dataMap.GetValueOrDefault(vocation, new AvatarModelData());

        if (ModelShowCaseLogicManager.Instance.AvatarList.ContainsKey(vocation) &&
            ModelShowCaseLogicManager.Instance.AvatarList[vocation].gameObject != null)
        {
            if (loaded != null)
            {
                loaded();
            }

            ResetViewPort(vocation);
            return;
        }

        AssetCacheMgr.GetInstance(ai.prefabName, (prefab, id, go) =>
        {
            var ety = new EtyAvatar();
            ety.vocation = vocation;
            ety.equipList = ci.EquipList;
            ety.weapon = ci.Weapon;
            ety.CreatePosition = Vector3.zero;
            var avatar = (go as GameObject);
            ety.gameObject = avatar;
            MogoFXManager.Instance.AddShadow(ety.gameObject, (uint)ety.gameObject.GetHashCode(), 1, 1, 1, 8);
            avatar.name = "ModelPool" + vocation.ToString();
            var cc = avatar.GetComponent<Collider>() as CharacterController;
            cc.radius = 0.5f;
            ety.animator = avatar.GetComponent<Animator>();
            ety.animator.applyRootMotion = false;


            ety.sfxHandler = avatar.AddComponent<SfxHandler>();

            ety.actorParent = avatar.AddComponent<ActorParent>();
            ety.actorParent.InitEquipment(vocation);
            //ety.actorParent.SetNakedEquid(ai.nakedEquipList);
            //ety.InitEquip();
            ety.gameObject.SetActive(false);
            if (ModelShowCaseLogicManager.Instance.AvatarList.ContainsKey(vocation))
            {
                ModelShowCaseLogicManager.Instance.AvatarList[vocation].sfxHandler.RemoveAllFX();
                AssetCacheMgr.ReleaseInstance(ModelShowCaseLogicManager.Instance.AvatarList[vocation].gameObject);
            }
            //ety.Hide();

            ModelShowCaseLogicManager.Instance.AvatarList[vocation] = ety;
            //avatar.transform.parent = transform;
            avatar.transform.localPosition = new Vector3(-1000, -1000, -1000);
            avatar.transform.localScale = Vector3.one;
            var ObjCamera = new GameObject();
            ObjCamera.transform.parent = avatar.transform.Find("slot_camera");
            ObjCamera.name = "Camera";
            ObjCamera.transform.localScale = new Vector3(1, 1, 1);
            ObjCamera.transform.localEulerAngles = new Vector3(270, 180, 0);
            switch (vocation)
            {
                case 1:
                    ObjCamera.transform.localPosition = new Vector3(0, -3.33f, 0);
                    break;
                case 2:
                    ObjCamera.transform.localPosition = new Vector3(0, -2.77f, 0);
                    break;
                case 3:
                    ObjCamera.transform.localPosition = new Vector3(0, -3f, 0);
                    break;
                case 4:
                    ObjCamera.transform.localPosition = new Vector3(0, -2.7f, 0);
                    break;
                default:
                    break;
            }

            var m_camera = ObjCamera.AddComponent<Camera>();
            m_camera.clearFlags = CameraClearFlags.Depth;
            m_camera.cullingMask = 1 << 8;
            m_camera.nearClipPlane = 0.01f;
            m_camera.farClipPlane = 5;
            m_camera.depth = 10;
            m_camera.fieldOfView = 40;
            var m_viewport = ObjCamera.AddComponent<UIViewport>();
            ModelShowCaseLogicManager.Instance.ViewportList[vocation] = m_viewport;
            m_viewport.sourceCamera = GameObject.Find("Camera").GetComponentInChildren<Camera>();//Camera.mainCamera;
            m_viewport.topLeft = left;
            m_viewport.bottomRight = right;
            if (loaded != null)
                loaded();
            // AssetCacheMgr.GetResource(ci.controller,
            //(obj) =>
            //{
            //    var controller = obj as RuntimeAnimatorController;
            //    if (ety.animator)
            //        ety.animator.runtimeAnimatorController = controller;
            //    if (loaded != null)
            //        loaded();
            //});
        });
    }

    public void LoadCreateCharacter(int voc, List<int> weaponList, Action callback)
    {
        //LoadCamera();
        //MogoWorld.inCity = false;
        //m_lastSelectedCharacter = null;

        CurrentVocation = voc;
        LoadCharacters(() =>
        {
            foreach (var item in ModelShowCaseLogicManager.Instance.AvatarList)
            {
                item.Value.gameObject.SetActive(false);
                if (item.Value.vocation == voc)
                {
                    RecursiveDo(0, item.Value, weaponList, callback);
                }
            }
        });
    }

    /// <summary>
    /// ��ȡ��ǰ��ɫģ��
    /// </summary>
    /// <returns></returns>
    private GameObject GetCurrentModel()
    {
        foreach (var item in ModelShowCaseLogicManager.Instance.AvatarList)
        {
            if (item.Value.vocation == CurrentVocation)
            {
                return item.Value.gameObject;
            }
        }

        return null;
    }
   
    #endregion

    private void ResetPos()
    {
        if (!IsModelDragEnable)
        {
            transform.position = Vector3.zero;
        }
        else
        {
            transform.localPosition = m_vecThisInit;
        }        
    }

    public void RecursiveDo(int id, EtyAvatar avatar, List<int> weaponList, Action callback)
    {
        if (id < weaponList.Count)
        {
            Equip(avatar, weaponList[id], () => { id++; RecursiveDo(id, avatar, weaponList, callback); });
        }
        else
        {
            if (callback != null)
            {
                callback();
            }
        }
    }

    public void Equip(EtyAvatar avatar, int _equipId, Action callback)
    {
        //Mogo.Util.LoggerHelper.Debug("_equipId:" + _equipId);
        if (avatar.gameObject == null)
        {
            Mogo.Util.LoggerHelper.Debug("Transform == null");
            //LoggerHelper.Error("Transform == null");
            return;
        }
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();

        if (!ItemEquipmentData.dataMap.ContainsKey(_equipId))
        {
            LoggerHelper.Error("can not find equip:" + _equipId);
            return;
        }
        ItemEquipmentData equip = ItemEquipmentData.dataMap[_equipId];
        if (equip.mode > 0)
        {
            if (avatar.actorParent == null)
            {
                Mogo.Util.LoggerHelper.Debug("the actor is null!!!!");
                //LoggerHelper.Error("the actor is null!!!!");
                return;
            }
            avatar.actorParent.m_isChangingWeapon = true;
            avatar.actorParent.Equip(equip.mode, () => 
            {
                if (equip.type == (int)EquipType.Weapon)
                {
                    //Mogo.Util.LoggerHelper.Debug("ControllerOfWeaponData:" + equip.id);
                    //Mogo.Util.LoggerHelper.Debug("ControllerOfWeaponData:" + equip.subtype);
                    //Mogo.Util.LoggerHelper.Debug("ControllerOfWeaponData:" + (int)vocation);
                    ControllerOfWeaponData controllerData = ControllerOfWeaponData.dataMap[equip.subtype];
                    RuntimeAnimatorController controller;
                    AssetCacheMgr.GetResource(controllerData.controller,
                    (obj) =>
                    {
                        controller = obj as RuntimeAnimatorController;
                        if (avatar.animator == null) return;
                        avatar.animator.runtimeAnimatorController = controller;


                        if (MogoWorld.inCity)
                        {
                            avatar.animator.SetInteger("Action", -1);
                        }
                        else
                        {
                            avatar.animator.SetInteger("Action", 0);
                        }
                        callback();
                    });
                }
                else
                {
                    callback();
                }
            });

            stopWatch.Stop();
        }
        else
        {
            callback();
        }
    }

    public void ResetViewPort(int vocation)
    {
        if (ModelShowCaseLogicManager.Instance.ViewportList.ContainsKey(vocation))
        {
            UIViewport viewport = ModelShowCaseLogicManager.Instance.ViewportList[vocation];
            viewport.topLeft = left;
            viewport.bottomRight = right;
        }
    }

    #region ��ת��ɫģ��

    private readonly static bool IsModelDragEnable = false;

    void OnDrag(Vector2 vec)
    {
        if (!IsModelDragEnable)
            return;

        GameObject goModel = GetCurrentModel();
        SetObjectLayer(10, goModel);
        if (goModel != null)
            goModel.transform.Rotate(new Vector3(0, -vec.x, 0));
    }

    /// <summary>
    /// ����ģ��Layer
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="obj"></param>
    public void SetObjectLayer(int layer, GameObject obj)
    {
        if (!IsModelDragEnable)
            return;

        if (obj != null)
        {
            obj.layer = layer;
            foreach (Transform item in obj.transform)
            {
                SetObjectLayer(layer, item.gameObject);
            }
        }        
    }

    /// <summary>
    /// ����ģ�����
    /// </summary>
    /// <param name="modelCamera"></param>
    /// <param name="fOffect"></param>
    /// <param name="xOffset"></param>
    /// <param name="yOffset"></param>
    /// <param name="zOffset"></param>
    /// <param name="fLookatOffsetY"></param>
    /// <param name="fLookatOffsetX"></param>
    public void SetCamera(Transform modelCamera, float fOffect = 0, float xOffset = 0, float yOffset = 0, float zOffset = 0, float fLookatOffsetY = 0f,
      float fLookatOffsetX = 0f)
    {
        if (!IsModelDragEnable)
        {
            modelCamera.gameObject.SetActive(false);
            return;
        }

        modelCamera.gameObject.SetActive(true);
        GameObject goModel = GetCurrentModel();
        CalOffsetPos(fOffect, xOffset, yOffset, zOffset, fLookatOffsetY, fLookatOffsetX);
        Vector3 pos = goModel.transform.Find("slot_camera").position;
        modelCamera.position = pos + goModel.transform.forward * fOffect;
        modelCamera.position += goModel.transform.right * xOffset;
        modelCamera.position += goModel.transform.up * yOffset;
        modelCamera.position += goModel.transform.forward * zOffset;
        modelCamera.localPosition += new Vector3(0, 0, goModel.transform.localPosition.z);
        modelCamera.LookAt(pos + new Vector3(fLookatOffsetX, fLookatOffsetY, 0));
        modelCamera.localRotation = Quaternion.identity;
        SetObjectLayer(10, goModel);
    }

    private void CalOffsetPos(float fOffect = 0, float xOffset = 0, float yOffset = 0, float zOffset = 0,
        float fLookatOffsetY = 0f, float fLookatOffsetX = 0f)
    {
        if (CurrentVocation == (int)Mogo.Game.Vocation.Assassin)
        {
            fOffect = 2.43f;

            xOffset = 0.49f;
            yOffset = 0.42f;
            zOffset = 0;

            fLookatOffsetY = 0.08f;
            fLookatOffsetX = 0f;
        }
        else if (CurrentVocation == (int)Mogo.Game.Vocation.Archer)
        {
            fOffect = 2.75f;

            xOffset = 0f;
            yOffset = 0.1f;
            zOffset = 0;
        }
        else if (CurrentVocation == (int)Mogo.Game.Vocation.Mage)
        {
            fOffect = 2.6f;

            xOffset = 0;
            yOffset = 0.15f;
            zOffset = 0;
        }
        else
        {
            fOffect = 2.82f;

            xOffset = 0.53f;
            yOffset = 0.59f;
            zOffset = 0;

            fLookatOffsetY = 0.02f;
            fLookatOffsetX = 0.02f;
        }
    }

    #endregion
}

using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using Mogo.Util;
using Mogo.GameData;

namespace Mogo.Game
{
    class EntityDrop : EntityParent
    {
        public int gold { get; set; }
        public int itemId { get; set; }
        public int belongAvatar { get; set; }

        private int m_glowFxId;

        private static String m_goldModelName = "";
        private static int m_vanishFxId;
        private static int m_pickUpFxId;
        private bool noModel = false;
        private uint tId = 0;

        public static int PickUpFxId
        {
            get { return m_pickUpFxId; }
        }

        private Vector3 tweenTarget;

        public EntityDrop()
            : base()
        {
            if (String.IsNullOrEmpty(m_goldModelName))
            {
                m_goldModelName = DropModelData.dataMap[80].prefab;
                m_vanishFxId = DropFXData.dataMap.First(t => t.Value.type == DropFxType.Vanish).Value.fx;
                m_pickUpFxId = DropFXData.dataMap.First(t => t.Value.type == DropFxType.PickUp).Value.fx;
            }
        }

        override public void OnMoveTo(GameObject g, Vector3 v)
        {
        }

        public override void CreateModel()
        {
            if (belongAvatar != MogoWorld.thePlayer.ID)//不是自己的东西不显示，目前Entity会被创建，统一在退出副本时销毁掉落实体
            {
                LoggerHelper.Debug("belongAvatar: " + belongAvatar + " " + MogoWorld.thePlayer.ID);
                return;
            }
            string modelName = null;
            if (gold != 0)//金币数量不为零，则这是金币
            {
                modelName = m_goldModelName;
            }
            else
            {//否则为装备
                int look = -1;
                int quality = -1;
                if (ItemEquipmentData.dataMap.ContainsKey(itemId))
                {
                    look = ItemEquipmentData.dataMap[itemId].look;
                    quality = ItemEquipmentData.dataMap[itemId].quality;
                }
                else if (ItemJewelData.dataMap.ContainsKey(itemId))
                {
                    look = ItemJewelData.dataMap[itemId].look;
                    quality = ItemJewelData.dataMap[itemId].quality;
                }
                else if (ItemMaterialData.dataMap.ContainsKey(itemId))
                {
                    look = ItemMaterialData.dataMap[itemId].look;
                    quality = ItemMaterialData.dataMap[itemId].quality;
                }
                if (look != -1)
                {
                    //var model = DropModelData.dataMap.FirstOrDefault(t => t.Value.type == equipment.type && t.Value.subtype == equipment.subtype);
                    var model = DropModelData.dataMap.Get(look);
                    if (model != null)
                        modelName = model.prefab;

                    var fx = DropFXData.dataMap.FirstOrDefault(t => t.Value.quality == quality);
                    if (fx.Value != null)
                        m_glowFxId = fx.Value.fx;
                }
            }
            LoggerHelper.Debug("EntityDrop create:" + ID);
            Action<UnityEngine.Object> action = (gameObject) =>
            {
                GameObject = gameObject as GameObject;
                Transform = GameObject.transform;
                Transform.position = position;
                //Transform.eulerAngles = rotation;
                Transform.localScale = scale;
                Transform.tag = "Item";
                UpdatePosition();

                sfxHandler = GameObject.AddComponent<SfxHandler>();
                BoxCollider collider = GameObject.AddComponent<BoxCollider>();
                collider.isTrigger = true;
                collider.size = new Vector3(2, 2, 4);
                var ap = GameObject.AddComponent<ActorDrop>();
                ap.theEntity = this;
                Actor = ap;
                if (m_glowFxId > 0)
                    sfxHandler.HandleFx(m_glowFxId);
                //ap.TweenTo(position, tweenTarget, 0.5f);

                Transform.position = new Vector3(tweenTarget.x, tweenTarget.y, tweenTarget.z);
                Vector3 tempVec = Transform.position;
                Mogo.Util.MogoUtils.GetPointInTerrain(tempVec.x, tempVec.z, out tempVec);
                Transform.position = new Vector3(tempVec.x, tempVec.y + 0.5f, tempVec.z);
                //    transform.position = tweenEnd + new Vector3(0, 0.5f, 0);

                ap.ThrowTo(Transform.position.y + 3f, 0.25f);
                tId = TimerHeap.AddTimer(1500, 0, DragToPlayer);
            };
            if (String.IsNullOrEmpty(modelName))
            {
                noModel = true;
                var go = new GameObject();
                action(go);
            }
            else
            {
                SubAssetCacheMgr.GetCharacterInstance(modelName,
                   (prefab, guid, gameObject) =>
                   {
                       action(gameObject);
                   }
               );
            }
        }

        private void DragToPlayer()
        {
            if (Transform == null) return;
            if (MogoWorld.thePlayer.Transform == null) return;
            (Actor as ActorDrop).DragTo(MogoWorld.thePlayer.Transform.Find("slot_camera"));
        }

        private void TimeOutPick()
        {
            MogoWorld.thePlayer.RpcCall("PickDropReq", ID);
            TimerHeap.AddTimer<string, uint>(30, 0, EventDispatcher.TriggerEvent, Events.FrameWorkEvent.AOIDelEvtity, ID);
        }

        public void SetTweenTarget(Vector3 target)
        {
            tweenTarget = target;
        }

        // 对象进入场景，在这里初始化各种数据， 资源， 模型等
        // 传入数据。
        override public void OnEnterWorld()
        {
            EventDispatcher.AddEventListener<GameObject, Vector3>(MogoMotor.ON_MOVE_TO, OnMoveTo);
            MogoWorld.DropPoints.Add(ID, tweenTarget);
        }

        // 对象从场景中删除， 在这里释放资源
        override public void OnLeaveWorld()
        {
            TimerHeap.DelTimer(tId);
            EventDispatcher.RemoveEventListener<GameObject, Vector3>(MogoMotor.ON_MOVE_TO, OnMoveTo);
            MogoWorld.DropPoints.Remove(ID);
            if (sfxHandler)
            {
                if (m_glowFxId > 0)
                {
                    sfxHandler.RemoveFXs(m_glowFxId);
                }
                if (m_vanishFxId > 0)
                {
                    sfxHandler.HandleFx(m_vanishFxId);
                }
            }
            base.OnLeaveWorld();
            if (noModel)
            {
                GameObject.DestroyObject(this.GameObject);
            }
        }
    }
}

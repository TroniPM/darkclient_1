using UnityEngine;
using System;
using System.Collections.Generic;
using Mogo.Util;
using Mogo.GameData;

namespace Mogo.Game
{
    class EntityTeleportSrc : EntityParent
    {
        public EntityTeleportSrc()
        {
        }

        override public void OnMoveTo(GameObject g, Vector3 v)
        {
        }

        public override void CreateModel()
        {
            //AvatarModelData data = AvatarModelData.dataMap[1013];
            LoggerHelper.Debug("TeleportPointSrcGearAgent create:" + ID);
            SubAssetCacheMgr.GetCharacterInstance("TeleportPointSrcGearAgent.prefab",
                (prefab, guid, gameObject) =>
                {
                    Transform = (gameObject as GameObject).transform;
                    Vector3 point;
                    if (MogoUtils.GetPointInTerrain(position.x, position.z, out point))
                        Transform.position = point;
                    //Transform.position = position;
                    Transform.eulerAngles = rotation;
                    Transform.localScale = scale;
                    Transform.tag = "Gear";

                    TeleportPointSrcGear gear = Transform.GetComponent<TeleportPointSrcGear>();
                    if (gear != null)
                    {
                        gear.ID = ID;
                        LoggerHelper.Debug("EntityTeleportSrc Gear ID: " + gear.ID);
                    }
                }
            );
        }


        // 对象进入场景，在这里初始化各种数据， 资源， 模型等
        // 传入数据。
        override public void OnEnterWorld()
        {
            // todo: 这里会加入数据解析
            sfxManager = new SfxManager(this);

            //AddUniqEventListener<GameObject, Vector3>(Events.OtherEvent.OnMoveTo, OnMoveTo);
            EventDispatcher.AddEventListener<GameObject, Vector3>(MogoMotor.ON_MOVE_TO, OnMoveTo);
            CreateModel();
        }

        // 对象从场景中删除， 在这里释放资源
        override public void OnLeaveWorld()
        {
            //RemoveUniqEventListener<GameObject, Vector3>(MogoMotor.ON_MOVE_TO, OnMoveTo);
            EventDispatcher.RemoveEventListener<GameObject, Vector3>(MogoMotor.ON_MOVE_TO, OnMoveTo);

        }
    }
}

using System;
using System.Collections.Generic;
using Mogo.Game;

namespace Mogo.RPC
{
    class OtherEntityPosSyncPluto : Pluto
    {
        protected override void DoDecode(byte[] data, ref int unLen)
        {
            var info = new CellAttachedInfo();
            info.id = (uint)VUInt32.Instance.Decode(data, ref unLen); // eid
            //info.face = (byte)VUInt8.Instance.Decode(data, ref unLen);// rotation
            UInt16 x = (UInt16)VUInt16.Instance.Decode(data, ref unLen);
            UInt16 y = (UInt16)VUInt16.Instance.Decode(data, ref unLen);
            info.x = (short)x;
            info.y = (short)y;
            UInt32 checkFlag = (UInt32)VUInt32.Instance.Decode(data, ref unLen);
            info.checkFlag = checkFlag;
            
            Arguments = new object[1] { info };
        }

        public override void HandleData()
        {
            var info = Arguments[0] as CellAttachedInfo;
            if (!MogoWorld.Entities.ContainsKey(info.id))
                return;
            var entity = MogoWorld.Entities[info.id];
            //entity.TurnTo(0, info.face * 2, 0);
            if (!(entity is EntityMercenary))
            {
                // >0移动到XY的服务器预计时间   ==0移动到XY    ==1当前服务器XY校验
                if (info.checkFlag == 0 )
                {
                    entity.MoveTo(info.position.x, info.position.z, 0, 0/*info.face * 2.0f*/, 0);
                }
                else if (info.checkFlag > 1)
                {
                    uint diaotaT = (uint)(UnityEngine.Time.realtimeSinceStartup * 1000) - MogoWorld.thePlayer.syncInfo[3];
                    uint curServerTime = MogoWorld.thePlayer.syncInfo[5] + diaotaT;
                    uint toTarXYUseTimeUsec = 0;
                    if (info.checkFlag > curServerTime)
                    {
                        toTarXYUseTimeUsec = info.checkFlag - curServerTime;
                    }
                    else
                    {
                        toTarXYUseTimeUsec = info.checkFlag - curServerTime;
                    }

                    MogoMotor theMotor = entity.motor;
                    if (theMotor == null)
                    {
                        return;
                    }
                    UnityEngine.Vector3 dstP = theMotor.targetToMoveTo;
                    float dis_entityP2dstP = UnityEngine.Vector2.Distance(new UnityEngine.Vector2(info.position.x, info.position.z),
        new UnityEngine.Vector2(entity.Transform.position.x, entity.Transform.position.z));//前端实体到目标距离
                    float adapterSpeed = dis_entityP2dstP / (toTarXYUseTimeUsec * 0.001f);
                    entity.speed = adapterSpeed;
                    theMotor.SetExrtaSpeed(entity.speed);
                  
                    entity.MoveTo(info.position.x, info.position.z, 0, 0/*info.face * 2.0f*/, 0);
                }
                else
                {
                    /*
                    if (entity != null && entity.Transform != null)
                    {
                        MogoMotor theMotor = entity.motor;
                        UnityEngine.Vector3 dstP = theMotor.targetToMoveTo;
                        float dis_entityP2dstP = UnityEngine.Vector2.Distance(new UnityEngine.Vector2(dstP.x, dstP.z),
        new UnityEngine.Vector2(entity.Transform.position.x, entity.Transform.position.z));//前端实体到目标距离
                        float dis_checkP2dstP = UnityEngine.Vector2.Distance(new UnityEngine.Vector2(dstP.x, dstP.z),
        new UnityEngine.Vector2(info.position.x, info.position.z));//后端实体到目标距离(忽略网络延迟)
                        float dis = UnityEngine.Vector2.Distance(new UnityEngine.Vector2(entity.Transform.position.x, entity.Transform.position.z),
new UnityEngine.Vector2(info.position.x, info.position.z));//后端实体到目标距离(忽略网络延迟)
                        //float dis = Math.Abs(dis_checkP2dstP - dis_entityP2dstP);

                        if ((dis_checkP2dstP < dis_entityP2dstP) && (dis > 0.4f))
                        {

                            entity.speed = entity.m_orgSpeed * 1.5f;
                            theMotor.SetExrtaSpeed(entity.speed);
                            //Mogo.Util.LoggerHelper.Error("need speed Up: " + entity.speed + dstP);
                        }
                        else if ((dis_checkP2dstP > dis_entityP2dstP) && (dis > 0.4f))
                        {
                            entity.speed = entity.m_orgSpeed * 0.7f;
                            theMotor.SetExrtaSpeed(entity.speed);
                            //Mogo.Util.LoggerHelper.Error("need speed Down: " + entity.speed + dstP);
                        }
                        else
                        {
                            entity.speed = entity.m_orgSpeed;
                            theMotor.SetExrtaSpeed(entity.speed);
                            //Mogo.Util.LoggerHelper.Error("need speed normal: " + entity.speed + dstP);
                        }

                    }
                    */
                }
            }
        }

        internal static Pluto Create()
        {
            return new OtherEntityPosSyncPluto();
        }
    }
}

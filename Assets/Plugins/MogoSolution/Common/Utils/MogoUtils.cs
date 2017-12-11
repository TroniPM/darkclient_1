#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MogoUtils
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.7.22
// 模块描述：游戏逻辑相关通用工具类。
//----------------------------------------------------------------*/
#endregion
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using System.Security;
using Mogo.GameData;
using System.Text.RegularExpressions;

namespace Mogo.Util
{

    /// <summary>
    /// 游戏逻辑相关通用工具类。
    /// </summary>
    public static class MogoUtils
    {
        #region 范围距离判断


        static public bool CrossPointOld(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            //Vector3 rst = Vector3.zero;
            float d = (v2.z - v1.z) * (v4.x - v3.x) - (v4.z - v3.z) * (v2.x - v1.x);
            if (d == 0)
            {//平行线
                return false;
            }
            //x0   =   [(x2-x1)*(x4-x3)*(y3-y1)+(y2-y1)*(x4-x3)*x1-(y4-y3)*(x2-x1)*x3]/d
            //y0   =   [(y2-y1)*(y4-y3)*(x3-x1)+(x2-x1)*(y4-y3)*y1-(x4-x3)*(y2-y1)*y3]/(-d)
            float x0 = ((v2.x - v1.x) * (v4.x - v3.x) * (v3.z - v1.z) + (v2.z - v1.z) * (v4.x - v3.x) * v1.x - (v4.z - v3.z) * (v2.x - v1.x) * v3.x) / d;
            float y0 = ((v2.z - v1.z) * (v4.z - v3.z) * (v3.x - v1.x) + (v2.x - v1.x) * (v4.z - v3.z) * v1.z - (v4.x - v3.x) * (v2.z - v1.z) * v3.z) / -d;
            //(x0-x1)*(x0-x2) <=0
            //(x0-x3)*(x0-x4) <=0
            //(y0-y1)*(y0-y2) <=0
            //(y0-y3)*(y0-y4) <=0
            if (((x0 - v1.x) * (x0 - v2.x) <= 0) &&
                ((x0 - v3.x) * (x0 - v4.x) <= 0) &&
                ((y0 - v1.z) * (y0 - v2.z) <= 0) &&
                ((y0 - v3.z) * (y0 - v4.z) <= 0))
            {//相交，交点为x0,y0
                //rst = new Vector3(x0, 0, y0);
                return true;
            }

            return false;
        }

        static private float mulpti(Vector3 ps, Vector3 pe, Vector3 p)
        {
            float m;
            m = (pe.x - ps.x) * (p.z - ps.z) - (p.x - ps.x) * (pe.z - ps.z);
            return m;
        }


        static public bool CrossPoint(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            if (Math.Max(v1.x, v2.x) >= Math.Min(v3.x, v4.x) &&
                Math.Max(v3.x, v4.x) >= Math.Min(v1.x, v2.x) &&
                Math.Max(v1.z, v2.z) >= Math.Min(v3.z, v4.z) &&
                Math.Max(v3.z, v4.z) >= Math.Min(v1.z, v2.z) &&
                mulpti(v1, v2, v3) * mulpti(v1, v2, v4) <= 0 &&
                mulpti(v3, v4, v1) * mulpti(v3, v4, v2) <= 0)
                return true;
            else
                return false;
        }

        static public bool InRect(Vector3 p, Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
        {//检测 p点在v0, v1, v2, v3构成的四边形中
            bool rst = false;
            int cnt = 0; //交点计数
            Vector3 p1 = p + new Vector3(50, 0, 0);
            if (CrossPoint(p, p1, v0, v1))
            {
                cnt++;
            }
            if (CrossPoint(p, p1, v1, v2))
            {
                cnt++;
            }
            if (CrossPoint(p, p1, v2, v3))
            {
                cnt++;
            }
            if (CrossPoint(p, p1, v3, v0))
            {
                cnt++;
            }
            if (cnt == 1)
            {
                rst = true;
            }
            return rst;
        }

        static public List<List<uint>> GetEntitiesWorldRange(float x1, float y1, float x2, float y2, LayerMask layerMask = LayerMask.Monster | LayerMask.Character | LayerMask.Trap)
        {//要求填的矩形和世界坐标轴平行
            List<List<uint>> list = new List<List<uint>>();
            List<uint> listDummy = new List<uint>();
            List<uint> listMonster = new List<uint>();
            List<uint> listPlayer = new List<uint>();
            List<uint> listMercenary = new List<uint>();
            list.Add(listDummy);
            list.Add(listMonster);
            list.Add(listPlayer);
            list.Add(listMercenary);
            if (!MogoWorld.Entities.ContainsKey(MogoWorld.thePlayer.ID))
            {
                MogoWorld.Entities.Add(MogoWorld.thePlayer.ID, MogoWorld.thePlayer);
            }
            if (x1 > x2 && y1 < y2)
            {
                float s = x1;
                x1 = x2;
                x2 = s;
                s = y1;
                y1 = y2;
                y2 = s;
            }
            //x1,y1为左上角,x2,y2为右下角
            foreach (var item in MogoWorld.Entities)
            {
                if (item.Value.Transform == null)
                {
                    continue;
                }
                Vector3 p = item.Value.Transform.position;
                if (p.x < x1 || p.z > y1 || p.x > x2 || p.z < y2)
                {
                    continue;
                }
                if (item.Value is EntityDummy)
                {
                    listDummy.Add(item.Key);
                }
                else if (item.Value is EntityMonster)
                {
                    listMonster.Add(item.Key);
                }
                else if (item.Value is EntityPlayer)
                {
                    listPlayer.Add(item.Key);
                }
                else if (item.Value is EntityMercenary)
                {
                    listMercenary.Add(item.Key);
                }
            }
            MogoWorld.Entities.Remove(MogoWorld.thePlayer.ID);
            return list;
        }

        static public List<List<uint>> GetEntitiesFrontLineNew(Transform t, float length, Vector3 direction, float width, float offsetX = 0, float offsetY = 0, float angleOffset = 0, LayerMask layerMask = LayerMask.Monster | LayerMask.Character | LayerMask.Trap)
        {
            return GetEntitiesFrontLineNew(t.localToWorldMatrix, t.rotation, t.forward, t.position, length, direction, width, offsetX, offsetY, angleOffset, layerMask);
        }

        /// <summary>
        /// 返回 角色前方，指定范围内的所有对象
        /// </summary>
        /// <param name="t"></param>
        /// <param name="distance"></param>
        /// <param name="layerMask"></param>
        /// <returns></returns>
        static public List<List<uint>> GetEntitiesFrontLine(this Transform t, float distance, Vector3 direction, float radius = 0.5f, float offset = 0, float angleOffset = 0, LayerMask layerMask = LayerMask.Monster | LayerMask.Character | LayerMask.Trap)
        {
            return GetEntitiesFrontLine(t.localToWorldMatrix, t.rotation, t.forward, t.position, distance, direction, radius, offset, angleOffset, layerMask);
        }

        /// <summary>
        /// 返回角色周围指定半径范围内的所有对象。
        /// </summary>
        /// <param name="t"></param>
        /// <param name="radius"></param>
        /// <param name="layerMask"></param>
        /// <returns></returns>
        static public List<List<uint>> GetEntitiesInRange(this Transform t, float radius, float offsetX = 0, float offsetY = 0, float angleOffset = 0, LayerMask layerMask = LayerMask.Monster | LayerMask.Character | LayerMask.Trap)
        {
            return GetEntitiesInRange(t.localToWorldMatrix, t.rotation, t.forward, t.position, radius, offsetX, offsetY, angleOffset, layerMask);
        }

        static public List<List<uint>> GetEntitiesInRange(Vector3 position, float radius, float offsetX = 0, float offsetY = 0, float angleOffset = 0, LayerMask layerMask = LayerMask.Monster | LayerMask.Character | LayerMask.Trap)
        {
            List<List<uint>> list = new List<List<uint>>();
            List<uint> listDummy = new List<uint>();
            List<uint> listMonster = new List<uint>();
            List<uint> listPlayer = new List<uint>();
            List<uint> listMercenary = new List<uint>();
            if (!MogoWorld.Entities.ContainsKey(MogoWorld.thePlayer.ID))
            {
                MogoWorld.Entities.Add(MogoWorld.thePlayer.ID, MogoWorld.thePlayer);
            }
            //遍历entities
            foreach (KeyValuePair<uint, Mogo.Game.EntityParent> pair in MogoWorld.Entities)
            {
                EntityParent entity = pair.Value;
                if (!entity.Transform)
                {
                    continue;
                }
                if ((1 << entity.Transform.gameObject.layer & (int)layerMask) == 0) continue;

                float entityRadius = ((float)entity.GetIntAttr("scaleRadius")) / 100f;
                if ((position - entity.Transform.position).magnitude > radius + entityRadius) continue;

                if (pair.Value is EntityDummy)
                {
                    listDummy.Add(pair.Key);
                }
                else if (pair.Value is EntityMonster)
                {
                    listMonster.Add(pair.Key);
                }
                else if (pair.Value is EntityPlayer)
                {
                    listPlayer.Add(pair.Key);
                }
                else if (pair.Value is EntityMercenary)
                {
                    listMercenary.Add(pair.Key);
                }

            }
            MogoWorld.Entities.Remove(MogoWorld.thePlayer.ID);
            list.Add(listDummy);
            list.Add(listMonster);
            list.Add(listPlayer);
            list.Add(listMercenary);

            return list;
        }

        /// <summary>
        /// 返回角色周围指定扇形范围内的所有对象。
        /// </summary>
        /// <param name="t"></param>
        /// <param name="radius"></param>
        /// <param name="angle"></param>
        /// <param name="layerMask"></param>
        /// <returns></returns>
        static public List<List<uint>> GetEntitiesInSector(this Transform t, float radius, float angle = 180f, float offsetX = 0, float offsetY = 0, float angleOffset = 0, LayerMask layerMask = LayerMask.Monster | LayerMask.Character | LayerMask.Trap)
        {
            return GetEntitiesInSector(t.localToWorldMatrix, t.rotation, t.forward, t.position, radius, angle, offsetX, offsetY, angleOffset, layerMask);
        }

        static public List<List<uint>> GetEntitiesFrontLineNew(Matrix4x4 ltwM, Quaternion rotation, Vector3 forward, Vector3 position, float length, Vector3 direction, float width, float offsetX = 0, float offsetY = 0, float angleOffset = 0, LayerMask layerMask = LayerMask.Monster | LayerMask.Character | LayerMask.Trap)
        {
            List<List<uint>> list = new List<List<uint>>();
            List<uint> listDummy = new List<uint>();
            List<uint> listMonster = new List<uint>();
            List<uint> listPlayer = new List<uint>();
            List<uint> listMercenary = new List<uint>();
            list.Add(listDummy);
            list.Add(listMonster);
            list.Add(listPlayer);
            list.Add(listMercenary);


            if (!MogoWorld.Entities.ContainsKey(MogoWorld.thePlayer.ID))
            {
                MogoWorld.Entities.Add(MogoWorld.thePlayer.ID, MogoWorld.thePlayer);
            }
            //float sin = (float)Math.Sin(angleOffset * Math.PI / 180);
            //float cos = (float)Math.Cos(angleOffset * Math.PI / 180);
            foreach (KeyValuePair<uint, Mogo.Game.EntityParent> pair in MogoWorld.Entities)
            {
                if (pair.Value.Transform == null)
                {
                    continue;
                }
                float r = pair.Value.GetIntAttr("scaleRadius") * 0.01f;
                Matrix4x4 m = ltwM;
                Matrix4x4 m1 = new Matrix4x4();
                m1.SetRow(0, new Vector4(0, 0, 0, (width + r) * 0.5f + offsetY)); //1
                m1.SetRow(1, new Vector4(0, 1, 0, 0));
                m1.SetRow(2, new Vector4(0, 0, 0, 0)); //-1
                m1.SetRow(3, new Vector4(0, 0, 0, 1));
                m = m * m1;
                Vector3 v0 = new Vector3(m.m03, m.m13, m.m23);

                m = ltwM;
                m1.SetRow(2, new Vector4(0, 0, 0, (length + r + offsetX)));
                m = m * m1;
                Vector3 v1 = new Vector3(m.m03, m.m13, m.m23);

                m = ltwM;
                m1.SetRow(0, new Vector4(0, 0, 0, -(width + r) * 0.5f + offsetY));
                m = m * m1;
                Vector3 v2 = new Vector3(m.m03, m.m13, m.m23);

                m = ltwM;
                m1.SetRow(2, new Vector4(0, 0, 0, (0 + offsetX)));
                m = m * m1;
                Vector3 v3 = new Vector3(m.m03, m.m13, m.m23);

                Vector3 p = pair.Value.Transform.position;
                if (!InRect(p, v0, v1, v2, v3))
                {
                    continue;
                }
                if (pair.Value is EntityDummy)
                {
                    listDummy.Add(pair.Key);
                }
                else if (pair.Value is EntityMonster)
                {
                    listMonster.Add(pair.Key);
                }
                else if (pair.Value is EntityPlayer)
                {
                    listPlayer.Add(pair.Key);
                }
                else if (pair.Value is EntityMercenary)
                {
                    listMercenary.Add(pair.Key);
                }
            }
            MogoWorld.Entities.Remove(MogoWorld.thePlayer.ID);
            return list;
        }

        /// <summary>
        /// 返回 角色前方，指定范围内的所有对象
        /// </summary>
        /// <param name="t"></param>
        /// <param name="distance"></param>
        /// <param name="layerMask"></param>
        /// <returns></returns>
        static public List<List<uint>> GetEntitiesFrontLine(Matrix4x4 ltwM, Quaternion rotation, Vector3 forward, Vector3 position, float distance, Vector3 direction, float radius = 0.5f, float offset = 0, float angleOffset = 0, LayerMask layerMask = LayerMask.Monster | LayerMask.Character | LayerMask.Trap)
        {
            List<List<uint>> list = new List<List<uint>>();
            List<uint> listDummy = new List<uint>();
            List<uint> listMonster = new List<uint>();
            List<uint> listPlayer = new List<uint>();

            RaycastHit[] hits = Physics.SphereCastAll(position, radius, direction, distance, (int)layerMask);

            foreach (RaycastHit hit in hits)
            {
                EntityParent entity = hit.transform.GetComponent<ActorParent>().GetEntity();
                if (entity is EntityDummy)
                {
                    listDummy.Add(entity.ID);
                }
                else if (entity is EntityMonster)
                {
                    listMonster.Add(entity.ID);
                }
                else if (entity is EntityPlayer)
                {
                    listPlayer.Add(entity.ID);
                }
            }
            list.Add(listDummy);
            list.Add(listMonster);
            list.Add(listPlayer);

            return list;
        }

        /// <summary>
        /// 返回角色周围指定半径范围内的所有对象。
        /// </summary>
        /// <param name="t"></param>
        /// <param name="radius"></param>
        /// <param name="layerMask"></param>
        /// <returns></returns>
        static public List<List<uint>> GetEntitiesInRange(Matrix4x4 ltwM, Quaternion rotation, Vector3 forward, Vector3 position, float radius, float offsetX = 0, float offsetY = 0, float angleOffset = 0, LayerMask layerMask = LayerMask.Monster | LayerMask.Character | LayerMask.Trap)
        {
            List<List<uint>> list = new List<List<uint>>();
            List<uint> listDummy = new List<uint>();
            List<uint> listMonster = new List<uint>();
            List<uint> listPlayer = new List<uint>();
            List<uint> listMercenary = new List<uint>();
            if (!MogoWorld.Entities.ContainsKey(MogoWorld.thePlayer.ID))
            {
                MogoWorld.Entities.Add(MogoWorld.thePlayer.ID, MogoWorld.thePlayer);
            }
            //float sin = (float)Math.Sin(angleOffset * Math.PI / 180);
            //float cos = (float)Math.Cos(angleOffset * Math.PI / 180);
            Matrix4x4 m = ltwM;
            Matrix4x4 m1 = new Matrix4x4();
            m1.SetRow(0, new Vector4(0, 0, offsetY, 0)); //1
            m1.SetRow(1, new Vector4(0, 1, 0, 0));
            m1.SetRow(2, new Vector4(0, 0, 0, offsetX)); //-1
            m1.SetRow(3, new Vector4(0, 0, 0, 1));
            m = m * m1;
            Vector3 posi = new Vector3(m.m03, m.m13, m.m23);
            //遍历entities
            foreach (KeyValuePair<uint, Mogo.Game.EntityParent> pair in MogoWorld.Entities)
            {
                EntityParent entity = pair.Value;
                if (!entity.Transform)
                {
                    continue;
                }
                if ((1 << entity.Transform.gameObject.layer & (int)layerMask) == 0) continue;

                float entityRadius = ((float)entity.GetIntAttr("scaleRadius")) / 100f;
                //if ((t.position - entity.Transform.position).magnitude > radius + entityRadius) continue;
                if ((posi - entity.Transform.position).magnitude > radius + entityRadius) continue;

                if (pair.Value is EntityDummy)
                {
                    listDummy.Add(pair.Key);
                }
                else if (pair.Value is EntityMonster)
                {
                    listMonster.Add(pair.Key);
                }
                else if (pair.Value is EntityPlayer)
                {
                    listPlayer.Add(pair.Key);
                }
                else if (pair.Value is EntityMercenary)
                {
                    listMercenary.Add(pair.Key);
                }

            }
            MogoWorld.Entities.Remove(MogoWorld.thePlayer.ID);
            list.Add(listDummy);
            list.Add(listMonster);
            list.Add(listPlayer);
            list.Add(listMercenary);

            return list;
        }

        /// <summary>
        /// 返回角色周围指定扇形范围内的所有对象。
        /// </summary>
        /// <param name="t"></param>
        /// <param name="radius"></param>
        /// <param name="angle"></param>
        /// <param name="layerMask"></param>
        /// <returns></returns>
        static public List<List<uint>> GetEntitiesInSector(Matrix4x4 ltwM, Quaternion rotation, Vector3 forward, Vector3 position, float radius, float angle = 180f, float offsetX = 0, float offsetY = 0, float angleOffset = 0, LayerMask layerMask = LayerMask.Monster | LayerMask.Character | LayerMask.Trap)
        {
            List<List<uint>> list = new List<List<uint>>();
            List<uint> listDummy = new List<uint>();
            List<uint> listMonster = new List<uint>();
            List<uint> listPlayer = new List<uint>();
            List<uint> listMercenary = new List<uint>();

            if (!MogoWorld.Entities.ContainsKey(MogoWorld.thePlayer.ID))
            {
                MogoWorld.Entities.Add(MogoWorld.thePlayer.ID, MogoWorld.thePlayer);
            }
            Matrix4x4 m = ltwM;
            Matrix4x4 m1 = new Matrix4x4();
            m1.SetRow(0, new Vector4(0, 0, 0, offsetY)); //1
            m1.SetRow(1, new Vector4(0, 1, 0, 0));
            m1.SetRow(2, new Vector4(0, 0, 0, offsetX)); //-1
            m1.SetRow(3, new Vector4(0, 0, 0, 1));
            m = m * m1;
            Vector3 posi = new Vector3(m.m03, m.m13, m.m23);
            //遍历entities
            foreach (KeyValuePair<uint, Mogo.Game.EntityParent> pair in MogoWorld.Entities)
            {
                EntityParent entity = pair.Value;
                if (entity.Transform == null)
                {
                    continue;
                }
                if ((1 << entity.Transform.gameObject.layer & (int)layerMask) == 0) continue;

                float entityRadius = ((float)entity.GetIntAttr("scaleRadius")) / 100f;
                //if ((t.position - entity.Transform.position).magnitude > radius + entityRadius) continue;
                if ((posi - entity.Transform.position).magnitude > radius + entityRadius) continue;

                //得到切线与（目标物体到人物线）的夹角a
                //float a = Mathf.Atan(entityRadius / (entity.Transform.position - t.position).magnitude);
                float a = Mathf.Asin(entityRadius / (entity.Transform.position - posi).magnitude);

                //得到目标点与人物正前方的夹角b
                //float b = Vector3.Angle((entity.Transform.position - t.position), t.forward);
                float b = Vector3.Angle((entity.Transform.position - posi), forward);

                //判断b - a 是否在 angle/2内
                if ((b - a) > angle / 2) continue;

                if (pair.Value is EntityDummy)
                {
                    listDummy.Add(pair.Key);
                }
                else if (pair.Value is EntityMonster)
                {
                    listMonster.Add(pair.Key);
                }
                else if (pair.Value is EntityPlayer)
                {
                    listPlayer.Add(pair.Key);
                }
                else if (pair.Value is EntityMercenary)
                {
                    listMercenary.Add(pair.Key);
                }

            }
            MogoWorld.Entities.Remove(MogoWorld.thePlayer.ID);
            list.Add(listDummy);
            list.Add(listMonster);
            list.Add(listPlayer);
            list.Add(listMercenary);

            return list;
        }

        static public List<Transform> GetTransformsInSector(this Transform t, Dictionary<Transform, float> transformDic, float radius, float angle = 180f, LayerMask layerMask = LayerMask.Trap)
        {
            List<Transform> resultList = new List<Transform>();


            //遍历entities
            foreach (KeyValuePair<Transform, float> pair in transformDic)
            {
                if ((1 << pair.Key.gameObject.layer & (int)layerMask) == 0) continue;

                float entityRadius = pair.Value;
                if ((t.position - pair.Key.position).magnitude > radius + entityRadius) continue;

                //得到切线与（目标物体到人物线）的夹角a
                float a = Mathf.Asin(entityRadius / (pair.Key.position - t.position).magnitude);

                //得到目标点与人物正前方的夹角b
                float b = Vector3.Angle((pair.Key.position - t.position), t.forward);

                //判断b - a 是否在 angle/2内
                if ((b - a) > angle / 2) continue;

                resultList.Add(pair.Key);
            }
            return resultList;
        }

        static public List<Transform> GetTransformsInCircle(this Transform t, Dictionary<Transform, float> transformDic, float radius, LayerMask layerMask = LayerMask.Trap)
        {
            List<Transform> resultList = new List<Transform>();

            //遍历entities
            foreach (KeyValuePair<Transform, float> pair in transformDic)
            {
                if ((1 << pair.Key.gameObject.layer & (int)layerMask) == 0) continue;

                float entityRadius = pair.Value;
                if ((t.position - pair.Key.position).magnitude > radius + entityRadius) continue;

                //得到切线与（目标物体到人物线）的夹角a
                float a = Mathf.Asin(entityRadius / (pair.Key.position - t.position).magnitude);

                //得到目标点与人物正前方的夹角b
                float b = Vector3.Angle((pair.Key.position - t.position), t.forward);

                //判断b - a 是否在 angle/2内
                if ((b - a) > 180) continue;

                resultList.Add(pair.Key);
            }
            return resultList;
        }

        static public List<Transform> GetTransformsInRange(Vector3 position, Dictionary<Transform, float> transformDic, float radius, LayerMask layerMask = LayerMask.Trap)
        {
            List<Transform> list = new List<Transform>();

            //遍历entities
            foreach (KeyValuePair<Transform, float> pair in transformDic)
            {
                if ((1 << pair.Key.gameObject.layer & (int)layerMask) == 0) continue;

                float entityRadius = pair.Value;
                if ((position - pair.Key.position).magnitude > radius + entityRadius) continue;

                list.Add(pair.Key);

            }
            return list;
        }

        static public List<Transform> GetTransformsFrontLineNew(this Transform t, Dictionary<Transform, float> transformDic, float length, Vector3 direction, float width, LayerMask layerMask = LayerMask.Trap)
        {
            List<Transform> list = new List<Transform>();

            foreach (KeyValuePair<Transform, float> pair in transformDic)
            {
                float r = pair.Value;
                Matrix4x4 m = t.localToWorldMatrix;
                Matrix4x4 m1 = new Matrix4x4();
                m1.SetRow(0, new Vector4(0, 0, 0, (width + r) * 0.5f)); //1
                m1.SetRow(1, new Vector4(0, 1, 0, 0));
                m1.SetRow(2, new Vector4(0, 0, 0, 0)); //-1
                m1.SetRow(3, new Vector4(0, 0, 0, 1));
                m = m * m1;
                Vector3 v0 = new Vector3(m.m03, m.m13, m.m23);

                m = t.localToWorldMatrix;
                m1.SetRow(2, new Vector4(0, 0, 0, (length + r)));
                m = m * m1;
                Vector3 v1 = new Vector3(m.m03, m.m13, m.m23);

                m = t.localToWorldMatrix;
                m1.SetRow(0, new Vector4(0, 0, 0, -(width + r) * 0.5f));
                m = m * m1;
                Vector3 v2 = new Vector3(m.m03, m.m13, m.m23);

                m = t.localToWorldMatrix;
                m1.SetRow(2, new Vector4(0, 0, 0, 0));
                m = m * m1;
                Vector3 v3 = new Vector3(m.m03, m.m13, m.m23);

                Vector3 p = pair.Key.position;
                if (!InRect(p, v0, v1, v2, v3))
                {
                    continue;
                }

                list.Add(pair.Key);
            }
            return list;
        }

        /// <summary>
        /// 在角色当前朝向的半径radius,扇形角度angle内，在对人物朝向进行最小角度的调整其中一个怪物。
        /// 如果区域内无怪，则不调整
        /// </summary>
        /// <param name="t"></param>
        /// <param name="radius"></param>
        /// <param name="angle"></param>
        /// <param name="layerMask"></param>
        static public void LookAtTargetInRange(this Transform t, float radius, float angle, LayerMask layerMask = LayerMask.Monster | LayerMask.Character | LayerMask.Trap)
        {
            Vector3 targetPos = Vector3.zero;
            float angleMax = Mathf.Infinity;
            bool isFindSome = false;
            //遍历entities
            foreach (KeyValuePair<uint, Mogo.Game.EntityParent> pair in MogoWorld.Entities)
            {
                EntityParent entity = pair.Value;
                if (entity.curHp <= 0) continue;
                if (entity.IsDeath()) continue;
                if (entity is EntityPlayer) continue;
                if (entity.Transform == null || t == null)
                {
                    continue;
                }
                if ((1 << entity.Transform.gameObject.layer & (int)layerMask) == 0) continue;

                float entityRadius = ((float)entity.GetIntAttr("scaleRadius")) / 100f;
                if ((t.position - entity.Transform.position).magnitude > radius + entityRadius) continue;

                //得到切线与（目标物体到人物线）的夹角a
                float a = Mathf.Atan(entityRadius / (entity.Transform.position - t.position).magnitude);

                //得到目标点与人物正前方的夹角b
                float b = Vector3.Angle((entity.Transform.position - t.position), t.forward);

                //判断b - a 是否在 angle/2内
                if ((b - a) > angle / 2) continue;

                //Vector3 tempDir = new Vector3(-t.forward.z, 0, t.forward.x).normalized;
                //Vector3 lineLeft = Vector3.Slerp(t.forward, tempDir, (angle / (2 * 90f)));
                //Vector3 lineRight = Vector3.Slerp(t.forward, -tempDir, (angle / (2 * 90f)));
                //if ((GetDisBetweenLineAndPoint(lineLeft, t.position) > entityRadius || GetDisBetweenLineAndPoint(lineRight, t.position) > entityRadius)
                //    && Vector3.Angle((entity.Transform.position - t.position), t.forward) > (angle / 2f)) continue;
                //if (Vector3.Angle((entity.Transform.position - t.position), t.forward) > (angle / 2f)) continue;

                isFindSome = true;
                if ((b - a) < angleMax)
                {
                    angleMax = b - a;
                    targetPos = pair.Value.Transform.position;
                }

            }

            if (isFindSome)
            {
                t.LookAt(new Vector3(targetPos.x, t.transform.position.y, targetPos.z));
            }
        }

        static public bool GetPointInTerrain(float x, float z, out Vector3 point)
        {
            //Mogo.Util.LoggerHelper.Debug("GetPointInTerrain");
            RaycastHit hit;
            //var flag = Physics.Raycast(new Vector3(x, 500, z), Vector3.down, out hit,1000, (int)LayerMask.Terrain);
            var flag = Physics.Linecast(new Vector3(x, 1000, z), new Vector3(x, -1000, z), out hit, (int)LayerMask.Terrain);
            if (flag)
            {
                //Mogo.Util.LoggerHelper.Debug("hit.point: " + hit.point);
                point = new Vector3(hit.point.x, hit.point.y + 0.2f, hit.point.z);
                return true;
            }
            else
            {
                point = new Vector3(x, 50, z);
                //Mogo.Util.LoggerHelper.Debug("hit nothing: " + x + " " + z);
                LoggerHelper.Warning("hit noting:" + x + "," + z);
                return false;
            }

        }

        /// <summary>
        /// 由近到远排序
        /// </summary>
        /// <param name="t"></param>
        /// <param name="gos"></param>
        /// <param name="count">返回数量</param>
        /// <returns></returns>
        static public void SortByDistance(this Transform t, List<GameObject> gos)
        {
            gos.Sort(delegate(GameObject a, GameObject b)
            {
                Vector3 aPos = a.transform.position;
                Vector3 bPos = b.transform.position;
                if (Vector3.Distance(t.position, aPos) >= Vector3.Distance(t.position, bPos)) return 1;
                else return -1;
            });

        }

        static public void SortByDistance(this Transform t, List<Transform> gos)
        {
            gos.Sort(delegate(Transform a, Transform b)
            {
                Vector3 aPos = a.position;
                Vector3 bPos = b.position;
                if (Vector3.Distance(t.position, aPos) >= Vector3.Distance(t.position, bPos)) return 1;
                else return -1;
            });

        }

        static public void SortByDistance(this Transform t, List<uint> gos)
        {
            gos.Sort(delegate(uint a, uint b)
            {
                Vector3 aPos = MogoWorld.Entities[a].position;
                Vector3 bPos = MogoWorld.Entities[b].position;
                if (Vector3.Distance(t.position, aPos) >= Vector3.Distance(t.position, bPos)) return 1;
                else return -1;
            });

        }

        /// <summary>
        /// 支持深层孩子
        /// </summary>
        static public Transform GetChild(Transform transform, string boneName)
        {
            Transform child = transform.Find(boneName);
            if (child == null)
            {
                foreach (Transform c in transform)
                {
                    child = GetChild(c, boneName);
                    if (child != null) return child;
                }
            }
            return child;
        }

        /// <summary>
        /// 得到所有孩子，无层次限制
        /// </summary>
        static public List<Transform> GetAllChild(Transform transform)
        {
            LoggerHelper.Debug(transform.name);
            List<Transform> children = new List<Transform>();
            children.AddRange(transform.GetComponentsInChildren<Transform>());
            foreach (Transform child in children)
            {
                children.AddRange(GetAllChild(child));
            }

            return children;
        }

        #endregion

        #region 获取设备参数

        /// <summary>
        /// 获取设备参数。
        /// </summary>
        /// <returns></returns>
        public static string GetDeviceInfo()
        {
            var props = typeof(SystemInfo).GetProperties();
            var needed = new Dictionary<string, string>();
            needed["deviceModel"] = "DM";
            needed["operatingSystem"] = "OS";
            needed["processorType"] = "PT";
            needed["processorCount"] = "PC";
            needed["graphicsDeviceName"] = "GDN";
            needed["systemMemorySize"] = "SMS";
            needed["graphicsMemorySize"] = "GMS";
            var sb = new System.Text.StringBuilder();

            foreach (var item in props)
            {
                if (needed.ContainsKey(item.Name))
                {
                    var value = item.GetGetMethod().Invoke(null, null);
                    sb.AppendFormat("{0}: {1}\n", needed[item.Name], value);
                }
            }

            return sb.ToString();
        }

        #endregion
        static public List<int> GetSpaceLevelID(int mapID)
        {
            List<int> spawnLevelID = new List<int>();

            var path = string.Concat(SystemConfig.ResourceFolder, SystemConfig.CONFIG_SUB_FOLDER, "spaces/s", mapID.ToString(), SystemConfig.CONFIG_FILE_EXTENSION);
            var xml = XMLParser.Load(path);
            if (xml == null)
            {
                return spawnLevelID;
            }

            foreach (SecurityElement item in xml.Children)
            {
                if (item.Tag != "mapname")
                {
                    foreach (SecurityElement subItem in item.Children)
                    {
                        foreach (SecurityElement subItem2 in subItem.Children)
                        {
                            if (subItem2 != null && subItem2.Tag == "levelID" && subItem2.Text != null)
                            {
                                String[] texts = Regex.Split(subItem2.Text.Trim(), @"[ ,]+");
                                spawnLevelID.AddRange(texts.Select(x => Int32.Parse(x)).ToList());
                            }
                        }
                    }
                }
            }
            return spawnLevelID.Distinct().ToList();
        }

        static public List<int> GetSpawnPointMonsterID(int levelID)
        {
            List<int> m_monsterIDs = new List<int>();
            if (SpawnPointLevelData.dataMap.ContainsKey(levelID))
            {
                var ids = SpawnPointLevelData.dataMap.Get(levelID).monsterId;
                if (ids != null)
                    m_monsterIDs.AddRange(ids);
            }
            List<int> models = new List<int>();

            foreach (var item in m_monsterIDs)
            {
                if (MonsterData.dataMap.ContainsKey(item))
                {
                    int model = MonsterData.GetData(item).model;
                    if (model > 0)
                    {
                        models.Add(model);
                    }
                }
                else
                    LoggerHelper.Error(String.Format("cannot find id {0} in monster.xml", item));
            }
            return models.Distinct().ToList();
        }
        static public void SetImageColor(UISprite sprite, int color = 0, int posZ = - 5)
        {
            Vector3 position = sprite.gameObject.transform.localPosition;
            sprite.gameObject.transform.localPosition = new Vector3(position.x, position.y, posZ);
            if (color == -1) return;

            switch (color)
            {
                case 0: sprite.ShowAsWhiteBlack(false); break;
                case 1: sprite.ShowAsLakeBlue(); break;
                case 2: sprite.ShowAsGreen(); break;
                case 3: sprite.ShowAsDeepBlue(); break;
                case 4: sprite.ShowAsPurpose(); break;
                case 5: sprite.ShowAsOrange(); break;
                case 6: sprite.ShowAsRed(); break;
                case 7: sprite.ShowAsYellow(); break;
                case 8: sprite.ShowAsRoseRed(); break;
                case 9: sprite.ShowAsGrassGreen(); break;
                case 10: sprite.ShowAsWhiteBlack(true); break;
                case 11: sprite.ShowAsDragonGreen(); break;
                case 12: sprite.ShowAsDragonBlue(); break;
                case 13: sprite.ShowAsDragonPurpose(); break;
                case 14: sprite.ShowAsDragonOrange(); break;
                case 15: sprite.ShowAsDragonDarkGold(); break;
                default: break;
            }

            //Mogo.Util.LoggerHelper.Debug(color + " !!!!!!!!!!!!!!!!!!!!!!!!");
        }

        public static string GetRedString(string str)
        {
            return string.Concat("[FF0000]", str, "[-]");
        }

        public static string GetStrWithQulityColor(string str, int quality)
        {
            if (!QualityColorData.dataMap.ContainsKey(quality))
                quality = 1;

            QualityColorData colorData = QualityColorData.dataMap[quality];
            string qualityColor = colorData.color;
            return string.Concat("[", qualityColor, "]", str, "[-]");
        }

        static public Vector3 ConvertWorldPos(Camera fromCamera, Camera toCamera, Vector3 pos)
        {
            return toCamera.ScreenToWorldPoint(fromCamera.WorldToScreenPoint(pos));
        }
        static public void AddBillboard(String name, Vector3 pos, Action<GameObject> loaded)
        {
            AssetCacheMgr.GetUIInstance(name + ".prefab",
                    (prefab, guid, go) =>
                    {
                        GameObject obj = go as GameObject;
                        obj.transform.parent = GameObject.Find("MogoMainUIPanel/BillboardList").transform;
                        obj.transform.position = MogoUtils.ConvertWorldPos(Camera.main, GameObject.Find("Camera").GetComponentsInChildren<Camera>(true)[0], pos);
                        obj.name = "DoorText";
                        if (loaded != null)
                        {
                            loaded(obj);
                        }
                    }
                );
        }

        public static List<KeyValuePair<int, TValue>> OrderByKey<TValue>(this Dictionary<int, TValue> dic)
        {
            var myList = dic.ToList();

            myList.Sort((firstPair, nextPair) =>
            {
                return firstPair.Key == nextPair.Key ? 0 : (firstPair.Key < nextPair.Key ? -1 : 1);
            });
            return myList;
        }

        public static List<KeyValuePair<TKey, TValue>> OrderByValue<TKey, TValue>(this Dictionary<TKey, TValue> dic, Func<TValue, int> comparer)
        { 
            var myList = dic.ToList();

            myList.Sort((firstPair, nextPair) =>
            {
                var firstPairValue = comparer(firstPair.Value);
                var nextPairValue = comparer(nextPair.Value);
                return firstPairValue == nextPairValue ? 0 : (firstPairValue < nextPairValue ? -1 : 1);
            }
            );
            return myList;
        }
    }
}
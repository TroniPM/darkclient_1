/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：SpellManager
// 创建者：Steven Yang
// 修改者列表：
// 创建日期：2013-3-5
// 模块描述：技能管理系统
//----------------------------------------------------------------*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Mogo.Game;
using Mogo.GameData;
using Mogo.Util;
using Mogo.FSM;

public class SkillManager
{
    public static bool showSkillRange = false;
    protected EntityParent theOwner;

    public SkillManager(EntityParent owner)
    {
        theOwner = owner;
    }

    virtual public void Clean()
    {
    }

    #region 攻击

    public void AttackEffect(int hitActionID, Matrix4x4 ltwm, Quaternion rotation, Vector3 forward, Vector3 position)
    {
        SkillAction s = SkillAction.dataMap[hitActionID];
        if (s.damageFlag == 0)
        {//等于1才有伤害
            return;
        }
        // dummyList, MonsterList, PlayerList, MercenaryList
        List<List<uint>> list = GetHitEntities(hitActionID, ltwm, rotation, forward, position);

        List<Transform> hitContainers = GetHitContainers(hitActionID);

        if (list.Count != 4)
        {
            return;
        }
        if (theOwner is EntityMyself && list[0].Count > 0)
        {
            // 客户端导向技能
            AttackDummy(hitActionID, list[0]);
        }
        if (theOwner is EntityMyself && list[3].Count > 0)
        {
            AttackMercenary(hitActionID, list[3]);
        }
        if (theOwner is EntityDummy && list[2].Count > 0)
        {
            AttackPlayer(hitActionID, list[2]);
        }
        if (theOwner is EntityDummy && list[3].Count > 0)
        {
            AttackMercenary(hitActionID, list[3]);
        }
        if (theOwner is EntityMercenary && theOwner.m_factionFlag > 0 && list[0].Count > 0)
        {
            // 客户端导向技能
            AttackDummy(hitActionID, list[0]);
        }

        if (hitContainers.Count > 0)
        {
            AttackContainers(theOwner, hitActionID, hitContainers);
        }

        #region 技能触发机关

        if (theOwner is EntityDummy && s.triggerEvent != 0)
        {
            AttackTriggerGearEvent(s.triggerEvent);
        }

        #endregion
    }

    private void AttackContainers(EntityParent theOwner, int hitActionID, List<Transform> hitContainers)
    {
        foreach (Transform target in hitContainers)
        {
            if (!Container.containers.ContainsKey(target))
                continue;

            if (!Container.containers[target].triggleEnable)
                continue;

            if ((theOwner is EntityMyself && Container.containers[target].isMyselfAttackable)
                || (theOwner is EntityMercenary && theOwner.ID == MogoWorld.theLittleGuyID && Container.containers[target].isLittelGuyAttackable)
                || (theOwner is EntityMercenary && theOwner.ID != MogoWorld.theLittleGuyID && Container.containers[target].isMercenarayAttackable)
                || (theOwner is EntityDummy && Container.containers[target].isDummyAttackable))
                Container.containers[target].OnDeath(hitActionID);
        }
    }

    private void AttackTriggerGearEvent(int eventID)
    {
        ClientEventData.TriggerGearEvent(eventID);
    }

    private void AttackDummy(int hitActionID, List<uint> dummys)
    {
        Dictionary<uint, List<int>> wounded = new Dictionary<uint, List<int>>();
        for (int i = 0; i < dummys.Count; i++)
        {
            List<int> harm = new List<int>();
            if (!MogoWorld.Entities.ContainsKey(dummys[i]))
            {
                continue;
            }
            EntityParent e = MogoWorld.Entities[dummys[i]];
            if (Utils.BitTest(e.stateFlag, StateCfg.NO_HIT_STATE) == 1 || Utils.BitTest(e.stateFlag, StateCfg.DEATH_STATE) == 1)
            {//不可击中状态
                continue;
            }
            harm = CalculateDamage.CacuDamage(hitActionID, theOwner.ID, dummys[i]);
            // harm[1] = 100000;
            if ((e as EntityDummy).MonsterData.clientBoss == 1)
            {//为体验关特制
                harm[1] = harm[1] * 1000 + RandomHelper.GetRandomInt(100);
            }
            //harm[1] = 10000;
            if (MogoWorld.unhurtDummy)
            {
                harm[1] = 0;
            }
            wounded.Add(dummys[i], harm);
            uint h = 0;
            h = e.curHp < harm[1] ? e.curHp : (uint)harm[1];
            e.curHp -= h;
        }
        TriggerDamage(hitActionID, wounded);
        for (int i = 0; i < dummys.Count; i++)
        {
            if (!MogoWorld.Entities.ContainsKey(dummys[i]))
            {
                continue;
            }
            EntityParent e = MogoWorld.Entities[dummys[i]];
            AttackEnemyGenAnger(e);//计算怒气
            if (e.curHp <= 0)
            {//前端怪死亡
                MogoWorld.thePlayer.RpcCall("CliEntityActionReq", e.ID, (uint)1, (uint)(e.Transform.position.x * 100.0f), (uint)(e.Transform.position.z * 100.0f));
                e.OnDeath(hitActionID);
                (e as EntityDummy).stateFlag = Mogo.Util.Utils.BitSet((e as EntityDummy).stateFlag, StateCfg.DEATH_STATE);
            }
        }
        TriggerCombo(dummys.Count);
    }

    private void AttackMercenary(int hitActionID, List<uint> mercenarys)
    {//只有dummy打mercenary才会进此方法,所以mercenary肯定是纯雇佣兵
        Dictionary<uint, List<int>> wounded = new Dictionary<uint, List<int>>();
        for (int i = 0; i < mercenarys.Count; i++)
        {
            List<int> harm = new List<int>();
            if (!MogoWorld.Entities.ContainsKey(mercenarys[i]))
            {
                continue;
            }
            EntityParent e = MogoWorld.Entities[mercenarys[i]];
            if (!(e.m_factionFlag > 0))
            {
                continue;
            }
            if (Utils.BitTest(e.stateFlag, StateCfg.NO_HIT_STATE) == 1 || Utils.BitTest(e.stateFlag, StateCfg.DEATH_STATE) == 1)
            {//不可击中状态
                continue;
            }
            harm = CalculateDamage.CacuDamage(hitActionID, theOwner.ID, mercenarys[i]);
            // harm[1] = 0;
            // harm[1] = 100000;
            wounded.Add(mercenarys[i], harm);
            if (theOwner is EntityMyself)
            {//用于标记是否是PVP主角打离线玩家
                harm.Add(1);
                continue;
            }
            if (e.curHp < harm[1])
            {
                e.curHp = 0;
                Int16 x = (Int16)(e.Transform.position.x * 100);
                Int16 z = (Int16)(e.Transform.position.z * 100);
                byte face = (byte)(e.Transform.eulerAngles.y * 0.5);
                MogoWorld.thePlayer.RpcCall("UpdateMercenaryCoord", e.ID, x, z, face, 0);
            }
            else
            {
                e.curHp -= (uint)harm[1];
            }
        }
        TriggerDamage(hitActionID, wounded);
    }

    private void AttackPlayer(int hitActionID, List<uint> players)
    {
        Dictionary<uint, List<int>> wounded = new Dictionary<uint, List<int>>();
        for (int i = 0; i < players.Count; i++)
        {
            List<int> harm = new List<int>();
            harm = CalculateDamage.CacuDamage(hitActionID, theOwner.ID, players[i]);
            harm[1] = harm[1];
            if (MogoWorld.unhurtMe)
            {
                harm[1] = 0;
            }
            wounded.Add(players[i], harm);
            if (players[i] == MogoWorld.thePlayer.ID)
            {
                EventDispatcher.TriggerEvent(Events.ComboEvent.ResetCombo);
                if (theOwner is EntityDummy && (theOwner as EntityDummy).MonsterData.clientBoss == 1)
                {//为体验关特制
                    continue;
                }
                if (MogoWorld.thePlayer.IsNewPlayer)
                {
                    continue;
                }
                if (MogoWorld.thePlayer.curHp <= harm[1])
                {
                    MogoWorld.thePlayer.curHp = 0;
                    MogoWorld.thePlayer.RpcCall("CliEntityActionReq", MogoWorld.thePlayer.ID, (uint)2, MogoWorld.thePlayer.curHp, (uint)0);
                }
                else
                {
                    MogoWorld.thePlayer.curHp -= (uint)harm[1];
                }
                AttackSelfGenAnger(MogoWorld.thePlayer); //产生怒气
                continue;
            }
            if (!MogoWorld.Entities.ContainsKey(players[i]))
            {
                continue;
            }
            EntityParent e = MogoWorld.Entities[players[i]];
            uint h = 0;
            h = e.curHp < harm[1] ? e.curHp : (uint)harm[1];
            e.curHp -= h;
        }
        TriggerDamage(hitActionID, wounded);
    }

    public void AttackEnemyGenAnger(EntityParent e)
    {
        if (!(theOwner is EntityMyself))
        {
            return;
        }
        uint p = 10 - (uint)(e.curHp / e.hp);
        int cp = (int)(p - e.angerStep);
        if (cp < 0)
        {
            cp = 0;
            return;
        }
        e.angerStep = (int)p;
        if ((MogoWorld.thePlayer.skillManager as PlayerSkillManager).isAnger)
        {
            return;
        }
        if (!(MogoWorld.thePlayer.skillManager as PlayerSkillManager).ChargeAble())
        {
            return;
        }
        MogoWorld.thePlayer.Anger += (uint)cp * (uint)(200 * 0.01f);
    }

    public void AttackSelfGenAnger(EntityParent e)
    {
        uint p = 100 - (uint)(e.curHp * 100 / e.hp);
        int cp = (int)(p - e.angerStep);
        if (cp < 0)
        {
            cp = 0;
            return;
        }
        e.angerStep = (int)p;
        if ((MogoWorld.thePlayer.skillManager as PlayerSkillManager).isAnger)
        {
            return;
        }
        if (!(MogoWorld.thePlayer.skillManager as PlayerSkillManager).ChargeAble())
        {
            return;
        }
        MogoWorld.thePlayer.Anger += (uint)cp * (uint)(200 * 0.01f);
    }

    // 发出攻击
    public void OnAttacking(int hitActionID, Matrix4x4 ltwm, Quaternion rotation, Vector3 forward, Vector3 position)
    {
        //能进本方法，说明已经能过前面的ID存在校验
        SkillAction action = SkillAction.dataMap[hitActionID];
        // 如果需要，关闭碰撞
        if (action.removeCollider == 1)
        {
            // 调用自己去除碰撞
            Physics.IgnoreLayerCollision(8, 11);
            TimerHeap.AddTimer(500, 0, () => { Physics.IgnoreLayerCollision(8, 11, false); });
        }
        if (action.freeze == 1)
        {//冻结周围, hitAction后解冻
            FreezeButMyself();
        }
        if (action.replication == 1)
        {//分身
            theOwner.CreateDuplication();
        }
        // 播动作
        int act = action.action;
        if (act > 0)
        {
            if (PlayerActionNames.names.ContainsKey(act))
            {
                theOwner.skillActName = PlayerActionNames.names[act];
            }
            else
            {
                theOwner.skillActName = "";
            }

            if (theOwner is EntityMercenary && theOwner.blackBoard.skillReversal == 0 && !theOwner.NotTurn())
            {//雇佣兵每action都会面朝敌人
                EntityParent enemy = theOwner.GetTargetEntity();
                if (enemy != null && enemy.Transform != null && theOwner.Transform != null)
                {
                    //theOwner.motor.SetTargetToLookAt(enemy.Transform);
                    theOwner.Transform.LookAt(new Vector3(enemy.Transform.position.x, theOwner.Transform.position.y, enemy.Transform.position.z));
                }
            }
            theOwner.SetAction(act);
        }
        AttackingFx(action);
        AttackingMove(action);
        AttackBuff(action);
        
        List<object> args = new List<object>();
        args.Add(ltwm);
        args.Add(rotation);
        args.Add(forward);
        args.Add(position);
        theOwner.delayAttackTimerID = TimerHeap.AddTimer((uint)(action.actionBeginDuration / theOwner.aiRate), 0, DelayAttack, hitActionID, args);
        if (theOwner is EntityDummy && action.spawnPoint > 0 && MogoWorld.IsClientMission)
        {
            MogoWorld.thePlayer.RpcCall("CliEntitySkillReq", (uint)1, (uint)action.spawnPoint);
        }
        //theOwner.breakAble = false;
    }

    private void AttackBuff(SkillAction action)
    {
        if (action.casterAddBuff != null)
        {
            foreach (int id in action.casterAddBuff)
            {
                theOwner.ClientAddBuff(id);
            }
        }
        if (action.casterDelBuff != null)
        {
            foreach (int id in action.casterDelBuff)
            {
                theOwner.ClientDelBuff(id);
            }
        }
    }

    private void AttackingMove(SkillAction action)
    {
        MogoMotor theMotor = theOwner.motor;
        if (theMotor == null)
        {
            return;
        }
        float extraSpeed = action.extraSpeed;
        if (extraSpeed != 0)
        {
            if (action.extraSt <= 0)
            {
                theMotor.SetExrtaSpeed(extraSpeed);
                theMotor.SetMoveDirection(theOwner.Transform.forward);
                TimerHeap.AddTimer<MogoMotor>((uint)action.extraSl, 0, (m) => { m.SetExrtaSpeed(0); }, theMotor);
            }
            else
            {
                TimerHeap.AddTimer<int>((uint)action.extraSt, 0, DelayExtraMove, action.id);
            }
        }
        else
        {
            theMotor.SetExrtaSpeed(0);
        }

        // 是否允许，在技能过程中使用 摇杆改变方向
        //if (theOwner is EntityMyself)
        //{
        //    theMotor.enableStick = action.enableStick > 0;
        //}

        if (action.teleportDistance > 0 && extraSpeed <= 0)
        {
            Vector3 dst = Vector3.zero;
            dst = theOwner.Transform.position + theOwner.Transform.forward * action.teleportDistance;
            theMotor.TeleportTo(dst);
        }
    }

    private void AttackingFx(SkillAction action)
    {
        if (!MogoWorld.showSkillFx)
        {
            return;
        }
        // 播放特效
        theOwner.PlaySfx(action.id);
        if (action.cameraTweenId > 0)
        {//有震屏,调用震屏接口
            TimerHeap.AddTimer<int, float>((uint)(action.cameraTweenST * 1000), 0, MogoMainCamera.Instance.Shake, action.cameraTweenId, action.cameraTweenSL);
        }
        if (action.sound != "")
        {//有技能释放音效
            //TimerHeap.AddTimer<string>(skill.soundST * 1000, 0, soundplayInterface, skill.sound);
        }
    }

    private void DelayExtraMove(int hitActionID)
    {
        SkillAction action = SkillAction.dataMap[hitActionID];
        MogoMotor theMotor = theOwner.motor;
        float extraSpeed = action.extraSpeed;
        theMotor.SetExrtaSpeed(extraSpeed);
        theMotor.SetMoveDirection(theOwner.Transform.forward);
        TimerHeap.AddTimer<MogoMotor>((uint)action.extraSl, 0, (m) => { m.SetExrtaSpeed(0); }, theMotor);
    }

    private void DelayAttack(int hitActionID, List<object> args)
    {
        Matrix4x4 ltwm = (Matrix4x4)args[0];
        Quaternion rotation = (Quaternion)args[1];
        Vector3 forward = (Vector3)args[2];
        Vector3 position = (Vector3)args[3];
        //theOwner.breakAble = true;
        if (theOwner is EntityDummy)
        {
            AttackEffect(hitActionID, ltwm, rotation, forward, position);
        }
        else if (theOwner is EntityMyself)
        {
            AttackEffect(hitActionID, ltwm, rotation, forward, position);
        }
        else if (theOwner is EntityMercenary)
        {
            AttackEffect(hitActionID, ltwm, rotation, forward, position);
        }
        if (showSkillRange)
        {
            ShowFightRange(hitActionID);
        }
    }

    private void ShowFightRange(int hitActionID)
    {
        SkillAction action = SkillAction.dataMap[hitActionID];
        float offsetX = action.hitXoffset;
        float offsetY = action.hitYoffset;
        float angleOffset = 180;
        switch (action.targetRangeType)
        {
            case 0://扇形
                {
                    float raidus = action.targetRangeParam[0] * 0.01f;
                    float angle = action.targetRangeParam[1];
                    var cube = new GameObject();
                    var filter = cube.AddComponent<MeshFilter>();
                    cube.AddComponent<MeshRenderer>();
                    DrawCircle(cube, filter, raidus, angle, offsetX, offsetY, angleOffset);
                    TimerHeap.AddTimer<GameObject>(1000, 0, (g) => { GameObject.Destroy(g); }, cube);
                    break;
                }
            case 1://圆形
                {
                    float raidus = action.targetRangeParam[0] * 0.01f;
                    float angle = 360;
                    var circle = new GameObject();
                    var filter = circle.AddComponent<MeshFilter>();
                    circle.AddComponent<MeshRenderer>();
                    DrawCircle(circle, filter, raidus, angle, offsetX, offsetY, angleOffset, action.castPosType);
                    TimerHeap.AddTimer<GameObject>(1000, 0, (g) => { GameObject.Destroy(g); }, circle);
                    break;
                }
            case 2://单体
                {
                    break;
                }
            case 3://矩形
                {
                    float h = action.targetRangeParam[0] * 0.01f;
                    float w = action.targetRangeParam[1] * 0.01f;
                    var cube = new GameObject();
                    var filter = cube.AddComponent<MeshFilter>();
                    cube.AddComponent<MeshRenderer>();
                    //float sin = (float)Math.Sin(angleOffset * Math.PI / 180);
                    //float cos = (float)Math.Cos(angleOffset * Math.PI / 180);
                    Matrix4x4 m = theOwner.Transform.localToWorldMatrix;
                    Matrix4x4 m1 = new Matrix4x4();
                    m1.SetRow(0, new Vector4(0, 0, 0, offsetY)); //1
                    m1.SetRow(1, new Vector4(0, 1, 0, 0));
                    m1.SetRow(2, new Vector4(0, 0, 0, offsetX)); //-1
                    m1.SetRow(3, new Vector4(0, 0, 0, 1));
                    m = m * m1;
                    Vector3 posi = new Vector3(m.m03, m.m13, m.m23); 
                    var mesh = new Mesh();
                    //cube.transform.position = theOwner.Transform.position;
                    cube.transform.position = posi;
                    cube.transform.rotation = theOwner.Transform.rotation;
                    cube.transform.Rotate(new Vector3(0, 90, 0));
                    var v0 = cube.transform.position + cube.transform.forward * w * 0.5f;

                    cube.transform.position = v0;
                    cube.transform.rotation = theOwner.Transform.rotation;
                    var v1 = cube.transform.position + cube.transform.forward * h;

                    //cube.transform.position = theOwner.Transform.position;
                    cube.transform.position = posi;
                    cube.transform.rotation = theOwner.Transform.rotation;
                    cube.transform.Rotate(new Vector3(0, -90, 0));
                    var v2 = cube.transform.position + cube.transform.forward * w * 0.5f;

                    cube.transform.position = v2;
                    cube.transform.rotation = theOwner.Transform.rotation;
                    var v3 = cube.transform.position + cube.transform.forward * h;

                    cube.transform.position = Vector3.zero;
                    cube.transform.rotation = new Quaternion();
                    mesh.vertices = new Vector3[]{v0, v1, v2, v3};
                    mesh.triangles = new int[]{2, 1, 0, 2, 3, 1};
                    filter.mesh = mesh;
                    TimerHeap.AddTimer<GameObject>(1000, 0, (g) => { GameObject.Destroy(g); }, cube);
                    break;
                }
            case 4://前方半圆
                {
                    float raidus = action.targetRangeParam[0] * 0.01f;
                    float angle = 180;
                    var cube = new GameObject();
                    var filter = cube.AddComponent<MeshFilter>();
                    cube.AddComponent<MeshRenderer>();
                    DrawCircle(cube, filter, raidus, angle, offsetX, offsetY, angleOffset);
                    TimerHeap.AddTimer<GameObject>(1000, 0, (g) => { GameObject.Destroy(g); }, cube);
                    break;
                }
            case 6://世界坐标,龙专用
                {
                    float x1 = action.targetRangeParam[0] * 0.01f;
                    float y1 = action.targetRangeParam[1] * 0.01f;
                    float x2 = action.targetRangeParam[2] * 0.01f;
                    float y2 = action.targetRangeParam[3] * 0.01f;
                    float minX = Math.Min(x1, x2);
                    float maxX = Math.Max(x1, x2);
                    float minY = Math.Min(y1, y2);
                    float maxY = Math.Max(y1, y2);
                    float radiusX = minX + (maxX - minX) * 0.5f;
                    float radiusY = minY + (maxY - minY) * 0.5f;
                    float radius = Vector2.Distance(new Vector2(x1, y1), new Vector2(x2, y2)) * 0.5f;
                    var cube = new GameObject();
                    var filter = cube.AddComponent<MeshFilter>();
                    cube.AddComponent<MeshRenderer>();
                    DrawCircle(cube, filter, radius, 360, new Vector3(radiusX, theOwner.Transform.position.y + 3, radiusY));
                    TimerHeap.AddTimer<GameObject>(1000, 0, (g) => { GameObject.Destroy(g); }, cube);
                    break;
                }
        }
    }

    private void DrawCircle(GameObject canvas, MeshFilter filter, float raidus, float angle, float offsetX = 0, float offsetY = 0, float angleOffset = 0, int posType = 0)
    {
        int ANGLE_STEP = 15;
        var mesh = new Mesh();
        int len = (int)Math.Floor(angle / ANGLE_STEP);
        len = len + 2;
        Vector3[] vs = new Vector3[len];
        float sin = (float)Math.Sin(angleOffset * Math.PI / 180);
        float cos = (float)Math.Cos(angleOffset * Math.PI / 180);
        //第一个为圆心
        EntityParent e = theOwner;
        if (posType == 2 && theOwner is EntityDummy)
        {
            if (theOwner.GetTargetEntity() != null)
            {
                e = theOwner.GetTargetEntity();
            }
        }
        Matrix4x4 m = e.Transform.localToWorldMatrix;
        Matrix4x4 m1 = new Matrix4x4();
        m1.SetRow(0, new Vector4(0, 0, 0, offsetY)); //1
        m1.SetRow(1, new Vector4(0, 1, 0, 0));
        m1.SetRow(2, new Vector4(0, 0, 0, offsetX)); //-1
        m1.SetRow(3, new Vector4(0, 0, 0, 1));
        m = m * m1;
        Vector3 v0 = new Vector3(m.m03, m.m13, m.m23);
        //vs[0] = theOwner.Transform.position;
        vs[0] = v0;
        for (int i = 1; i < len; i++)
        {
            //canvas.transform.position = theOwner.Transform.position;
            canvas.transform.position = v0;
            canvas.transform.rotation = e.Transform.rotation;
            canvas.transform.Rotate(new Vector3(0, -angle * 0.5f, 0));
            if (i != len - 1)
            {//非最后一个点
                canvas.transform.Rotate(new Vector3(0, ANGLE_STEP * i, 0));
                var v = canvas.transform.position + canvas.transform.forward * raidus;
                vs[i] = v;
            }
            else
            {//最后一个顶点
                //float r = angle - ANGLE_STEP * (i - 1);
                canvas.transform.Rotate(new Vector3(0, angle, 0));
                var v = canvas.transform.position + canvas.transform.forward * raidus;
                vs[i] = v;
            }
        }
        //三角形数
        int tc = len - 2;
        int[] triangles = new int[tc * 3];
        for (int j = 0; j < tc; j++)
        {
            triangles[j * 3] = 0;
            triangles[j * 3 + 1] = j + 1;
            if (j != 23)
            {
                triangles[j * 3 + 2] = j + 2;
            }
            else
            {
                triangles[j * 3 + 2] = 1;
            }
        }
        canvas.transform.position = Vector3.zero;
        canvas.transform.rotation = new Quaternion();
        mesh.vertices = vs;
        mesh.triangles = triangles;
        filter.mesh = mesh;
    }

    private void DrawCircle(GameObject canvas, MeshFilter filter, float raidus, float angle, Vector3 position)
    {
        int ANGLE_STEP = 15;
        var mesh = new Mesh();
        int len = (int)Math.Floor(angle / ANGLE_STEP);
        len = len + 2;
        Vector3[] vs = new Vector3[len];
        //第一个为圆心
        vs[0] = position;
        for (int i = 1; i < len; i++)
        {
            canvas.transform.position = position;
            canvas.transform.rotation = theOwner.Transform.rotation;
            canvas.transform.Rotate(new Vector3(0, -angle * 0.5f, 0));
            if (i != len - 1)
            {//非最后一个点
                canvas.transform.Rotate(new Vector3(0, ANGLE_STEP * i, 0));
                var v = canvas.transform.position + canvas.transform.forward * raidus;
                vs[i] = v;
            }
            else
            {//最后一个顶点
                //float r = angle - ANGLE_STEP * (i - 1);
                canvas.transform.Rotate(new Vector3(0, angle, 0));
                var v = canvas.transform.position + canvas.transform.forward * raidus;
                vs[i] = v;
            }
        }
        //三角形数
        int tc = len - 2;
        int[] triangles = new int[tc * 3];
        for (int j = 0; j < tc; j++)
        {
            triangles[j * 3] = 0;
            triangles[j * 3 + 1] = j + 1;
            if (j != 23)
            {
                triangles[j * 3 + 2] = j + 2;
            }
            else
            {
                triangles[j * 3 + 2] = 1;
            }
        }

        canvas.transform.position = Vector3.zero;
        canvas.transform.rotation = new Quaternion();
        mesh.vertices = vs;
        mesh.triangles = triangles;
        filter.mesh = mesh;
    }

    private void FreezeButMyself()
    {
    }

    // 根据技能id， 获取到受攻击者列表。
    // 返回值 是一个三元组。 分别是 dummy list， monster list， player list
    private List<List<uint>> GetHitEntities(int hitActionID, Matrix4x4 ltwm, Quaternion rotation, Vector3 forward, Vector3 position)
    {
        var spellData = SkillAction.dataMap[hitActionID];

        // 目标类型 0 敌人， 1 自己  2 队友  3  友方
        int targetType = spellData.targetType;
        // 攻击范围类型。  0  扇形范围 1  圆形范围， 2， 单体。 3  直线范围 4 前方范围
        int targetRangeType = spellData.targetRangeType;
        // 攻击范围参数。 针对不同类型，有不同意义。 浮点数列表
        List<float> targetRangeParam = spellData.targetRangeParam;
        float offsetX = spellData.hitXoffset;
        float offsetY = spellData.hitYoffset;
        float angleOffset = 180;
        // 最大攻击人数
        //int maxTargetCount = spellData.maxTargetCount;
        // 触发伤害特效帧数
        //int damageTriggerFrame = spellData.damageTriggerFrame;
        
        List<List<uint>> entities = new List<List<uint>>();

        if (targetType == (int)TargetType.Myself)
        {
            List<uint> listDummy = new List<uint>();
            List<uint> listMonster = new List<uint>();
            List<uint> listPlayer = new List<uint>();
            List<uint> listMercenary = new List<uint>();
            listPlayer.Add(theOwner.ID);
            entities.Add(listDummy);
            entities.Add(listMonster);
            entities.Add(listPlayer);
            entities.Add(listMercenary);
            return entities;
        }
        if (theOwner.Transform == null)
        {
            return entities;
        }
        Matrix4x4 entityltwm = theOwner.Transform.localToWorldMatrix;
        Quaternion entityrotation = theOwner.Transform.rotation;
        Vector3 entityforward = theOwner.Transform.forward;
        Vector3 entityposition = theOwner.Transform.position;
        if (spellData.castPosType == 0)
        {
            entityltwm = ltwm;
            entityrotation = rotation;
            entityforward = forward;
            entityposition = position;
        }
        TargetRangeType rangeType = (TargetRangeType)targetRangeType;
        switch (rangeType)
        {
            case TargetRangeType.CircleRange:
                if (targetRangeParam.Count >= 1)
                {
                    float radius = targetRangeParam[0] * 0.01f;
                    if (spellData.castPosType == 2 && theOwner is EntityDummy)
                    {
                        EntityParent e = theOwner.GetTargetEntity();
                        if (e != null)
                        {
                            entities = MogoUtils.GetEntitiesInRange(e.Transform.position, radius, offsetX, offsetY, angleOffset);
                        }
                    }
                    else
                    {
                        //entities = MogoUtils.GetEntitiesInRange(theOwner.Transform, radius, offsetX, offsetY, angleOffset);
                        entities = MogoUtils.GetEntitiesInRange(entityltwm, entityrotation, entityforward, entityposition, radius, offsetX, offsetY, angleOffset);
                    }
                    //entities = Utils.GetEntities(theOwner.Transform, radius, 150f);
                }
                break;
            case TargetRangeType.SectorRange:
                if (targetRangeParam.Count >= 2)
                {
                    float radius = targetRangeParam[0] * 0.01f;
                    float angle = targetRangeParam[1];
                    //entities = MogoUtils.GetEntitiesInSector(theOwner.Transform, radius, angle, offsetX, offsetY, angleOffset);
                    entities = MogoUtils.GetEntitiesInSector(entityltwm, entityrotation, entityforward, entityposition, radius, angle, offsetX, offsetY, angleOffset);
                    //entities = Utils.GetEntities(theOwner.Transform, radius, angle);
                }
                break;
            case TargetRangeType.SingeTarget:
                if (targetRangeParam.Count >= 1)
                {
                    float radius = targetRangeParam[0] * 0.01f;
                    float angle = 150;
                    //entities = MogoUtils.GetEntitiesInSector(theOwner.Transform, radius, angle, offsetX, offsetY, angleOffset);
                    entities = MogoUtils.GetEntitiesInSector(entityltwm, entityrotation, entityforward, entityposition, radius, angle, offsetX, offsetY, angleOffset);
                    //entities = Utils.GetEntities(theOwner.Transform, radius, angle);
                    MogoUtils.SortByDistance(theOwner.Transform, entities[0]);
                    MogoUtils.SortByDistance(theOwner.Transform, entities[1]);
                    MogoUtils.SortByDistance(theOwner.Transform, entities[2]);
                    if (entities.Count > 1)
                    {
                        for (int i = 1; i < entities.Count; i++)
                        {
                            entities.RemoveAt(i);
                        }
                    }
                }
                break;
            case TargetRangeType.WorldRange:
                if (targetRangeParam.Count >= 4)
                {
                    float x1 = targetRangeParam[0] * 0.01f;
                    float y1 = targetRangeParam[1] * 0.01f;
                    float x2 = targetRangeParam[2] * 0.01f;
                    float y2 = targetRangeParam[3] * 0.01f;
                    float minX = Math.Min(x1, x2);
                    float maxX = Math.Max(x1, x2);
                    float minY = Math.Min(y1, y2);
                    float maxY = Math.Max(y1, y2);
                    float radiusX = minX + (maxX - minX) * 0.5f;
                    float radiusY = minY + (maxY - minY) * 0.5f;
                    float radius = Vector2.Distance(new Vector2(x1, y1), new Vector2(x2, y2)) * 0.5f;
                    entities = MogoUtils.GetEntitiesInRange(new Vector3(radiusX, 0, radiusY), radius);
                }
                break;
            case TargetRangeType.LineRange:
            default:
                if (targetRangeParam.Count >= 2)
                {
                    float length = targetRangeParam[0] * 0.01f;
                    float width = targetRangeParam[1] * 0.01f;
                    //entities = MogoUtils.GetEntitiesFrontLineNew(theOwner.Transform, length, theOwner.Transform.forward, width, offsetX, offsetY, angleOffset);
                    entities = MogoUtils.GetEntitiesFrontLineNew(entityltwm, entityrotation, entityforward, entityposition, length, entityforward, width, offsetX, offsetY, angleOffset);
                    //entities = Utils.GetEntities(theOwner.Transform, length, width);
                }
                break;
        }
        return entities;

    }

    public List<Transform> GetHitContainers(int hitActionID)
    {
        var spellData = SkillAction.dataMap[hitActionID];

        // 目标类型 0 敌人， 1 自己  2 队友  3  友方
        int targetType = spellData.targetType;
        // 攻击范围类型。  0  扇形范围 1  圆形范围， 2， 单体。 3  直线范围 4 前方范围
        int targetRangeType = spellData.targetRangeType;
        // 攻击范围参数。 针对不同类型，有不同意义。 浮点数列表
        List<float> targetRangeParam = spellData.targetRangeParam;
        // 最大攻击人数
        //int maxTargetCount = spellData.maxTargetCount;
        // 触发伤害特效帧数
        //int damageTriggerFrame = spellData.damageTriggerFrame;

        List<Transform> result = new List<Transform>();

        if (targetType == (int)TargetType.Myself)
            return result;

        if (theOwner.Transform == null)
            return result;

        TargetRangeType rangeType = (TargetRangeType)targetRangeType;
        switch (rangeType)
        {
            case TargetRangeType.CircleRange:
                if (targetRangeParam.Count >= 1)
                {
                    float radius = targetRangeParam[0] * 0.01f;
                    result = MogoUtils.GetTransformsInSector(theOwner.Transform, Container.containerRange, radius);
                }
                break;

            case TargetRangeType.SectorRange:
                if (targetRangeParam.Count >= 2)
                {
                    float radius = targetRangeParam[0] * 0.01f;
                    float angle = targetRangeParam[1];
                    result = MogoUtils.GetTransformsInSector(theOwner.Transform, Container.containerRange, radius, angle);
                }
                break;

            case TargetRangeType.SingeTarget:
                if (targetRangeParam.Count >= 1)
                {
                    float radius = targetRangeParam[0] * 0.01f;
                    float angle = 150;
                    result = MogoUtils.GetTransformsInSector(theOwner.Transform, Container.containerRange, radius, angle);

                    //entities = Utils.GetEntities(theOwner.Transform, radius, angle);
                    MogoUtils.SortByDistance(theOwner.Transform, result);
                    if (result.Count > 1)
                    {
                        for (int i = 1; i < result.Count; i++)
                            result.RemoveAt(i);
                    }
                }
                break;

            case TargetRangeType.WorldRange:
                if (targetRangeParam.Count >= 4)
                {
                    float x1 = targetRangeParam[0] * 0.01f;
                    float y1 = targetRangeParam[1] * 0.01f;
                    float x2 = targetRangeParam[2] * 0.01f;
                    float y2 = targetRangeParam[3] * 0.01f;
                    float minX = Math.Min(x1, x2);
                    float maxX = Math.Max(x1, x2);
                    float minY = Math.Min(y1, y2);
                    float maxY = Math.Max(y1, y2);
                    float radiusX = minX + (maxX - minX) * 0.5f;
                    float radiusY = minY + (maxY - minY) * 0.5f;
                    float radius = Vector2.Distance(new Vector2(x1, y1), new Vector2(x2, y2)) * 0.5f;
                    result = MogoUtils.GetTransformsInRange(new Vector3(radiusX, 0, radiusY),  Container.containerRange, radius);
                }
                break;

            case TargetRangeType.LineRange:
            default:
                if (targetRangeParam.Count >= 2)
                {
                    float length = targetRangeParam[0] * 0.01f;
                    float width = targetRangeParam[1] * 0.01f;
                    result = MogoUtils.GetTransformsFrontLineNew(theOwner.Transform, Container.containerRange, length, theOwner.Transform.forward, width);
                }
                break;
        }
        return result;
    }

    // 触发效果
    protected void TriggerDamage(int hitActionID, Dictionary<uint, List<int>> wounded)
    {
        foreach (var i in wounded)
        {
            EventDispatcher.TriggerEvent<int, uint, uint, List<int>>(Events.FSMMotionEvent.OnHit, hitActionID, theOwner.ID, i.Key, i.Value);
        }
    }

    protected void TriggerCombo(int num)
    {
        EventDispatcher.TriggerEvent(Events.ComboEvent.AddCombo, num);
    }

    public bool IsInSkillRange(int skillId, uint targetId)
    {
        var spellData = SkillAction.dataMap[SkillData.dataMap[skillId].skillAction[0]];

        // 目标类型 0 敌人， 1 自己  2 队友  3  友方
        //int targetType = spellData.targetType;
        // 攻击范围类型。  0  扇形范围 1  圆形范围， 2， 单体。 3  直线范围 4 前方范围
        int targetRangeType = spellData.targetRangeType;
        // 攻击范围参数。 针对不同类型，有不同意义。 浮点数列表
        List<float> targetRangeParam = spellData.targetRangeParam;
        // 最大攻击人数
        //int maxTargetCount = spellData.maxTargetCount;
        // 触发伤害特效帧数
        //int damageTriggerFrame = spellData.damageTriggerFrame;

        List<List<uint>> entities = new List<List<uint>>();
        TargetRangeType rangeType = (TargetRangeType)targetRangeType;
        switch (rangeType)
        {
            case TargetRangeType.CircleRange:
                if (targetRangeParam.Count >= 1)
                {
                    float radius = targetRangeParam[0] * 0.01f;
                    entities = MogoUtils.GetEntitiesInRange(theOwner.Transform, radius);
                }
                break;
            case TargetRangeType.SectorRange:
                if (targetRangeParam.Count >= 2)
                {
                    float radius = targetRangeParam[0] * 0.01f;
                    float angle = targetRangeParam[1];
                    entities = MogoUtils.GetEntitiesInSector(theOwner.Transform, radius, angle);
                }
                break;
            case TargetRangeType.SingeTarget:
                if (targetRangeParam.Count >= 1)
                {
                    float radius = targetRangeParam[0] * 0.01f;
                    float angle = 150;
                    entities = MogoUtils.GetEntitiesInSector(theOwner.Transform, radius, angle);
                }
                break;
            case TargetRangeType.WorldRange:
                if (targetRangeParam.Count >= 4)
                {
                    float x1 = targetRangeParam[0] * 0.01f;
                    float y1 = targetRangeParam[1] * 0.01f;
                    float x2 = targetRangeParam[2] * 0.01f;
                    float y2 = targetRangeParam[3] * 0.01f;
                    float minX = Math.Min(x1, x2);
                    float maxX = Math.Max(x1, x2);
                    float minY = Math.Min(y1, y2);
                    float maxY = Math.Max(y1, y2);
                    float radiusX = minX + (maxX - minX) * 0.5f;
                    float radiusY = minY + (maxY - minY) * 0.5f;
                    float radius = Vector2.Distance(new Vector2(x1, y1), new Vector2(x2, y2)) * 0.5f;
                    entities = MogoUtils.GetEntitiesInRange(new Vector3(radiusX, 0, radiusY), radius);
                }
                break;
            case TargetRangeType.LineRange:
            default:
                if (targetRangeParam.Count >= 2)
                {
                    float length = targetRangeParam[0] * 0.01f;
                    float width = targetRangeParam[1] * 0.01f;
                    entities = MogoUtils.GetEntitiesFrontLine(theOwner.Transform, length, theOwner.Transform.forward, width);
                }
                break;
        }
        if (entities.Count < 3)
        {
            return false;
        }
        if (entities[0].Count > 0 && entities[0].Contains(targetId))
        {
            return true;
        }
        if (entities[1].Count > 0 && entities[1].Contains(targetId))
        {
            return true;
        }
        if (entities[2].Count > 0 && entities[2].Contains(targetId))
        {
            return true;
        }
        return false;
    }

    #endregion
    public void BuffUseSkill(int id)
    {//前端用血瓶专用
        SkillData s = SkillData.dataMap.GetValueOrDefault(id, null);
        if (s == null || s.skillAction == null || s.skillAction.Count == 0)
        {
            return;
        }
        SkillAction action = SkillAction.dataMap.GetValueOrDefault(s.skillAction[0], null);
        if (action == null || action.casterHeal == null || action.casterHeal.Count < 2)
        {
            return;
        }
        theOwner.curHp =  theOwner.curHp + (uint)(theOwner.curHp * action.casterHeal[0] * 0.01f + action.casterHeal[1]);
        if (theOwner.curHp > theOwner.hp)
        {
            theOwner.curHp = theOwner.hp;
        }
    }

}

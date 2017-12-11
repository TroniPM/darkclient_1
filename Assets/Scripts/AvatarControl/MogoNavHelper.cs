/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MogoNavHelper
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：2013-8-23
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Mogo.Util;

public class MogoNavHelper
{
    Transform m_parent;
    GameObject m_navGo;
    NavMeshAgent m_navAgent;

    public MogoNavHelper(Transform parent)
    {
        m_parent = parent;
        //添加一个子GameObject到parent中，用于寻路
        m_navAgent = parent.GetComponentInChildren<NavMeshAgent>();
        if (m_navAgent != null)
        {
            m_navGo = m_navAgent.gameObject;
            m_navAgent.enabled = false;
        }
        else
        {
            m_navGo = new GameObject();
            Utils.MountToSomeObjWithoutPosChange(m_navGo.transform, parent);
            ResetPositionToCLoseToNavMesh();
            m_navAgent = m_navGo.AddComponent<NavMeshAgent>();
            m_navAgent.enabled = false;
            m_navAgent.height = 2f;
            m_navAgent.radius = 1f;
        }

    }

    public NavMeshPath GetPathByTarget(Vector3 target)
    {
        if (m_navGo == null)
        {
            m_navGo = new GameObject();
        }



        //m_navAgent.Stop();
        //防止因为位置不对而抛出"无效navMesh"的错，在位置调整后设回true
        //m_navAgent.enabled = false;

        //Vector3 sourcePostion = m_parent.position;//The position to place agent
        //NavMeshHit closestHit;
        //if (NavMesh.SamplePosition(sourcePostion, out closestHit, 500, 1))
        //{

        //    m_navGo.transform.position = closestHit.position;
        //}
        //else
        //{

        //}
        //m_navAgent.enabled = true;

        NavMeshPath path = new NavMeshPath();

        //LoggerHelper.Error("before target:" + target);
        target = GetPointCloseToTheMesh(target);
        //LoggerHelper.Error("after target:" + target);
        if (!ResetPositionToCLoseToNavMesh())
        {
            LoggerHelper.Warning("can not find the navmesh!");
            return path;
        }

        //if (NavMesh.SamplePosition(target, out closestHit, 500, 1))
        //{

        //    target = closestHit.position;
        //}

        try
        {
            NavMesh.CalculatePath(m_navGo.transform.position, target, -1, path);
            //m_navAgent.CalculatePath(target, path);

            if (path.corners.Length <= 0)
            {
                //LoggerHelper.Debug("path.corners.Length <= 0:"+1);
                m_navAgent.enabled = true;

                m_navAgent.SetDestination(target);
                path = m_navAgent.path;
                if (path.corners.Length <= 0)
                {
                    //LoggerHelper.Error("path.corners.Length <= 0:" + 2);
                }
                m_navAgent.Stop();
                //m_navAgent.enabled = false;
            }
        }
        catch
        {
            //LoggerHelper.Error("fuck!");
        }
        //foreach (Vector3 v in path.corners)
        //{
        //LoggerHelper.Error(v);
        //}
        // 为去除警告暂时屏蔽以下代码
        //bool hasHit;
        for (int i = 0; i < path.corners.Length; i++)
        {
            MogoUtils.GetPointInTerrain(path.corners[i].x, path.corners[i].z, out  path.corners[i]);
        }

        return path;
    }



    private bool ResetPositionToCLoseToNavMesh()
    {
        Vector3 sourcePostion = m_parent.position;//The position to place agent
        m_navGo.transform.position = sourcePostion;
        NavMeshHit closestHit;
        if (NavMesh.SamplePosition(sourcePostion, out closestHit, 10, -1))
        {
            m_navGo.transform.position = closestHit.position;
            return true;
        }
        else
        {
            return false;
        }

    }

    private Vector3 GetPointCloseToTheMesh(Vector3 sourcePostion)
    {
        NavMeshHit closestHit;
        if (NavMesh.SamplePosition(sourcePostion, out closestHit, 10, -1))
        {
            return closestHit.position;
        }
        else
        {
            return sourcePostion;
        }
    }

}
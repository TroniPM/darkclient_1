/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：PathPoint
// 创建者：Joe Mo
// 修改者列表：
// 创建日期：
// 模块描述：寻路点,随便绑在某个物体上,id必须唯一
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mogo.Util;


public class PathPoint : MonoBehaviour
{
    public static Dictionary<int, Vector3> pathPointDic = new Dictionary<int, Vector3>();
    public static Dictionary<int, float> pathPointDistance = new Dictionary<int, float>();

    public static Dictionary<int, Vector3> tempPathPointDic = new Dictionary<int, Vector3>();
    public static Dictionary<int, float> tempPathPointDistance = new Dictionary<int, float>();

    protected static readonly int RANDOM_SUBPATHPOINT = 5; 

    public int id;
    public float distance;

    void Start()
    {
        if (!pathPointDic.ContainsKey(id))
        {
            pathPointDic.Add(id, transform.position);
            if (!pathPointDistance.ContainsKey(id))
                pathPointDistance.Add(id, distance);
        }
    }

    protected static void AddSubPathPoint(int id, Vector3 position, float stopDistance)
    {
        if (!pathPointDic.ContainsKey(id))
        {
            pathPointDic.Add(id, position);
            if (!pathPointDistance.ContainsKey(id))
                pathPointDistance.Add(id, stopDistance);
        }
    }

    public static int GetFixNearestPlace(Vector3 playerPosition, int main, Vector3 stardardPosition)
    {
        int result = 0;

        tempPathPointDic.Clear();
        tempPathPointDistance.Clear();

        if (pathPointDistance[main] <= 1.5)
            return result;

        UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();
        if (!UnityEngine.AI.NavMesh.CalculatePath(playerPosition, pathPointDic[main], -1, path))
            return result;

        if (path.corners.Length < 2)
            return result;

        for (int i = 0; i < RANDOM_SUBPATHPOINT; i++)
        {
            Vector3 fixPosition = GetRandomVector3InRangeCircle(pathPointDistance[main] - 1.5f);

            tempPathPointDic.Add(100 * main + i, fixPosition + stardardPosition);
            tempPathPointDistance.Add(100 * main + i, 1);
        }

        Vector3 turn = path.corners[path.corners.Length - 2];

        float standardDistance = Vector3.Distance(turn, pathPointDic[main]);
        float resultDistance = standardDistance;
        float offset = standardDistance;

        foreach (var tempData in tempPathPointDic)
        {
            float tempSourceDistance = Vector3.Distance(pathPointDic[main], tempData.Value);
            if (tempSourceDistance < 0.6f)
            {
                continue;
            }

            float tempDistance1 = Vector3.Distance(tempData.Value, pathPointDic[main]);
            float tempDistance2 = Vector3.Distance(tempData.Value, turn);

            if (tempDistance1 < standardDistance && tempDistance2 < standardDistance
                && tempDistance1 > 0 && tempDistance2 > 0)
            {
                return tempData.Key;
            }
        }

        return result;
    }


    public static int GetNearestPlace(Vector3 playerPosition, int main)
    {
        return GetFixNearestPlace(playerPosition, main, pathPointDic[main]);
    }


    private static System.Random globalRandomGenerator = Utils.CreateRandom();

    public static float GetRandomFloat(float min, float max)
    {
        if (min < max)
            return (float)globalRandomGenerator.NextDouble() * (max - min) + min;
        else
            return max;
    }

    public static Vector3 GetRandomVector3InRangeCircle(float rangeTo, float rangeFrom = 0, float angleTo = 360, float angleFrom = 0, float y = 0)
    {
        float length = GetRandomFloat(rangeFrom, rangeTo);
        float angle = GetRandomFloat(angleFrom, angleTo);
        return new Vector3((float)(length * Math.Sin(angle * Math.PI / 180.0)),
            y,
            (float)(length * Math.Cos(angle * Math.PI / 180.0)));
    }
}

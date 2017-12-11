using UnityEngine;
using System.Collections.Generic;
using System;
using Mogo.Util;

public class SubPathPoint : PathPoint
{
    public int mainPathPoint;

    public static Dictionary<int, List<int>> pathPointLink = new Dictionary<int, List<int>>();

    void Start()
    {
        PathPoint.AddSubPathPoint(id, transform.position, distance);
        AddMainPathPointLink();
    }

    public void AddMainPathPointLink()
    {
        if (!pathPointLink.ContainsKey(mainPathPoint))
            pathPointLink.Add(mainPathPoint, new List<int>());

        if (pathPointLink[mainPathPoint] == null)
            pathPointLink[mainPathPoint] = new List<int>();

        if (!pathPointLink[mainPathPoint].Contains(id))
            pathPointLink[mainPathPoint].Add(id);
    }

    public static int GetNearestSubPoint(Vector3 playerPosition, int main)
    {
        if (!pathPointLink.ContainsKey(main))
            return main;

        if (pathPointLink[main] == null)
            return main;

        int result = 0;
        float resultDistance = 0;

        if (pathPointDic.ContainsKey(main))
        {
            result = main;
            resultDistance = Vector3.Distance(playerPosition, pathPointDic[main]);
        }

        foreach (int subID in pathPointLink[main])
        {
            float tempDistance = -1;

            if (pathPointDic.ContainsKey(subID))
                tempDistance = Vector3.Distance(playerPosition, pathPointDic[subID]);

            if (tempDistance < resultDistance && tempDistance >= 0)
            {
                result = subID;
                resultDistance = tempDistance;
            }
        }

        return result;
    }
}


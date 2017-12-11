/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MultiPlatformControllerGear
// 创建者：Key Pan
// 修改者列表：Key Pan
// 创建日期：
// 最后修改日期：
// 模块描述：
// 代码版本：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using Mogo.Util;

public class MultiPlatformControllerGear : GearParent
{
    public int width = 5;
    public int height = 3;
    public int moveDownCD = 1000;

    public bool beginVertical;

    public SimplePlatformGear[] platforms;

    public MultiPlatformWall wallUp;
    public MultiPlatformWall wallDown;
    public MultiPlatformWall wallLeft;
    public MultiPlatformWall wallRight;

    private bool[] platformActive;
    private bool[][] boundActive;

    // private uint nextBreachTimerID;

    void Start()
    {
        if (platforms.Length != width * height)
        {
            LoggerHelper.Debug("Length is not matching width * height!");
            enabled = false;
        }

        if (width < 2 || height < 2)
        {
            LoggerHelper.Debug("Width and height must be bigger than 2!");
            enabled = false;
        }

        gearType = "MutiPlatformControllerGear";

        beginVertical = false;

        platformActive = new bool[platforms.Length];

        for (uint i = 0; i < platforms.Length; i++)
        {
            platforms[i].ID = i;
            platformActive[i] = true;
        }

        boundActive = new bool[2][];
        boundActive[0] = new bool[width];
        boundActive[1] = new bool[height];

        for (int i = 1; i < width; i++)
        {
            boundActive[0][i] = true;
        }
        for (int i = 1; i < height; i++)
        {
            boundActive[1][i] = true;
        }

        // nextBreachTimerID = uint.MaxValue;
    }

    public void OnRandomBreach()
    {
        int breachResult = -1;
        bool isWidthEnd = false, isHeightEnd = false;
        int countWidth = 0, countHeight = 0;
        for (int i = 0; i < boundActive[0].Length; i++)
        {
            if (boundActive[0][i])
                countWidth++;
        }
        for (int i = 0; i < boundActive[1].Length; i++)
        {
            if (boundActive[1][i])
                countHeight++;
        }

        if (countWidth < 1)
            isWidthEnd = true;
        if (countHeight < 1)
            isHeightEnd = true;

        bool randomResult = RandomHelper.GetRandomBoolean();

        if (!beginVertical || (isHeightEnd && !isWidthEnd) || (!isWidthEnd && !isHeightEnd && randomResult))
        {
            do
            {
                breachResult = RandomHelper.GetRandomInt(width - 1) + 1;
            }
            while (!boundActive[0][breachResult]);

            //nextBreachTimerID = 
            TimerHeap.AddTimer((uint)moveDownCD, 0, OnMoveDown, true, breachResult);
        }
        else if ((isWidthEnd && !isHeightEnd) || ((!isWidthEnd && !isHeightEnd && !randomResult)))
        {
            do
            {
                breachResult = RandomHelper.GetRandomInt(height - 1) + 1;
            }
            while (!boundActive[1][breachResult]);

            //nextBreachTimerID = 
            TimerHeap.AddTimer((uint)moveDownCD, 0, OnMoveDown, false, breachResult);
        }
        else if (isWidthEnd && isHeightEnd)
        {
            //nextBreachTimerID = 
            TimerHeap.AddTimer((uint)moveDownCD, 0, OnMoveDownLast);
        }
        beginVertical = true;
    }

    private void OnMoveDown(bool horizontal, int breachResult)
    {
        int curPlatform = 0;
        int count = 0;
        float distance = float.MaxValue; 

        for (int platformIndex = 0; platformIndex < platforms.Length; platformIndex++)
        {
            if (platforms[platformIndex].isOnPlatform)
            {
                count++;

                if (Vector3.Distance(platforms[platformIndex].gameObject.transform.position, platforms[platformIndex].playerTransform.position) < distance)
                    curPlatform = platformIndex;

                // break;
            }
        }

        LoggerHelper.Debug(count + " all curPlatform");
        LoggerHelper.Debug(curPlatform + " curPlatform");

        if (horizontal)
        {
            boundActive[0][breachResult] = false;
            bool side = true;

            if (curPlatform % width < breachResult)
            {
                for (int i = breachResult; i < width; i++)
                {
                    boundActive[0][i] = false;
                }
            }
            else
            {
                side = false;
                for (int i = 0; i < breachResult; i++)
                {
                    boundActive[0][i] = false;
                }
            }

            for (int i = 0; i < platforms.Length; i++)
            {
                if (side)
                {
                    if (i % width >= breachResult)
                    {
                        if (platformActive[i])
                        {
                            platforms[i].OnMoveDown();
                            platformActive[i] = false;
                            // LoggerHelper.Debug("hehe" + i);
                        }
                    }
                }
                else
                {
                    if (i % width < breachResult)
                    {
                        if (platformActive[i])
                        {
                            platforms[i].OnMoveDown();
                            platformActive[i] = false;
                            // LoggerHelper.Debug("hehe" + i);
                        }
                    }
                }
            }


            if (side)
            {
                wallRight.transform.localPosition = wallRight.sourcePosition + new Vector3(0, 0, 4) * (width - breachResult - 1) + new Vector3(0, 0, 3.4f);
            }
            else
            {

                wallLeft.transform.localPosition = wallLeft.sourcePosition + new Vector3(0, 0, -4) * (breachResult - 1) + new Vector3(0, 0, -3.4f);
            }
        }
        else
        {
            boundActive[1][breachResult] = false;
            bool side = true;

            if (curPlatform / width < breachResult)
            {
                for (int i = breachResult; i < height; i++)
                {
                    boundActive[1][i] = false;
                }
            }
            else
            {
                side = false;
                for (int i = 0; i < breachResult; i++)
                {
                    boundActive[1][i] = false;
                }
            }

            for (int i = 0; i < platforms.Length; i++)
            {
                if (side)
                {
                    if (i / width >= breachResult)
                    {
                        if (platformActive[i])
                        {
                            platforms[i].OnMoveDown();
                            platformActive[i] = false;
                            // LoggerHelper.Debug("hehe" + i);
                        }
                    }
                }
                else
                {
                    if (i / width < breachResult)
                    {
                        if (platformActive[i])
                        {
                            platforms[i].OnMoveDown();
                            platformActive[i] = false;
                            // LoggerHelper.Debug("hehe" + i);
                        }
                    }
                }
            }

            if (side)
            {
                wallDown.transform.localPosition = wallDown.sourcePosition + new Vector3(4, 0, 0) * (height - breachResult - 1) + new Vector3(3.4f, 0, 0);

            }
            else
            {
                wallUp.transform.localPosition = wallUp.sourcePosition + new Vector3(-4, 0, 0) * (breachResult - 1) + new Vector3(-3.4f, 0, 0);
            }
        }
    }

    private void OnMoveDownLast()
    {
        for (int i = 0; i <  platformActive.Length; i++)
        {
            if (platformActive[i])
            {
                platforms[i].OnMoveDown();
                platformActive[i] = false;
                LoggerHelper.Debug("Avatar dead");
                break;
            }
        }
    }

    //void OnGUI()
    //{
    //    if (GUI.Button(new Rect(160, 0, 150, 50), "MoveDown"))
    //    {
    //        OnRandomBreach();
    //    }

    //    /*if (GUI.Button(new Rect(0, 0, 150, 50), "Begin"))
    //    {
    //        int luckyPlace = RandomHelper.GetRandomInt(15);
    //        platforms[luckyPlace].Test();
    //        LoggerHelper.Debug("Choose: " + luckyPlace);
    //    }*/
    //}
}


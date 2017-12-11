/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：RandomHelper
// 创建者：Key Pan
// 修改者列表：Key Pan
// 创建日期：20130227
// 最后修改日期：20130228
// 模块描述：各种随机数
// 代码版本：测试版V1.1
//----------------------------------------------------------------*/

using UnityEngine;
using System;
using System.Collections;

namespace Mogo.Util
{
    public class RandomHelper
    {
        private static System.Random globalRandomGenerator = Utils.CreateRandom();

        public static void GenerateNewRandomGenerator()
        {
            globalRandomGenerator = Utils.CreateRandom();
        }

        public static Byte GetRandomByte()
        {
            Byte[] temp = new Byte[1];
            globalRandomGenerator.NextBytes(temp);
            return temp[0];
        }

        public static void GetRandomBytes(Byte[] bytes)
        {
            globalRandomGenerator.NextBytes(bytes);
        }

        public static Color GetRandomColor()
        {
            Byte[] temp = new Byte[4];
            globalRandomGenerator.NextBytes(temp);
            return new Color(temp[0], temp[1], temp[2], temp[3]);
        }

        public static Color GetRandomColor(int alpha)
        {
            Byte[] temp = new Byte[3];
            globalRandomGenerator.NextBytes(temp);

            if (alpha < 0)
                alpha = 0;
            if (alpha > 255)
                alpha = 255;

            return new Color(temp[0], temp[1], temp[2], alpha);
        }

        public static float GetRandomFloat()
        {
            return (float)globalRandomGenerator.NextDouble();
        }

        public static float GetRandomFloat(float max)
        {
            return (float)globalRandomGenerator.NextDouble() * max;
        }

        public static float GetRandomFloat(float min, float max)
        {
            if (min < max)
                return (float)globalRandomGenerator.NextDouble() * (max - min) + min;
            else
                return max;
        }

        public static int GetRandomInt(bool positive = true)
        {
            if (positive)
                return globalRandomGenerator.Next();
            else
            {
                int sign = globalRandomGenerator.Next() % 2;
                if (sign == 0)
                    return globalRandomGenerator.Next();
                else
                    return -globalRandomGenerator.Next();
            }
        }

        public static int GetRandomInt(int max)
        {
            if (max > 0)
                return globalRandomGenerator.Next() % max;
            else if (max < 0)
                return -globalRandomGenerator.Next() % max;
            else
                return 0;
        }

        public static int GetRandomInt(int min, int max)
        {
            if (min < max)
                return globalRandomGenerator.Next(min, max);
            else
                return max;
        }

        public static Vector3 GetRandomNormalVector3()
        {
            return new Vector3(GetRandomInt(false),
                GetRandomInt(false),
                GetRandomInt(false)).normalized;
        }

        public static Vector3 GetRandomVector3()
        {
            return new Vector3(GetRandomInt(false) * GetRandomFloat(),
                GetRandomInt(false) * GetRandomFloat(),
                GetRandomInt(false) * GetRandomFloat());
        }

        public static Vector3 GetRandomVector3(float xMax, float yMax = 0, float zMax = 0)
        {
            return new Vector3(GetRandomFloat(xMax), GetRandomFloat(yMax), GetRandomFloat(zMax));
        }

        public static Vector3 GetRandomVector3(float xMin, float xMax, 
            float yMin, float yMax, 
            float zMin, float zMax)
        {
            return new Vector3(GetRandomFloat(xMin, xMax),
                GetRandomFloat(yMin, yMax),
                GetRandomFloat(zMin, zMax));
        }

        public static Vector2 GetRandomVector2()
        {
            return new Vector2(GetRandomInt(false) * GetRandomFloat(),
                GetRandomInt(false) * GetRandomFloat());
        }

        public static Vector2 GetRandomVector2(float xMax, float yMax = 0)
        {
            return new Vector3(GetRandomFloat(xMax), GetRandomFloat(yMax));
        }

        public static Vector2 GetRandomVector2(float xMin, float xMax,
            float yMin, float yMax)
        {
            return new Vector3(GetRandomFloat(xMin, xMax),
                GetRandomFloat(yMin, yMax));
        }

        public static Vector3 GetRandomVector3InRangeCircle(float range, float y = 0)
        {
            float length = GetRandomFloat(0, range);
            float angle = GetRandomFloat(0, 360);
            return new Vector3((float)(length * Math.Sin(angle * (Math.PI / 180))),
                y, 
                (float)(length * Math.Cos(angle * (Math.PI / 180))));
        }

        public static bool GetRandomBoolean()
        {
            if (GetRandomInt(2) == 0)
                return false;
            return true;
        }
    }
}

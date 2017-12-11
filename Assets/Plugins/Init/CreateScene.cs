#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：CreateScene
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.4.15
// 模块描述：场景初始化类。
//----------------------------------------------------------------*/
#endregion

using UnityEngine;

public class CreateScene : MonoBehaviour
{
    void Awake()
    {
        //在场景初始化时触发场景加载完成事件，以解决场景模型Draw call优化问题。
        //if (Driver.Instance.LevelWasLoaded != null)
        //    Driver.Instance.LevelWasLoaded();
    }

    //void OnLevelWasLoaded()
    //{
    //    //在场景初始化时触发场景加载完成事件，以解决场景模型Draw call优化问题。
    //    if (Driver.Instance.LevelWasLoaded != null)
    //        Driver.Instance.LevelWasLoaded();
    //}
}
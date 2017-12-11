/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：SlotCueHandler
// 创建者：Key Pan
// 修改者列表：Key Pan
// 创建日期：20130225   
// 最后修改日期：20130226
// 模块描述：暂时处理插槽的类
// 代码版本：测试版
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using Mogo.GameData;

public class SlotCueHandler : MonoBehaviour {

	// string：骨骼路径，HashSet<int>：骨骼下当前绑的特效的 id - index 对
	private Dictionary<string, List<KeyValuePair<int, int>>> fxList;
	private SfxHandler handler;

	void Awake()
	{
		fxList = new Dictionary<string, List<KeyValuePair<int, int>>>();
		handler = gameObject.GetComponent<SfxHandler>();
	}

	void Update ()
	{
	}

	public void AddSlotCue(int id, string bone_path)
	{
		//handler.HandleSlotCue(id, bone_path);
	}

	public void SetFxList(int id, string bone_path, int index)
	{
        if (fxList.ContainsKey(bone_path))
        {
            // to do 通过判断删除其他的应该被删除的
            // 比较列表的每一项
            foreach (KeyValuePair<int, int> item in fxList[bone_path])
            {
                //if (FXData.dataMap[id].fx_type == FXData.dataMap[item.Key].fx_type)
                //{
                //    // 相同即表示可以删除
                //    RemoveSlotFx(item.Key, bone_path);

                //    // 有可能已经删除了整个bone_path，无法遍历，必须break;
                //    if (!fxList.ContainsKey(bone_path))
                //        break;
                //}

                if (id == item.Key)
                {
                    RemoveSlotFx(item.Key, bone_path);

                    if (!fxList.ContainsKey(bone_path))
                        break;
                }
            }
        }

        // 不能用else哦，因为RemoveSlotFx里面有可能把骨骼的Key删除

        if (!fxList.ContainsKey(bone_path))
            fxList.Add(bone_path, new List<KeyValuePair<int, int>>());

		fxList[bone_path].Add(new KeyValuePair<int, int>(id, index));
	}

	// 删除第一个id为指定id的特效
	public void RemoveSlotFx(int id, string bone_path)
	{
		if (fxList.ContainsKey(bone_path))
		{
			int i;
			for (i = 0; i < fxList[bone_path].Count; i++)
			{
				if (fxList[bone_path][i].Key == id)
					break;
			}

			handler.RemoveSlotCue(id, fxList[bone_path][i].Value);

			fxList[bone_path].RemoveAt(i);

			if (fxList[bone_path].Count == 0)
			{
				fxList.Remove(bone_path);
			}
		}
	}
}

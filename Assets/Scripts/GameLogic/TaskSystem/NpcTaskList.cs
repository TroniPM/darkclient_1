///*----------------------------------------------------------------
//// Copyright (C) 2013 广州，爱游
////
//// 模块名：NpcMissionList
//// 创建者：Joe Mo
//// 修改者列表：
//// 创建日期：
//// 模块描述：
////----------------------------------------------------------------*/
//using UnityEngine;
//using System.Collections.Generic;

//namespace Mogo.Task
//{
//    public class NpcTaskList
//    {

//        public List<int> AcceptableTasks;
//        public List<int> UnderwayTasks;
//        public List<int> FinishedTasks;
//        public int versionId;//判断是否需要更新用
//        public NpcTaskList()
//        {
//            AcceptableTasks = new List<int>();
//            UnderwayTasks = new List<int>();
//            FinishedTasks = new List<int>();
//            versionId = 0;
//        }

//        public bool HasTask()
//        {
//            return AcceptableTasks.Count > 0 || FinishedTasks.Count > 0;//|| UnderwayMissions.Count > 0 
//        }

//        public int GetTask()
//        {
//            if (FinishedTasks.Count > 0) return FinishedTasks[0];
//            else if (AcceptableTasks.Count > 0) return AcceptableTasks[0];
//            else if (UnderwayTasks.Count > 0) return UnderwayTasks[0];
//            else return -1;
//        }

//        public int GetTask(out TaskManager.TaskState state)
//        {
//            if (FinishedTasks.Count > 0) {
//                state = TaskManager.TaskState.Done;
//                return FinishedTasks[0];
//            }
//            else if (AcceptableTasks.Count > 0) {
//                state = TaskManager.TaskState.Acceptable;
//                return AcceptableTasks[0];
//            }
//            else if (UnderwayTasks.Count > 0)
//            {
//                state = TaskManager.TaskState.Underway;
//                return UnderwayTasks[0];
//            }
//            else
//            {
//                state = TaskManager.TaskState.Acceptable;
//                return -1;
//            } 
//        }

//        public void Clear()
//        {
//            AcceptableTasks.Clear();
//            FinishedTasks.Clear();
//            UnderwayTasks.Clear();
//        }
//    }
//}


using System;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class RunePosiLock : GameData<RunePosiLock>
    {
        public int level { get; protected set; }

        static public readonly string fileName = "xml/RunePosiUnLock";
        //static public Dictionary<int, RunePosiLock> dataMap { get; set; }
    }
}

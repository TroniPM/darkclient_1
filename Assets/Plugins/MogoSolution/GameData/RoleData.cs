using System;
using System.Collections.Generic;

namespace Mogo.GameData
{
#if UNITY_IPHONE
	public class RoleData : GameData
#else
    public class RoleData : GameData<RoleData>
#endif
    {
        public Dictionary<int, int> powerSfx { get; protected set; }

        static public readonly string fileName = "xml/role_data";
#if UNITY_IPHONE
        static public Dictionary<int, RoleData> dataMap { get; set; }
#endif
        public RoleData()
        {
            powerSfx = new Dictionary<int, int>();
        }
    }
}

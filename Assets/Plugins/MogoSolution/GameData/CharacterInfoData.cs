using System.Collections.Generic;

namespace Mogo.GameData
{
    /// <summary>
    /// 角色信息。
    /// </summary>
    public class CharacterInfoData : GameData<CharacterInfoData>
    {
        public int vocation { get; protected set; }
        public int discription { get; protected set; }
        public byte attack { get; protected set; }
        public byte defence { get; protected set; }
        public byte range { get; protected set; }
        public UnityEngine.Vector3 Location { get; protected set; }
        public List<int> EquipList { get; protected set; }
        public int Weapon { get; protected set; }
        public string controller { get; set; }

        static public readonly string fileName = "xml/CharacterInfoData";
        //public static Dictionary<int, CharacterInfoData> dataMap { get; set; }
    }
}
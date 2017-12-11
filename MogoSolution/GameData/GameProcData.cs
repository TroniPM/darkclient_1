using System.Collections;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class GameProcData : GameData<GameProcData>
    {
        public byte ProcType { get; protected set; }
        public ushort Progress { get; protected set; }
        public string Paras { get; protected set; }

        static public readonly string fileName = "xml/GameProcData";

        private static Dictionary<ProcType, Dictionary<string, GameProcData>> m_procData;

        public static Dictionary<ProcType, Dictionary<string, GameProcData>> ProcData
        {
            get
            {
                if (m_procData == null)
                {
                    m_procData = new Dictionary<ProcType, Dictionary<string, GameProcData>>();
                    foreach (var item in dataMap.Values)
                    {
                        if (string.IsNullOrEmpty(item.Paras))
                            continue;
                        var type = (ProcType)item.ProcType;
                        if (!m_procData.ContainsKey(type))
                            m_procData.Add(type, new Dictionary<string, GameProcData>());
                        m_procData[type][item.Paras.ToLower()] = item;
                    }
                }

                return m_procData;
            }
        }
    }

    public enum ProcType : byte
    {
        None = 0,
        ChangeScene = 1,
        CreateCharacter = 2,
        TriggerSpawnPoint = 3,
        NotifyToClientEvent = 4,
        ShowLoginReward = 5,
        GetLoginReward = 6,
        HandInTask = 7,
        OpenInstanceUI = 8,
        ClientTeleport = 9,
        BattleWin = 10,
        GuideUI = 11
    }
}
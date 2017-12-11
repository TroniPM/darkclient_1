using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mogo.GameData
{
    public class GuideXMLData : GameData<GuideXMLData>
    {
        /// <summary>
        /// 表格对应数据
        /// </summary>
        public int group { get; protected set; }
        public int step { get; protected set; }
        public Dictionary<string,string> restriction { get; protected set; }
        public int conditionEvent { get; set; }
        public string event_arg1 { get; set; }
        public string event_arg2 { get; set; }
        public List<int> guideLevel { get; set; }
        public int guideTimes { get; set; }
        public int priority { get; set; }
        public int target { get; set; }
        public string target_arg1 { get; set; }
        public string target_arg2 { get; set; }
        public int openGUI { get; set; }
        public string openDialog { get; set; }
        public int text { get; set; }
        static public readonly string fileName = "xml/guide";
        //static public Dictionary<int, GuideXMLData> dataMap { get; set; }
        /// <summary>
        /// 二次抽取数据
        /// </summary>
        private static List<int> m_events;
        static public List<int> ConditionEvent
        {
            get
            {
                if (m_events == null)
				{
#if UNITY_IPHONE
					m_events=new List<int>();
					foreach(var v in dataMap)
					{
						m_events.Add(v.Value.conditionEvent);
					}
					m_events.Distinct();
#else
                    m_events = (from data in dataMap select data.Value.conditionEvent).Distinct().ToList();
#endif
				}
                return m_events;
            }
        }
        private static Dictionary<int, List<int>> m_links;
        static public Dictionary<int, List<int>> Links
        {
            get
            {
                if (m_links == null)
                {

#if UNITY_IPHONE
					m_links=new Dictionary<int, List<int>>();
					List<int> temp;
					foreach(var v in dataMap)
					{
						if(!m_links.ContainsKey(v.Value.group))
						{
							temp=new List<int>();
							temp.Add(v.Value.step);
							m_links.Add(v.Key,temp);
						}
						else
						{
							m_links[v.Value.group].Add(v.Value.step);
							m_links[v.Value.group].Sort();
						}
					}
#else
                    m_links = dataMap.GroupBy(a => a.Value.group)
                            .ToDictionary(gdc => gdc.Key, 
                                gdc => gdc.OrderBy(x=>x.Value.step)
                            .Select(x=>x.Value.id)
                            .ToList());
#endif
				}
                return m_links;
            }
        }
        /// <summary>
        /// 数据读取方法，包括切片和联合
        /// </summary>
        /// <param name="events"></param>
        /// <returns></returns>
        static public List<int> GetIDByEvent(int events)
        {
#if UNITY_IPHONE
			List<int> 	lRet=new List<int>();
			foreach(var v in dataMap)
			{
				if(v.Value.conditionEvent==events)
				{
					lRet.Add(v.Key);
				}
			}
			return lRet;
#else
            var d = from t in dataMap
                    where t.Value.conditionEvent == events
                    select t.Key;
            return d.ToList();
#endif
        }
    }
}
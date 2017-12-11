/*----------------------------------------------------------------
// Copyright (C) 2013 ���ݣ�����
//
// ģ������HpTypesData
// �����ߣ�Charles Zuo
// �޸����б�
// �������ڣ� 2013.07.09
// ģ��������
//----------------------------------------------------------------*/

using System.Collections;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class HpTypesData : GameData<HpTypesData>
    {
        public int buffId { get; set; }
        public int cd { get; set; }
        static public readonly string fileName = "xml/HpTypes";
        //static public Dictionary<int, HpTypesData> dataMap { get; set; }
        public static int GetHpBottleCD(int vipLevel)
        {
            int hpType = 0;
            foreach (var item in PrivilegeData.dataMap[vipLevel].hpBottles)
	        {
		        hpType = item.Key;
	        }
            return dataMap[hpType].cd;
        }
    }
}

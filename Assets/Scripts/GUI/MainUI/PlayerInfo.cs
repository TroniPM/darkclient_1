/*----------------------------------------------------------------
// Copyright (C) 2013 ���ݣ�����
//
// ģ������PlayerInfo
// �����ߣ�MaiFeo
// �޸����б�
// �������ڣ�
// ģ�������������������Ϣ
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

public class PlayerInfo : MonoBehaviour
{
	void Start () 
    {
        MainUIViewManager.Instance.SetPlayerLevel(60);
	}
}

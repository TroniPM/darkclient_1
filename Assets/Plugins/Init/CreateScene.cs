#region ģ����Ϣ
/*----------------------------------------------------------------
// Copyright (C) 2013 ���ݣ�����
//
// ģ������CreateScene
// �����ߣ�Ash Tang
// �޸����б�
// �������ڣ�2013.4.15
// ģ��������������ʼ���ࡣ
//----------------------------------------------------------------*/
#endregion

using UnityEngine;

public class CreateScene : MonoBehaviour
{
    void Awake()
    {
        //�ڳ�����ʼ��ʱ����������������¼����Խ������ģ��Draw call�Ż����⡣
        //if (Driver.Instance.LevelWasLoaded != null)
        //    Driver.Instance.LevelWasLoaded();
    }

    //void OnLevelWasLoaded()
    //{
    //    //�ڳ�����ʼ��ʱ����������������¼����Խ������ģ��Draw call�Ż����⡣
    //    if (Driver.Instance.LevelWasLoaded != null)
    //        Driver.Instance.LevelWasLoaded();
    //}
}
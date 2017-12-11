/*----------------------------------------------------------------
// Copyright (C) 2013 ���ݣ�����
//
// ģ������Bullet
// �����ߣ�Joe Mo
// �޸����б�
// �������ڣ�2013-5-27
// ģ������: ����������������ڻ��򣬹������������Ч���ţ�׷��Ŀ�����ʾ�߼���
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using Mogo.GameData;
using Mogo.Util;

public class ActorBullet : MonoBehaviour
{

    public float speed = 10;
    public Transform target;
    public Vector3 targetPosition;
    public bool isSetup = false;
    //public int boomId;
    // Use this for initialization

    public System.Action OnDestroy = null;



    public void Setup(Transform target, float speed, Vector3 targetPosition)
    {
        this.target = target;

        this.targetPosition = targetPosition;

        if (target != null)
        {
            targetPosition = target.position;
        }
        this.speed = speed;
        //this.boomId = boomId;

        //if (boomId != -1)
        //{
        //    FXData data = FXData.dataMap[boomId];
        //    targetPosition = MogoUtils.GetChild(target, data.slot);

        //}

        isSetup = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSetup) return;
        //����Ŀ��
        if (target != null)
        {
            targetPosition = target.position;
        }
      
        transform.LookAt(targetPosition);
        float step = speed * Time.deltaTime;
        float distance = Vector3.Distance(targetPosition, transform.position);

        if (distance < step)
        {
            //����Ŀ��
            transform.position = targetPosition;

            
            //if (boomId != -1 && target != null)
            //{
            //    target.GetComponent<SfxHandler>().HandleFx(boomId);
            //}
            if (OnDestroy != null)
                OnDestroy();
        }
        else
        {
            transform.Translate(step * transform.forward, Space.World);
        }

    }
}

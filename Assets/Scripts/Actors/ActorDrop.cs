using System;
using System.Collections.Generic;
using UnityEngine;
using Mogo.Game;
using Mogo.Util;

class ActorDrop : ActorParent<EntityDrop>
{

    //private Vector3 speed;
    //private Vector3 tweenEnd;
    //private bool tweenAble = false;
    //private float cntTime = 0; //缓动累计时间
    //private float totalTime = 0; //缓动总时

    public float m_rotateSpeed = 180f;
    private float m_vY = 0;
    private float m_fromY;
    private float m_toY;
    private float m_gravity = 40f;
    private float m_iniV;
    private bool m_isThrowing = false;

    private float m_shakeSpeed;
    private float m_dy;
    private bool m_isShaking = false;
    private float yDelta;


    private Transform m_target;
    private float m_dragV;
    private float m_dragA;
    private bool m_isDraging;
    public bool m_hasTrigger = false;
    /// <summary>
    /// 缓动
    /// </summary>
    /// <param name="src">起始点</param>
    /// <param name="dst">目标点</param>
    /// <param name="time">移动时间(s)</param>
    public void TweenTo(Vector3 src, Vector3 dst, float time)
    {
        //tweenEnd = dst;
        //speed = new Vector3((dst.x - src.x) / time, (dst.y - src.y) / time, (dst.z - src.z) / time);
        //totalTime = time;
        //tweenAble = true;
    }

    /// <summary>
    /// 抛到空中
    /// </summary>
    /// <param name="dst"></param>
    /// <param name="time">到空中顶点的时间，所以总时间是time*2</param>
    public void ThrowTo(float targetY, float time)
    {
        m_fromY = transform.position.y;
        m_toY = targetY;
        float dy = targetY - transform.position.y;
        m_iniV = Mathf.Sqrt(2 * m_gravity * dy);
        m_vY = m_iniV;
        m_isThrowing = true;
    }

    /// <summary>
    /// 上下循环移动
    /// </summary>
    /// <param name="dy">振幅</param>
    /// <param name="time">周期</param>
    public void ShakeY(float dy, float time)
    {
        m_shakeSpeed = dy * 4 / time;
        m_dy = dy;
        yDelta = 0;
        m_isShaking = true;

    }


    // 每帧调用
    void Update()
    {


        if (m_isDraging)
        {
            DoDrag();
        }
        else
        {
            if (m_isThrowing)
            {
                DoThrow();
            }
            else if (m_isShaking)
            {
                DoShake();
            }
        }

        transform.Rotate(0, m_rotateSpeed * Time.deltaTime, 0, Space.World);


    }

    private void DoShake()
    {
        float dt = Time.deltaTime;
        yDelta = m_shakeSpeed * dt + yDelta;
        while (Mathf.Abs(yDelta) > m_dy)
        {
            if (yDelta > 0) yDelta = m_dy * 2 - yDelta;
            else yDelta = -m_dy * 2 - yDelta;
            m_shakeSpeed = -m_shakeSpeed;
        }

        transform.position = new Vector3(transform.position.x, m_fromY + yDelta, transform.position.z);
    }

    private void DoThrow()
    {
        float dt = Time.deltaTime;
        m_vY -= m_gravity * dt;

        transform.Translate(m_vY * dt * Vector3.up, Space.World);

        if (transform.position.y < m_fromY)
        {
            transform.position = new Vector3(transform.position.x, m_fromY, transform.position.z);
            m_isThrowing = false;
            ShakeY(0.09f, 2f);
        }
    }

    private void DoDrag()
    {
        m_dragV += Time.deltaTime * m_dragA;
        float step = m_dragV * Time.deltaTime;
        Vector3 d = m_target.position - transform.position;
        if (step > d.magnitude)
        {
            transform.position = m_target.position;
            m_isDraging = false;
            if (!m_hasTrigger)
            {

                Pick();
                m_hasTrigger = true;
            }

        }
        else
        {
            transform.Translate(step * d.normalized, Space.World);
        }
    }

    void Pick()
    {
        MogoWorld.thePlayer.RpcCall("PickDropReq", theEntity.ID);
        List<int> dropInfo = Mogo.GameLogic.LocalServer.LocalServerSceneManager.Instance.GetDropByEntityId(theEntity.ID);
        if (dropInfo != null)
        {
            if (dropInfo[4] > 0)
            {
                BillboardLogicManager.Instance.AddAloneBattleBillboard(MogoWorld.thePlayer.Transform.Find("slot_billboard").position, dropInfo[4], AloneBattleBillboardType.Gold);
            }
        }

        TimerHeap.AddTimer<string, uint>(30, 0, EventDispatcher.TriggerEvent, Events.FrameWorkEvent.AOIDelEvtity, theEntity.ID);
    }

    void OnTriggerStay(Collider collider)
    {

        if (collider.tag == "Player" && !m_hasTrigger)
        {
            m_hasTrigger = true;
            Pick();
        }
    }

    void OnTriggerEnter(Collider collider)
    {

        if (collider.tag == "Player" && !m_hasTrigger)
        {
            m_hasTrigger = true;
            Pick();
            m_hasTrigger = true;
        }
    }

    void OnTriggerExit(Collider collider)
    {
    }

    public void DragTo(Transform t, float v = 0, float a = 30)
    {
        m_target = t;
        m_dragV = v;
        m_dragA = a;
        m_isDraging = true;
    }
}

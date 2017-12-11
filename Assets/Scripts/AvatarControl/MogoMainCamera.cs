// ģ����   :  MogoMainCamera
// ������   :  Ī׿��
// �������� :  2012-1-18
// ��    �� :  ��ͷ����,�ڵ��ﴦ��

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;
using Mogo.GameData;
using System;

public class MogoMainCamera : MonoBehaviour
{
    public static GameObject instance;
    public static MogoMainCamera Instance;
    //����Ŀ��
    public Transform target;
    public Transform m_target2;
    //����Ŀ��
    public float m_distance = 7f;
    //�Ƕȣ�0Ϊˮƽ,90���ӣ�
    public float m_rotationX = 45.0f;
    public float m_rotationY = 135.0f;

    public const float DISTANCE = 7f;
    public const float ROTATION_X = 45.0f;
    public const float ROTATION_Y = 135.0f;

    //�Ƿ����ڵ���͸������
    //public bool enableShiedTransparent = false;
    public float shiedAlpha = 0.5f;
    public string shiedShader = "Transparent/Diffuse";

    MogoCameraLighting m_camLighting;
    bool m_bPlayCamLighting = false;
    // Ϊȥ��������ʱ�������´���
    //float m_fCurrentTime = 0f;
    //float m_fAnimTime = 0f;
    //bool m_bIsToWhite = true;

    #region �������ű���
    private CameraAnimData m_currentShake;
    private float m_animStarTime;
    private float m_animLength;
    private bool m_isShaking = false;

    private float xSpeed;
    private float xDelta;//����ͬ��
    private float ySpeed;
    private float yDelta;
    private float zSpeed;
    private float zDelta;

    private CGAnim m_currentAnim = null;

    private Transform targetToCloseTo;
    private Vector3 targetToCloseToPosition;
    private Vector3 targetToCloseToRotation;
    private float targetToCloseToDistance;
    private float targetToCloseToRotationY;
    private float targetToCloseToRotationX;

    private Vector3 currentMoveSpeed = new Vector3(0, 0, 0);
    private Vector3 currentRotateSpeed = new Vector3(0, 0, 0);
    private float moveTime;
    private float moveSpeed;
    private float rotateTime;
    private float rotateSpeed;
    private float closeToTargetBeginTime;
    private Action m_closetToTargetOnDone;

    #endregion �������ű���

    public int m_state = LOCK_SIGHT;
    public int CurrentState
    {
        get
        {
            return m_state;
        }
        set
        {
            if (value != VICTORY && m_state == VICTORY)
            {
                m_camera.fieldOfView = 50;
                transform.parent = null;
            }
            m_state = value;
        }
    }
    public const int NONE = 0;
    public const int LOCK_SIGHT = 1;
    public const int CG = 2;
    public const int CLOST_TO_TARGET = 3;
    public const int LOCK_SIGHT2 = 4; //��׮
    public const int VICTORY = 5;//����ʤ����д
    public const int LOCK_SIGHT3 = 6;//�����ˣ�ͬʱ�ˣ��֣������3��ͬһ��ƽ��

    HashSet<string> withoutTestObjectTags = new HashSet<string>();

    //����͸������ָ�
    HashSet<GameObject> lastObjects = new HashSet<GameObject>();
    Dictionary<int, Shader> shaderDic = new Dictionary<int, Shader>();
    Dictionary<int, Color> colorDic = new Dictionary<int, Color>();

    Camera m_camera;

    void Start()
    {
        Instance = this;
        instance = this.gameObject;
        if (target == null)
        {
            GameObject go = GameObject.FindWithTag("Player");
            if (go == null) return;
            target = go.transform;
            target = MogoUtils.GetChild(target, "slot_camera");
        }


        //�ڴ������Ҫ�ų���������,�Ժ������layer�������
        //withoutTestObjectTags.Add("MainCamera");
        //withoutTestObjectTags.Add("terrain");
        //withoutTestObjectTags.Add("Player");

        EventDispatcher.TriggerEvent(Events.OtherEvent.MainCameraComplete);
        m_camera = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (!target)
            return;

        switch (CurrentState)
        {
            case LOCK_SIGHT:
                LockSight();
                break;
            case LOCK_SIGHT2:
                LockSight2();
                break;
            case CG:
                DoPlayCGAnim();
                break;
            case CLOST_TO_TARGET:
                CloseToTarget(targetToCloseTo, targetToCloseToDistance, targetToCloseToRotationY, targetToCloseToRotationX, moveTime, rotateTime, m_closetToTargetOnDone);
                break;
            case LOCK_SIGHT3:
                LockSight3(m_target2);
                break;
            default:
                break;
        }

        if (m_isShaking)
        {
            PlayShake();
        }
        //�ڵ���͸������
        //if (enableShiedTransparent)
        //{
        //    DoWhenTargetShield();
        //}

    }

    private void LockSight3(Transform target2)
    {
        CurrentState = LOCK_SIGHT3;
        m_target2 = target2;

        transform.position = target.position;
        transform.position -= Vector3.forward * m_distance;
        transform.LookAt(target);
        transform.RotateAround(target.position, new Vector3(1, 0, 0), m_rotationX);

        //�ָ���
        Vector3 v1 = target.position - target2.position;
        Vector2 vec1 = new Vector2(v1.x, v1.z).normalized;

        //�˸������
        Vector3 v2 = transform.position - target.position;
        Vector2 vec2 = new Vector2(v2.x, v2.z).normalized;

        //����н�
        float angle = Vector2.Angle(vec1, vec2);

        //�жϷ���ת��
        if (vec2.x > vec1.x) angle = -angle;

        transform.RotateAround(target.position, new Vector3(0, 1, 0), angle);
    }

    public void LockSight2()
    {
        CurrentState = LOCK_SIGHT2;
        transform.LookAt(target);
    }

    public void ChangeLockSightData(float distance, float rotationY, float rotationX, float _moveTime, float _rotateTime, bool isLock = true)
    {
        if (isLock)
        {
            PlayCGAnim(m_distance, distance, m_rotationX, rotationX, m_rotationY, rotationY, 0, 0, _moveTime, target, LockSight);
        }
        else
        {
            CloseToTarget(target, distance, rotationY, rotationX, _moveTime, _rotateTime, LockSight);
        }
        m_distance = distance;
        m_rotationX = rotationX;
        m_rotationY = rotationY;

    }

    /// <summary>
    /// �õ�����Ŀ��
    /// </summary>
    private void GetRotationAndPositon(Transform t, float dis, float rotationY, float rotationX, out Vector3 position, out Vector3 rotation)
    {
        position = Vector3.zero;
        rotation = Vector3.zero;

        if (!t)
            return;
        Vector3 originalPos = transform.position;
        Vector3 originalRotation = transform.eulerAngles;

        transform.position = t.position;
        Transform slot_camera = t.Find("slot_camera");
        if (slot_camera != null)
        {
            //Mogo.Util.LoggerHelper.Debug("slot_camera != null");
            transform.position = slot_camera.position;
            t = slot_camera;
        }
        transform.position -= Vector3.forward * dis;
        transform.LookAt(t);
        transform.RotateAround(t.position, new Vector3(1, 0, 0), rotationX);
        transform.RotateAround(t.position, new Vector3(0, 1, 0), rotationY);

        //�õ�λ����ת��
        position = transform.position;
        rotation = transform.eulerAngles;

        //�ָ�
        transform.position = originalPos;
        transform.eulerAngles = originalRotation;
    }

    public void ResetToNormal()
    {
        m_distance = DISTANCE;
        m_rotationX = ROTATION_X;
        m_rotationY = ROTATION_Y;
        LockSight();
    }

    public void LockSight()
    {
        CurrentState = LOCK_SIGHT;

        if (this == null) return;
        if (transform == null) return;
        if (target == null) return;
        transform.position = target.position;
        transform.position -= Vector3.forward * m_distance;
        transform.LookAt(target);
        transform.RotateAround(target.position, new Vector3(1, 0, 0), m_rotationX);
        transform.RotateAround(target.position, new Vector3(0, 1, 0), m_rotationY);
    }

    /// <summary>
    /// ���ž�ͷ����
    /// </summary>
    /// <param name="cameraAnimId">����id</param>
    /// <param name="length">����</param>
    public void Shake(int cameraAnimId, float length)
    {
        CameraAnimData data = CameraAnimData.dataMap.GetValueOrDefault(cameraAnimId, null);
        if (data == null) return;
        m_currentShake = data;
        m_animStarTime = Time.time;
        m_animLength = length;
        xDelta = 0;
        yDelta = 0;
        zDelta = 0;
        xSpeed = data.xRate * 4 * data.xSwing;
        ySpeed = data.yRate * 4 * data.ySwing;
        zSpeed = data.zRate * 4 * data.zSwing;
        m_isShaking = true;
    }

    public void Shake(float _xSwing, float _ySwing, float _zSwing, int _xRate, int _yRate, int _zRate, float _length)
    {
        CameraAnimData data = new CameraAnimData()
        {
            xRate = _xRate,
            yRate = _yRate,
            zRate = _zRate,
            xSwing = _xSwing,
            ySwing = _ySwing,
            zSwing = _zSwing,
        };
        if (data == null) return;
        m_currentShake = data;
        m_animStarTime = Time.time;
        m_animLength = _length;
        xDelta = 0;
        yDelta = 0;
        zDelta = 0;
        xSpeed = data.xRate * 4 * data.xSwing;
        ySpeed = data.yRate * 4 * data.ySwing;
        zSpeed = data.zRate * 4 * data.zSwing;
        m_isShaking = true;
    }

    private void PlayShake()
    {
        //Mogo.Util.LoggerHelper.Debug("m_animStarTime:" + m_animStarTime);
        //Mogo.Util.LoggerHelper.Debug("Time.time:" + Time.time);
        //Mogo.Util.LoggerHelper.Debug("deltaTime:" + (Time.time - m_animStarTime) * 1000);

        if ((Time.time - m_animStarTime) > m_animLength)
        {
            m_isShaking = false;
            xDelta = 0;
            yDelta = 0;
            zDelta = 0;
            return;
        }

        xDelta = xSpeed * Time.deltaTime + xDelta;
        yDelta = ySpeed * Time.deltaTime + yDelta;
        zDelta = zSpeed * Time.deltaTime + zDelta;

        while (Mathf.Abs(xDelta) > m_currentShake.xSwing)
        {
            if (xDelta > 0) xDelta = m_currentShake.xSwing * 2 - xDelta;
            else xDelta = -m_currentShake.xSwing * 2 - xDelta;
            xSpeed = -xSpeed;
        }
        while (Mathf.Abs(yDelta) > m_currentShake.ySwing)
        {
            if (yDelta > 0) yDelta = m_currentShake.ySwing * 2 - yDelta;
            else yDelta = -m_currentShake.ySwing * 2 - yDelta;
            ySpeed = -ySpeed;
        }
        while (Mathf.Abs(zDelta) > m_currentShake.zSwing)
        {
            if (zDelta > 0) zDelta = m_currentShake.zSwing * 2 - zDelta;
            else zDelta = -m_currentShake.zSwing * 2 - zDelta;
            zSpeed = -zSpeed;
        }


        //if (Mathf.Abs(xDelta) > m_currentShake.xSwing)
        //{
        //    if (xDelta > 0) xDelta = m_currentShake.xSwing * 2 - xDelta;
        //    else xDelta = -m_currentShake.xSwing * 2 - xDelta;
        //    xSpeed = -xSpeed;
        //}
        //if (Mathf.Abs(yDelta) > m_currentShake.ySwing)
        //{
        //    if (yDelta > 0) yDelta = m_currentShake.ySwing * 2 - yDelta;
        //    else yDelta = -m_currentShake.ySwing * 2 - yDelta;
        //    ySpeed = -ySpeed;
        //}
        //if (Mathf.Abs(zDelta) > m_currentShake.zSwing)
        //{
        //    if (zDelta > 0) zDelta = m_currentShake.zSwing * 2 - zDelta;
        //    else zDelta = -m_currentShake.zSwing * 2 - zDelta;
        //    zSpeed = -zSpeed;
        //}

        transform.Translate(new Vector3(xDelta, yDelta, zDelta), Space.Self);

    }

    public class CGAnim
    {
        public float originalDis;
        public float targetDis;
        public float orginalRx;
        public float targetRx;
        public float orginalRy;
        public float targetRy;
        public float originalPx;
        public float targetPx;
        public float length;
        public float startTime;
        public Vector3 targetPos;
        public Transform target;
        public Action onDone;

        public Vector3 actualPos
        {
            get
            {
                if (target == null) return targetPos;
                return target.position;
            }
        }
    }

    public void PlayCGAnim(string animPath)
    {
		Animation animation = GetComponent<Animation> ();
        if (animation[animPath] == null)
        {
            AssetCacheMgr.GetResourceAutoRelease(animPath,
                (obj) =>
                {
                    AnimationClip clip = obj as AnimationClip;
                    animation.AddClip(clip, animPath);
                    animation.Play(animPath);
                    CurrentState = CG;

                });
        }
        else
        {
            animation.Play(animPath);
            CurrentState = CG;
        }
    }

    public void OnCGAnimStop()
    {
        Mogo.Util.LoggerHelper.Debug("OnCGAnimStop");
        CurrentState = NONE;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_target">Ŀ��</param>
    /// <param name="_moveTime">positon�ƶ�ʱ��</param>
    /// <param name="_rotateTime">rotationת��ʱ��</param>
    private void CloseToTarget(Vector3 _position, Vector3 _rotation, float _moveTime, float _rotateTime, Action OnDone = null)
    {
        if (CurrentState != CLOST_TO_TARGET)
        {
            targetToCloseToPosition = _position;
            targetToCloseToRotation = _rotation;
            CurrentState = CLOST_TO_TARGET;
            closeToTargetBeginTime = Time.time;
            moveTime = _moveTime;
            rotateTime = _rotateTime;
            m_closetToTargetOnDone = OnDone;
        }
        else if (Time.time - closeToTargetBeginTime > _moveTime && Time.time - closeToTargetBeginTime > _rotateTime)
        {
            CurrentState = NONE;
            if (OnDone != null)
            {
                OnDone();
            }
            return;
        }
        else
        {
            _moveTime = moveTime - (Time.time - closeToTargetBeginTime);
            _rotateTime = rotateTime - (Time.time - closeToTargetBeginTime);
        }

        Vector3 position = Vector3.SmoothDamp(transform.position, targetToCloseToPosition, ref currentMoveSpeed, _moveTime);

        Vector3 targetRotation = targetToCloseToRotation;
        float x = targetRotation.x;
        float y = targetRotation.y;
        float z = targetRotation.z;

        if (x - transform.eulerAngles.x > 180) x -= 360f;
        if (y - transform.eulerAngles.y > 180) y -= 360f;
        if (z - transform.eulerAngles.z > 180) z -= 360f;

        if (transform.eulerAngles.x - x > 180) x += 360f;
        if (transform.eulerAngles.y - y > 180) y += 360f;
        if (transform.eulerAngles.z - z > 180) z += 360f;

        targetRotation = new Vector3(x, y, z);

        Vector3 rotation = Vector3.SmoothDamp(transform.eulerAngles, targetRotation, ref currentRotateSpeed, _rotateTime);

        transform.position = position;
        transform.eulerAngles = rotation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_target">Ŀ��</param>
    /// <param name="distance">����Ŀ�����</param>
    /// <param name="rotationY">��Ŀ��y��Ƕ�</param>
    /// <param name="rotationX">��Ŀ��x��Ƕ�</param>
    /// <param name="_moveTime">pos�ƶ�ʱ��</param>
    /// <param name="_rotateTime">rotationת��ʱ��</param>
    public void CloseToTarget(Transform _target, float distance, float rotationY, float rotationX, float _moveTime, float _rotateTime, Action OnDone = null)
    {
        Vector3 posDir;
        if (CurrentState != CLOST_TO_TARGET)
        {
            //Mogo.Util.LoggerHelper.Debug("CloseToTarget:" + _target.name + ",distance:" + distance + ",rotationY:" + m_rotationY + ",rotationX:" + m_rotationX + ",_moveTime:" + _moveTime + ",_rotateTime:" + _rotateTime);
            targetToCloseTo = _target;
            targetToCloseToDistance = distance;
            targetToCloseToRotationX = rotationX;
            targetToCloseToRotationY = rotationY;
            CurrentState = CLOST_TO_TARGET;
            closeToTargetBeginTime = Time.time;
            moveTime = _moveTime;
            rotateTime = _rotateTime;
            m_closetToTargetOnDone = OnDone;
            currentMoveSpeed = Vector3.zero;
            currentRotateSpeed = Vector3.zero;
            GetRotationAndPositon(_target, distance, rotationY, rotationX, out targetToCloseToPosition, out targetToCloseToRotation);

            posDir = targetToCloseToPosition - transform.position;
            moveSpeed = posDir.magnitude / moveTime;
            currentMoveSpeed = moveSpeed * posDir.normalized / 1.3f;
            //currentRotateSpeed = Vector3.zero;
        }
        //(Time.time - closeToTargetBeginTime > _moveTime && Time.time - closeToTargetBeginTime > _rotateTime) ||
        else if (
            (IsCloseEnough(transform.position, targetToCloseToPosition, 0.01f)) && IsCloseEnough(transform.eulerAngles, targetToCloseToRotation, 0.01f))
        {
            //Mogo.Util.LoggerHelper.Debug("on done");
            CurrentState = NONE;
            if (OnDone != null)
            {
                OnDone();
            }
            return;
        }
        else
        {
            _moveTime = moveTime - (Time.time - closeToTargetBeginTime);
            _rotateTime = rotateTime - (Time.time - closeToTargetBeginTime);
        }
        GetRotationAndPositon(_target, distance, rotationY, rotationX, out targetToCloseToPosition, out targetToCloseToRotation);


        Vector3 position = Vector3.SmoothDamp(transform.position, targetToCloseToPosition, ref currentMoveSpeed, _moveTime);
        //posDir = (targetToCloseToPosition - transform.position).normalized;
        //Vector3 position = transform.position + posDir * moveSpeed;

        Vector3 targetRotation = targetToCloseToRotation;
        float x = targetRotation.x;
        float y = targetRotation.y;
        float z = targetRotation.z;

        if (x - transform.eulerAngles.x > 180) x -= 360f;
        if (y - transform.eulerAngles.y > 180) y -= 360f;
        if (z - transform.eulerAngles.z > 180) z -= 360f;

        if (transform.eulerAngles.x - x > 180) x += 360f;
        if (transform.eulerAngles.y - y > 180) y += 360f;
        if (transform.eulerAngles.z - z > 180) z += 360f;

        targetRotation = new Vector3(x, y, z);

        Vector3 rotation = Vector3.SmoothDamp(transform.eulerAngles, targetRotation, ref currentRotateSpeed, _rotateTime);

        transform.position = position;
        transform.eulerAngles = rotation;
    }

    private bool IsCloseEnough(Vector3 v1, Vector3 v2, float range)
    {
        return (v2 - v1).magnitude < range;
    }

    /// <summary>
    /// ����CG��ͷ�����ӿ�
    /// </summary>
    /// <param name="originalDis">ԭ�߶�</param>
    /// <param name="targetDis">Ŀ��߶�</param>
    /// <param name="orginalRx">��x����Ƕ�</param>
    /// <param name="targetRx"></param>
    /// <param name=" orginalRy">��y����Ƕ�</param>
    /// <param name="targetRy"></param>
    /// <param name="originalPx">x������</param>
    /// <param name="targetPx"></param>
    /// <param name="length">ʱ�䳤�ȣ��룩</param>
    public void PlayCGAnim
        (float _originalDis, float _targetDis,
         float _orginalRx, float _targetRx,
         float _orginalRy, float _targetRy,
        float _originalPx, float _targetPx,
        float _length, Transform _target, Action _onDone = null)
    {
        if (_target == null) return;
        Vector3 position = _target.position;
        Transform slotCamera = _target.Find("slot_camera");
        if (slotCamera != null)
        {
            _target = slotCamera;
            position = slotCamera.position;
        }
        PlayCGAnim(_originalDis, _targetDis,
          _orginalRx, _targetRx,
          _orginalRy, _targetRy,
         _originalPx, _targetPx,
         _length, position, _target, _onDone);

        //m_currentAnim = new CGAnim()
        //{
        //    originalDis = _originalDis,
        //    targetDis = _targetDis,
        //    orginalRx = _orginalRx,
        //    targetRx = _targetRx,
        //    orginalRy = _orginalRy,
        //    targetRy = _targetRy,
        //    originalPx = _originalPx,
        //    targetPx = _targetPx,
        //    length = _length,
        //    startTime = Time.time,
        //    targetPos = position,
        //    target = slotCamera,
        //    onDone = _onDone
        //};
        //m_state = CG;
    }

    /// <summary>
    /// ����CG��ͷ�����ӿ�
    /// </summary>
    /// <param name="originalDis">ԭ�߶�</param>
    /// <param name="targetDis">Ŀ��߶�</param>
    /// <param name="orginalRx">��x����Ƕ�</param>
    /// <param name="targetRx"></param>
    /// <param name="orginalRy">��y����Ƕ�</param>
    /// <param name="targetRy"></param>
    /// <param name="originalPx">x������</param>
    /// <param name="targetPx"></param>
    /// <param name="length">ʱ�䳤�ȣ��룩</param>
    public void PlayCGAnim
        (float _originalDis, float _targetDis,
         float _orginalRx, float _targetRx,
         float _orginalRy, float _targetRy,
        float _originalPx, float _targetPx,
        float _length, Vector3 _targetPos, Transform _target = null, Action _onDone = null)
    {
        Mogo.Util.LoggerHelper.Debug("PlayCGAnim");
        m_currentAnim = new CGAnim()
        {
            originalDis = _originalDis,
            targetDis = _targetDis,
            orginalRx = _orginalRx,
            targetRx = _targetRx,
            orginalRy = _orginalRy,
            targetRy = _targetRy,
            originalPx = _originalPx,
            targetPx = _targetPx,
            length = _length,
            startTime = Time.time,
            targetPos = _targetPos,
            target = _target,
            onDone = _onDone
        };
        CurrentState = CG;
    }

    private void DoPlayCGAnim()
    {
        if (m_currentAnim == null) return;
        float timeElapse = Time.time - m_currentAnim.startTime;

        if (timeElapse >= m_currentAnim.length || m_currentAnim.length == 0)
        {
            Action temp = m_currentAnim.onDone;
            CurrentState = NONE;
            m_currentAnim = null;

            if (temp != null)
            {
                temp();
            }
            return;
        }

        transform.position = m_currentAnim.actualPos;

        float timeRate = timeElapse / m_currentAnim.length;

        float dis = Mathf.Lerp(m_currentAnim.originalDis, m_currentAnim.targetDis, timeRate);
        float rotationX = Mathf.Lerp(m_currentAnim.orginalRx, m_currentAnim.targetRx, timeRate);
        float rotationY = Mathf.Lerp(m_currentAnim.orginalRy, m_currentAnim.targetRy, timeRate);
        float dx = Mathf.Lerp(m_currentAnim.originalPx, m_currentAnim.targetPx, timeRate);

        transform.position -= Vector3.forward * dis;
        transform.LookAt(m_currentAnim.actualPos);
        transform.RotateAround(m_currentAnim.actualPos, new Vector3(1, 0, 0), rotationX);
        transform.RotateAround(m_currentAnim.actualPos, new Vector3(0, 1, 0), rotationY);
        transform.Translate(new Vector3(dx, 0, 0), Space.Self);
    }

    public void PlayVictoryCG()
    {

        int weaponType = InventoryManager.Instance.GetCurrentWeaponType();
        if (!VictorCameraCfgData.dataMap.ContainsKey(weaponType)) return;
        VictorCameraCfgData data = VictorCameraCfgData.dataMap.Get(weaponType);
        Transform parent = MogoWorld.thePlayer.Transform.Find(data.Path);
        if (parent == null)
        {
            LoggerHelper.Error("can not find " + data.Path);
            return;
        } 
        Utils.MountToSomeObjWithoutPosChange(transform, parent);
        m_camera.fieldOfView = 32;
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;

		Animation ani = MogoWorld.thePlayer.Transform.Find(data.path).GetComponent<Animation>();
        ani.Stop();
        ani.Play();


        MogoWorld.thePlayer.SetAction(40);
        MogoWorld.thePlayer.AddCallbackInFrames(() => { MogoWorld.thePlayer.SetAction(999); });

        CurrentState = VICTORY;
    }

    private void DoWhenTargetShield()
    {
        //�ڳ�����ͼ�п��Կ�����������
        Debug.DrawLine(target.position, transform.position, Color.red);
        //���ǳ����������������

        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.forward, Vector3.Distance(target.position, transform.position));

        ResetLastObjects();

        DealWithShiedObjects(hits);
    }

    private void DealWithShiedObjects(RaycastHit[] hits)
    {
        int i = 0;
        RaycastHit hit;
        while (i < hits.Length)
        {
            hit = hits[i];

            string name = hit.collider.gameObject.tag;
            if (!withoutTestObjectTags.Contains(name))
            {
                //����ײ�Ĳ��������Ҳ���ǵ��� ��ôֱ���ƶ������������
                //transform.position = hit.point;

                GameObject obj = hit.collider.gameObject;
				Renderer renderer = obj.GetComponent<Renderer> ();
				if (renderer == null) return;

				Material m = renderer.GetComponent<Material>();
                int id = obj.GetInstanceID();
                lastObjects.Add(obj);
                if (!colorDic.ContainsKey(id))
                {
                    colorDic.Add(id, m.color);
                    shaderDic.Add(id, m.shader);
                }

                Color color = m.color;
                color.a = shiedAlpha;
                m.SetColor("_Color", color);
                m.shader = Shader.Find(shiedShader);
            }
            i++;
        }
    }

    private void ResetLastObjects()
    {
        foreach (GameObject go in lastObjects)
        {
            if (go == null) 
				continue;

			Renderer renderer = go.GetComponent<Renderer> ();
			Material m = renderer.GetComponent<Material>();
            m.color = colorDic[go.GetInstanceID()];
            m.shader = shaderDic[go.GetInstanceID()];
        }
        lastObjects.Clear();
        colorDic.Clear();
        shaderDic.Clear();
    }

    void Awake()
    {
        //m_camLighting = GetComponentsInChildren<MogoCameraLighting>(true)[0];
        m_camLighting = gameObject.AddComponent<MogoCameraLighting>();
        //m_camLighting.enabled = false;
        //gameObject.AddComponent<FadeCamera>().enabled = false;

        //float screenWidth = camera.GetScreenWidth();
        //float screenHeight = camera.GetScreenHeight();

        //float manualAspect = 1280f / 720f;

        //float manualWidth = screenHeight * manualAspect;

        //float widthDelta = screenWidth - manualWidth;
        //float widthDeltaPercent = widthDelta / screenWidth;

        //camera.rect = new Rect(widthDeltaPercent * 0.5f, 0, manualWidth / screenWidth, 1);
    }

    public void FadeToAllWhite(Vector2 center, float lastTime)
    {
        //m_fCurrentTime = 0;
        //m_camLighting.enabled = true;
        //m_camLighting.SetCenter(center);
        //m_bPlayCamLighting = true;
        //m_fAnimTime = lastTime;
        //m_bIsToWhite = true;
        //m_camLighting.SetFadeColor(new Color(1, 1, 1, 1));



        m_camLighting.enabled = true;
        m_bPlayCamLighting = true;
        m_camLighting.SetFadeColor(new Color(1, 1, 1, 0));
        m_camLighting.SetLastTime(lastTime);
        m_camLighting.StartFade(true);
    }

    public void FadeToNoneWhite(Vector2 center, float lastTime)
    {
        //m_fCurrentTime = 0;
        //m_camLighting.SetCenter(center);
        //m_bPlayCamLighting = true;
        //m_fAnimTime = lastTime;
        //m_bIsToWhite = false;


        m_camLighting.enabled = true;
        m_bPlayCamLighting = true;
        m_camLighting.SetFadeColor(new Color(1, 1, 1, 1));
        m_camLighting.SetLastTime(lastTime);
        m_camLighting.StartFade(false);
    }

    public void Reset()
    {
        CurrentState = NONE;
    }

    public void FadeToAllBlack(Vector2 center, float lastTime)
    {
        //m_fCurrentTime = 0;
        //m_camLighting.enabled = true;
        //m_camLighting.SetCenter(center);
        //m_bPlayCamLighting = true;
        //m_fAnimTime = lastTime;
        //m_bIsToWhite = true;
        //m_camLighting.SetFadeColor(new Color(0, 0, 0, 1));
    }


    //void Update()
    //{
    //    //if (m_bPlayCamLighting)
    //    //{
    //    //    if (m_bIsToWhite)
    //    //    {
    //    //        m_camLighting.SetControll(0 + m_fCurrentTime / m_fAnimTime * 30);
    //    //    }
    //    //    else
    //    //    {
    //    //        m_camLighting.SetControll(30 - 30 * m_fCurrentTime / m_fAnimTime);
    //    //    }

    //    //    m_fCurrentTime += Time.deltaTime;

    //    //    if (m_fCurrentTime > m_fAnimTime)
    //    //    {
    //    //        m_bPlayCamLighting = false;
    //    //        m_fCurrentTime = 0;

    //    //        if (!m_bIsToWhite)
    //    //            m_camLighting.enabled = false;
    //    //        else
    //    //            m_camLighting.SetControll(30);
    //    //    }
    //    //}

    //    if (Input.GetKeyDown(KeyCode.RightAlt))
    //    {
    //        FadeToAllWhite(new Vector2(0, 0), 0.1f);
    //    }

    //    if (Input.GetKeyDown(KeyCode.LeftAlt))
    //    {
    //        FadeToNoneWhite(new Vector2(0, 0), 0.1f);
    //    }
    //}
}
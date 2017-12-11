using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mogo.GameData;
using Mogo.Util;


public class MogoFXManager
{
    private static MogoFXManager m_instance;

    public static MogoFXManager Instance
    {
        get
        {
            if (m_instance == null)
            {

                m_instance = new MogoFXManager();
            }

            return MogoFXManager.m_instance;

        }
    }

    public List<GameObject> m_listShadow = new List<GameObject>();
    public List<GameObject> m_listShadowReciver = new List<GameObject>();
    public List<GameObject> m_listHalo = new List<GameObject>();
    public List<GameObject> m_listUIParticle = new List<GameObject>();
    public List<GameObject> m_listEnemyObj = new List<GameObject>();
    public List<GameObject> m_listEnemyScreenObj = new List<GameObject>();

    private Camera m_camUIFX;
    private GameObject m_goUIParticleAnimRoot;

    private Shader m_hitShader;
    private Shader m_monsterShader;

    private GameObject m_goUIAttachedObj;
    private GameObject m_goUIAttachFX;
    private Camera m_camUIAttachFX;
    private Transform m_billboardPanel;
    public Transform BillboardPanel
    {
        get
        {
            if (!m_billboardPanel)
                m_billboardPanel = GameObject.Find("BillboardPanel").transform;
            return m_billboardPanel;
        }
    }
    private GameObject m_mogoMainUIPanel;

    public GameObject MogoMainUIPanel
    {
        get
        {
            if (!m_mogoMainUIPanel)
                m_mogoMainUIPanel = GameObject.Find("MogoMainUIPanel");
            return m_mogoMainUIPanel;
        }
    }

    private Camera m_camBillboard;

    private GameObject m_pointer;
    private bool m_pointerLock = false;

    public Camera GetBillboardCamera()
    {
        if (m_camBillboard == null)
        {
            m_camBillboard = GameObject.Find("BillboardCamera").GetComponentsInChildren<Camera>(true)[0];
        }

        return m_camBillboard;
    }

    public Camera GetUIFXCamera()
    {
        if (m_camUIFX == null)
        {
            m_camUIFX = GameObject.Find("UIFXCamera").GetComponentsInChildren<Camera>(true)[0];
        }

        return m_camUIFX;
    }

    public void Initialize()
    {
        //m_hitShader = Shader.Find("Mogo/FakeLight");
        //m_monsterShader = Shader.Find("Mogo/MonsterShader");

        //if (m_hitShader == null)
        //{
        //    MogoGlobleUIManager.Instance.Info("FakeLight shader is Null");
        //}

        //if (m_monsterShader == null)
        //{
        //    MogoGlobleUIManager.Instance.Info("MonsterShader is Null");
        //}

        AssetCacheMgr.GetUIResource("MogoMonsterHitShader.shader", (obj) => { m_hitShader = (Shader)obj; });
        AssetCacheMgr.GetUIResource("MonsterShader.shader", (obj) => { m_monsterShader = (Shader)obj; });
        // m_hitShader = Shader.Find("Mobile/Particles/Additive");
    }

    public Shader GetHitShader()
    {
        //if (m_hitShader == null)
        //{
        //    m_hitShader = Shader.Find("Mogo/FakeLight");
        //}

        return m_hitShader;
    }

    public Shader GetMonsterShader()
    {
        //if (m_monsterShader == null)
        //{
        //    m_monsterShader = Shader.Find("Mogo/MonsterShader");
        //}

        return m_monsterShader;
    }

    public GameObject GetUIFXRoot()
    {
        if (m_goUIParticleAnimRoot == null)
        {
            m_goUIParticleAnimRoot = GameObject.Find("UIParticleAnimRoot");
        }

        return m_goUIParticleAnimRoot;
    }

    bool m_bUIFXAttached = false;

    public enum AlphaFadeType
    {
        AFT_ONCE,
        AFT_LOOP,
        AFT_PINGPONG
    }

    public enum HitColorType
    {
        HCT_WHITE,
        HCT_RED
    }

    public void AlphaFadeIn(GameObject goFade, float fadeTime, AlphaFadeType aft = AlphaFadeType.AFT_ONCE)
    {
        MogoTweenAlpha mogoTA;

        if (goFade.GetComponentsInChildren<MogoTweenAlpha>(true).Length > 0)
        {
            mogoTA = goFade.GetComponentsInChildren<MogoTweenAlpha>(true)[0];
        }
        else
        {
            mogoTA = goFade.AddComponent<MogoTweenAlpha>();
        }

        mogoTA.Reset();
        mogoTA.alpha = 0f;
        mogoTA.from = 0f;
        mogoTA.to = 1f;

        switch (aft)
        {
            case AlphaFadeType.AFT_ONCE:
                mogoTA.style = UITweener.Style.Once;
                break;

            case AlphaFadeType.AFT_LOOP:
                mogoTA.style = UITweener.Style.Loop;
                break;

            case AlphaFadeType.AFT_PINGPONG:
                mogoTA.style = UITweener.Style.PingPong;
                break;
        }

        mogoTA.duration = fadeTime;
        mogoTA.ignoreTimeScale = true;
        mogoTA.method = UITweener.Method.EaseInOut;
        mogoTA.enabled = true;
    }

    public void ResetAlpha(GameObject goFade)
    {
        MogoTweenAlpha mogoTA;

        if (goFade.GetComponentsInChildren<MogoTweenAlpha>(true).Length > 0)
        {
            mogoTA = goFade.GetComponentsInChildren<MogoTweenAlpha>(true)[0];
            mogoTA.alpha = 1;
        }

    }

    public void AlphaFadeOut(GameObject goFade, float fadeTime, AlphaFadeType aft = AlphaFadeType.AFT_ONCE)
    {
        if (goFade == null)
            return;

        MogoTweenAlpha mogoTA;

        if (goFade.GetComponentsInChildren<MogoTweenAlpha>(true).Length > 0)
        {
            mogoTA = goFade.GetComponentsInChildren<MogoTweenAlpha>(true)[0];
        }
        else
        {
            mogoTA = goFade.AddComponent<MogoTweenAlpha>();
        }

        mogoTA.Reset();
        mogoTA.alpha = 1f;
        mogoTA.from = 1f;
        mogoTA.to = 0f;

        switch (aft)
        {
            case AlphaFadeType.AFT_ONCE:
                mogoTA.style = UITweener.Style.Once;
                break;

            case AlphaFadeType.AFT_LOOP:
                mogoTA.style = UITweener.Style.Loop;
                break;

            case AlphaFadeType.AFT_PINGPONG:
                mogoTA.style = UITweener.Style.PingPong;
                break;
        }

        mogoTA.duration = fadeTime;
        mogoTA.ignoreTimeScale = true;
        mogoTA.method = UITweener.Method.EaseInOut;
        mogoTA.enabled = true;
    }

    public void AddShadow(GameObject obj, uint playerId, float scaleX = 1, float scaleY = 1, float scaleZ = 1, int layer = 0)
    {

        AssetCacheMgr.GetUIInstance("MogoShadow.prefab", (prefab, id, go) =>
        {
            if (obj != null)
            {
                GameObject shadow = (GameObject)go;

                //obj.AddComponent<MogoShadowReceiver>();
                shadow.transform.parent = obj.transform;
                shadow.transform.localPosition = new Vector3(0, 0.01f, 0);
                shadow.layer = layer;
                shadow.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);

                shadow.name = string.Concat("shadow_", playerId.ToString());
                m_listShadow.Add(shadow);
                m_listShadowReciver.Add(obj);
            }
        });
    }

    public void AttachShadow(GameObject obj, string shadowName, float rotationX = 0, float rotationY = 0, float rotationZ = 0,
        float scaleX = 1, float scaleY = 1, float scaleZ = 1, float posX = 0, float posY = 0.01f, float posZ = 0)
    {
        AssetCacheMgr.GetUIInstance("MogoShadow.prefab", (prefab, id, go) =>
        {
            if (obj != null)
            {
                GameObject shadow = (GameObject)go;

                //obj.AddComponent<MogoShadowReceiver>();
                shadow.transform.parent = obj.transform;
                shadow.transform.localPosition = new Vector3(posX, posY, posZ);
                shadow.transform.localEulerAngles = new Vector3(rotationX, rotationY, rotationZ);
                shadow.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);

                //shadow.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);

                shadow.name = shadowName;
                m_listShadow.Add(shadow);
                m_listShadowReciver.Add(obj);
            }
        });
    }

    public void RemoveShadow(uint playerId)
    {
        //LoggerHelper.Debug("Trying To ReMoveShadow");
        for (int i = 0; i < m_listShadow.Count; ++i)
        {
            if (m_listShadow[i] != null && m_listShadow[i].name == string.Concat("shadow_", playerId.ToString()))
            {
                GameObject go = m_listShadow[i];
                GameObject goRecv = m_listShadowReciver[i];

                //GameObject.Destroy(goRecv.GetComponentsInChildren<MogoShadowReceiver>(true)[0]);

                m_listShadowReciver.Remove(goRecv);

                AssetCacheMgr.ReleaseInstance(go);
                m_listShadow.Remove(go);

                break;
            }
        }
    }

    public void RemoveAllShadow()
    {
        int index = 0;

        for (int i = 0; i < m_listShadow.Count; ++i)
        {
            index = i;

            var go = m_listShadow[index];

            //if (m_listShadowReciver[index])
            //    GameObject.Destroy(m_listShadowReciver[index].GetComponentsInChildren<MogoShadowReceiver>(true)[0]);
            AssetCacheMgr.ReleaseInstance(go);
        }

        m_listShadowReciver.Clear();
        m_listShadow.Clear();
    }

    public void FloatText(string text, GameObject goParent = null, bool hideGo = false)
    {
        AssetCacheMgr.GetUIInstance("MogoFloatText.prefab", (prefab, id, go) =>
        {
            GameObject obj = (GameObject)go;

            if (goParent == null)
                goParent = MogoMainUIPanel;

            obj.transform.parent = MogoUIManager.Instance.transform;
            obj.transform.position = goParent.transform.position;
            obj.transform.localScale = new Vector3(1, 1, 1);

            UILabel lbl = obj.transform.GetComponentInChildren<UILabel>();

            lbl.text = text;

            if (hideGo)
                goParent.SetActive(false);

            TimerHeap.AddTimer(2000, 0, () => { AssetCacheMgr.ReleaseInstance(obj); });
        });
    }

    public void GetHit(GameObject goTarget, float lastTime = 0.2f, HitColorType hct = HitColorType.HCT_WHITE)
    {
        //return; //---------------By MaiFeo



        //Color32 c = new Color32(0, 0, 0, 0);

        //switch (hct)
        //{
        //    case HitColorType.HCT_WHITE:
        //        c = new Color32(64, 64, 64, 0);
        //        break;

        //    case HitColorType.HCT_RED:
        //        c = new Color32(255, 0, 0, 0);
        //        break;
        //}

        SkinnedMeshRenderer[] smrList = goTarget.GetComponentsInChildren<SkinnedMeshRenderer>(true);

        if (smrList == null)
            return;


        for (int i = 0; i < smrList.Length; ++i)
        {
            //smrList[i].material.shader = GetHitShader();
            smrList[i].material.SetFloat("_FinalPower", 3);

            if (smrList[i].material.shader != null)
            {
                LoggerHelper.Debug(string.Concat("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ ",
                    smrList[i].material.shader.name));
            }
            else
            {
                LoggerHelper.Error("@@@@@@@@@@@@@@@@@@@@@@@@@@@ HitShader is NULL");
                //MogoGlobleUIManager.Instance.Info("HitShader is NULL");
            }
            //goTarget.GetComponentsInChildren<SkinnedMeshRenderer>(true)[i].material.SetColor("_HitColor", c);
        }

        TimerHeap.AddTimer<GameObject>((uint)(0.1 * 1000), 0, (obj) => { ReleaseHit(obj); }, goTarget);
        //MogoTweenAlpha mogoTA;
        //MonsterGetHit mgh;

        //if (goTarget.GetComponentsInChildren<MogoTweenAlpha>(true).Length > 0)
        //{
        //    mogoTA = goTarget.GetComponentsInChildren<MogoTweenAlpha>(true)[0];
        //    mgh = goTarget.GetComponentsInChildren<MonsterGetHit>(true)[0];
        //}
        //else
        //{
        //    mogoTA = goTarget.AddComponent<MogoTweenAlpha>();
        //    mgh = goTarget.AddComponent<MonsterGetHit>();
        //}

        //mogoTA.Reset();
        //mogoTA.from = 1f;
        //mogoTA.to = 1f;

        //mogoTA.eventReceiver = goTarget;
        //mogoTA.callWhenFinished = "OnHitOver";
        //mogoTA.duration = lastTime;
        //mogoTA.ignoreTimeScale = true;
        //mogoTA.enabled = true;
    }

    public void ReleaseHit(GameObject goTarget)
    {
        //return; //---------------By MaiFeo

        if (goTarget == null)
            return;
        SkinnedMeshRenderer[] smrList = goTarget.GetComponentsInChildren<SkinnedMeshRenderer>(true);

        if (smrList == null)
            return;

        for (int i = 0; i < smrList.Length; ++i)
        {
            smrList[i].material.SetFloat("_FinalPower", 1);
            //smrList[i].material.shader = GetMonsterShader();
            // goTarget.GetComponentsInChildren<SkinnedMeshRenderer>(true)[i].material.SetColor("_HitColor", new Color32(0, 0, 0, 0));
        }
    }


    public void AddPointerToTarget(GameObject goTarget, uint playerId, Vector3 targetPos)
    {
        AssetCacheMgr.GetUIInstance("fx_jiaodi.prefab", (prefab, id, go) =>
        {
            GameObject halo = (GameObject)go;

            ///goTarget.AddComponent<MogoShadowReceiver>();
            halo.transform.parent = goTarget.transform;
            halo.transform.localPosition = new Vector3(0, 0.1f, 0);
            halo.name = string.Concat("halo_", playerId);

            MogoFootHalo fh = halo.AddComponent<MogoFootHalo>();
            fh.Target = targetPos;

            m_listHalo.Add(halo);
        });
    }

    public void AblePointerToTarget()
    {
        if (m_pointer != null)
            m_pointer.SetActive(true);
    }

    public void DisatblePointerToTarget()
    {
        if (m_pointer != null)
            m_pointer.SetActive(false);
    }

    public void UpdatePointerToTarget(Vector3 targetPos)
    {

        //LoggerHelper.Debug("UpdatePointerToTarget " + targetPos);

        if (m_pointer == null)
        {

            if (!m_pointerLock)
            {
                m_pointerLock = true;
                AssetCacheMgr.GetUIInstance("fx_jiaodi.prefab", (prefab, id, go) =>
                {
                    m_pointerLock = false;
                    GameObject halo = (GameObject)go;
                    m_pointer = halo;

                    ///goTarget.AddComponent<MogoShadowReceiver>();
                    halo.transform.parent = MogoWorld.thePlayer.Transform;

                    halo.transform.localPosition = new Vector3(0, 0.1f, 0);
                    halo.name = string.Concat("halo_", MogoWorld.thePlayer.ID);

                    MogoFootHalo fh = halo.AddComponent<MogoFootHalo>();
                    fh.Target = targetPos;
                    //fh.RotateTo(0);
                    m_listHalo.Add(halo);
                });
            }

        }
        else
        {
            m_pointer.SetActive(true);
            MogoFootHalo fh = m_pointer.GetComponent<MogoFootHalo>();
            fh.Target = targetPos;
        }


    }

    public void ReleasePointerToTarget(uint playerId)
    {
        for (int i = 0; i < m_listHalo.Count; ++i)
        {
            if (m_listHalo[i] != null && m_listHalo[i].name == string.Concat("halo_", playerId.ToString()))
            {
                GameObject go = m_listHalo[i];

                AssetCacheMgr.ReleaseInstance(go);
                m_listHalo.Remove(go);

                break;
            }
        }
    }

    //public void ShowScreenSand(bool isShow)
    //{
    //    //GameObject.Find("BillboardPanel").transform.FindChild("SandFX").gameObject.SetActive(isShow);

    //    float fadeTime = UIFXData.dataMap.Get(1).fadetime;

    //    if (isShow)
    //    {
    //        GameObject.Find("BillboardPanel").GetComponentsInChildren<SandFX>(true)[0].Play(fadeTime);
    //    }
    //    else
    //    {
    //        GameObject.Find("BillboardPanel").GetComponentsInChildren<SandFX>(true)[0].Stop(fadeTime);
    //    }
    //}

    /// <summary>
    /// ������Ч,������Ч���Զ�����λ��
    /// </summary>
    /// <param name="fxId"></param>
    /// <param name="RelatedCamera">���յ����</param>
    /// <param name="xOffset"></param>
    /// <param name="yOffset"></param>
    /// <param name="zOffset"></param>
    /// <param name="widget"></param>
    /// <param name="fxName"></param>
    public void AttachUIFX(int fxId, Camera RelatedCamera, float xOffset = 0, float yOffset = 0, float zOffset = 0, GameObject widget = null, string fxName = "")
    {
        int logicType = UIFXData.dataMap[fxId].logicType;
        int renderType = UIFXData.dataMap[fxId].renderType;
        int programType = UIFXData.dataMap[fxId].programType;
        float fadeTime = UIFXData.dataMap[fxId].fadetime;
        float duration = UIFXData.dataMap[fxId].duration;
        string goName = UIFXData.dataMap[fxId].goName;

        if (fxName != "")
        {
            goName = fxName;
        }

        string fxPrefab = UIFXData.dataMap[fxId].fxPrefab;
        string attachedWidget = UIFXData.dataMap[fxId].attachedWidget;

        if (duration > 0)
        {
            TimerHeap.AddTimer((uint)(duration * 1000), 0, () => { DetachUIFX(fxId); });
        }

        Vector3 pos = Vector3.zero;
        GameObject parentTrans = null;
        LoggerHelper.Debug(fxPrefab + " !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! " + programType);

        if (logicType == 1)
        {
            if (widget == null)
            {
                parentTrans = GameObject.Find(attachedWidget);
            }
            else
            {
                parentTrans = widget;
            }

            if (programType == 1)
            {
                AssetCacheMgr.GetUIInstance(fxPrefab + ".prefab", (prefab, id, go) =>
                {
                    LoggerHelper.Debug(fxPrefab + " @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
                    GameObject obj = (GameObject)go;

                    obj.name = goName;

                    Vector3 scale = obj.transform.localScale;

                    obj.transform.parent = parentTrans.transform;

                    obj.AddComponent<SandFX>().scrollSpeed = 3f;

                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localScale = scale;

                    obj.SetActive(true);
                    obj.SetActive(false);

                    obj.GetComponentsInChildren<UIFXUnit>(true)[0].Play(fadeTime);

                    m_listUIParticle.Add(obj);
                });
            }
            else
            {
                LoggerHelper.Debug("Run Herereereereerererererererererereeer");
                if (duration == 0)
                {
                    LoggerHelper.Debug(parentTrans.name);
                    AttachParticleAnim(fxPrefab + ".prefab", 8000, parentTrans.transform.position, RelatedCamera, xOffset, yOffset, zOffset);
                }
                else if (duration == -1)
                {
                    if (parentTrans != null)
                    {
                        if (parentTrans.transform != null)
                        {
                            AttachParticleAnim(fxPrefab + ".prefab", goName, parentTrans.transform.position, RelatedCamera,
                                xOffset, yOffset, zOffset);
                        }
                    }

                }
                else
                {
                    AttachParticleAnim(fxPrefab + ".prefab", (uint)(duration * 1000f), parentTrans.transform.position, RelatedCamera);
                }
            }
        }
        else if (logicType == 2)
        {
            if (duration == -1)
            {
                AttachParticleAnim(fxPrefab + ".prefab", goName, widget, RelatedCamera, xOffset, yOffset, zOffset);
            }
        }
    }

    public void DetachUIFX(int fxId)
    {
        string goName = UIFXData.dataMap[fxId].goName;
        int logicType = UIFXData.dataMap[fxId].logicType;
        float fadeTime = UIFXData.dataMap[fxId].fadetime;

        if (logicType == 1)
        {
            LoggerHelper.Debug("DetachUIFX....................................");
            for (int i = 0; i < m_listUIParticle.Count; ++i)
            {
                if (m_listUIParticle[i] != null && m_listUIParticle[i].name == goName)
                {
                    if (m_listUIParticle[i].GetComponentsInChildren<UIFXUnit>(true).Length > 0)
                    {
                        m_listUIParticle[i].GetComponentsInChildren<UIFXUnit>(true)[0].Stop(fadeTime);
                        m_listUIParticle.RemoveAt(i);
                    }
                    else
                    {
                        AssetCacheMgr.ReleaseInstance(m_listUIParticle[i], false);
                        m_listUIParticle.RemoveAt(i);
                    }

                    //break;
                }
            }
        }
        else
        {
            DetachParticleAnim(goName);
        }

    }

    public void ShowUIFX(int fxId, bool isShow)
    {
        string goName = UIFXData.dataMap[fxId].goName;

        for (int i = 0; i < m_listUIParticle.Count; ++i)
        {
            if (m_listUIParticle[i] != null && m_listUIParticle[i].name == goName)
            {
                m_listUIParticle[i].SetActive(isShow);

                break;
            }
        }
    }

    /// <summary>
    /// ���ڸ�����Чλ��(��������ӳٺ�У��λ��)
    /// </summary>
    /// <param name="goFx"></param>
    /// <param name="uiWidgetPos"></param>
    /// <param name="RelatedCamera"></param>
    public void TransformToFXCameraPos(GameObject goFx, Vector3 uiWidgetPos, Camera RelatedCamera)
    {
        if (goFx)
        {
            Vector3 pos = RelatedCamera.WorldToScreenPoint(uiWidgetPos);
            pos = GetUIFXCamera().ScreenToWorldPoint(pos);
            goFx.transform.position = pos;
        }     
    }

    /// <summary>
    /// ��ͨ������Ч�ӿ�(û���Զ�����λ��)
    /// </summary>
    /// <param name="animName"></param>
    /// <param name="fxName"></param>
    /// <param name="uiWidgetPos"></param>
    /// <param name="RelatedCamera"></param>
    /// <param name="xOffset"></param>
    /// <param name="yOffset"></param>
    /// <param name="zOffset"></param>
    /// <param name="action"></param>
    public void AttachParticleAnim(string animName, string fxName, Vector3 uiWidgetPos, Camera RelatedCamera,
        float xOffset = 0, float yOffset = 0, float zOffset = 0, Action action = null)
    {
        Vector3 pos = RelatedCamera.WorldToScreenPoint(uiWidgetPos);
        pos = GetUIFXCamera().ScreenToWorldPoint(pos);
        UICamera uiMainCam = MogoUIManager.Instance.GetMainUICamera().GetComponentsInChildren<UICamera>(true)[0];
        //uiMainCam.enabled = false;
        //uiMainCam.eventReceiverMask = 0;
        TimerHeap.AddTimer(1000, 0, () => { uiMainCam.enabled = true; });

        AssetCacheMgr.GetInstance(animName, (prefab, id, go) =>
        {
            GameObject obj = (GameObject)go;

            obj.name = fxName;

            obj.transform.parent = GetUIFXRoot().transform;
            obj.transform.localScale = Vector3.one;

            obj.transform.position = pos;
            //LoggerHelper.Debug(obj.transform.localPosition);
            obj.transform.localPosition += new Vector3(xOffset, yOffset, zOffset);
            //LoggerHelper.Debug(obj.transform.localPosition);
            //MogoUIManager.Instance.GetMainUICamera().GetComponentsInChildren<UICamera>(true)[0].enabled = true;
            //MogoUIManager.Instance.GetMainUICamera().GetComponentsInChildren<UICamera>(true)[0].eventReceiverMask = 1 << 10;
            m_listUIParticle.Add(obj);

            if (action != null)
                action();
        });
    }

    public void AttachParticleAnim(string animName, uint duration, Vector3 uiWidgetPos, Camera RelatedCamera,
        float xOffset = 0, float yOffset = 0, float zOffset = 0)
    {
        Vector3 pos = RelatedCamera.WorldToScreenPoint(uiWidgetPos);
        pos = GetUIFXCamera().ScreenToWorldPoint(pos);

        UICamera uiMainCam = MogoUIManager.Instance.GetMainUICamera().GetComponentsInChildren<UICamera>(true)[0];
        //uiMainCam.enabled = false;
        //uiMainCam.eventReceiverMask = 0;
        TimerHeap.AddTimer(1000, 0, () => { uiMainCam.enabled = true; });

        LoggerHelper.Debug(uiWidgetPos);

        AssetCacheMgr.GetInstance(animName, duration, (prefab, id, go) =>
        {
            GameObject obj = (GameObject)go;

            obj.transform.parent = GetUIFXRoot().transform;
            obj.transform.localScale = Vector3.one;

            obj.transform.position = pos + new Vector3(xOffset, yOffset, zOffset);

            //MogoUIManager.Instance.GetMainUICamera().GetComponentsInChildren<UICamera>(true)[0].enabled = true;
            //MogoUIManager.Instance.GetMainUICamera().GetComponentsInChildren<UICamera>(true)[0].eventReceiverMask = 1 << 10;


        });
    }

    /// <summary>
    /// ��Ҫ��������ӿڣ�Only������
    /// </summary>
    /// <param name="animName"></param>
    /// <param name="fxName"></param>
    /// <param name="attchedWidget"></param>
    /// <param name="RelatedCamera"></param>
    /// <param name="xOffset"></param>
    /// <param name="yOffset"></param>
    /// <param name="zOffset"></param>
    /// <param name="action"></param>
    public void AttachParticleAnim(string animName, string fxName, GameObject attchedWidget, Camera RelatedCamera,
        float xOffset = 0, float yOffset = 0, float zOffset = 0, Action action = null)
    {
        UICamera uiMainCam = MogoUIManager.Instance.GetMainUICamera().GetComponentsInChildren<UICamera>(true)[0];
        //uiMainCam.enabled = false;
        //uiMainCam.eventReceiverMask = 0;
        //TimerHeap.AddTimer(1000, 0, () => { uiMainCam.enabled = true; });//�����true
        m_bUIFXAttached = true;

        m_goUIAttachedObj = attchedWidget;

        m_camUIAttachFX = RelatedCamera;

        Vector3 pos = RelatedCamera.WorldToScreenPoint(attchedWidget.transform.position);
        pos = GetUIFXCamera().ScreenToWorldPoint(pos);


        AssetCacheMgr.GetInstance(animName, (prefab, id, go) =>
        {
            GameObject obj = (GameObject)go;

            obj.name = fxName;

            obj.transform.parent = GetUIFXRoot().transform;
            obj.transform.localScale = Vector3.one;

            obj.transform.position = pos + new Vector3(xOffset, yOffset, zOffset);

            m_goUIAttachFX = obj;

            m_listUIParticle.Add(obj);
            //MogoUIManager.Instance.GetMainUICamera().GetComponentsInChildren<UICamera>(true)[0].enabled = true;
            //MogoUIManager.Instance.GetMainUICamera().GetComponentsInChildren<UICamera>(true)[0].eventReceiverMask = 1 << 10;

            if (action != null)
                action();
        });
    }

    public void DetachParticleAnim(string fxName = "")
    {
        for (int i = 0; i < m_listUIParticle.Count; ++i)
        {
            if (m_listUIParticle[i] == m_goUIAttachFX)
            {
                m_listUIParticle.RemoveAt(i);
                break;
            }
        }

        AssetCacheMgr.ReleaseInstance(m_goUIAttachFX);

        m_goUIAttachedObj = null;
        m_goUIAttachFX = null;

        m_bUIFXAttached = false;
    }

    public void ShowAllUIParticleAnim(bool isShow)
    {
        GetUIFXCamera().gameObject.SetActive(isShow);
    }

    public void ShowUIParticleAnim(string name, bool isShow)
    {
        for (int i = 0; i < m_listUIParticle.Count; ++i)
        {
            if (m_listUIParticle[i].name == name)
            {
                m_listUIParticle[i].SetActive(isShow);
                break;
            }
        }
    }

    public void ReleaseAllParticleAnim()
    {
        for (int i = 0; i < m_listUIParticle.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(m_listUIParticle[i].gameObject);
        }

        m_listUIParticle.Clear();
    }

    public void ReleaseParticleAnim(string name)
    {
        for (int i = 0; i < m_listUIParticle.Count; ++i)
        {
            if (m_listUIParticle[i] == null)
            {
                m_listUIParticle.Remove(m_listUIParticle[i]);
            }
            else
            {
                if (m_listUIParticle[i].name == name)
                {
                    AssetCacheMgr.ReleaseInstance(m_listUIParticle[i]);
                    m_listUIParticle.Remove(m_listUIParticle[i]);

                    break;
                }
            }
        }
    }

    /// <summary>
    /// ͨ�����ֲ�����Ч
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public GameObject FindParticeAnim(string name)
    {
        for (int i = 0; i < m_listUIParticle.Count; ++i)
        {
            if (m_listUIParticle[i].name == name)
            {
                return m_listUIParticle[i];
            }
        }

        return null;
    }

    public void Process()
    {
        if (m_bUIFXAttached)
        {
            if (m_goUIAttachedObj != null && m_goUIAttachFX != null)
            {
                Vector3 pos = m_camUIAttachFX.WorldToScreenPoint(m_goUIAttachedObj.transform.position);
                pos = GetUIFXCamera().ScreenToWorldPoint(pos);

                m_goUIAttachFX.transform.position = pos;
            }
        }

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    SceneCameraTurnGray();
        //}

        //if (Input.GetKeyDown(KeyCode.LeftAlt))
        //{
        //    SceneCameraTurnColor();
        //}

        UpdateEnemyInScreen();

    }

    public void SetObjShader(GameObject go, string shaderName, string color = "")
    {
        float r = 0, g = 0, b = 0;

        if (color != null && color.Contains(","))
        {
            string[] c = new string[3];
            c = color.Split(',');

            r = float.Parse(c[0]) / 255.0f;
            g = float.Parse(c[1]) / 255.0f;
            b = float.Parse(c[2]) / 255.0f;
        }

        AssetCacheMgr.GetUIResource(shaderName, (obj) =>
        {
            Material mat;
            if (go.GetComponentsInChildren<SkinnedMeshRenderer>(true).Length > 0)
            {
                mat = go.GetComponentsInChildren<SkinnedMeshRenderer>(true)[0].sharedMaterial;
            }
            else
            {
                mat = go.GetComponentsInChildren<MeshRenderer>(true)[0].sharedMaterial;
            }
            mat.shader = (Shader)obj;

            if (shaderName == "MogoFakeLight.shader")
            {
                mat.SetColor("_RimColor", new Color(r, g, b));
                mat.SetColor("_Color", Color.white);
                mat.SetFloat("_RimWidth", 0.05f);
                mat.SetFloat("_RimPower", 0.05f);
                mat.SetFloat("_FinalPower", 1.2f);
            }

        });
    }

    public void HandleUIFX(int id)
    {
        //switch (id)
        //{
        //    case 1:
        //        ShowScreenSand(true);
        //        break;
        //}

        AttachUIFX(id, GameObject.Find("Camera").GetComponent<Camera>());
    }


    public void AddEnemyInScreen(GameObject go, uint enemyID)
    {
        if (m_listEnemyObj.Contains(go))
            return;

        AssetCacheMgr.GetUIInstance("EnemyInScreen.prefab", (prefab, id, obj) =>
        {
            GameObject gameObj = (GameObject)obj;

            gameObj.name = enemyID.ToString();
            gameObj.transform.parent = BillboardPanel;
            gameObj.transform.localScale = Vector3.one;
            //LoggerHelper.Debug(Camera.mainCamera.name);
            gameObj.transform.position = Vector3.zero;

            m_listEnemyScreenObj.Add(gameObj);

            GameObject tmp = go;
            tmp.name = enemyID.ToString();

            m_listEnemyObj.Add(tmp);
        });
    }

    public void RemoveEnemyInScreen(uint id)//�˷������Ż�����һ�ִ�����ʽ��4��forѭ��̫������
    {
        //LoggerHelper.Debug("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ Trying to Remove EnenmyInScreen " + id); 
        //LoggerHelper.Debug("Befor Remove " + m_listEnemyObj.Count + " " + m_listEnemyScreenObj.Count);
        for (int i = 0; i < m_listEnemyObj.Count; ++i)
        {
            if (m_listEnemyObj[i] == null)
            {
                //LoggerHelper.Debug("m_listEnemyObj " + i + " is NULL");
                m_listEnemyObj.RemoveAt(i--);
            }
        }

        for (int i = 0; i < m_listEnemyScreenObj.Count; ++i)
        {
            if (m_listEnemyScreenObj[i] == null)
            {
                //LoggerHelper.Debug("m_listEnemyScreenObj " + i + " is NULL");
                m_listEnemyScreenObj.RemoveAt(i--);
            }
        }


        for (int i = 0; i < m_listEnemyScreenObj.Count; ++i)
        {
            if (m_listEnemyScreenObj[i].name == id.ToString())
            {
                //Debug.LogError("Release " + id.ToString());
                AssetCacheMgr.ReleaseInstance(m_listEnemyScreenObj[i]);
                m_listEnemyScreenObj.RemoveAt(i);

                //m_listEnemyObj.RemoveAt(i);
                break;
            }
        }

        for (int i = 0; i < m_listEnemyObj.Count; ++i)
        {
            if (m_listEnemyObj[i].name == id.ToString())
            {
                m_listEnemyObj.RemoveAt(i);

                break;
            }
        }
        //LoggerHelper.Debug("After Remove " + m_listEnemyObj.Count + " " + m_listEnemyScreenObj.Count);
    }


    public void HideEnemyInScreen(GameObject go)
    {
        if (m_listEnemyObj.Contains(go))
        {
            int index = m_listEnemyObj.IndexOf(go);

            m_listEnemyScreenObj[index].gameObject.SetActive(false);
        }
    }

    private float GetEnemyInScreenY(float x, Vector3 posEnemy, Vector3 posCenter)
    {
        float y = x * ((posEnemy.y - posCenter.y) / (posEnemy.x - posCenter.x)) +
            (posCenter.y * (posEnemy.x - posCenter.x) - posCenter.x * (posEnemy.y - posCenter.y)) / (posEnemy.x - posCenter.x);

        return y;
    }

    private float GetEnemyInScreenX(float y, Vector3 posEnemy, Vector3 posCenter)
    {
        float x = (y - (posCenter.y * (posEnemy.x - posCenter.x) - posCenter.x * (posEnemy.y - posCenter.y)) / (posEnemy.x - posCenter.x))
            * ((posEnemy.x - posCenter.x) / (posEnemy.y - posCenter.y));

        return x;
    }

    public void UpdateEnemyInScreen()//�˷������Ż����Ƕ�if elseû����
    {
        if (m_listEnemyObj.Count == 0 || GetBillboardCamera() == null)
            return;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        bool isBehideCamera = false;

        float angleZ;

        //LoggerHelper.Debug(m_listEnemyScreenObj.Count + " " + m_listEnemyObj.Count  );
        for (int i = 0; i < m_listEnemyScreenObj.Count; ++i)
        {
            //LoggerHelper.Debug(m_listEnemyScreenObj[i].name);
            if (m_listEnemyObj.Count > i && m_listEnemyObj[i] != null && m_listEnemyScreenObj[i] != null && Camera.main != null)
            {
                //LoggerHelper.Debug(m_listEnemyScreenObj[i].name + " In");
                Vector3 pos = Camera.main.WorldToScreenPoint(m_listEnemyObj[i].transform.position);
                Vector3 oriPos = Camera.main.WorldToScreenPoint(MogoWorld.thePlayer.Transform.position);

                if (pos.z < 0)
                {
                    isBehideCamera = true;
                }
                else
                {
                    isBehideCamera = false;
                }

                //LoggerHelper.Debug(m_listEnemyObj.Count + " " + m_listEnemyObj[i].name + " " + pos);

                if (pos.x < 0)
                {
                    if (pos.y > screenHeight)
                    {
                        if (isBehideCamera == false)
                        {
                            float y = GetEnemyInScreenY(0, pos, oriPos);

                            if (y > screenHeight)
                            {
                                float x = GetEnemyInScreenX(screenHeight, pos, oriPos);

                                m_listEnemyScreenObj[i].transform.position = GetBillboardCamera().ScreenToWorldPoint(new Vector3(x, screenHeight, pos.z));

                                if (!m_listEnemyScreenObj[i].activeSelf)
                                    m_listEnemyScreenObj[i].SetActive(true);

                                m_listEnemyScreenObj[i].transform.localEulerAngles = new Vector3(0, 0, 180);
                            }
                            else if (y < 0)
                            {
                                float x = GetEnemyInScreenX(0, pos, oriPos);

                                m_listEnemyScreenObj[i].transform.position = GetBillboardCamera().ScreenToWorldPoint(new Vector3(x, 0, pos.z));

                                if (!m_listEnemyScreenObj[i].activeSelf)
                                    m_listEnemyScreenObj[i].SetActive(true);

                                m_listEnemyScreenObj[i].transform.localEulerAngles = new Vector3(0, 0, 0);
                            }
                            else
                            {

                                m_listEnemyScreenObj[i].transform.position = GetBillboardCamera().ScreenToWorldPoint(new Vector3(0, y, pos.z));

                                if (!m_listEnemyScreenObj[i].activeSelf)
                                    m_listEnemyScreenObj[i].SetActive(true);


                                m_listEnemyScreenObj[i].transform.localEulerAngles = new Vector3(0, 0, -90);
                            }
                        }
                        else
                        {
                            float y = GetEnemyInScreenY(screenWidth, pos, oriPos);

                            if (y > screenHeight)
                            {
                                float x = GetEnemyInScreenX(screenHeight, pos, oriPos);

                                m_listEnemyScreenObj[i].transform.position = GetBillboardCamera().ScreenToWorldPoint(new Vector3(x, screenHeight, pos.z));

                                if (!m_listEnemyScreenObj[i].activeSelf)
                                    m_listEnemyScreenObj[i].SetActive(true);


                                m_listEnemyScreenObj[i].transform.localEulerAngles = new Vector3(0, 0, 180);

                            }
                            else if (y < 0)
                            {
                                float x = GetEnemyInScreenX(0, pos, oriPos);

                                m_listEnemyScreenObj[i].transform.position = GetBillboardCamera().ScreenToWorldPoint(new Vector3(x, 0, pos.z));

                                if (!m_listEnemyScreenObj[i].activeSelf)
                                    m_listEnemyScreenObj[i].SetActive(true);


                                m_listEnemyScreenObj[i].transform.localEulerAngles = new Vector3(0, 0, 0);
                            }
                            else
                            {

                                m_listEnemyScreenObj[i].transform.position = GetBillboardCamera().ScreenToWorldPoint(new Vector3(screenWidth, y, pos.z));

                                if (!m_listEnemyScreenObj[i].activeSelf)
                                    m_listEnemyScreenObj[i].SetActive(true);


                                m_listEnemyScreenObj[i].transform.localEulerAngles = new Vector3(0, 0, 90);
                            }
                        }

                    }
                    else if (0 < pos.y && pos.y < screenHeight)
                    {


                        float y = GetEnemyInScreenY(0, pos, oriPos);

                        m_listEnemyScreenObj[i].transform.position = GetBillboardCamera().ScreenToWorldPoint(new Vector3(0, y, pos.z));

                        if (!m_listEnemyScreenObj[i].activeSelf)
                            m_listEnemyScreenObj[i].SetActive(true);


                        m_listEnemyScreenObj[i].transform.localEulerAngles = new Vector3(0, 0, -90);

                    }
                    else
                    {
                        if (isBehideCamera == false)
                        {
                            float y = GetEnemyInScreenY(0, pos, oriPos);

                            if (y < 0)
                            {
                                float x = GetEnemyInScreenX(0, pos, oriPos);

                                m_listEnemyScreenObj[i].transform.position = GetBillboardCamera().ScreenToWorldPoint(new Vector3(x, 0, pos.z));

                                if (!m_listEnemyScreenObj[i].activeSelf)
                                    m_listEnemyScreenObj[i].SetActive(true);


                                m_listEnemyScreenObj[i].transform.localEulerAngles = new Vector3(0, 0, 0);

                            }
                            else if (y > screenHeight)
                            {
                                float x = GetEnemyInScreenX(screenHeight, pos, oriPos);

                                m_listEnemyScreenObj[i].transform.position = GetBillboardCamera().ScreenToWorldPoint(new Vector3(x, screenHeight, pos.z));

                                if (!m_listEnemyScreenObj[i].activeSelf)
                                    m_listEnemyScreenObj[i].SetActive(true);


                                m_listEnemyScreenObj[i].transform.localEulerAngles = new Vector3(0, 0, 180);

                            }
                            else
                            {

                                m_listEnemyScreenObj[i].transform.position = GetBillboardCamera().ScreenToWorldPoint(new Vector3(0, y, pos.z));

                                if (!m_listEnemyScreenObj[i].activeSelf)
                                    m_listEnemyScreenObj[i].SetActive(true);


                                m_listEnemyScreenObj[i].transform.localEulerAngles = new Vector3(0, 0, -90);
                            }
                        }
                        else
                        {
                            float y = GetEnemyInScreenY(screenWidth, pos, oriPos);

                            if (y > screenHeight)
                            {
                                float x = GetEnemyInScreenX(screenHeight, pos, oriPos);

                                m_listEnemyScreenObj[i].transform.position = GetBillboardCamera().ScreenToWorldPoint(new Vector3(x, screenHeight, pos.z));

                                if (!m_listEnemyScreenObj[i].activeSelf)
                                    m_listEnemyScreenObj[i].SetActive(true);


                                m_listEnemyScreenObj[i].transform.localEulerAngles = new Vector3(0, 0, 180);

                            }
                            else if (y < 0)
                            {
                                float x = GetEnemyInScreenX(0, pos, oriPos);

                                m_listEnemyScreenObj[i].transform.position = GetBillboardCamera().ScreenToWorldPoint(new Vector3(x, 0, pos.z));

                                if (!m_listEnemyScreenObj[i].activeSelf)
                                    m_listEnemyScreenObj[i].SetActive(true);


                                m_listEnemyScreenObj[i].transform.localEulerAngles = new Vector3(0, 0, 0);

                            }
                            else
                            {

                                m_listEnemyScreenObj[i].transform.position = GetBillboardCamera().ScreenToWorldPoint(new Vector3(screenWidth, y, pos.z));

                                if (!m_listEnemyScreenObj[i].activeSelf)
                                    m_listEnemyScreenObj[i].SetActive(true);


                                m_listEnemyScreenObj[i].transform.localEulerAngles = new Vector3(0, 0, 90);
                            }
                        }
                    }
                }
                else if (pos.x > screenWidth)
                {
                    if (pos.y > screenHeight)
                    {
                        if (isBehideCamera == false)
                        {
                            float y = GetEnemyInScreenY(screenWidth, pos, oriPos);

                            if (y > screenHeight)
                            {
                                float x = GetEnemyInScreenX(screenHeight, pos, oriPos);

                                m_listEnemyScreenObj[i].transform.position = GetBillboardCamera().ScreenToWorldPoint(new Vector3(x, screenHeight, pos.z));

                                if (!m_listEnemyScreenObj[i].activeSelf)
                                    m_listEnemyScreenObj[i].SetActive(true);


                                m_listEnemyScreenObj[i].transform.localEulerAngles = new Vector3(0, 0, 180);

                            }
                            else if (y < 0)
                            {
                                float x = GetEnemyInScreenX(0, pos, oriPos);

                                m_listEnemyScreenObj[i].transform.position = GetBillboardCamera().ScreenToWorldPoint(new Vector3(x, 0, pos.z));

                                if (!m_listEnemyScreenObj[i].activeSelf)
                                    m_listEnemyScreenObj[i].SetActive(true);

                                m_listEnemyScreenObj[i].transform.localEulerAngles = new Vector3(0, 0, 0);
                            }
                            else
                            {

                                m_listEnemyScreenObj[i].transform.position = GetBillboardCamera().ScreenToWorldPoint(new Vector3(screenWidth, y, pos.z));

                                if (!m_listEnemyScreenObj[i].activeSelf)
                                    m_listEnemyScreenObj[i].SetActive(true);



                                m_listEnemyScreenObj[i].transform.localEulerAngles = new Vector3(0, 0, 90);
                            }
                        }
                        else
                        {
                            float y = GetEnemyInScreenY(0, pos, oriPos);

                            if (y < 0)
                            {
                                float x = GetEnemyInScreenX(0, pos, oriPos);

                                m_listEnemyScreenObj[i].transform.position = GetBillboardCamera().ScreenToWorldPoint(new Vector3(x, 0, pos.z));

                                if (!m_listEnemyScreenObj[i].activeSelf)
                                    m_listEnemyScreenObj[i].SetActive(true);


                                m_listEnemyScreenObj[i].transform.localEulerAngles = new Vector3(0, 0, 0);

                            }
                            else if (y > screenHeight)
                            {
                                float x = GetEnemyInScreenX(screenHeight, pos, oriPos);

                                m_listEnemyScreenObj[i].transform.position = GetBillboardCamera().ScreenToWorldPoint(new Vector3(x, screenHeight, pos.z));

                                if (!m_listEnemyScreenObj[i].activeSelf)
                                    m_listEnemyScreenObj[i].SetActive(true);


                                m_listEnemyScreenObj[i].transform.localEulerAngles = new Vector3(0, 0, 180);

                            }
                            else
                            {

                                m_listEnemyScreenObj[i].transform.position = GetBillboardCamera().ScreenToWorldPoint(new Vector3(0, y, pos.z));

                                if (!m_listEnemyScreenObj[i].activeSelf)
                                    m_listEnemyScreenObj[i].SetActive(true);


                                m_listEnemyScreenObj[i].transform.localEulerAngles = new Vector3(0, 0, -90);
                            }
                        }
                    }
                    else if (0 < pos.y && pos.y < screenHeight)
                    {
                        float y = GetEnemyInScreenY(screenWidth, pos, oriPos);

                        m_listEnemyScreenObj[i].transform.position = GetBillboardCamera().ScreenToWorldPoint(new Vector3(screenWidth, y, pos.z));

                        if (!m_listEnemyScreenObj[i].activeSelf)
                            m_listEnemyScreenObj[i].SetActive(true);


                        m_listEnemyScreenObj[i].transform.localEulerAngles = new Vector3(0, 0, 90);
                    }
                    else
                    {
                        if (isBehideCamera == false)
                        {
                            float y = GetEnemyInScreenY(screenWidth, pos, oriPos);

                            if (y < 0)
                            {
                                float x = GetEnemyInScreenX(0, pos, oriPos);

                                m_listEnemyScreenObj[i].transform.position = GetBillboardCamera().ScreenToWorldPoint(new Vector3(x, 0, pos.z));

                                if (!m_listEnemyScreenObj[i].activeSelf)
                                    m_listEnemyScreenObj[i].SetActive(true);


                                m_listEnemyScreenObj[i].transform.localEulerAngles = new Vector3(0, 0, 0);

                            }
                            else if (y > screenHeight)
                            {
                                float x = GetEnemyInScreenX(screenHeight, pos, oriPos);

                                m_listEnemyScreenObj[i].transform.position = GetBillboardCamera().ScreenToWorldPoint(new Vector3(x, screenHeight, pos.z));

                                if (!m_listEnemyScreenObj[i].activeSelf)
                                    m_listEnemyScreenObj[i].SetActive(true);


                                m_listEnemyScreenObj[i].transform.localEulerAngles = new Vector3(0, 0, 180);

                            }
                            else
                            {

                                m_listEnemyScreenObj[i].transform.position = GetBillboardCamera().ScreenToWorldPoint(new Vector3(screenWidth, y, pos.z));

                                if (!m_listEnemyScreenObj[i].activeSelf)
                                    m_listEnemyScreenObj[i].SetActive(true);


                                m_listEnemyScreenObj[i].transform.localEulerAngles = new Vector3(0, 0, 90);
                            }
                        }
                        else
                        {
                            float y = GetEnemyInScreenY(0, pos, oriPos);

                            if (y < 0)
                            {
                                float x = GetEnemyInScreenX(0, pos, oriPos);

                                m_listEnemyScreenObj[i].transform.position = GetBillboardCamera().ScreenToWorldPoint(new Vector3(x, 0, pos.z));

                                if (!m_listEnemyScreenObj[i].activeSelf)
                                    m_listEnemyScreenObj[i].SetActive(true);


                                m_listEnemyScreenObj[i].transform.localEulerAngles = new Vector3(0, 0, 0);

                            }
                            else if (y > screenHeight)
                            {
                                float x = GetEnemyInScreenX(screenHeight, pos, oriPos);

                                m_listEnemyScreenObj[i].transform.position = GetBillboardCamera().ScreenToWorldPoint(new Vector3(x, screenHeight, pos.z));

                                if (!m_listEnemyScreenObj[i].activeSelf)
                                    m_listEnemyScreenObj[i].SetActive(true);


                                m_listEnemyScreenObj[i].transform.localEulerAngles = new Vector3(0, 0, 180);

                            }
                            else
                            {

                                m_listEnemyScreenObj[i].transform.position = GetBillboardCamera().ScreenToWorldPoint(new Vector3(0, y, pos.z));

                                if (!m_listEnemyScreenObj[i].activeSelf)
                                    m_listEnemyScreenObj[i].SetActive(true);


                                m_listEnemyScreenObj[i].transform.localEulerAngles = new Vector3(0, 0, -90);
                            }
                        }
                    }
                }
                else
                {
                    if (pos.y > screenHeight)
                    {
                        //float x = ((pos.y - oriPos.y) / (pos.x - oriPos.x)) * 0 + (oriPos.y * (pos.x - oriPos.x) - oriPos.x * (pos.y - oriPos.y)) / (pos.x - oriPos.x);
                        if (isBehideCamera == false)
                        {
                            float x = GetEnemyInScreenX(screenHeight, pos, oriPos);

                            m_listEnemyScreenObj[i].transform.position = GetBillboardCamera().ScreenToWorldPoint(new Vector3(x, screenHeight, pos.z));

                            if (!m_listEnemyScreenObj[i].activeSelf)
                                m_listEnemyScreenObj[i].SetActive(true);


                            m_listEnemyScreenObj[i].transform.localEulerAngles = new Vector3(0, 0, 180);
                        }
                        else
                        {
                            float x = GetEnemyInScreenX(0, pos, oriPos);

                            m_listEnemyScreenObj[i].transform.position = GetBillboardCamera().ScreenToWorldPoint(new Vector3(x, 0, pos.z));

                            if (!m_listEnemyScreenObj[i].activeSelf)
                                m_listEnemyScreenObj[i].SetActive(true);


                            m_listEnemyScreenObj[i].transform.localEulerAngles = new Vector3(0, 0, 0);
                        }
                    }
                    else if (0 < pos.y && pos.y < screenHeight)
                    {
                        //LoggerHelper.Debug(m_listEnemyScreenObj[i].name + " isFalse");
                        if (m_listEnemyScreenObj[i].activeSelf)
                            m_listEnemyScreenObj[i].SetActive(false);
                    }
                    else
                    {
                        if (isBehideCamera == false)
                        {
                            float x = GetEnemyInScreenX(0, pos, oriPos);

                            m_listEnemyScreenObj[i].transform.position = GetBillboardCamera().ScreenToWorldPoint(new Vector3(x, 0, pos.z));

                            if (!m_listEnemyScreenObj[i].activeSelf)
                                m_listEnemyScreenObj[i].SetActive(true);


                            m_listEnemyScreenObj[i].transform.localEulerAngles = new Vector3(0, 0, 0);
                        }
                        else
                        {
                            float x = GetEnemyInScreenX(screenHeight, pos, oriPos);

                            m_listEnemyScreenObj[i].transform.position = GetBillboardCamera().ScreenToWorldPoint(new Vector3(x, screenHeight, pos.z));

                            if (!m_listEnemyScreenObj[i].activeSelf)
                                m_listEnemyScreenObj[i].SetActive(true);


                            m_listEnemyScreenObj[i].transform.localEulerAngles = new Vector3(0, 0, 180);
                        }
                    }
                }
            }

            angleZ = m_listEnemyScreenObj[i].transform.localEulerAngles.z;
            switch ((int)angleZ)
            {
                case 0:
                    m_listEnemyScreenObj[i].transform.localPosition += new Vector3(0, 17, 0);
                    break;

                case 180:
                    m_listEnemyScreenObj[i].transform.localPosition += new Vector3(0, -17, 0);
                    break;

                case -90:
                case 270:
                    m_listEnemyScreenObj[i].transform.localPosition += new Vector3(17, 0, 0);
                    break;

                case 90:
                    m_listEnemyScreenObj[i].transform.localPosition += new Vector3(-17, 0, 0);
                    break;
            }
        }
    }

    public void AttachUpgrateFX()
    {
        AssetCacheMgr.GetUIInstance("fx_renwu_shengji.prefab", (prefab, id, obj) =>
        {
            GameObject go = (GameObject)obj;
            go.transform.parent = MogoWorld.thePlayer.GameObject.transform.Find("slot_billboard");//�˷������Ż�����slot_billboard��ȡ�����ŵ�EntityMySelf��
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;

            TimerHeap.AddTimer<GameObject, bool>(5000, 0, AssetCacheMgr.ReleaseInstance, go, true);
        }
        );
    }

    public void AttachTaskOverFX()
    {
        AssetCacheMgr.GetUIInstance("fx_renwu_wancheng.prefab", (prefab, id, obj) =>
        {
            GameObject go = (GameObject)obj;
            go.transform.parent = MogoWorld.thePlayer.GameObject.transform.Find("slot_billboard");//�˷������Ż�����slot_billboard��ȡ�����ŵ�EntityMySelf��
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;

            TimerHeap.AddTimer<GameObject, bool>(5000, 0, AssetCacheMgr.ReleaseInstance, go, true);
        }
        );
    }

    public void SceneCameraTurnGray()
    {
        if (Camera.main == null)
            return;

        if (SystemConfig.Instance.GraphicQuality < 2)
            return;

        if (Camera.main.GetComponent<MogoGray>() == null)
        {
            AssetCacheMgr.GetUIResource("MogoGray.mat", (obj) =>
            {
                Camera.main.gameObject.AddComponent<MogoGray>().Mat = (Material)obj;
                Camera.main.GetComponent<MogoGray>().enabled = true;
            });
        }
        else
        {
            Camera.main.GetComponent<MogoGray>().enabled = true;
        }


    }

    public void SceneCameraTurnColor()
    {
        if (Camera.main == null)
            return;

        if (SystemConfig.Instance.GraphicQuality < 2)
            return;

        if (Camera.main.GetComponent<MogoGray>() == null)
            return;

        Camera.main.GetComponent<MogoGray>().enabled = false;
    }

    public void SetUIFXCameraEnable(bool isEnable)
    {
        if (m_camUIFX != null)
        {
            m_camUIFX.gameObject.SetActive(isEnable);
        }
    }

}

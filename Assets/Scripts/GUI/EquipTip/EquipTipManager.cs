// ģ����   :  EquipTipManager
// ������   :  Ī׿��
// �������� :  2012-6-24
// ��    �� :  װ��tipȫ�ֶ�̬�ӿ�

using UnityEngine;
using System.Collections.Generic;
using System;
using Mogo.Util;
using Mogo.GameData;

public class EquipTipManager : MonoBehaviour
{
    public static EquipTipManager Instance;

    private Dictionary<string, string> m_widgetToFullName = new Dictionary<string, string>();

    private GameObject m_equipTip;
    private GameObject m_equipTipDetail;
    //private SimpleDragCamera m_equipTipCamera;
    private UILabel m_equipDetailName;
    private UILabel m_equipDetailNeedLevel;
    private UILabel m_equipDetailGrowLevel;
    private UISlicedSprite m_equipDetailImageBG;
    private UILabel m_equipDetailLblEquip;
    private UILabel m_equipDetailNeedJob;
    private UILabel m_equipDetailExtra;
    private UISlicedSprite m_equipDetailImageFG;
    private UISlicedSprite m_equipDetailImageUsed;
    private UILabel m_equipDetailNeedLevellbl;

    private UILabel m_equipDetailNeedLevellblCurrent;
    private GameObject m_equipTipCurrent;
    private GameObject m_equipTipDetailCurrent;
    private MyDragableCamera m_equipTipCameraCurrent;
    private UILabel m_equipDetailNameCurrent;
    private UILabel m_equipDetailNeedLevelCurrent;
    private UILabel m_equipDetailGrowLevelCurrent;
    private UISlicedSprite m_equipDetailImageBGCurrent;
    private UILabel m_equipDetailLblEquipCurrent;
    private UILabel m_equipDetailNeedJobCurrent;
    private UILabel m_equipDetailExtraCurrent;
    private UISlicedSprite m_equipDetailImageFGCurrent;
    private UISlicedSprite m_equipDetailImageUsedCurrent;

    // װ��ս����
    private GameObject m_goGOEquipTipDetailScore;
    private UILabel m_lblEquipTipDetailScoreNum;
    private GameObject m_goGOEquipTipCurrentDetailScore;
    private UILabel m_lblEquipTipCurrentDetailScoreNum;

    Transform center;
    Transform right;
    // Ϊȥ��������ʱ�������´���
    //private int fontSize = 22;
    private UILabel m_despLbl;
    private UILabel DespLbl
    {
        get
        {

            return m_despLbl;
        }
    }

    //Ԥ����
    private Dictionary<string, Queue<GameObject>> m_prefabDic;

    void Awake()
    {
        Instance = GetComponentsInChildren<EquipTipManager>(true)[0];

        FillFullNameData(transform);

        m_equipTip = transform.Find(m_widgetToFullName["EquipTip"]).gameObject;
        m_equipTipDetail = transform.Find(m_widgetToFullName["EquipTipDetail3"]).gameObject;
        m_equipDetailName = transform.Find(m_widgetToFullName["EquipTipNameText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipDetailNeedLevel = transform.Find(m_widgetToFullName["EquipTipDetailNeedLevelNum"]).GetComponentsInChildren<UILabel>(true)[0];
        //m_equipDetailGrowLevel = m_myTransform.FindChild(m_widgetToFullName["EquipTipDetailGrowLevelNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipDetailNeedJob = transform.Find(m_widgetToFullName["EquipTipDetailNeedJobText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipDetailExtra = transform.Find(m_widgetToFullName["EquipTipDetailExtraText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipDetailImageFG = transform.Find(m_widgetToFullName["EquipTipDetailImageFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        m_equipDetailImageBG = transform.Find(m_widgetToFullName["EquipTipDetailImageBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        m_equipDetailImageUsed = transform.Find(m_widgetToFullName["EquipTipDetailImageUsed"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        m_equipTipCamera = transform.Find(m_widgetToFullName["EquipTipDetailCamera"]).GetComponentsInChildren<MyDragableCamera>(true)[0];
        m_equipTipCamera.GetComponent<UIViewport>().sourceCamera = GameObject.Find("GlobleUICamera").GetComponent<Camera>();
        m_equipTipCameraPosBegin = transform.Find(m_widgetToFullName["EquipTipDetailBegin"]);
        m_equipTipCameraArea = transform.Find(m_widgetToFullName["EquipTipDetailInfoBG"]);
        m_equipDetailNeedLevellbl = transform.Find(m_widgetToFullName["EquipTipDetailNeedLevelText"]).GetComponentsInChildren<UILabel>(true)[0];


        m_equipTipCurrent = transform.Find(m_widgetToFullName["EquipTipCurrent"]).gameObject;
        m_equipDetailNeedLevellblCurrent = transform.Find(m_widgetToFullName["EquipTipCurrentDetailNeedLevelText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipTipDetailCurrent = transform.Find(m_widgetToFullName["EquipTipCurrentDetail3"]).gameObject;
        m_equipDetailNameCurrent = transform.Find(m_widgetToFullName["EquipTipCurrentNameText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipDetailNeedLevelCurrent = transform.Find(m_widgetToFullName["EquipTipCurrentDetailNeedLevelNum"]).GetComponentsInChildren<UILabel>(true)[0];
        //m_equipDetailGrowLevel = m_myTransform.FindChild(m_widgetToFullName["EquipTipDetailGrowLevelNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipDetailNeedJobCurrent = transform.Find(m_widgetToFullName["EquipTipCurrentDetailNeedJobText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipDetailExtraCurrent = transform.Find(m_widgetToFullName["EquipTipCurrentDetailExtraText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipDetailImageFGCurrent = transform.Find(m_widgetToFullName["EquipTipCurrentDetailImageFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        m_equipDetailImageBGCurrent = transform.Find(m_widgetToFullName["EquipTipCurrentDetailImageBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        m_equipDetailImageUsedCurrent = transform.Find(m_widgetToFullName["EquipTipCurrentDetailImageUsed"]).GetComponentsInChildren<UISlicedSprite>(true)[0];

        m_equipTipCameraCurrent = transform.Find(m_widgetToFullName["EquipTipCurrentDetailCamera"]).GetComponentsInChildren<MyDragableCamera>(true)[0];
        m_equipTipCameraCurrent.GetComponent<UIViewport>().sourceCamera = GameObject.Find("GlobleUICamera").GetComponent<Camera>();
        m_equipTipCameraPosBeginCurrent = transform.Find(m_widgetToFullName["EquipTipCurrentDetailBegin"]);
        m_equipTipCameraAreaCurrent = transform.Find(m_widgetToFullName["EquipTipCurrentDetailInfoBG"]);

        center = transform.Find(m_widgetToFullName["Center"]);
        right = transform.Find(m_widgetToFullName["Right"]);


        //if (m_equipTipCurrent == null) Mogo.Util.LoggerHelper.Debug("fuck!");


        transform.Find(m_widgetToFullName["EquipTipClose"]).gameObject.AddComponent<EquipTipClose>();

        m_goGOEquipTipDetailScore = transform.Find(m_widgetToFullName["GOEquipTipDetailScore"]).gameObject;
        m_lblEquipTipDetailScoreNum = transform.Find(m_widgetToFullName["EquipTipDetailScoreNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_goGOEquipTipCurrentDetailScore = transform.Find(m_widgetToFullName["GOEquipTipCurrentDetailScore"]).gameObject;
        m_lblEquipTipCurrentDetailScoreNum = transform.Find(m_widgetToFullName["EquipTipCurrentDetailScoreNum"]).GetComponentsInChildren<UILabel>(true)[0];


        //Ԥ����
        InitPrefabDic();

        this.gameObject.SetActive(false);
        m_equipTipCurrent.SetActive(false);

    }

    const string EquipTipSuitNameTitle = "EquipTipSuitNameTitle.prefab";
    const string EquipTipSuitAttr = "EquipTipSuitAttr.prefab";
    const string PackageEquipInfoAttr = "PackageEquipInfoAttr.prefab";
    const string PackageEquipInfoDiamon = "PackageEquipInfoDiamon.prefab";
    const string PropInfoDetailEffectNum = "PropInfoDetailEffectNum.prefab";
    const string PropInfoDetailStack = "PropInfoDetailStack.prefab";
    const string PropInfoDetailPrice = "PropInfoDetailPrice.prefab";
    const string MaterialInfoDetailLevel = "MaterialInfoDetailLevel.prefab";
    const string MaterialInfoDetailType = "MaterialInfoDetailType.prefab";
    const string EquipTipDetailBtn = "EquipTipDetailBtn.prefab";
    const string FumoTitle = "PackageEquipInfoEnhantTitle.prefab";
    const string FumoAttr = "PackageEquipInfoEnhant.prefab";

    private void AddPrefab(string prefabName, int num)
    {
        if (!m_prefabDic.ContainsKey(prefabName)) m_prefabDic.Add(prefabName, new Queue<GameObject>());
        for (int i = 0; i < num; i++)
        {
            AssetCacheMgr.GetUIInstance(prefabName, (str, id, obj) =>
            {
                GameObject go = obj as GameObject;
                m_prefabDic[prefabName].Enqueue(go);
                //Debug.LogError("Enqueue:" + prefabName);
                Utils.MountToSomeObjWithoutPosChange(go.transform, center);
                go.name = prefabName.Split('.')[0];
                go.SetActive(false);

            });
        }
    }
    private void RecyclePrefab(string name, GameObject go)
    {
        go.SetActive(false);
        //Debug.LogError("Enqueue:" + name);
        m_prefabDic[name].Enqueue(go);
    }

    private void ReleaseInstance()
    {
        foreach (GameObject go in gos)
        {
            RecyclePrefab(go.name + ".prefab", go);
        }
        foreach (GameObject go in btnGos)
        {
            go.name = EquipTipDetailBtn.Split('.')[0];
            RecyclePrefab(EquipTipDetailBtn, go);
        }
        gos.Clear();
        btnGos.Clear();
    }

    private void InitPrefabDic()
    {
        m_prefabDic = new Dictionary<string, Queue<GameObject>>();

        AddPrefab(EquipTipSuitNameTitle, 2);
        AddPrefab(EquipTipSuitAttr, 10);
        AddPrefab(PackageEquipInfoAttr, 20);
        AddPrefab(PackageEquipInfoDiamon, 10);
        AddPrefab(PropInfoDetailEffectNum, 1);
        AddPrefab(PropInfoDetailStack, 1);
        AddPrefab(PropInfoDetailPrice, 1);
        AddPrefab(MaterialInfoDetailLevel, 1);
        AddPrefab(MaterialInfoDetailType, 1);
        AddPrefab(EquipTipDetailBtn, 5);
        AddPrefab(FumoTitle, 2);
        AddPrefab(FumoAttr, 10);

        if (m_despLbl == null)
        {
            AssetCacheMgr.GetUIInstance(PropInfoDetailEffectNum,
             (prefab, guid, gameObject) =>
             {
                 GameObject go = gameObject as GameObject;
                 Transform root = transform.Find(m_widgetToFullName["EquipTipDetailList"]);
                 Utils.MountToSomeObjWithoutPosChange(go.transform, root);
                 UILabel lable = go.transform.GetComponentsInChildren<UILabel>(true)[0];

                 m_despLbl = lable;
                 go.SetActive(false);
             }
             );
        }
    }

    UnityEngine.Object GetChildObj<T>(string name) where T : UnityEngine.Component
    {
        return transform.Find(m_widgetToFullName[name]).GetComponentsInChildren<T>(true)[0];
    }

    private void FillFullNameData(Transform rootTransform)
    {
        for (int i = 0; i < rootTransform.GetChildCount(); ++i)
        {
            m_widgetToFullName.Add(rootTransform.GetChild(i).name, Utils.GetFullName(transform, rootTransform.GetChild(i)));
            FillFullNameData(rootTransform.GetChild(i));
        }
    }

    private void GetInstance(string prefabName, Action<GameObject> onLoaded)
    {
        if (m_prefabDic.ContainsKey(prefabName) && m_prefabDic[prefabName].Count > 0)
        {

            GameObject go = m_prefabDic[prefabName].Dequeue();
            //Debug.LogError("Dequeue:" + prefabName);
            go.SetActive(true);
            onLoaded(go);
        }
        else
        {
            AssetCacheMgr.GetUIInstance(prefabName,
           (prefab, guid, gameObject) =>
           {
               //Debug.LogError("prefabNAme:" + prefabName);
               (gameObject as GameObject).name = prefabName.Split('.')[0];
               onLoaded(gameObject as GameObject);
           });
        }
    }

    public const float GAP = 18;
    public const float BTN_GAP = 70;
    private List<GameObject> gos = new List<GameObject>();
    private List<GameObject> btnGos = new List<GameObject>();
    private MyDragableCamera m_equipTipCamera;
    private Transform m_equipTipCameraPosBegin;
    private Transform m_equipTipCameraArea;
    private Transform m_equipTipCameraAreaCurrent;
    private Transform m_equipTipCameraPosBeginCurrent;

    public void ShowEquipTip
        (string suitName, List<string> suitAttr, List<string> attrs, List<string> jewels,
        List<string> slotIcons, List<ButtonInfo> buttonList, FumoTipUIInfo fumoInfo = null)
    {
        if (this.gameObject.activeSelf && !m_equipTipCurrent.activeSelf) ReleaseInstance();
        //Debug.LogError("ShowEquipTip");
        EventDispatcher.TriggerEvent(SettingEvent.UIUpPlaySound, "ItemTip");

        float attrGap = 10;
        float gap = -12;

        Transform root = transform.Find(m_widgetToFullName["EquipTipDetailList"]);
        //����+�Ű�
        int i = 0;
        //����
        //�����װ��
        if (suitName != "")
        {
            var _gap = gap;
            GetInstance("EquipTipSuitNameTitle.prefab",
            (gameObject) =>
            {
                GameObject go = gameObject as GameObject;
                UILabel lable = go.transform.Find("EquipTipSuitName").GetComponent<UILabel>();


                Vector3 scale = go.transform.localScale;
                Vector3 position = go.transform.localPosition;
                Vector3 angle = go.transform.localEulerAngles;
                go.transform.parent = root;
                go.transform.localScale = scale;
                go.transform.localEulerAngles = angle;
                go.transform.localPosition = new Vector3(0, _gap, 0);
                gos.Add(go);

                lable.text = suitName;
            }
            );
            gap -= GAP + 22;

            //Debug.LogError(gap);
        }

        //�����װ����
        if (suitAttr != null)
        {
            for (i = 0; i < suitAttr.Count; i++)
            {
                var index = i;
                var _gap = gap;
                GetInstance("EquipTipSuitAttr.prefab",
                (gameObject) =>
                {
                    //PackageEquipInfoDiamonHole0Text
                    GameObject go = gameObject as GameObject;
                    UILabel lable = go.transform.GetComponent<UILabel>();


                    Utils.MountToSomeObjWithoutPosChange(go.transform, root);
                    //Vector3 scale = go.transform.localScale;
                    Vector3 position = go.transform.localPosition;
                    //Vector3 angle = go.transform.localEulerAngles;
                    //go.transform.parent = root;
                    //go.transform.localScale = scale;
                    //go.transform.localEulerAngles = angle;
                    go.transform.localPosition = new Vector3(position.x, _gap - index * (attrGap + 22), 0);
                    gos.Add(go);

                    lable.text = suitAttr[index];
                }
                );
            }
            if (suitAttr.Count > 0)
            {
                gap -= GAP + (i * attrGap + 22);
            }



            //Debug.LogError(gap);
        }

        //Debug.LogError(gap);
        for (i = 0; i < attrs.Count; i++)
        {
            var index = i;
            var _gap = gap;
            GetInstance("PackageEquipInfoAttr.prefab",
             (gameObject) =>
             {
                 //PackageEquipInfoDiamonHole0Text
                 GameObject go = gameObject as GameObject;
                 UILabel lable = go.transform.Find("PackageEquipInfoDiamonHole0Text").GetComponent<UILabel>();

                 Vector3 scale = go.transform.localScale;
                 Vector3 position = go.transform.localPosition;
                 Vector3 angle = go.transform.localEulerAngles;
                 go.transform.parent = root;
                 go.transform.localScale = scale;
                 go.transform.localEulerAngles = angle;
                 go.transform.localPosition = new Vector3(0, _gap - index * (attrGap + 22), 0);
                 gos.Add(go);

                 lable.text = attrs[index];
             }
             );
        }
        gap -= GAP + i * (attrGap + 22);


        // ��ħ
        if (fumoInfo != null)
        {
            var _gap = gap;
            //����
            GetInstance(FumoTitle,
             (gameObject) =>
             {
                 //PackageEquipInfoDiamonHole0Text
                 GameObject go = gameObject as GameObject;
                 UILabel lable = go.transform.Find("PackageEquipInfoEnhantTitleText").GetComponent<UILabel>();
                 lable.text = fumoInfo.fumoTitle;
                 Utils.MountToSomeObjWithoutPosChange(go.transform, root);

                 go.transform.localPosition = new Vector3(0, _gap, 0);
                 gos.Add(go);
             }
             );
            gap -= GAP + 22;

            var _gap2 = gap;
            for (i = 0; i < fumoInfo.fomoDesp.Count; i++)
            {
                int index = i;
                GetInstance(FumoAttr,
                (gameObject) =>
                {
                    //PackageEquipInfoDiamonHole0Text
                    GameObject go = gameObject as GameObject;
                    UILabel lable = go.transform.Find("PackageEquipInfoEnhantText").GetComponent<UILabel>();
                    lable.text = fumoInfo.fomoDesp[index];
                    Utils.MountToSomeObjWithoutPosChange(go.transform, root);

                    go.transform.localPosition = new Vector3(0, _gap2 - index * (attrGap + 22), 0);
                    gos.Add(go);
                }
                );
            }
            gap -= GAP + i * (attrGap + 22);
        }


        //��ʯ
        for (i = 0; i < jewels.Count; i++)
        {
            float _gap = gap;
            var index = i;
            GetInstance("PackageEquipInfoDiamon.prefab",
            (gameObject) =>
            {
                //PackageEquipInfoDiamonHole0Text
                GameObject go = gameObject as GameObject;
                UILabel lable = go.transform.Find("PackageEquipInfoDiamonHole12Text").GetComponent<UILabel>();
                UISprite sprite = go.transform.Find("PackageEquipInfoDiamonHole12FG").GetComponent<UISprite>();


                Vector3 scale = go.transform.localScale;
                Vector3 position = go.transform.localPosition;
                Vector3 angle = go.transform.localEulerAngles;
                go.transform.parent = root;
                go.transform.localScale = scale;
                go.transform.localEulerAngles = angle;
                go.transform.localPosition = new Vector3(0, _gap - index * (attrGap + 22), 0);
                gos.Add(go);

                lable.text = jewels[index];
                //if (lable.text.Equals(LanguageData.GetContent(910)))
                //{
                sprite.spriteName = slotIcons[index];
                //}
            }
            );
        }
        gap -= GAP + i * (attrGap + 22);

        //����ȼ���
        Transform detail3 = transform.Find(m_widgetToFullName["EquipTipDetail3"]);
        detail3.gameObject.SetActive(true);
        detail3.localPosition = new Vector3(0, gap, 0);
        //m_equipDetailNeedJob.GetComponent<UILabel>().text = LanguageData.GetContent(912, vocation);

        gap -= 56;

        //m_equipTipCamera.height = gap - 400;
        //m_equipTipCamera.Reset();
        //show
        float minY;
        minY = -gap + 3 - m_equipTipCameraArea.localScale.y;//;
        //Debug.LogError("m_equipTipCamera.camera.pixelRect.height:" + m_equipTipCamera.camera.pixelRect.height);
        //Debug.LogError("m_equipTipCameraArea.localScale.y:" + m_equipTipCameraArea.localScale.y);
        minY = minY > 0 ? minY : 0;
        m_equipTipCamera.MINY = m_equipTipCameraPosBegin.localPosition.y - minY;
        m_equipTipCamera.transform.localPosition = m_equipTipCameraPosBegin.localPosition;

        InitButtonList(buttonList);

        this.gameObject.SetActive(true);

    }

    public void ShowEquipTipCurrent
       (string suitName, List<string> suitAttr, List<string> attrs, List<string> jewels, List<string> slotIcons, string vocation, List<ButtonInfo> buttonList)
    {
        ReleaseInstance();
        m_equipTipCurrent.SetActive(true);
        m_equipTip.transform.localPosition = right.localPosition;
        float gap = -12;
        float attrGap = 10;
        Transform root = transform.Find(m_widgetToFullName["EquipTipCurrentDetailList"]);
        //����+�Ű�

        int i = 0;
        //����

        //�����װ��
        if (suitName != "")
        {
            var _gap = gap;
            GetInstance("EquipTipSuitNameTitle.prefab",
            (gameObject) =>
            {
                GameObject go = gameObject as GameObject;
                UILabel lable = go.transform.Find("EquipTipSuitName").GetComponent<UILabel>();


                Vector3 scale = go.transform.localScale;
                Vector3 position = go.transform.localPosition;
                Vector3 angle = go.transform.localEulerAngles;
                go.transform.parent = root;
                go.transform.localScale = scale;
                go.transform.localEulerAngles = angle;
                go.transform.localPosition = new Vector3(0, _gap, 0);
                gos.Add(go);

                lable.text = suitName;
            }
            );

            gap -= GAP + 22;

            //var _gap = gap;
            //GetInstance("EquipTipSuitName.prefab",
            //(gameObject) =>
            //{
            //    GameObject go = gameObject as GameObject;
            //    UILabel lable = go.transform.GetComponent<UILabel>();


            //    Vector3 scale = go.transform.localScale;
            //    Vector3 position = go.transform.localPosition;
            //    Vector3 angle = go.transform.localEulerAngles;
            //    go.transform.parent = root;
            //    go.transform.localScale = scale;
            //    go.transform.localEulerAngles = angle;
            //    go.transform.localPosition = new Vector3(position.x, originalY + _gap, position.z);
            //    gos.Add(go);

            //    lable.text = suitName;
            //}
            //);

            //gap -= GAP;
        }

        //�����װ����
        if (suitAttr != null)
        {
            for (i = 0; i < suitAttr.Count; i++)
            {
                var index = i;
                var _gap = gap;
                GetInstance("EquipTipSuitAttr.prefab",
                (gameObject) =>
                {
                    //PackageEquipInfoDiamonHole0Text
                    GameObject go = gameObject as GameObject;
                    UILabel lable = go.transform.GetComponent<UILabel>();


                    Vector3 scale = go.transform.localScale;
                    Vector3 position = go.transform.localPosition;
                    Vector3 angle = go.transform.localEulerAngles;
                    go.transform.parent = root;
                    go.transform.localScale = scale;
                    go.transform.localEulerAngles = angle;
                    go.transform.localPosition = new Vector3(0, _gap - (index * attrGap + 22), 0);
                    gos.Add(go);

                    lable.text = suitAttr[index];
                }
                );
            }
            if (suitAttr.Count > 0)
                gap -= GAP + i * (attrGap + 22);

        }

        for (i = 0; i < attrs.Count; i++)
        {
            var index = i;
            var _gap = gap;
            GetInstance("PackageEquipInfoAttr.prefab",
            (gameObject) =>
            {

                //PackageEquipInfoDiamonHole0Text
                GameObject go = gameObject as GameObject;
                UILabel lable = go.transform.Find("PackageEquipInfoDiamonHole0Text").GetComponent<UILabel>();


                Vector3 scale = go.transform.localScale;
                Vector3 position = go.transform.localPosition;
                Vector3 angle = go.transform.localEulerAngles;
                go.transform.parent = root;
                go.transform.localScale = scale;
                go.transform.localEulerAngles = angle;
                go.transform.localPosition = new Vector3(position.x, _gap - index * (attrGap + 22), position.z);
                gos.Add(go);

                lable.text = attrs[index];
            }
            );
        }
        gap -= GAP + i * (attrGap + 22);

        //��ʯ
        for (i = 0; i < jewels.Count; i++)
        {
            float _gap = gap;
            var index = i;
            GetInstance("PackageEquipInfoDiamon.prefab",
            (gameObject) =>
            {
                //PackageEquipInfoDiamonHole0Text
                GameObject go = gameObject as GameObject;
                UILabel lable = go.transform.Find("PackageEquipInfoDiamonHole12Text").GetComponent<UILabel>();
                UISprite sprite = go.transform.Find("PackageEquipInfoDiamonHole12FG").GetComponent<UISprite>();


                //if (lable.text.Equals(LanguageData.GetContent(910)))
                //{
                //    sprite.gameObject.SetActive(false);
                //}
                Vector3 scale = go.transform.localScale;
                Vector3 position = go.transform.localPosition;
                Vector3 angle = go.transform.localEulerAngles;
                go.transform.parent = root;
                go.transform.localScale = scale;
                go.transform.localEulerAngles = angle;
                go.transform.localPosition = new Vector3(0, _gap + -index * (attrGap + 22), 0);
                gos.Add(go);

                lable.text = jewels[index];
                sprite.spriteName = slotIcons[index];
            }
            );
        }
        gap -= GAP + i * (attrGap + 22);

        //����ȼ���
        Transform detail3 = m_equipTipDetailCurrent.transform;
        detail3.gameObject.SetActive(true);
        detail3.localPosition = new Vector3(0, gap, 0);
        m_equipDetailNeedJobCurrent.GetComponent<UILabel>().text = LanguageData.GetContent(912, vocation);

        gap -= 56;

        //show
        float minY;
        minY = -gap + 3 - m_equipTipCameraAreaCurrent.localScale.y;
        minY = minY > 0 ? minY : 0;
        m_equipTipCameraCurrent.MINY = m_equipTipCameraPosBeginCurrent.localPosition.y - minY;
        m_equipTipCameraCurrent.transform.localPosition = m_equipTipCameraPosBeginCurrent.localPosition;

        InitButtonList(buttonList);

        this.gameObject.SetActive(true);
    }

    public void SetVocationNeedText(string str)
    {
        m_equipDetailNeedJob.GetComponent<UILabel>().text = str;
    }

    public void ShowPropTip(string desp, string stack, string price, List<ButtonInfo> buttonList)
    {
        EventDispatcher.TriggerEvent(SettingEvent.UIUpPlaySound, "ItemTip");
        m_equipDetailImageUsed.gameObject.SetActive(false);
        float gap = -20;

        ReleaseInstance();


        Transform root = transform.Find(m_widgetToFullName["EquipTipDetailList"]);
        //����+�Ű�

        //int i = 0;

        var _gap = gap;
        //�������
        GetInstance("PropInfoDetailEffectNum.prefab",
        (gameObject) =>
        {

            GameObject go = gameObject as GameObject;
            UILabel lable = go.transform.GetComponentsInChildren<UILabel>(true)[0];

            Utils.MountToSomeObjWithoutPosChange(go.transform, root);
            Vector3 position = go.transform.localPosition;
            go.transform.localPosition = new Vector3(0, _gap, 0);
            gos.Add(go);

            lable.text = desp;
        }
        );

        //int lineCount = despLineWidth / fontSize;
        //lineCount = desp.Length / lineCount + 1;
        //int height = lineCount * fontSize;
        DespLbl.text = desp;
        int height = (int)(DespLbl.relativeSize.y * DespLbl.transform.localScale.y);
        gap -= height + GAP;

        //��Ӷѵ�
        if (stack != "")
        {
            var _gap2 = gap;
            GetInstance("PropInfoDetailStack.prefab",
            (gameObject) =>
            {
                //PackageEquipInfoDiamonHole0Text
                GameObject go = gameObject as GameObject;
                UILabel lable = go.transform.Find("PropInfoDetailStackNum").GetComponent<UILabel>();

                Utils.MountToSomeObjWithoutPosChange(go.transform, root);
                Vector3 position = go.transform.localPosition;
                go.transform.localPosition = new Vector3(0, _gap2, 0);
                gos.Add(go);
                lable.text = stack;
            }
            );
            gap -= GAP + 22;
        }


        //��Ӽ�Ǯ
        if (price != "")
        {
            var _gap3 = gap;
            GetInstance("PropInfoDetailPrice.prefab",
            (gameObject) =>
            {
                //PackageEquipInfoDiamonHole0Text
                GameObject go = gameObject as GameObject;
                Utils.MountToSomeObjWithoutPosChange(go.transform, root);
                UILabel lable = go.transform.Find("PropInfoDetailPriceNum").GetComponent<UILabel>();
                UISprite icon = go.transform.Find("PropInfoDetailPriceIcon").GetComponent<UISprite>();

                lable.text = price;
                float dis = lable.font.CalculatePrintedSize(lable.text, false, UIFont.SymbolStyle.None).x * 22;
                Vector3 position = lable.transform.localPosition;
                icon.transform.localPosition = new Vector3(12/*position.x + dis + 2*/, icon.transform.localPosition.y, position.z);

                position = go.transform.localPosition;
                go.transform.localPosition = new Vector3(0, _gap3, 0);
                gos.Add(go);
            }
            );
        }

        gap -= 22;


        //m_equipTipCamera.height = gap - 400;
        //m_equipTipCamera.Reset();
        //show

        float minY;
        minY = -gap + 3 - m_equipTipCameraArea.localScale.y;
        minY = minY > 0 ? minY : 0;
        m_equipTipCamera.MINY = m_equipTipCameraPosBegin.localPosition.y - minY;
        m_equipTipCamera.transform.localPosition = m_equipTipCameraPosBegin.localPosition;

        InitButtonList(buttonList);
        this.gameObject.SetActive(true);
    }

    public void ShowMaterialTip(string level, string desp, string stack, string price, List<ButtonInfo> buttonList)
    {
        EventDispatcher.TriggerEvent(SettingEvent.UIUpPlaySound, "ItemTip");
        m_equipDetailImageUsed.gameObject.SetActive(false);
        float GAP = 18;
        float gap = -20;

        ReleaseInstance();

        Transform root = transform.Find(m_widgetToFullName["EquipTipDetailList"]);
        //����+�Ű�

        //int i = 0;

        //��ӵȼ�
        var _gap = gap;
        GetInstance("MaterialInfoDetailLevel.prefab",
        (gameObject) =>
        {
            GameObject go = gameObject as GameObject;
            Utils.MountToSomeObjWithoutPosChange(go.transform, root);
            UILabel lable = go.transform.Find("MaterialInfoDetailLevelNum").GetComponent<UILabel>();
            lable.text = level;

            Vector3 position = go.transform.localPosition;
            go.transform.localPosition = new Vector3(0, _gap, 0);
            gos.Add(go);
        }
        );
        gap -= 22 + GAP;

        var _gap6 = gap;
        //��Ӳ�������
        GetInstance("MaterialInfoDetailType.prefab",
        (gameObject) =>
        {
            GameObject go = gameObject as GameObject;

            Utils.MountToSomeObjWithoutPosChange(go.transform, root);
            Vector3 position = go.transform.localPosition;
            go.transform.localPosition = new Vector3(0, _gap6, 0);
            gos.Add(go);
        }
        );
        gap -= 22 + GAP;

        var _gap5 = gap;
        //�������
        GetInstance("PropInfoDetailEffectNum.prefab",
        (gameObject) =>
        {

            GameObject go = gameObject as GameObject;
            Utils.MountToSomeObjWithoutPosChange(go.transform, root);
            UILabel lable = go.transform.GetComponentsInChildren<UILabel>(true)[0];
            lable.text = desp;

            Vector3 position = go.transform.localPosition;
            go.transform.localPosition = new Vector3(0, _gap5, 0);
            gos.Add(go);
        }
        );

        //int lineCount = despLineWidth / fontSize;
        //lineCount = desp.Length / lineCount + 1;
        //int height = lineCount * fontSize;

        DespLbl.text = desp;
        int height = (int)(DespLbl.relativeSize.y * DespLbl.transform.localScale.y);
        gap -= height + GAP;

        //��Ӷѵ�
        if (stack != "")
        {
            var _gap2 = gap;
            GetInstance("PropInfoDetailStack.prefab",
            (gameObject) =>
            {
                //PackageEquipInfoDiamonHole0Text
                GameObject go = gameObject as GameObject;
                Utils.MountToSomeObjWithoutPosChange(go.transform, root);
                UILabel lable = go.transform.Find("PropInfoDetailStackNum").GetComponent<UILabel>();
                lable.text = stack;

                Vector3 position = go.transform.localPosition;
                go.transform.localPosition = new Vector3(0, _gap2, 0);
                gos.Add(go);
            }
            );
            gap -= GAP + 22;
        }



        //��Ӽ�Ǯ
        if (price != "")
        {
            var _gap3 = gap;
            GetInstance("PropInfoDetailPrice.prefab",
            (gameObject) =>
            {
                //PackageEquipInfoDiamonHole0Text
                GameObject go = gameObject as GameObject;
                Utils.MountToSomeObjWithoutPosChange(go.transform, root);
                UILabel lable = go.transform.Find("PropInfoDetailPriceNum").GetComponent<UILabel>();
                UISprite icon = go.transform.Find("PropInfoDetailPriceIcon").GetComponent<UISprite>();
                lable.text = price;

                float dis = lable.font.CalculatePrintedSize(lable.text, false, UIFont.SymbolStyle.None).x * 22;
                Vector3 position = lable.transform.localPosition;
                icon.transform.localPosition = new Vector3(12/*position.x + dis + 2*/, icon.transform.localPosition.y, position.z);

                position = go.transform.localPosition;
                go.transform.localPosition = new Vector3(0, _gap3, 0);
                gos.Add(go);
            }
            );
        }

        gap -= 22;

        //gap -= 10;//Ԥ��λ��
        //gap = -gap;
        float minY;
        minY = -gap + 3 - m_equipTipCameraArea.localScale.y;
        minY = minY > 0 ? minY : 0;
        m_equipTipCamera.MINY = m_equipTipCameraPosBegin.localPosition.y - minY;
        m_equipTipCamera.transform.localPosition = m_equipTipCameraPosBegin.localPosition;
        //m_equipTipCamera.height = gap - 400;
        //m_equipTipCamera.Reset();
        //show

        InitButtonList(buttonList);

        this.gameObject.SetActive(true);
    }

    public void ShowJewelTip(string level, string type, string desp, string stack, List<ButtonInfo> buttonList)
    {
        EventDispatcher.TriggerEvent(SettingEvent.UIUpPlaySound, "ItemTip");
        m_equipDetailImageUsed.gameObject.SetActive(false);
        float gap = -20;
        ReleaseInstance();

        Transform root = transform.Find(m_widgetToFullName["EquipTipDetailList"]);
        //����+�Ű�

        //int i = 0;

        //��ӵȼ�
        var _gap = gap;
        GetInstance("MaterialInfoDetailLevel.prefab",
        (gameObject) =>
        {
            GameObject go = gameObject as GameObject;
            Utils.MountToSomeObjWithoutPosChange(go.transform, root);
            UILabel lable = go.transform.Find("MaterialInfoDetailLevelNum").GetComponent<UILabel>();
            lable.text = level;

            Vector3 position = go.transform.localPosition;
            go.transform.localPosition = new Vector3(0, _gap, 0);
            gos.Add(go);
        }
        );
        gap -= 22 + GAP;

        //�������
        var _gap6 = gap;
        GetInstance("MaterialInfoDetailType.prefab",
        (gameObject) =>
        {
            GameObject go = gameObject as GameObject;
            Utils.MountToSomeObjWithoutPosChange(go.transform, root);
            UILabel lable = go.transform.Find("MaterialInfoDetailTypeNum").GetComponent<UILabel>();
            lable.text = type;

            Vector3 position = go.transform.localPosition;
            go.transform.localPosition = new Vector3(0, _gap6, 0);
            gos.Add(go);
        }
        );
        gap -= 22 + GAP;

        //�������
        var _gap5 = gap;
        GetInstance("PropInfoDetailEffectNum.prefab",
        (gameObject) =>
        {
            GameObject go = gameObject as GameObject;
            Utils.MountToSomeObjWithoutPosChange(go.transform, root);
            UILabel lable = go.transform.GetComponentsInChildren<UILabel>(true)[0];
            lable.text = desp;

            Vector3 position = go.transform.localPosition;
            go.transform.localPosition = new Vector3(0, _gap5, 0);
            gos.Add(go);
        }
        );

        DespLbl.text = desp;
        int height = (int)(DespLbl.relativeSize.y * DespLbl.transform.localScale.y);
        gap -= height + GAP;

        //��Ӷѵ�
        if (stack != "")
        {
            var _gap2 = gap;
            GetInstance("PropInfoDetailStack.prefab",
            (gameObject) =>
            {
                //PackageEquipInfoDiamonHole0Text
                GameObject go = gameObject as GameObject;
                Utils.MountToSomeObjWithoutPosChange(go.transform, root);
                UILabel lable = go.transform.Find("PropInfoDetailStackNum").GetComponent<UILabel>();
                lable.text = stack;

                Vector3 position = go.transform.localPosition;
                go.transform.localPosition = new Vector3(0, _gap2, 0);
                gos.Add(go);
            }
            );
            gap -= GAP + 22;
        }

        float minY;
        minY = -gap + 3 - m_equipTipCameraArea.localScale.y;
        minY = minY > 0 ? minY : 0;
        m_equipTipCamera.MINY = m_equipTipCameraPosBegin.localPosition.y - minY;
        m_equipTipCamera.transform.localPosition = m_equipTipCameraPosBegin.localPosition;

        //m_equipTipCamera.height = gap - 400;
        //m_equipTipCamera.Reset();
        //show

        InitButtonList(buttonList);

        this.gameObject.SetActive(true);
    }

    private void Reset()
    {
    }

    /// <summary>
    /// ���а�ť
    /// </summary>
    /// <param name="buttonList"></param>
    private void InitButtonList(List<ButtonInfo> buttonList)
    {
        Transform btnRoot = transform.Find(m_widgetToFullName["EquipTipDetailBtnList"]);
        int count = buttonList == null ? 0 : buttonList.Count;
        for (int i = count - 1; i >= 0; i--)
        {
            int index = i;
            GetInstance("EquipTipDetailBtn.prefab",
            (gameObject) =>
            {
                //PackageEquipInfoDiamonHole0Text
                GameObject go = gameObject as GameObject;
                go.name = "EquipTipDetailBtn" + buttonList[index].id;

                EquipTipBtn btn = go.GetComponent<EquipTipBtn>();
                if (btn == null) btn = go.AddComponent<EquipTipBtn>();
                btn.Init(buttonList[index].text, buttonList[index].action);

                Vector3 scale = go.transform.localScale;
                Vector3 position = go.transform.localPosition;
                Vector3 angle = go.transform.localEulerAngles;
                go.transform.parent = btnRoot;
                go.transform.localScale = scale;
                go.transform.localEulerAngles = angle;
                go.transform.localPosition = new Vector3(position.x, (count - index - 1) * BTN_GAP, position.z);
                btnGos.Add(go);

                if (MogoUIManager.Instance.WaitingWidgetName == go.name)
                {
                    EventDispatcher.TriggerEvent("WaitingWidgetFinished");
                }
            }
            );
        }
    }

    /// <summary>
    /// �ر���ʾ��
    /// </summary>
    public void CloseEquipTip()
    {
        ReleaseInstance();
        m_equipTip.transform.localPosition = center.localPosition;
        m_equipTipCurrent.SetActive(false);
        this.gameObject.SetActive(false);
        m_equipDetailImageUsed.gameObject.SetActive(false);
        //transform.FindChild(m_widgetToFullName["EquipTipDetail3"]).gameObject.SetActive(false);

        m_equipTipDetail.SetActive(false);
        m_equipTipDetailCurrent.SetActive(false);
        m_goGOEquipTipDetailScore.SetActive(false);
        m_goGOEquipTipCurrentDetailScore.SetActive(false);
    }

    public void SetEquipDetailInfoImage(string p, int color = 0)
    {
        m_equipDetailImageFG.atlas = MogoUIManager.Instance.GetAtlasByIconName(p);
        m_equipDetailImageFG.spriteName = p;
        MogoUtils.SetImageColor(m_equipDetailImageFG, color);
    }

    public void ShowEquipDetailInfoImageUsed(bool p)
    {
        m_equipDetailImageUsed.gameObject.SetActive(p);
    }

    public void SetEquipDetailInfoName(string p)
    {
        m_equipDetailName.text = p;
    }

    public void SetEquipImage(int id)
    {
        //InventoryManager.SetIcon(id, m_equipDetailImageFG, 0, null, m_equipDetailImageBG);
        InventoryManager.TrySetIcon(id, m_equipDetailImageFG, 0, null, m_equipDetailImageBG);

    }
    public void SetEquipDetailInfoImageBg(string p)
    {
        m_equipDetailImageBG.spriteName = p;
    }

    public void SetEquipDetailInfoLevelText(string txt)
    {
        m_equipDetailNeedLevellbl.text = txt;
    }

    public void SetEquipDetailInfoScoreNum(int score)
    {
        m_lblEquipTipDetailScoreNum.text = score.ToString();
        m_goGOEquipTipDetailScore.SetActive(true);
    }

    public void SetEquipDetailInfoLevelTextCurrent(string txt)
    {
        m_equipDetailNeedLevellblCurrent.text = txt;
    }

    public void SetEquipDetailInfoImageCurrent(string p, int color = 0)
    {
        m_equipDetailImageFGCurrent.atlas = MogoUIManager.Instance.GetAtlasByIconName(p);
        m_equipDetailImageFGCurrent.spriteName = p;

        MogoUtils.SetImageColor(m_equipDetailImageFG, color);
    }

    public void ShowEquipDetailInfoImageUsedCurrent(bool p)
    {
        m_equipDetailImageUsedCurrent.gameObject.SetActive(p);
    }

    public void SetEquipDetailInfoNameCurrent(string p)
    {
        m_equipDetailNameCurrent.text = p;
    }

    public void SetEquipDetailInfoImageBgCurrent(string p)
    {
        m_equipDetailImageBGCurrent.spriteName = p;
    }

    public void SetEquipDetailInfoScoreNumCurrent(int score)
    {
        m_lblEquipTipCurrentDetailScoreNum.text = score.ToString();
        m_goGOEquipTipCurrentDetailScore.SetActive(true);
    }
}

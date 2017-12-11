using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mogo.Game;
using Mogo.Util;
using Mogo.GameData;

public enum ChooseDecomposeEquip
{
    Waste = 1,
    All =2,
}

public class DecomposeUIViewManager : MogoUIBehaviour
{
    private static DecomposeUIViewManager m_instance;
    public static DecomposeUIViewManager Instance { get {return DecomposeUIViewManager.m_instance; }}

    private const int PACKAGEITEMNUM = 40;
    private const int DIAMONDHOLENUM = 13;
    private const float PACKAGEITEMSPACE = 0.125f;

    private const int ICONGRIDNUM = 10;
    private const float ICONGRIDSPACE = 0.065f;

    private Transform m_transPackageItemList;
    private Camera m_dragCamera;
    private Camera m_dragIconCamera;
    private List<GameObject> m_listPackageGrid = new List<GameObject>();
    private List<UISlicedSprite> m_listPackageGridFG = new List<UISlicedSprite>();
    private List<UISlicedSprite> m_listPackageGridBG = new List<UISlicedSprite>();
    private List<GameObject> m_listPackageCheckGridFG = new List<GameObject>();
    private List<GameObject> m_listPackageCheckGridBG = new List<GameObject>();
    private List<GameObject> m_listPackageCheckGrid = new List<GameObject>();
    private List<GameObject> m_listPackageLock = new List<GameObject>();
    private List<GameObject> m_listEquipmentGrid = new List<GameObject>();

    private SimpleDragCamera m_equipTipCamera;
    private UILabel m_equipDetailName;
    private UILabel m_equipDetailNeedLevel;
    private UILabel m_equipDetailGrowLevel;
    private UISlicedSprite m_equipDetailImageBG;
    private UILabel m_equipDetailLblEquip;
    private UILabel m_equipDetailNeedJob;
    private UILabel m_equipDetailExtra;
    private UISlicedSprite m_equipDetailImageFG;
    private UISlicedSprite m_equipDetailImageUsed;

    private GameObject m_goGODecomposeChooseEquip;
    private UITexture m_texGril;

    private GameObject m_goGODecomposeJewlTip;
    private Transform m_transDecomposeDialogIconList;

    public static Dictionary<string, string> ButtonTypeToEventDown = new Dictionary<string, string>();    

    void Awake()
    {
        m_instance = this;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);      
        Initialize();
        
        m_transPackageItemList = m_myTransform.Find(m_widgetToFullName["DecomposePackageItemList"]);
        m_dragCamera = m_myTransform.Find(m_widgetToFullName["DecomposePackageItemListCamera"]).GetComponentsInChildren<Camera>(true)[0];
        m_dragIconCamera = m_myTransform.Find(m_widgetToFullName["DecomposeDialogIconListCamera"]).GetComponentsInChildren<Camera>(true)[0];

        m_dragCamera.GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = GameObject.Find("Camera").GetComponentsInChildren<Camera>(true)[0];
        m_dragIconCamera.GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = GameObject.Find("Camera").GetComponentsInChildren<Camera>(true)[0];
        m_dragIconCamera.GetComponentsInChildren<UIViewport>(true)[0].topLeft = GameObject.Find("EquipmentUIIconListBGTopLeft").transform;
        m_dragIconCamera.GetComponentsInChildren<UIViewport>(true)[0].bottomRight = GameObject.Find("EquipmentUIIconListBGBottomRight").transform;

        m_transDecomposeDialogIconList = m_myTransform.Find(m_widgetToFullName["DecomposeDialogIconList"]);

        m_equipDetailName = m_myTransform.Find(m_widgetToFullName["PackageEquipInfoNameText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipDetailNeedLevel = m_myTransform.Find(m_widgetToFullName["PackageEquipInfoDetailNeedLevelNum"]).GetComponentsInChildren<UILabel>(true)[0];
        //m_equipDetailGrowLevel = m_myTransform.FindChild(m_widgetToFullName["PackageEquipInfoDetailGrowLevelNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipDetailNeedJob = m_myTransform.Find(m_widgetToFullName["PackageEquipInfoDetailNeedJobType"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipDetailExtra = m_myTransform.Find(m_widgetToFullName["PackageEquipInfoDetailExtraText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipDetailImageFG = m_myTransform.Find(m_widgetToFullName["PackageEquipInfoDetailImageFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        m_equipDetailImageBG = m_myTransform.Find(m_widgetToFullName["PackageEquipInfoDetailImageBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        m_equipDetailImageUsed = m_myTransform.Find(m_widgetToFullName["PackageEquipInfoDetailImageUsed"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        m_equipTipCamera = m_myTransform.Find(m_widgetToFullName["PackageEquipInfoDetailBG"]).GetComponentsInChildren<SimpleDragCamera>(true)[0];

        m_goGODecomposeChooseEquip = m_myTransform.Find(m_widgetToFullName["GODecomposeChooseEquip"]).gameObject;

        m_texGril = m_myTransform.Find(m_widgetToFullName["DecomposeLeftGirl"]).GetComponentsInChildren<UITexture>(true)[0];
        m_goGODecomposeJewlTip = m_myTransform.Find(m_widgetToFullName["GODecomposeJewlTip"]).gameObject;

        // ChineseData
        m_myTransform.Find(m_widgetToFullName["DecomposeChooseEquipTitle"]).GetComponentsInChildren<UILabel>(true)[0].text
            = LanguageData.GetContent(46000);
        m_myTransform.Find(m_widgetToFullName["DecomposeChooseEquipAllText"]).GetComponentsInChildren<UILabel>(true)[0].text
            = LanguageData.GetContent(46002);
        m_myTransform.Find(m_widgetToFullName["DecomposeChooseEquipWasteText"]).GetComponentsInChildren<UILabel>(true)[0].text
            = LanguageData.GetContent(46001);
        m_myTransform.Find(m_widgetToFullName["DecomposeJewlTipText"]).GetComponentsInChildren<UILabel>(true)[0].text
            = LanguageData.GetContent(1087);

        for (int i = 0; i < PACKAGEITEMNUM; ++i)
        {
            int index = i;
            AssetCacheMgr.GetUIInstance("DecomposeDialogPackageGrid.prefab", (prefab, guid, go) =>
            {
                GameObject obj = (GameObject)go;
                obj.transform.parent = m_transPackageItemList;
                obj.transform.localPosition = new Vector3(PACKAGEITEMSPACE * ((index % 5) + index / 10 * 5), -PACKAGEITEMSPACE * ((index / 5) % 2), 0);
                obj.transform.localScale = new Vector3(0.0008f, 0.0008f, 1);
                obj.name = "DecomposeGrid" + index.ToString();
                obj.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_dragCamera;

                obj.AddComponent<DecomposeUIEquipmentGrid>().id = index;

                m_listPackageGrid.Add(obj);

                obj = obj.transform.Find("DecomposeDialogPackageGridCheckBG").gameObject;
                obj.name = "DecomposeDialogPackageGridCheckBG" + index.ToString();
                obj.AddComponent<DecomposeUIEquipmentCheckGrid>().id = index;
                m_listPackageCheckGridBG.Add(obj);

                obj = obj.transform.parent.Find("DecomposeDialogPackageGridCheckFG").gameObject;
                obj.SetActive(false);
                m_listPackageCheckGridFG.Add(obj);

                obj = obj.transform.parent.Find("DecomposeDialogPackageGridLock").gameObject;
                m_listPackageLock.Add(obj);

                UISlicedSprite ss = obj.transform.parent.Find("DecomposeDialogPackageGridFG").GetComponentsInChildren<UISlicedSprite>(true)[0];
                UISlicedSprite ssbg = obj.transform.parent.Find("DecomposeDialogPackageGridBG").GetComponentsInChildren<UISlicedSprite>(true)[0];

                m_listPackageGridFG.Add(ss);
                m_listPackageGridBG.Add(ssbg);

                if (m_listPackageGrid.Count == PACKAGEITEMNUM)
                {
                    EventDispatcher.TriggerEvent(DecomposeManager.ON_DECOMPOSE_SHOW);

                    if (MogoUIManager.Instance.DecomposeUILoaded != null)
                    {
                        MogoUIManager.Instance.DecomposeUILoaded();
                    }

                    MogoGlobleUIManager.Instance.ShowWaitingTip(false);
                }
            });
        }

        for (int i = 0; i < ICONGRIDNUM; ++i)
        {
            int index = i;
            AssetCacheMgr.GetUIInstance("DecomposeDialogIconGrid.prefab", (prefab, guid, go) =>
            {
                GameObject obj = (GameObject)go;
                obj.transform.parent = m_transDecomposeDialogIconList;
                obj.transform.localPosition = new Vector3(0, -ICONGRIDSPACE * index, 0);
                obj.transform.localScale = new Vector3(0.0008f, 0.0008f, 1);
                obj.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_dragIconCamera;
                obj.name = "EquipmentGrid" + index.ToString();
                // var s = m_transDecomposeDialogIconList.GetComponentsInChildren<MogoSingleButtonList>(true)[0] as MogoSingleButtonList;
                //s.SingleButtonList.Add(obj.GetComponentsInChildren<MogoSingleButton>(true)[0]);

                obj.GetComponentsInChildren<UILabel>(true)[0].text = BodyName.names[index];
                obj.AddComponent<DecomposeUIEquipmentGrid>().id = index;

                m_listEquipmentGrid.Add(obj);
            });
        }
    }

    #region �¼�

    void Initialize()
    {
        DecomposeUILogicManager.Instance.Initialize();
        DecomposeUIDict.ButtonTypeToEventUp.Add("DecomposeButton", OnDecomposeButtonUp);
        DecomposeUIDict.ButtonTypeToEventUp.Add("PackageEquipInfoDetailSail", OnPackageDetailUnLockUp);
        DecomposeUIDict.ButtonTypeToEventUp.Add("PackageEquipInfoClose", OnPackageDetailCloseUp);
        DecomposeUIDict.ButtonTypeToEventUp.Add("DecomposeChooseEquipWaste", OnDecomposeChooseEquipWasteUp);
        DecomposeUIDict.ButtonTypeToEventUp.Add("DecomposeChooseEquipAll", OnDecomposeChooseEquipAllUp);

        DecomposeUIDict.DECOMPOSEUICHECKGRIDUP += OnDecomposeCheckGridUp;
    }

    public void Release()
    {
        DecomposeUILogicManager.Instance.Release();
        DecomposeUIDict.DECOMPOSEUICHECKGRIDUP -= OnDecomposeCheckGridUp;


        DecomposeUIDict.ButtonTypeToEventUp.Clear();

        for (int i = 0; i < m_listPackageGrid.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(m_listPackageGrid[i]);
            m_listPackageGrid[i] = null;
        }
        m_listPackageGrid.Clear();

        //for (int i = 0; i < m_listPackageCheckGridBG.Count; ++i)
        //{
        //    AssetCacheMgr.ReleaseInstance(m_listPackageCheckGridBG[i]);
        //    m_listPackageCheckGridBG[i] = null;
        //}
        m_listPackageCheckGridBG.Clear();
        //for (int i = 0; i < m_listPackageCheckGridFG.Count; ++i)
        //{
        //    AssetCacheMgr.ReleaseInstance(m_listPackageCheckGridFG[i]);
        //    m_listPackageCheckGridFG[i] = null;
        //}
        m_listPackageCheckGridFG.Clear();
        //for (int i = 0; i < m_listPackageLock.Count; ++i)
        //{
        //    AssetCacheMgr.ReleaseInstance(m_listPackageLock[i]);
        //    m_listPackageLock[i] = null;
        //}
        m_listPackageLock.Clear();
        //for (int i = 0; i < m_listPackageGridFG.Count; ++i)
        //{
        //    AssetCacheMgr.ReleaseInstance(m_listPackageGridFG[i]);
        //    m_listPackageGridFG[i] = null;
        //}
        m_listPackageGridFG.Clear();
        //for (int i = 0; i < m_listPackageGridBG.Count; ++i)
        //{
        //    AssetCacheMgr.ReleaseInstance(m_listPackageGridBG[i]);
        //    m_listPackageGridBG[i] = null;
        //}
        m_listPackageGridBG.Clear();
        for (int i = 0; i < m_listEquipmentGrid.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(m_listEquipmentGrid[i]);
            m_listEquipmentGrid[i] = null;
        }
        for (int i = 0; i < gos.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(gos[i]);
            gos[i] = null;
        }
    }

    void OnDecomposeButtonUp()
    {
        if (DecomposeUIDict.DECOMPOSEBUTTONUP != null)
        {
            DecomposeUIDict.DECOMPOSEBUTTONUP();
        }
    }

    void OnPackageDetailUnLockUp()
    {
        if (DecomposeUIDict.UNLOCKBTNUP != null)
        {
            DecomposeUIDict.UNLOCKBTNUP();
        }
    }

    void OnDecomposeCheckGridUp(int id)
    {
        //m_listPackageCheckGridFG[id].SetActive(!m_listPackageCheckGridFG[id].activeSelf);
    }

    /// <summary>
    /// ����װ��
    /// </summary>
    /// <param name="i"></param>
    void OnDecomposeChooseEquipWasteUp()
    {
        if (DecomposeUIDict.DECOMPOSECHOOSEEQUIPWASTE != null)
            DecomposeUIDict.DECOMPOSECHOOSEEQUIPWASTE();
    }

    /// <summary>
    /// ȫ��װ��
    /// </summary>
    /// <param name="i"></param>
    void OnDecomposeChooseEquipAllUp()
    {
        if (DecomposeUIDict.DECOMPOSECHOOSEEQUIPALL != null)
            DecomposeUIDict.DECOMPOSECHOOSEEQUIPALL();
    }

    void OnPackageDetailCloseUp()
    {
        Mogo.Util.LoggerHelper.Debug("decompose:OnPackageDetailCloseUp");
        ShowPackageDetailInfo(false);
    }

    #endregion

    #region ������Ϣ

    public void SetUnLockBtnName(string name)
    {
        m_myTransform.Find(m_widgetToFullName["PackageEquipInfoDetailSailText"]).GetComponentsInChildren<UILabel>(true)[0].text = name;
    }

    public void SetPackageGridImage(string name, int id)
    {
        m_listPackageGridFG[id].atlas = MogoUIManager.Instance.GetAtlasByIconName(name);
        m_listPackageGridFG[id].spriteName = name;
    }

    public void SetPackageGridImageBG(string name, int id)
    {
        m_listPackageGridBG[id].spriteName = name;
    }

    public void ShowPacakgeGridCheckFG(bool isShow, int id)
    {
        m_listPackageCheckGridFG[id].SetActive(isShow);
    }

    public void ShowPackageGridCheck(bool isShow, int id)
    {
        m_listPackageCheckGridBG[id].SetActive(isShow);
    }

    public void ShowPackageGridLock(bool isShow, int id)
    {
        m_listPackageLock[id].SetActive(isShow);
    }

    public void SetEquipDetailInfoNeedLevel(int level)
    {
        m_equipDetailNeedLevel.text = level.ToString();
    }
    public void SetEquipDetailInfoGrowLevel(string level)
    {
        m_equipDetailGrowLevel.text = level;
    }

    public void SetEquipDetailInfoNeedJob(string job)
    {
        m_equipDetailNeedJob.text = job;
    }

    public void SetEquipDetailInfoExtra(string text)
    {
        m_equipDetailExtra.text = text;
    }

    public void SetEquipDetailInfoImage(string imgName)
    {
        m_equipDetailImageFG.atlas = MogoUIManager.Instance.GetAtlasByIconName(imgName);
        m_equipDetailImageFG.spriteName = imgName;
    }

    public void ShowEquipDetailInfoImageUsed(bool isShow)
    {
        m_equipDetailImageUsed.gameObject.SetActive(isShow);
    }

    public void ShowPackageDetailInfo(bool show)
    {
        m_myTransform.Find(m_widgetToFullName["PackageEquipInfo"]).gameObject.SetActive(show);
        m_equipTipCamera.Reset();
    }

    public void SetCheckGridUp(int index, bool isChecked)
    {
        m_listPackageCheckGridFG[index].SetActive(isChecked);
    }

    public void SetEquipDetailInfoName(string p)
    {
        m_equipDetailName.text = p;
    }

    public void SetEquipDetailInfoImageBg(string p)
    {
        m_equipDetailImageBG.spriteName = p;
    }

    /// <summary>
    /// ѡ����ʾװ��
    /// </summary>
    /// <param name="id"></param>
    public void SetEquipChoose(int id)
    {
        if (id == (int)ChooseDecomposeEquip.Waste)
        {
            m_goGODecomposeChooseEquip.GetComponent<MogoSingleButtonList>().SetCurrentDownButton(0);
        }
        else if (id == (int)ChooseDecomposeEquip.All)
        {
            m_goGODecomposeChooseEquip.GetComponent<MogoSingleButtonList>().SetCurrentDownButton(1);
        }
    }

    #region װ��tip

    public const float GAP = 30;
    public List<GameObject> gos = new List<GameObject>();
    public void ShowEquipInfoDetail
        (List<string> attrs, List<string> jewels, string level, string vocation)
    {
        float gap = 0;
        foreach (GameObject go in gos)
        {
            AssetCacheMgr.ReleaseInstance(go);
        }
        gos.Clear();

        Transform root = m_myTransform.Find(m_widgetToFullName["PackageEquipInfoDetailList"]);
        //����+�Ű�

        int i = 0;
        //����
        for (i = 0; i < attrs.Count; i++)
        {
            var index = i;
            AssetCacheMgr.GetUIInstance("PackageEquipInfoAttr.prefab",
            (prefab, guid, gameObject) =>
            {
                //PackageEquipInfoDiamonHole0Text
                GameObject go = gameObject as GameObject;
                UILabel lable = go.transform.Find("PackageEquipInfoDiamonHole0Text").GetComponent<UILabel>();
                lable.text = attrs[index];

                Vector3 scale = go.transform.localScale;
                Vector3 position = go.transform.localPosition;
                Vector3 angle = go.transform.localEulerAngles;
                go.transform.parent = root;
                go.transform.localScale = scale;
                go.transform.localEulerAngles = angle;
                go.transform.localPosition = new Vector3(position.x, position.y - index * GAP, position.z);
                gos.Add(go);
            }
            );
        }
        gap -= i * GAP - 30;

        //��ʯ
        for (i = 0; i < jewels.Count; i++)
        {
            var index = i;
            AssetCacheMgr.GetUIInstance("PackageEquipInfoDiamon.prefab",
            (prefab, guid, gameObject) =>
            {
                //PackageEquipInfoDiamonHole0Text
                GameObject go = gameObject as GameObject;
                UILabel lable = go.transform.Find("PackageEquipInfoDiamonHole12Text").GetComponent<UILabel>();
                lable.text = jewels[index];

                Vector3 scale = go.transform.localScale;
                Vector3 position = go.transform.localPosition;
                Vector3 angle = go.transform.localEulerAngles;
                go.transform.parent = root;
                go.transform.localScale = scale;
                go.transform.localEulerAngles = angle;
                go.transform.localPosition = new Vector3(position.x, gap + position.y - index * GAP, position.z);
                gos.Add(go);
            }
            );
        }
        gap -= i * GAP + 30;

        //����ȼ���
        Transform detail3 = m_myTransform.Find(m_widgetToFullName["PackageEquipInfoDetail3"]);
        detail3.localPosition = new Vector3(detail3.localPosition.x, gap, detail3.localPosition.z);
        m_equipDetailNeedJob.GetComponent<UILabel>().text = vocation;
        m_equipDetailNeedLevel.GetComponent<UILabel>().text = level;

        gap -= 200;
        gap = -gap;

        m_equipTipCamera.height = gap - 500;
        //show
        ShowPackageDetailInfo(true);
    }

    #endregion

    #region ��ʯ�Ž�������ʾ��

    /// <summary>
    /// ��ʾ/���ر�ʯ�Ž�������ʾ��
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowDecomposeJewlTip(uint millisecond = 7000)
    {
        ShowDecomposeJewlTip(true);
        TimerHeap.AddTimer(millisecond, 0, () =>
        {
             ShowDecomposeJewlTip(false);
        });
    }

    public void ShowDecomposeJewlTip(bool isShow)
    {
        m_goGODecomposeJewlTip.SetActive(isShow);
    }

    #endregion

    #endregion 

    #region ����򿪺͹ر�

    protected override void OnEnable()
    {
        base.OnEnable();
        //if (!SystemSwitch.DestroyResource)
        //{
        //    return;
        //}
        //if (m_texGril.mainTexture == null)
        //{
        //    AssetCacheMgr.GetUIResource("gn-npc.png", (obj) => 
        //    {
        //        m_texGril.mainTexture = (Texture)obj;
        //    });
        //}

        ShowDecomposeJewlTip(false);
    }

    public void ReleaseUIAndResources()
    {
        ReleasePreLoadResources();
        MogoUIManager.Instance.DestroyDecomposeUI();
    }

    void ReleasePreLoadResources()
    {
        AssetCacheMgr.ReleaseResourceImmediate("DecomposeDialogPackageGrid.prefab");
        AssetCacheMgr.ReleaseResourceImmediate("DecomposeDialogIconGrid.prefab");
    }


    void OnDisable()
    {
        if (SystemSwitch.DestroyAllUI)
        {
            ReleaseUIAndResources();
        }
        //if (!SystemSwitch.DestroyResource)
        //{
        //    return;
        //}
        //if (m_texGril != null)
        //{
        //    m_texGril.mainTexture = null;
        //    AssetCacheMgr.ReleaseResourceImmediate("gn-npc.png");
        //}
    }

    #endregion
}

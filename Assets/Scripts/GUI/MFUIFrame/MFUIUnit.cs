using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MFUIUnit : MonoBehaviour
{
    protected Transform m_myTransform;
    protected GameObject m_myGameObject;
    static Dictionary<string, UILabel> m_dictLabel = new Dictionary<string, UILabel>();
    static Dictionary<string, UISprite> m_dictSprite = new Dictionary<string, UISprite>();
    static Dictionary<string, UITexture> m_dictTexture = new Dictionary<string, UITexture>();
    static Dictionary<string, Transform> m_dictTransform = new Dictionary<string, Transform>();
    static Dictionary<string, MFUIButtonHandler> m_dictButtonAction = new Dictionary<string, MFUIButtonHandler>();

    public MFUIManager.MFUIID ID = MFUIManager.MFUIID.None;

    public MFUILogicUnit LogicUnit;

    bool m_bIsResourcesLoaded = false;
    bool m_bIsDirty = false;


    void Awake()
    {
        m_myTransform = transform;
        m_myGameObject = gameObject;

        MFUIUtils.FillFullNameData(m_myTransform, m_myTransform);
        FillAllDefaultWidgets(m_myTransform);


        MFUIGameObjectPool.GetSingleton().UIResourcesLoadedCB += OnUIResourcesLoaded;
        LoadResources();

    }

    void OnUIResourcesLoaded(MFUIManager.MFUIID id)
    {
        if (id != ID)
            return;

        Create();
        m_bIsResourcesLoaded = true;
        //Show();

        if (IsUIDirty())
        {
            Show();
        }
    }

    void OnEnable() 
    {
        if (m_bIsResourcesLoaded)
        {
            CallWhenEnable();
        }
    }

    void OnDisable()
    {
        if (m_bIsResourcesLoaded)
        {
            CallWhenDisable();
        }
    }

    void Create()
    {

        //foreach (var item in m_dictTransform)
        //{
        //    Debug.LogError(item.Value.name);
        //}
        CallWhenCreate();

        if (LogicUnit == null)
        {
            MFUIUtils.MFUIDebug("Not Attach LogicUnit ! ");
            return;
        }

        Debug.Log(m_myGameObject.name);
        LogicUnit.FillBufferedData();
    }

    void OnDestroy()
    {
        CallWhenDestroy();

        //foreach (var item in m_dictButtonAction)
        //{
        //    item.Value.ClickHandler = null;
        //    item.Value.PressHandler = null;
        //    item.Value.DragHandler = null;
        //}

        //m_dictLabel.Clear();
        //m_dictSprite.Clear();
        //m_dictButtonAction.Clear();
        //m_dictTransform.Clear();

        //m_dictLabel = null;
        //m_dictSprite = null;
        //m_dictTransform = null;
        //m_dictButtonAction = null;

        UnLoadResources();

        m_bIsResourcesLoaded = false;
        m_bIsDirty = false;

        //MFUIManager.GetSingleton().DictUIIDToOBj.Remove(ID);

        MFUIManager.GetSingleton().ReleaseUI(ID);
        m_myGameObject = null;
        m_myTransform = null;


        DetachLogicUnit();
        MFUIGameObjectPool.GetSingleton().UIResourcesLoadedCB -= OnUIResourcesLoaded;


    }

    void Update()
    {
        CallWhenUpdate();

    }

    public void SetUIDirty()
    {
        m_bIsDirty = true;

        Show();
    }

    public bool IsUIDirty()
    {
        return m_bIsDirty;
    }

    public void Show()
    {
        //MFUILoading.GetSingleton().Hide();
        if (m_bIsResourcesLoaded && m_bIsDirty)
        {
            CallWhenShow();
            m_bIsDirty = false;
            //Mogo.Util.TimerHeap.AddTimer(500,0,()=>{
            //});
            MogoGlobleUIManager.Instance.ShowWaitingTip(false);

            MFUIManager.CurrentUI = ID;

        }
    }

    public void Hide()
    {
        MogoGlobleUIManager.Instance.ShowWaitingTip(true);
        if (m_bIsResourcesLoaded)
        {
            //MFUILoading.GetSingleton().Show();
            CallWhenHide();
        }
    }

    public void LoadResources()
    {
        CallWhenLoadResources();
    }

    public void UnLoadResources()
    {
        CallWhenUnloadResources();
        MFUIResourceManager.GetSingleton().ReleasePreLoadResource(ID);
    }

    public void AttachLogicUnit(MFUILogicUnit logicUnit)
    {
        LogicUnit = logicUnit;
    }

    public void DetachLogicUnit()
    {
        LogicUnit = null;
    }




    public virtual void CallWhenCreate()
    {
        
    }

    public virtual void CallWhenDestroy()
    {
 
    }

    public virtual void CallWhenEnable()
    {
 
    }

    public virtual void CallWhenDisable()
    {
 
    }

    public virtual void CallWhenShow()
    {
    }

    public virtual void CallWhenHide()
    {
    }

    public virtual void CallWhenLoadResources()
    {
       
    }

    public virtual void CallWhenUnloadResources()
    {
        
    }

    public virtual void CallWhenUpdate()
    {
 
    }






    public UILabel RegisterLabel(string name, Transform rootTran = null)
    {
        UILabel lbl;

        if (m_dictLabel.ContainsKey(name))
        {
            MFUIUtils.MFUIDebug(string.Concat(name, " is Already in the Label Dict ! Now Instead it of this !"));
        }

        if (rootTran == null)
        {
            lbl = m_myTransform.Find(MFUIUtils.GetFullName(name)).GetComponentsInChildren<UILabel>(true)[0];
        }
        else
        {
            lbl = rootTran.Find(name).GetComponentsInChildren<UILabel>(true)[0];
        }

        m_dictLabel[name] = lbl;

        return lbl;
    }

    public UISprite RegisterSprite(string name, Transform rootTran = null)
    {
        UISprite sp;

        if (m_dictSprite.ContainsKey(name))
        {
            MFUIUtils.MFUIDebug(string.Concat(name, " is Already in the Sprite Dict ! Now Instead it of this !"));
        }

        if (rootTran == null)
        {
            sp = m_myTransform.Find(MFUIUtils.GetFullName(name)).GetComponentsInChildren<UISprite>(true)[0];
        }
        else
        {
            sp = rootTran.Find(name).GetComponentsInChildren<UISprite>(true)[0];
        }

        m_dictSprite[name] = sp;

        return sp;
    }

    public UITexture RegisterTexture(string name, Transform rootTran = null)
    {
        UITexture tex;

        if (m_dictTexture.ContainsKey(name))
        {
            MFUIUtils.MFUIDebug(string.Concat(name, " is Already in the Texture Dict ! Now Instead it of this !"));
        }

        if (rootTran == null)
        {
            tex = m_myTransform.Find(MFUIUtils.GetFullName(name)).GetComponentsInChildren<UITexture>(true)[0];
        }
        else
        {
            tex = rootTran.Find(name).GetComponentsInChildren<UITexture>(true)[0];
        }

        m_dictTexture[name] = tex;

        return tex;
    }

    public MFUIButtonHandler RegisterButtonHandler(string name, Transform rootTran = null)
    {
        MFUIButtonHandler bh;

        if (m_dictButtonAction.ContainsKey(name))
        {
            MFUIUtils.MFUIDebug(string.Concat(name, " is Already in the buttonHandler Dict ! Now Instead it of this !"));
        }

        if (rootTran == null)
        {
            bh = m_myTransform.Find(MFUIUtils.GetFullName(name)).GetComponentsInChildren<MFUIButtonHandler>(true)[0];
        }
        else
        {
            bh = rootTran.Find(name).GetComponentsInChildren<MFUIButtonHandler>(true)[0];
        }

        m_dictButtonAction[name] = bh;

        return bh;
    }

    public Transform GetTransform(string name)
    {
        if (!m_dictTransform.ContainsKey(name))
        {
            MFUIUtils.MFUIDebug(string.Concat(name, " is not found in TransformDict , now Return Null instead"));

            return null;
        }
        else
        {
            return m_dictTransform[name];
        }
    }
    public UILabel GetLabel(string name)
    {
        if (!m_dictLabel.ContainsKey(name))
        {
            MFUIUtils.MFUIDebug(string.Concat(name, " is not found in LabelDict , now Return Null instead"));

            return null;
        }
        else
        {
            return m_dictLabel[name];
        }
    }

    public UISprite GetSprite(string name)
    {
        if (!m_dictSprite.ContainsKey(name))
        {
            MFUIUtils.MFUIDebug(string.Concat(name, " is not found in SpriteDict , now Return Null instead"));

            return null;
        }
        else
        {
            return m_dictSprite[name];
        }
    }

    public UITexture GetTexture(string name)
    {
        if (!m_dictTexture.ContainsKey(name))
        {
            MFUIUtils.MFUIDebug(string.Concat(name, " is not found in TextrueDict , now Return Null instead"));

            return null;
        }
        else
        {
            return m_dictTexture[name];
        }
    }

    public MFUIButtonHandler GetButtonHandler(string name)
    {
        if (!m_dictButtonAction.ContainsKey(name))
        {
            MFUIUtils.MFUIDebug(string.Concat(name, " is not found in ClickAction Dict , now Return Null instead"));

            return null;
        }
        else
        {
            return m_dictButtonAction[name];
        }
    }






    public void SetLabelText(string name,string text)
    {
        if (!m_dictLabel.ContainsKey(name))
        {
            MFUIUtils.MFUIDebug(string.Concat(name, " is not found in labelDict , SetLabelText Failed!"));

            return;
        }
        else
        {
            m_dictLabel[name].text = text;
        }
    }

    public void SetSpriteImage(string name, string imgName)
    {
        if (!m_dictSprite.ContainsKey(name))
        {
            MFUIUtils.MFUIDebug(string.Concat(name, " is not found in spriteDict , SetSpriteImage Failed!"));

            return;
        }
        else
        {
            m_dictSprite[name].spriteName = imgName;
        }
    }

    public void SetTexture(string name, Texture tex)
    {
        if (!m_dictTexture.ContainsKey(name))
        {
            MFUIUtils.MFUIDebug(string.Concat(name, " is not found in textureDict , SetTexture Failed!"));

            return;
        }
        else
        {
            m_dictTexture[name].mainTexture = tex;
        }
    }

    public void SetButtonClickHandler(string name,Action<int> clickHandler)
    {
        if (!m_dictButtonAction.ContainsKey(name))
        {
            MFUIUtils.MFUIDebug(string.Concat(name, " is not found in ClickHandler Dict , SetButtonClickHandler Failed !"));

            return;
        }
        else
        {
            m_dictButtonAction[name].ClickHandler = clickHandler;
        }
    }

    public void SetButtonPressHandler(string name, Action<bool, int> pressHandler)
    {
        if (!m_dictButtonAction.ContainsKey(name))
        {
            MFUIUtils.MFUIDebug(string.Concat(name, " is not found in PressHandler Dict , SetButtonPressHandler Failed !"));

            return;
        }
        else
        {
            m_dictButtonAction[name].PressHandler = pressHandler;
        }
    }

    public void SetButtonDragHandler(string name, Action<Vector2, int> dragHandler)
    {
        if (!m_dictButtonAction.ContainsKey(name))
        {
            MFUIUtils.MFUIDebug(string.Concat(name, " is not found in Draghandler Dict , SetButtonDragHandler Failed !"));

            return;
        }
        else
        {
            m_dictButtonAction[name].DragHandler = dragHandler;
        }
    }

    private void FillDefaultWidget(Transform rootTransform)
    {
        if (rootTransform == null)
            return;

        UIWidget[] widgetArr = null;

        widgetArr = rootTransform.GetComponentsInChildren<UILabel>(true);

        for (int i = 0; i < widgetArr.Length; ++i)
        {
            if (m_dictLabel.ContainsKey(widgetArr[i].name))
            {
                MFUIUtils.MFUIDebug(string.Concat(widgetArr[i].name, " is Already in the Label Dict ! Now Instead it of this !"));
            }

            m_dictLabel[widgetArr[i].name] = (UILabel)widgetArr[i];
        }

        widgetArr = rootTransform.GetComponentsInChildren<UISprite>(true);

        for (int i = 0; i < widgetArr.Length; ++i)
        {
            if (m_dictSprite.ContainsKey(widgetArr[i].name))
            {
                MFUIUtils.MFUIDebug(string.Concat(widgetArr[i].name, " is Already in the Sprite Dict ! Now Instead it of this !"));
            }

            m_dictSprite[widgetArr[i].name] = (UISprite)widgetArr[i];
        }

        widgetArr = rootTransform.GetComponentsInChildren<UITexture>(true);

        for (int i = 0; i < widgetArr.Length; ++i)
        {
            if (m_dictTexture.ContainsKey(widgetArr[i].name))
            {
                MFUIUtils.MFUIDebug(string.Concat(widgetArr[i].name, " is Already in the Texture Dict ! Now Instead it of this !"));
            }

            m_dictTexture[widgetArr[i].name] = (UITexture)widgetArr[i];
        }

        Transform[] transArr = null;

        transArr = rootTransform.GetComponentsInChildren<Transform>(true);

        for (int i = 0; i < transArr.Length; ++i)
        {
            if (m_dictLabel.ContainsKey(transArr[i].name) || m_dictSprite.ContainsKey(transArr[i].name))
            {
                continue;
            }
            else if (m_dictTransform.ContainsKey(transArr[i].name))
            {
                MFUIUtils.MFUIDebug(string.Concat(transArr[i].name, " is Already in the Transform Dict ! Now Instead it of this !"));
            }

            m_dictTransform[transArr[i].name] = transArr[i];
        }

        //if (widgetArr.Length > 0)
        //{
        //    if (m_dictLabel.ContainsKey(rootTransform.name))
        //    {
        //        MFUIUtils.MFUIDebug(string.Concat(name, " is Already in the Label Dict ! Now Instead it of this !"));
        //    }

        //    m_dictLabel[rootTransform.name] = (UILabel)widgetArr[0];
        //}

        //widgetArr = rootTransform.GetComponentsInChildren<UISprite>(true);

        //if (widgetArr.Length > 0)
        //{
        //    if (m_dictSprite.ContainsKey(rootTransform.name))
        //    {
        //        MFUIUtils.MFUIDebug(string.Concat(name, " is Already in the Sprite Dict ! Now Instead it of this !"));
        //    }

        //    m_dictSprite[rootTransform.name] = (UISprite)widgetArr[0];
        //}

    }
    public void FillAllDefaultWidgets(Transform rootTransform)
    {

        if (rootTransform != null)
        {
            FillDefaultWidget(rootTransform);
            //for (int i = 0; i < rootTransform.childCount; ++i)
            //{
            //    FillDefaultWidget(rootTransform.GetChild(i));
            //}
        }
    }

}

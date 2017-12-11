using UnityEngine;

public class MogoUICursor : MonoBehaviour
{
    static MogoUICursor mInstance;

    public Camera uiCamera;

    Transform mTrans;
    //UISprite mSprite;

    //UIAtlas mAtlas;
    //string mSpriteName;

    public bool IsDragging = true;

    void Awake() 
    { 
        mInstance = this;
    }
    void OnDestroy() 
    {
        mInstance = null; 
    }

    void Start()
    {
        mTrans = transform;
        //mSprite = GetComponentInChildren<UISprite>();
        //mAtlas = mSprite.atlas;
        //mSpriteName = mSprite.spriteName;
        //mSprite.depth = 100;
        if (uiCamera == null) uiCamera = NGUITools.FindCameraForLayer(gameObject.layer);
    }

    void Update()
    {
        //if (mSprite.atlas != null)
        //{

        if (IsDragging)
        {
            Vector3 pos = Input.mousePosition;

            if (uiCamera != null)
            {
                pos.x = Mathf.Clamp01(pos.x / Screen.width);
                pos.y = Mathf.Clamp01(pos.y / Screen.height);
                mTrans.position = uiCamera.ViewportToWorldPoint(pos);

                if (uiCamera.orthographic)
                {
                    mTrans.localPosition = NGUIMath.ApplyHalfPixelOffset(mTrans.localPosition, mTrans.localScale);
                }
            }
            else
            {
                pos.x -= Screen.width * 0.5f;
                pos.y -= Screen.height * 0.5f;
                mTrans.localPosition = NGUIMath.ApplyHalfPixelOffset(pos, mTrans.localScale);
            }
        }
        //}
    }
    static public void Clear()
    {
        //Set(mInstance.mAtlas, mInstance.mSpriteName);
    }

    static public void Set(UIAtlas atlas, string sprite)
    {
        if (mInstance != null)
        {
            //mInstance.mSprite.atlas = atlas;
            //mInstance.mSprite.spriteName = sprite;
            //mInstance.mSprite.MakePixelPerfect();
            mInstance.Update();
        }
    }
}

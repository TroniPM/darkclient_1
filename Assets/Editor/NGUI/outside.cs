using UnityEngine;
using System.Collections;
public class outside : MonoBehaviour 
{
    public Texture2D CursorTex;
    private Vector3 m_vec3MouseOld;
    private Vector3 m_vec3MouseNew;

    void Start () 
    {

    }

    void OnHover(bool isOver)
    {
        if (isOver)
        {

            Cursor.SetCursor(CursorTex, Vector2.zero, CursorMode.Auto);
            Mogo.Util.LoggerHelper.Debug("HoverIn");
        }
        else
        {
            Cursor.SetCursor(null,Vector2.zero,CursorMode.Auto);
            Mogo.Util.LoggerHelper.Debug("HoverOut");
        }
    }


    void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            m_vec3MouseOld = Input.mousePosition;
        }
    }

    void OnDrag()
    {
        Mogo.Util.LoggerHelper.Debug("OnDrag");

        m_vec3MouseNew = Input.mousePosition;



        Mogo.Util.LoggerHelper.Debug(" mouseOld = " + m_vec3MouseOld + " mouseNew = " + m_vec3MouseNew);
        //transform.localScale = new Vector3(transform.localScale.x + m_vec3MouseNew.x - m_vec3MouseOld.x,
        //    transform.localScale.y, transform.localScale.z);
        //transform.parent.GetChild(0).GetComponentInChildren<UISlicedSprite>().transform.localScale = new Vector3(
        //    transform.parent.GetChild(0).GetComponentInChildren<UISlicedSprite>().transform.localScale.x + m_vec3MouseNew.x - m_vec3MouseOld.x,
        //    transform.parent.GetChild(0).GetComponentInChildren<UISlicedSprite>().transform.localScale.y,
        //    transform.parent.GetChild(0).GetComponentInChildren<UISlicedSprite>().transform.localScale.z);

        transform.localScale = new Vector3(transform.localScale.x + (m_vec3MouseNew.x - m_vec3MouseOld.x)*0.005f,
            transform.localScale.y, transform.localScale.z);

        m_vec3MouseOld = m_vec3MouseNew;
    }
}

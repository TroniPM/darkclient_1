using UnityEngine;
using System.Collections;

public class MFUIGameObject
{
    public GameObject MFUIgameObject;

    Vector3 m_vec3InfiPos;
    Vector3 m_vec3CurrentPos;

    public MFUIGameObject()
    { }
    public MFUIGameObject(GameObject go)
    {
        MFUIgameObject = go;

        m_vec3InfiPos = new Vector3(888888, 888888, 888888);
        m_vec3CurrentPos = Vector3.zero;
    }

    ~MFUIGameObject()
    {
        if (MFUIgameObject != null)
        {
            GameObject.Destroy(MFUIgameObject);//未生效，暂不处理
            MFUIgameObject = null;
        }
    }

    public void SetActive(bool isActive)
    {
        MFUIgameObject.SetActive(isActive);
    }

}

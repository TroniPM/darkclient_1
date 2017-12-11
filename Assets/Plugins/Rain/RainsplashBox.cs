#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：CreateScene
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.4.15
// 模块描述：场景初始化类。
//----------------------------------------------------------------*/
#endregion

using UnityEngine;

public class RainsplashBox : MonoBehaviour
{
    private MeshFilter mf;
    private Bounds bounds;

    private RainsplashManager manager;

    public void Start()
    {
        transform.localRotation = Quaternion.identity;

        manager = transform.parent.GetComponent<RainsplashManager>();
        bounds = new Bounds(new Vector3(transform.position.x, 0.0f, transform.position.z),
                             new Vector3(manager.areaSize, Mathf.Max(manager.areaSize, manager.areaHeight), manager.areaSize));

        mf = GetComponent<MeshFilter>();
        mf.sharedMesh = manager.GetPreGennedMesh();

        enabled = false;
    }

    void OnBecameVisible()
    {
        enabled = true;
    }

    void OnBecameInvisible()
    {
        enabled = false;
    }

    void OnDrawGizmos()
    {
        if (transform.parent)
        {
            manager = transform.parent.GetComponent<RainsplashManager>();
            Gizmos.color = new Color(0.5f, 0.5f, 0.65f, 0.5f);
            if (manager)
                Gizmos.DrawWireCube(transform.position + new Vector3((float)(transform.up.x * manager.areaHeight * 0.5), (float)(transform.up.y * manager.areaHeight * 0.5), (float)(transform.up.z * manager.areaHeight * 0.5)),
                                        new Vector3(manager.areaSize, manager.areaHeight, manager.areaSize));
        }
    }

}
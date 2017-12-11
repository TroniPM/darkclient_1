#region ģ����Ϣ
/*----------------------------------------------------------------
// Copyright (C) 2013 ���ݣ�����
//
// ģ������CreateScene
// �����ߣ�Ash Tang
// �޸����б�
// �������ڣ�2013.4.15
// ģ��������������ʼ���ࡣ
//----------------------------------------------------------------*/
#endregion

using UnityEngine;

public class RainBox : MonoBehaviour
{
    private MeshFilter mf;
    private Vector3 defaultPosition;
    private Bounds bounds;

    private RainManager manager;

    private Transform cachedTransform;
    private float cachedMinY;
    private float cachedAreaHeight;
    private float cachedFallingSpeed;

    void Start()
    {
        manager = transform.parent.GetComponent<RainManager>();

        bounds = new Bounds(new Vector3(transform.position.x, manager.minYPosition, transform.position.z),
                             new Vector3(manager.areaSize * 1.35f, Mathf.Max(manager.areaSize, manager.areaHeight) * 1.35f, manager.areaSize * 1.35f));

        mf = GetComponent<MeshFilter>();
        mf.sharedMesh = manager.GetPreGennedMesh();

        cachedTransform = transform;
        cachedMinY = manager.minYPosition;
        cachedAreaHeight = manager.areaHeight;
        cachedFallingSpeed = manager.fallingSpeed;

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

    void Update()
    {
        cachedTransform.position -= Vector3.up * Time.deltaTime * cachedFallingSpeed;

        if (cachedTransform.position.y + cachedAreaHeight < cachedMinY)
        {
            cachedTransform.position = cachedTransform.position + new Vector3((float)(Vector3.up.x * cachedAreaHeight * 2.0), (float)(Vector3.up.y * cachedAreaHeight * 2.0), (float)(Vector3.up.z * cachedAreaHeight * 2.0));
        }
    }
}
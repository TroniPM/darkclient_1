using UnityEngine;
using System.Collections.Generic;
using System;
using Mogo.Util;

public class MogoShadow : MonoBehaviour
{
    MeshFilter m_meshFilter;
    Mesh m_mesh;

    Vector3[] m_vectices;
    int[] m_indices;
    Vector2[] m_TexCoords;
    Vector3[] m_normals;

    string shadowName;
    // 为去除警告暂时屏蔽以下代码
    //bool m_bIs = true;

    public int column = 3;
    public int row = 3;
    public float tileWidth;

    void Start()
    {
        m_meshFilter = (MeshFilter)transform.GetComponent(typeof(MeshFilter));
        m_mesh = m_meshFilter.mesh;

        m_vectices = new Vector3[column * row];
        m_indices = new int[6 * (column-1) * (row-1)];
        m_TexCoords = new Vector2[column * row];
        m_normals = new Vector3[column * row];

        for (int i = 0; i < column; ++i)
        {
            for (int j = 0; j < row; ++j)
            {
                m_vectices[i * column + j].x = -(column - 1) * tileWidth * 0.5f + j * tileWidth;
                m_vectices[i * column + j].y = 2;
                m_vectices[i * column + j].z = (column - 1) * tileWidth * 0.5f - i * tileWidth;
            }
        }

        //m_vectices[0] = new Vector3(-1, 2, 1);
        //m_vectices[1] = new Vector3(0, 2, 1);
        //m_vectices[2] = new Vector3(1, 2, 1);

        //m_vectices[3] = new Vector3(-1, 2, 0);
        //m_vectices[4] = new Vector3(0, 2, 0);
        //m_vectices[5] = new Vector3(1, 2, 0);

        //m_vectices[6] = new Vector3(-1, 2, -1);
        //m_vectices[7] = new Vector3(0, 2, -1);
        //m_vectices[8] = new Vector3(1, 2, -1);

        //m_indices[0] = 3;
        //m_indices[1] = 0;
        //m_indices[2] = 1;

        //m_indices[3] = 3;
        //m_indices[4] = 1;
        //m_indices[5] = 4;

        //m_indices[6] = 4;
        //m_indices[7] = 1;
        //m_indices[8] = 2;

        //m_indices[9] = 4;
        //m_indices[10] = 2;
        //m_indices[11] = 5;

        //m_indices[12] = 6;
        //m_indices[13] = 3;
        //m_indices[14] = 4;

        //m_indices[15] = 6;
        //m_indices[16] = 4;
        //m_indices[17] = 7;

        //m_indices[18] = 7;
        //m_indices[19] = 4;
        //m_indices[20] = 5;

        //m_indices[21] = 7;
        //m_indices[22] = 5;
        //m_indices[23] = 8;

        for (int i = 0; i < column-1; ++i)
        {
            for (int j = 0; j < row - 1; ++j)
            {
                m_indices[6 * ((i * (column - 1)) + j) + 0] = column + column * i + j;
                m_indices[6 * ((i * (column - 1)) + j) + 1] = 0 + column * i + j;
                m_indices[6 * ((i * (column - 1)) + j) + 2] = 1 + column * i + j;

                m_indices[6 * ((i * (column - 1)) + j) + 3] = column + column * i + j;
                m_indices[6 * ((i * (column - 1)) + j) + 4] = 1 + column * i + j;
                m_indices[6 * ((i * (column - 1)) + j) + 5] = column + 1 + column * i + j;
            }
        }

        m_TexCoords[0] = new Vector2(0,0);
        m_TexCoords[1] = new Vector2(0.5f,0);
        m_TexCoords[2] = new Vector2(1,0);
        m_TexCoords[3] = new Vector2(0,0.5f);
        m_TexCoords[4] = new Vector2(0.5f,0.5f);
        m_TexCoords[5] = new Vector2(1,0.5f);
        m_TexCoords[6] = new Vector2(0,1);
        m_TexCoords[7] = new Vector2(0.5f,1);
        m_TexCoords[8] = new Vector2(1,1);

        float UVSpace = 1.0f / (column - 1);

        for (int i = 0; i < column; ++i)
        {
            for (int j = 0; j < row; ++j)
            {
                m_TexCoords[i * column + j] = new Vector2(i * UVSpace, j * UVSpace);
            }
        }

        for (int i = 0; i < column * row; ++i)
        {
            m_normals[i] = new Vector3(0, 1, 0);
        }

        m_mesh.vertices = m_vectices;

        m_mesh.triangles = m_indices;

        m_mesh.uv = m_TexCoords;

       // m_mesh.normals = m_normals;

        shadowName = name + "_ShadowPos";
        EventDispatcher.AddEventListener<Vector3>(shadowName, OnUpdateShadowPos);

        Mogo.Util.LoggerHelper.Debug(shadowName);
    }

    void Update()
    {
        for (int i = 0; i < column * row; ++i)
        {
            RaycastHit hit = new RaycastHit();

            Physics.Raycast(new Vector3((m_vectices[i] + transform.position).x, 100, (m_vectices[i] + transform.position).z), new Vector3(0, -1, 0), out hit, 10000,1<<9);

            m_vectices[i] = new Vector3(m_vectices[i].x, hit.point.y + 0.5f, m_vectices[i].z);
     
        }

        m_mesh.vertices = m_vectices;


    }

    void OnUpdateShadowPos(Vector3 pos)
    {
        transform.position = new Vector3(pos.x, 0, pos.z);
    }
    public void Release()
    {
        EventDispatcher.RemoveEventListener<Vector3>(shadowName, OnUpdateShadowPos);
    }

}

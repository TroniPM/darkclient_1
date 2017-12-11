using UnityEngine;
using System.Collections.Generic;
using System;
using Mogo.Util;

public class MogoSimpleShadow : MonoBehaviour
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

    void Start()
    {
        m_meshFilter = (MeshFilter)transform.GetComponent(typeof(MeshFilter));
        m_mesh = m_meshFilter.mesh;

        m_vectices = new Vector3[4];
        m_indices = new int[6];
        m_TexCoords = new Vector2[4];
        m_normals = new Vector3[4];

        m_vectices[0] = new Vector3(-0.5f, 0, 0.5f);
        m_vectices[1] = new Vector3(-0.5f, 0, -0.5f);
        m_vectices[2] = new Vector3(0.5f,0,-0.5f);
        m_vectices[3] = new Vector3(0.5f, 0, 0.5f);

        m_indices[0] = 0;
        m_indices[1] = 2;
        m_indices[2] = 1;

        m_indices[3] = 0;
        m_indices[4] = 3;
        m_indices[5] = 2;

        m_TexCoords[0] = new Vector2(0, 1);
        m_TexCoords[1] = new Vector2(0, 0);
        m_TexCoords[2] = new Vector2(1, 0);
        m_TexCoords[3] = new Vector2(1, 1);

        m_mesh.vertices = m_vectices;

        m_mesh.triangles = m_indices;

        m_mesh.uv = m_TexCoords;

        m_mesh.normals = m_normals;

        shadowName = name + "_ShadowPos";

    }
}

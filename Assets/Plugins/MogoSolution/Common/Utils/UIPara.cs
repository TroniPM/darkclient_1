#region ģ����Ϣ
/*----------------------------------------------------------------
// Copyright (C) 2013 ���ݣ�����
//
// ģ������UIPara
// �����ߣ�Ash Tang
// �޸����б��
// �������ڣ�2013.2.26
// ģ��������UI������ʾ�ࡣ
//----------------------------------------------------------------*/
#endregion
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

[RequireComponent(typeof(GUIText))]
public class UIPara : MonoBehaviour
{
    public float updateInterval = 0.5f;

    // checkbox item for picking up the information
    public bool isPaticleCountShown = false;
    public bool isFpsShown = true;
    public bool isVerticesShown = false;
    public bool isTrianglesShown = false;
    public bool isMeshesShown = false;

    private float accum = 0.0f; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private float timeleft; // Left time for current interval	// Use this for initialization

    // private String meshInfo;

    void Start()
    {
        DontDestroyOnLoad(this);
        timeleft = updateInterval;
    }
    // Update is called once per frame
    void Update()
    {
        timeleft -= Time.deltaTime;
        accum += (float)(Time.timeScale / Time.deltaTime);
        ++frames;
        try
        {
            float fps = 0;
            Int32 particleCount = 0;
            int vertices = 0;
            int meshTriangles = 0;
            int skinnedMeshTriangles = 0;
            int meshCount = 0;

            if (timeleft <= 0.0)
            {
                var objs = UnityEngine.Object.FindObjectsOfType(typeof(GameObject));

                if (isFpsShown)
                    HandleFPS(ref fps);
                if (isPaticleCountShown)
                    HandleParticleCount(objs, ref particleCount);
                if (isVerticesShown || isTrianglesShown || isMeshesShown)           // HandleVerticesInfo() will be called if any of these values is true.
                    HandleVerticesInfo(objs, ref vertices, ref meshTriangles, ref skinnedMeshTriangles, ref meshCount);

                UpdateMessage(fps, particleCount, vertices, meshTriangles, skinnedMeshTriangles, meshCount);
                timeleft = updateInterval;
                accum = 0.0f;
                frames = 0;
            }
        }
        catch (Exception ex)
        {
            print(ex.Message);
        }
    }

    // Update the text
    private void UpdateMessage(        
            float fps,
            Int32 particleCount,
            int vertices,
            int meshTriangles,
            int skinnedMeshTriangles,
            int meshCount)
    {
        StringBuilder sb = new StringBuilder();
        if (isFpsShown)             // Show FPS
            sb.AppendFormat("FPS: {0}", fps.ToString("f2"));
        sb.Append("\n");
        if (isPaticleCountShown)    // Show the number of paticles
            sb.AppendFormat("Paticle count: {0}", particleCount);
        sb.Append("\n");
        if (isVerticesShown)        // Show vertices 
            sb.AppendFormat("vertices: {0}", vertices);
        sb.Append("\n");
        if (isTrianglesShown)       // Show triangles
            sb.AppendFormat("triangles: {0}(m: {1}; s: {2})", meshTriangles + skinnedMeshTriangles, meshTriangles, skinnedMeshTriangles);
        sb.Append("\n");
        if (isMeshesShown)          // Show meches     
            sb.AppendFormat("meshes: {0}", meshCount);
        sb.Append("\n");
        GetComponent<GUIText>().text = sb.ToString();
    }

    private void HandleFPS(ref float fps)
    {
        fps = accum / frames;
    }

    private void HandleParticleCount(UnityEngine.Object[] objs, ref Int32 particleCount)
    {
        particleCount = 0;
        foreach (GameObject go in objs)
        {
            if (go.GetComponent<ParticleEmitter>() != null)
            {
                particleCount += go.GetComponent<ParticleEmitter>().particleCount;
            }
        }
    }

    // count the number of vertices
    private void HandleVerticesInfo(UnityEngine.Object[] objs, ref int vertices, ref int meshTriangles, ref int skinnedMeshTriangles, ref int meshCount)
    {
        vertices = 0;
        skinnedMeshTriangles = 0;
        meshTriangles = 0;
        meshCount = 0;
        foreach (GameObject go in objs)
        {
            HandleMeshFilterInfo(go, ref vertices, ref meshTriangles, ref meshCount);
            HandleSkinnedMeshRendererInfo(go, ref vertices, ref skinnedMeshTriangles, ref meshCount);
        }
    }

    // MeshFilter
    private void HandleMeshFilterInfo(UnityEngine.GameObject go, ref int vertices, ref int meshTriangles, ref int meshCount)
    {

        Component[] meshFilters = go.GetComponentsInChildren(typeof(MeshFilter));

        for (int meshFiltersIndex = 0; meshFiltersIndex < meshFilters.Length; meshFiltersIndex++)
        {
            MeshFilter meshFilter = (MeshFilter)meshFilters[meshFiltersIndex];
            vertices += meshFilter.sharedMesh.vertexCount;
            meshTriangles += meshFilter.sharedMesh.triangles.Length / 3;
            meshCount++;
        }
    }

    // SkinnedMeshRenderer
    private void HandleSkinnedMeshRendererInfo(UnityEngine.GameObject go, ref int vertices, ref int skinnedMeshTriangles, ref int meshCount)
    {

        Component[] skinnedMeshes = go.GetComponentsInChildren(typeof(SkinnedMeshRenderer));

        for (int skinnedMeshIndex = 0; skinnedMeshIndex < skinnedMeshes.Length; skinnedMeshIndex++)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)skinnedMeshes[skinnedMeshIndex];
            vertices += skinnedMeshRenderer.sharedMesh.vertexCount;
            skinnedMeshTriangles += skinnedMeshRenderer.sharedMesh.triangles.Length / 3;
            meshCount++;
        }
    }
}
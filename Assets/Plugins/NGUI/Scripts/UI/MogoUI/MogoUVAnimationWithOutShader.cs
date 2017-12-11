using UnityEngine;
using System.Collections;

public class MogoUVAnimationWithOutShader : MonoBehaviour 
{
	int scrollSpeed  = 30;
	int countX = 8;
	int countY = 8;
	
	MeshFilter m_meshFilter;
    Mesh m_mesh;
	
	 Vector3[] m_vectices;
    int[] m_indices;
    Vector2[] m_TexCoords;
	
	float offsetX = 0.0f;
	float offsetY = 0.0f;
	Vector2 singleTexSize;
	
	void Start()
	{
		 m_meshFilter = (MeshFilter)transform.GetComponent(typeof(MeshFilter));
        m_mesh = m_meshFilter.mesh;

        m_vectices = new Vector3[4];
        m_indices = new int[6];
        m_TexCoords = new Vector2[4];
		
        m_vectices[0] = new Vector3(-1,-1,0);
		m_vectices[1] = new Vector3(-1,1,0);
		m_vectices[2] = new Vector3(1,1,0);
		m_vectices[3] = new Vector3(1,-1,0);

        m_indices[0] = 0;
		m_indices[1] = 1;
		m_indices[2] = 2;
		m_indices[3] = 0;
		m_indices[4] = 2;
		m_indices[5] = 3;

        m_TexCoords[0] = new Vector2(0,0);
        m_TexCoords[1] = new Vector2(0,1);
        m_TexCoords[2] = new Vector2(1,1);
        m_TexCoords[3] = new Vector2(1,0);

        m_mesh.vertices = m_vectices;

        m_mesh.triangles = m_indices;

        m_mesh.uv = m_TexCoords;
		
		singleTexSize = new Vector2(1.0f / countX,1.0f / countY);
		GetComponent<Renderer>().material.mainTextureScale = singleTexSize;
		
	}
	
	void Update()
	{
		float frame = Mathf.Floor(Time.time * scrollSpeed);
		offsetX = frame / countX;
		offsetY = -(frame - frame % countX) / countY / countX;
		
		GetComponent<Renderer>().material.SetTextureOffset("_MainTex",new Vector2(offsetX,offsetY));
	} 
}

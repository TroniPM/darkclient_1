using UnityEngine;
using System.Collections;

public class MogoUVAnimationWithShader : MonoBehaviour {

	MeshFilter m_meshFilter;
    Mesh m_mesh;

    Vector3[] m_vectices;
    int[] m_indices;
    Vector2[] m_TexCoords;
	
	float m_deltaTime;
	
	public int FPS = 30;
	public int Cols = 8;
	public int Rows = 8;
	
	float m_frameSpeed;
	
	int m_iIndex = 0;

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

        m_TexCoords[0] = new Vector2(0,1);
        m_TexCoords[1] = new Vector2(0,0);
        m_TexCoords[2] = new Vector2(1,0);
        m_TexCoords[3] = new Vector2(1,1);

        m_mesh.vertices = m_vectices;

        m_mesh.triangles = m_indices;

        m_mesh.uv = m_TexCoords;
		
		m_frameSpeed = 1.0f / FPS;
		
		SetGridWidth(1.0f / Cols);
		SetGridHeight(1.0f / Rows);
	}
	
	void Update()
	{	
		
		m_deltaTime += Time.deltaTime;
		
		if(m_deltaTime > m_frameSpeed)
		{
			if(++m_iIndex > Rows * Cols-1)
				m_iIndex = 0;
			SetIndex(m_iIndex);
			m_deltaTime = 0;
		}
		
	}
	
	public void SetIndex(int i)
	{
		m_iIndex = i;	
		GetComponent<Renderer>().material.SetFloat("_IndexX",i % Cols);
		GetComponent<Renderer>().material.SetFloat("_IndexY",i / Cols);
	}
	
	public void SetGridWidth(float width)
	{

		GetComponent<Renderer>().material.SetFloat("_Width",width);
	}
	
	public void SetGridHeight(float height)
	{
		
		GetComponent<Renderer>().material.SetFloat("_Height",height);
	}
}

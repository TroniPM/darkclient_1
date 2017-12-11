using UnityEngine;

public class MogoTweenAlpha : UITweener
{
    public float from = 1f;
    public float to = 1f;

    Transform m_myTransform;
    Material m_goMaterial;
    System.Collections.Generic.List<Material> m_goMatList = new System.Collections.Generic.List<Material>();


    public float alpha
    {
        get
        {
            if (m_goMaterial != null)
            {
                return m_goMaterial.GetColor("_Color").a;
            }
            return 0f;
        }
        set
        {
            //if (m_goMaterial != null)
            //{
            //    m_goMaterial.SetColor("_Color", new Color(m_goMaterial.GetColor("_Color").r, m_goMaterial.GetColor("_Color").g,
            //        m_goMaterial.GetColor("_Color").b, value));

            //    //m_myTransform.GetComponentsInChildren<SkinnedMeshRenderer>(true)[0].material = m_goMaterial;
            //}

            for (int i = 0; i < m_goMatList.Count; ++i)
            {
				if(m_goMatList[i]!=null)
				{
                	m_goMatList[i].SetColor("_Color", new Color(m_goMatList[i].GetColor("_Color").r, m_goMatList[i].GetColor("_Color").g,
                    	m_goMatList[i].GetColor("_Color").b, value));
				}
            }
        }
    }

    void Awake()
    {
        m_myTransform = transform;
        //m_goMaterial = m_myTransform.GetComponentsInChildren<SkinnedMeshRenderer>(true)[0].sharedMaterial;
        var smr = m_myTransform.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        for (int i = 0; i < smr.Length; ++i)
        {
            m_goMatList.Add(smr[i].sharedMaterial);
        }

        var mr = m_myTransform.GetComponentsInChildren<MeshRenderer>(true);
        for (int i = 0; i < mr.Length; ++i)
        {
            m_goMatList.Add(mr[i].sharedMaterial);
        }
    }

    /// <summary>
    /// Interpolate and update the alpha.
    /// </summary>

    override protected void OnUpdate(float factor, bool isFinished) { alpha = Mathf.Lerp(from, to, factor); }

    ///// <summary>
    ///// Start the tweening operation.
    ///// </summary>

    //static public TweenAlpha Begin(GameObject go, float duration, float alpha)
    //{
    //    TweenAlpha comp = UITweener.Begin<TweenAlpha>(go, duration);
    //    comp.from = comp.alpha;
    //    comp.to = alpha;

    //    if (duration <= 0f)
    //    {
    //        comp.Sample(1f, true);
    //        comp.enabled = false;
    //    }
    //    return comp;
    //}
}

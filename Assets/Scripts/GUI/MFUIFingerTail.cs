using UnityEngine;
using System.Collections;
using Mogo.Util;
using Mogo.GameData;

public class MFUIFingerTail : MonoBehaviour
{
    public Camera UICamera;
    public GameObject GoTail;

    private MeshFilter m_meshFilter;
    private Mesh m_mesh;
    private GeometricRecognizer m_recognizer;

    public int SampleTimes = 1; //SampleTimes帧采样一次
    private int deltaSampleNum = 0;
    private Vector3 LastTouchPoint = Vector3.zero;
    private Vector3 beginTouchPoint = Vector3.zero;
    private Vector3 releaseTouchPoint = Vector3.zero;
    private bool StartDragging = false;
    public float TailWidth = 1f;    

    private System.Collections.Generic.List<Vector3> m_vectices = new System.Collections.Generic.List<Vector3>();
    private System.Collections.Generic.List<int> m_indices = new System.Collections.Generic.List<int>();
    private System.Collections.Generic.List<Vector2> m_TexCoords = new System.Collections.Generic.List<Vector2>();
    private System.Collections.Generic.List<Point2D> m_listPoint = new System.Collections.Generic.List<Point2D>();

    private bool m_bLoadResourceInsteadOfAwake = false;
    public void LoadResourceInsteadOfAwake()
    {
        if (m_bLoadResourceInsteadOfAwake)
            return;

        m_bLoadResourceInsteadOfAwake = true;

        m_meshFilter = (MeshFilter)GoTail.transform.GetComponent<MeshFilter>();
        m_mesh = m_meshFilter.mesh;
        m_recognizer = new GeometricRecognizer();
        m_recognizer.LoadTemplates();
    }

    #region 手势轨迹

    void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            m_mesh.Clear();            
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                LastTouchPoint = UICamera.ScreenToWorldPoint(Input.GetTouch(0).position);
                beginTouchPoint = UICamera.ScreenToWorldPoint(Input.GetTouch(0).position);
            }
            else
            {
                LastTouchPoint = UICamera.ScreenToWorldPoint(Input.mousePosition);
                beginTouchPoint = UICamera.ScreenToWorldPoint(Input.mousePosition);
            }           
        }
        else
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                releaseTouchPoint = UICamera.ScreenToWorldPoint(Input.GetTouch(0).position);
            }
            else
            {
                releaseTouchPoint = UICamera.ScreenToWorldPoint(Input.mousePosition);
            }
            RecognitionResult result = m_recognizer.recongnize(m_listPoint);
            StartDragging = false;
            m_indices.Clear();
            m_vectices.Clear();
            m_TexCoords.Clear();
            m_listPoint.Clear();
            m_mesh.Clear();

            // 发送手势结果事件
            if (result.name.Equals(GestureName.Triangle))
            {
                EventDispatcher.TriggerEvent(Events.UIBattleEvent.OnSpriteSkill, (int)GestureNameEnum.Triangle);
            }
            else if (result.name.Equals(GestureName.Circle))
            {
                EventDispatcher.TriggerEvent(Events.UIBattleEvent.OnSpriteSkill, (int)GestureNameEnum.Circle);
            }
            else if (result.name.Equals(GestureName.Rectangle))
            {
                EventDispatcher.TriggerEvent(Events.UIBattleEvent.OnSpriteSkill, (int)GestureNameEnum.Rectangle);
            }
            else if (result.name.Equals(GestureName.HorizontalLine))
            {
                EventDispatcher.TriggerEvent(Events.UIBattleEvent.OnSpriteSkill, (int)GestureNameEnum.HorizontalLine);
            }
            else if (result.name.Equals(GestureName.ZShape))
            {
                EventDispatcher.TriggerEvent(Events.UIBattleEvent.OnSpriteSkill, (int)GestureNameEnum.ZShape);
            }
            else
            {
                EventDispatcher.TriggerEvent(Events.UIBattleEvent.OnSpriteSkill, (int)GestureNameEnum.None);
            }
            Debug.LogError(releaseTouchPoint);
            Debug.LogError(beginTouchPoint);
            Vector3 direction = releaseTouchPoint - beginTouchPoint;
            CastSkill(direction);
        }
    }

    void OnDrag(Vector2 dir)
    {
        StartDragging = true;
    }
    void CastSkill(Vector2 dir)
    {
        int skillId = (int)MogoWorld.thePlayer.ElfEquipSkillId;
        Debug.LogError(skillId);
        Debug.LogError(dir);
        MogoWorld.thePlayer.CastSkill(skillId,dir);
        SkillData s = SkillData.dataMap[skillId];
        MainUIViewManager.Instance.SpellOneCD(s.cd[0]);
        MainUIViewManager.Instance.ShowMainUISpriteSkillPanel(false);
    }

    void Update()
    {
        if (!StartDragging)
        {
            ReleaseFingerTailFX();
            return;
        }

        ++deltaSampleNum;

        if (deltaSampleNum >= SampleTimes)
        {
            UpdateMesh();
            deltaSampleNum = 0;
        }
    }

    public void ClearMesh()
    {
        m_mesh.Clear();
        deltaSampleNum = 0;
    }

    void UpdateMesh()
    {
        Vector3 lastPoint = LastTouchPoint;
        Vector3 currentPoint = Vector3.zero;

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            currentPoint = UICamera.ScreenToWorldPoint(Input.GetTouch(0).position);
            ShowFingerTailFX(currentPoint);
        }
        else
        {
            currentPoint = UICamera.ScreenToWorldPoint(Input.mousePosition);
            ShowFingerTailFX(currentPoint);
        }

        if (m_vectices.Count == 0)
        {
            m_vectices.Add(lastPoint);
        }


        if (lastPoint == currentPoint)
            return;

        m_listPoint.Add(new Point2D(currentPoint.x, currentPoint.y));

        CalculateMeshVertexPos(lastPoint, currentPoint);

        m_mesh.vertices = m_vectices.ToArray();

        CalculateMeshVertexIndex();

        LastTouchPoint = currentPoint;

        CalculateMeshVertexTexCoord();
    }

    void CalculateMeshVertexPos(Vector3 lastPoint, Vector3 curentPoint)
    {
        if (curentPoint.y != lastPoint.y)
        {
            float k = (lastPoint.x - curentPoint.x) / (curentPoint.y - lastPoint.y);


            Vector3 p0 = Vector3.zero;
            Vector3 p1 = Vector3.zero;

            float deltaX = Mathf.Sqrt(TailWidth * TailWidth / (k * k + 1));


            if ((curentPoint - lastPoint).y > 0)
            {
                p0.x = curentPoint.x - deltaX;
                p1.x = curentPoint.x + deltaX;
            }
            else
            {
                p0.x = curentPoint.x + deltaX;
                p1.x = curentPoint.x - deltaX;
            }


            p0.y = k * p0.x + curentPoint.y - k * curentPoint.x;
            p1.y = k * p1.x + curentPoint.y - k * curentPoint.x;

            p0.z = 0;
            p1.z = 0;

            m_vectices.Add(p0);
            m_vectices.Add(p1);
        }
        else
        {
            Vector3 p0 = Vector3.zero;
            Vector3 p1 = Vector3.zero;

            p0.x = curentPoint.x;
            p1.x = curentPoint.x;

            if ((curentPoint.x - lastPoint.x) > 0)
            {
                p0.y = curentPoint.y + TailWidth;
                p1.y = curentPoint.y - TailWidth;
            }
            else
            {

                p0.y = curentPoint.y - TailWidth;
                p1.y = curentPoint.y + TailWidth;
            }

            m_vectices.Add(p0);
            m_vectices.Add(p1);
        }
    }

    void CalculateMeshVertexIndex()
    {
        m_indices.Clear();

        int triangleNum = m_vectices.Count - 2;

        m_indices.Add(0);
        m_indices.Add(1);
        m_indices.Add(2);

        for (int i = 0; i < (triangleNum - 2) * 0.5f; ++i)
        {
            m_indices.Add(2 + i * 2);
            m_indices.Add(1 + i * 2);
            m_indices.Add(3 + i * 2);

            m_indices.Add(2 + i * 2);
            m_indices.Add(3 + i * 2);
            m_indices.Add(4 + i * 2);
        }

        m_indices.Add(2 + (triangleNum - 2));
        m_indices.Add(1 + (triangleNum - 2));
        m_indices.Add(3 + (triangleNum - 2));

        m_mesh.triangles = m_indices.ToArray();
    }

    void CalculateMeshVertexTexCoord()
    {
        m_TexCoords.Clear();
        float offset = 1.0f / m_vectices.Count;
        m_TexCoords.Add(new Vector2(0f,0.5f));
        for (int i = 1; i < m_vectices.Count; ++i)
        {
            m_TexCoords.Add(new Vector2(i*offset, i%2));
        }
        m_mesh.uv = m_TexCoords.ToArray();
    }

    #endregion

    #region 手势特效

    private readonly static string FingerTailFXName = "fx_ui_sz.prefab";
    private string m_fxFingerTail = "FingerTailFx";
    private uint m_iFingerTailFXTimerID;
    private readonly static uint FingerTailFXLife = 1000;

    /// <summary>
    /// 显示手势特效
    /// </summary>
    /// <param name="index"></param>
    private void ShowFingerTailFX(Vector3 position)
    {
        GameObject goFX = MogoFXManager.Instance.FindParticeAnim(m_fxFingerTail);
        if (goFX != null)
        {
            TimerHeap.DelTimer(m_iFingerTailFXTimerID);
            m_iFingerTailFXTimerID = TimerHeap.AddTimer((uint)(FingerTailFXLife), 0, () => { ReleaseFingerTailFX(); });
        }
        else
        {
            m_iFingerTailFXTimerID = TimerHeap.AddTimer((uint)(FingerTailFXLife), 0, () => { ReleaseFingerTailFX(); });
            MogoFXManager.Instance.AttachParticleAnim(FingerTailFXName, m_fxFingerTail, position,
                MogoUIManager.Instance.GetMainUICamera(), 0, 0, 0, () =>
                {

                });
        }

        RefreshFingerTailFXPos(position);
    }

    /// <summary>
    /// 刷新手势特效Pos
    /// </summary>
    private void RefreshFingerTailFXPos(Vector3 position)
    {
        GameObject goFX = MogoFXManager.Instance.FindParticeAnim(m_fxFingerTail);
        if (goFX != null)
        {
            MogoFXManager.Instance.TransformToFXCameraPos(goFX, position, MogoUIManager.Instance.GetMainUICamera());
        }
    }

    /// <summary>
    /// 释放手势特效
    /// </summary>
    private void ReleaseFingerTailFX()
    {
        MogoFXManager.Instance.ReleaseParticleAnim(m_fxFingerTail);
    }

    #endregion
}

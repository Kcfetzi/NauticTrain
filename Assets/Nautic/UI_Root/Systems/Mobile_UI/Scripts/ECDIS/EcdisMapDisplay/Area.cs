using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Area : MaskableGraphic
{
    [SerializeField] private bool m_IsStatic;
    [SerializeField] private List<RectTransform> m_Points;

    private RectTransform m_RectTransform;
    public List<RectTransform> m_SortedPoints;

    public Vector2 m_CenterPoint;


    protected override void Awake()
    {
        m_RectTransform = rectTransform;
        CalculatePoints();
    }

    private void Update()
    {
        if (m_IsStatic)
        {
            return;
        }

        m_RectTransform.sizeDelta = m_Points.GetRectSize();
        SetVerticesDirty();
    }

    private void CalculatePoints()
    {
        if (m_Points.Count < 3)
            return;
        
        m_CenterPoint = m_Points.MiddlePosition();
        m_RectTransform.anchoredPosition = m_CenterPoint;
        m_SortedPoints = m_Points.SortCircular(m_CenterPoint);
    }
    
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        base.OnPopulateMesh(vh);
        if (!m_IsStatic)
            CalculatePoints();
        
        vh.Clear();
        
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        for (int i = 0; i < m_SortedPoints.Count; i++)
        {
            Vector2 point = m_SortedPoints[i].anchoredPosition;
            Vector2 nextPoint;
            if (i == m_SortedPoints.Count - 1)
            {
                nextPoint = m_SortedPoints[0].anchoredPosition;
            }
            else
            {
                nextPoint = m_SortedPoints[i + 1].anchoredPosition;
            }
            
            vertex.position =m_CenterPoint;
            vh.AddVert(vertex);
            vertex.position = point - m_CenterPoint;
            vh.AddVert(vertex);
            vertex.position = nextPoint - m_CenterPoint;
            vh.AddVert(vertex);
            vh.AddTriangle(i * 3, i * 3 + 1, i * 3 + 2);
        }
    }
    
    
    public void Test()
    {
        Debug.Log("hit");
    }
}
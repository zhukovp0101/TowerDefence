using System;
using System.Linq.Expressions;
using UnityEngine;

namespace DefaultNamespace
{
    public class MovementCursor : MonoBehaviour
    {
        [SerializeField]
        private int m_GridWidth;
        [SerializeField]
        private int m_GridHeight;

        [SerializeField]
        private float m_NodeSize;
        
        private Vector3 m_Origin;
        
        [SerializeField]
        private MovementAgent m_MovementAgent;
        
        [SerializeField]
        private GameObject m_Cursor;

        private Camera m_Camera;

        private void OnValidate()
        {
            float width = m_GridWidth * m_NodeSize;
            float height = m_GridHeight * m_NodeSize;
            
            // Default plane size is 10 by 10
            transform.localScale = new Vector3(
                width * 0.1f,
                1f, 
                height * 0.1f);

            m_Origin = transform.position - 
                       (new Vector3(width, 0, height) * 0.5f);
        }

        private void Update()
        {
            if (m_Camera == null)
            {
                return;
            }

            Vector3 mousePos = Input.mousePosition;

            Ray ray = m_Camera.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform != transform)
                {
                    return;
                }
                
                Vector2 nodeCenter = GetNodeCenter(GetGridCoordinates(hit.point.x, hit.point.z));
                Vector3 targetPosition = new Vector3(
                    nodeCenter.x,
                    hit.point.y,
                    nodeCenter.y);
                
                if (Input.GetMouseButton(0))
                {
                    m_MovementAgent.SetTarget(targetPosition);
                }
                m_Cursor.transform.position = targetPosition;
                m_Cursor.SetActive(true);
            }
            else
            {
                m_Cursor.SetActive(false);
            }
        }
        
        private void Awake()
        {
            m_Camera = Camera.main;
        }

        private Vector2 GetNodeCenter(Vector2Int gridCoordinates)
        {
            return new Vector2(gridCoordinates.x * m_NodeSize + m_Origin.x + m_NodeSize * 0.5f, 
                gridCoordinates.y * m_NodeSize + m_Origin.z + m_NodeSize * 0.5f);
        }

        private Vector2Int GetGridCoordinates(float x, float y)
        {
            return new Vector2Int((int)((x - m_Origin.x) / m_NodeSize), (int)((y - m_Origin.z) / m_NodeSize));
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            
            //Vertical lines
            float height = m_GridHeight * m_NodeSize;
            for (int i = 0; i <= m_GridWidth; i++)
            {
                Vector3 from = new Vector3(i * m_NodeSize, 0f, 0f) + m_Origin;
                Vector3 to = new Vector3(i * m_NodeSize, 0f, height) + m_Origin;
                Gizmos.DrawLine(from, to);
            }
            
            //Horizontal lines
            float width = m_GridWidth * m_NodeSize;
            for (int i = 0; i <= m_GridHeight; i++)
            {
                Vector3 from = new Vector3(0f, 0f, i * m_NodeSize) + m_Origin;
                Vector3 to = new Vector3(width, 0f, i * m_NodeSize) + m_Origin;
                Gizmos.DrawLine(from, to);
            }
        }
    }
}
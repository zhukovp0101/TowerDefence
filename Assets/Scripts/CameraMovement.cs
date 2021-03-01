using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private float m_Speed;
        
    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
            
        Vector3 delta = new Vector3(horizontal, 0f, vertical) * (m_Speed * Time.deltaTime);
        transform.Translate(delta, Space.World);
    }
}
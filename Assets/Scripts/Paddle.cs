using UnityEngine;

public class Paddle : MonoBehaviour
{
    [SerializeField] float maxDragDistance = 3;
    
    void OnMouseDrag()
    {
        Vector3 newPosition = transform.position;
        newPosition.y = Mathf.Clamp(GetMousePosition().y, -maxDragDistance, maxDragDistance);
        transform.position = newPosition;
    }

    Vector3 GetMousePosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}

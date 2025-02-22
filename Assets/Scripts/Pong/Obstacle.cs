using UnityEngine;

namespace PixelWorld.Pong
{
    public class Obstacle : MonoBehaviour
    {
        [SerializeField] float speed = 2f;
        [SerializeField] float moveDistance = 3f; 
        [SerializeField] int verticalDirection = 1;
        Vector3 startPosition;

        void Start()
        {
            startPosition = transform.position;
        }

        void Update()
        {
            MoveObstacle();
        }

        void MoveObstacle()
        {
            transform.position += Vector3.up * verticalDirection * speed * Time.deltaTime;

            if (Mathf.Abs(transform.position.y - startPosition.y) >= moveDistance)
            {
                verticalDirection *= -1; 
                //transform.position = new Vector3(transform.position.x, startPosition.y + (moveDistance * direction), transform.position.z);
            }
        }
    }
}
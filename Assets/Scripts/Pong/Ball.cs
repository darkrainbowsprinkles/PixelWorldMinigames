using UnityEngine;

namespace PixelWorld.Pong
{
    public class Ball : MonoBehaviour
    {
        [SerializeField] float speed = 1;
        [SerializeField] int scorePoints = 10;
        Rigidbody2D rigidBody;
        Vector3 startPosition;

        public void Reset()
        {
            rigidBody.velocity = Vector2.zero;
            transform.position = startPosition;
            Launch();
        }

        public int GetScorePoints()
        {
            return scorePoints;
        }

        void Awake()
        {
            rigidBody = GetComponent<Rigidbody2D>();
        }

        void Start()
        {
            startPosition = transform.position;
            Launch();
        }

        void Launch()
        {
            rigidBody.velocity = GetRandomDirection() * speed;
        }

        Vector2 GetRandomDirection()
        {
            float randomX = Random.Range(0,2) == 0 ? -1 : 1;
            float randomY = Random.Range(0,2) == 0 ? -1 : 1;
            return new Vector2(randomX, randomY);
        }
    }
}
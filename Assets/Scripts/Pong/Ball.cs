using UnityEngine;

namespace PixelWorld.Pong
{
    public class Ball : MonoBehaviour
    {
        [SerializeField] float speed = 1;
        [SerializeField] int scorePoints = 10;
        [SerializeField] int paddleHitPenalty = 5;
        [SerializeField, Range(0,1)] float splitChance = 0.2f;
        Rigidbody2D rigidBody;
        Vector3 startPosition;
        bool isSplitBall = false;

        public void Reset()
        {
            rigidBody.velocity = Vector2.zero;
            transform.position = startPosition;
            Launch();
        }

        public int GetPaddleHitPenalty()
        {
            return paddleHitPenalty;
        }

        public int GetScorePoints()
        {
            return scorePoints;
        }

        public bool IsSplitBall()
        {
            return isSplitBall;
        }

        public void TrySplit()
        {
            if(isSplitBall)
            {
                return;
            }

            if(Random.value < splitChance)
            {
                Ball newBall = Instantiate(this, transform.position, Quaternion.identity);
                newBall.scorePoints *= 2;
                newBall.isSplitBall = true;
                newBall.GetComponent<SpriteRenderer>().color = Color.yellow;
                LaunchOppositeDirection(newBall);
            }
        }

        void LaunchOppositeDirection(Ball newBall)
        {
            Vector2 currentVelocity = rigidBody.velocity.normalized;
            newBall.rigidBody.velocity = -currentVelocity * speed;
            newBall.transform.position += (Vector3)currentVelocity * 0.2f;
            rigidBody.velocity = currentVelocity * speed;
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
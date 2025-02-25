using UnityEngine;

namespace PixelWorld.Utils
{
    public class LaneManager : MonoBehaviour
    {
        [SerializeField, Range(1, 10)] int numberOfPlayers = 10;
        [SerializeField] GameObject lanePrefab;

        void Start()
        {
            RefreshLanes();
        }

        void RefreshLanes()
        {
            foreach(Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            for(int i = 0; i < numberOfPlayers; i++)
            {
                Instantiate(lanePrefab, transform);
            }
        }
    }
}
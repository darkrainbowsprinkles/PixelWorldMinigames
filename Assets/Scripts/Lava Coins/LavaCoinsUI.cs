using UnityEngine;

namespace PixelWorld.FabulousFred
{
    public class LavaCoinsUI : MonoBehaviour
    {
        [SerializeField, Range(1, 10)] int numberOfPlayers = 10;
        [SerializeField] LavaCoinLaneUI lanePrefab;

        void Start()
        {
            RefreshLanes();
            SpawnLanes();
        }

        void RefreshLanes()
        {
            foreach(Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        void SpawnLanes()
        {
            for(int i = 0; i < numberOfPlayers; i++)
            {
                Instantiate(lanePrefab, transform);
            }
        }
    }
}
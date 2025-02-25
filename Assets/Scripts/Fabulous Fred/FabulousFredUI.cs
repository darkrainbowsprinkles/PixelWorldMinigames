using UnityEngine;

namespace PixelWorld.FabulousFred
{
    public class FabulousFredUI : MonoBehaviour
    {
        [SerializeField, Range(1, 8)] int numberOfPlayers = 8;
        [SerializeField] FabulousFredLaneUI lanePrefab;

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
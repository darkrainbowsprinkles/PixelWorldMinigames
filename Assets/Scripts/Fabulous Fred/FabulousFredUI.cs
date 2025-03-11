using System.Linq;
using UnityEngine;

namespace PixelWorld.FabulousFred
{
    public class FabulousFredUI : MonoBehaviour
    {
        [SerializeField] FabulousFredLaneUI lanePrefab;
        [SerializeField, Range(0,10)] int playerNumber = 10;
        int[] sequenceIndexes;

        void Start()
        {
            RefreshLanes();
            FillSequenceIndexes();
            SpawnLanes();
        }

        void RefreshLanes()
        {
            foreach(Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        void FillSequenceIndexes()
        {
            int count = 0;
            int buttonCount = lanePrefab.GetButtonCount();
            int rowSize = lanePrefab.GetRowSize();

            sequenceIndexes = new int[buttonCount / rowSize];

            for(int i = 0; i < buttonCount; i += rowSize)
            {
                int randomButtonIndex = Random.Range(i, i + rowSize);
                sequenceIndexes[count] = randomButtonIndex;
                count++;
            }
        }

        void SpawnLanes()
        {
            for(int i = 0; i < playerNumber; i++)
            {
                var laneInstance = Instantiate(lanePrefab, transform);
                laneInstance.SetData(sequenceIndexes);
            }
        }
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PixelWorld.FabulousFred
{
    public class LaneUI : MonoBehaviour
    {
        const float tileSelectionDuration = 1;
        Button[] tiles;

        void Start()
        {
            FillTiles();
            StartCoroutine(TileSelectionSequence(20));
        }

        void FillTiles()
        {
            tiles = new Button[transform.childCount];

            for(int i = 0; i < transform.childCount; i++)
            {   
                tiles[i] = transform.GetChild(i).GetComponent<Button>();
            }
        }

        IEnumerator TileSelectionSequence(int selectionTimes)
        {
            int tileCount = 0;

            while(tileCount < selectionTimes)
            {
                int randomTileIndex = Random.Range(0, tiles.Length);

                print(randomTileIndex);

                yield return StartCoroutine(SimulateClickRoutine(tiles[randomTileIndex]));

                tileCount++;
            }
        }

        IEnumerator SimulateClickRoutine(Button tile)
        {
            ExecuteEvents.Execute(tile.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
            yield return new WaitForSeconds(tileSelectionDuration);
            ExecuteEvents.Execute(tile.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerUpHandler);
        }
    }
}
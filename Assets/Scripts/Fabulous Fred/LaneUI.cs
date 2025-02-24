using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PixelWorld.FabulousFred
{
    public class LaneUI : MonoBehaviour
    {
        const float tileSelectionDuration = 1;
        Button[] tiles;
        Dictionary<Button, int> tileSequence = new();
        int tileSequenceCount = 0;

        void Start()
        {
            FillTiles();
            StartCoroutine(TileSelectionSequence(3));
        }

        void FillTiles()
        {
            tiles = new Button[transform.childCount];

            for(int i = 0; i < transform.childCount; i++)
            {   
                Button tile = transform.GetChild(i).GetComponent<Button>();
                tiles[i] = tile;
                tile.onClick.AddListener(() => CheckTileSequence(tile));
            }
        }

        void CheckTileSequence(Button tile)
        {
            if(tileSequence.ContainsKey(tile) && tileSequence[tile] == tileSequenceCount)
            {
                print("Right sequence");
                tileSequenceCount++;
            }
            else
            {
                print("Wrong sequence");
                tileSequenceCount = 0;
            }
        }

IEnumerator TileSelectionSequence(int selectionTimes)
{
    int tileCount = 0;
    Button lastSelectedTile = null; // Keep track of the last selected tile

    while (tileCount < selectionTimes)
    {
        int randomTileIndex;
        Button randomTile;

        // Ensure the new tile is different from the last selected one
        do
        {
            randomTileIndex = Random.Range(0, tiles.Length);
            randomTile = tiles[randomTileIndex];
        } 
        while (randomTile == lastSelectedTile); 

        lastSelectedTile = randomTile; // Update last selected tile

        tileSequence[randomTile] = tileCount;

        yield return StartCoroutine(SimulateClickRoutine(randomTile));

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
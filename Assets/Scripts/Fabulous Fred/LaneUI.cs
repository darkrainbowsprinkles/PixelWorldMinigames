using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PixelWorld.FabulousFred
{
    public class LaneUI : MonoBehaviour
    {
        [SerializeField] int selectionTimes = 3;
        [SerializeField] float resetDelay = 2;
        [SerializeField] float showSequenceDelay = 1;
        const float tileSelectionDuration = 1;
        Button[] tiles;
        Dictionary<Button, int> tileSequence = new();
        Dictionary<Button, ColorBlock> originalTileColors = new();
        int tileSequenceCount = 0;
        bool isSimulatingClick = false;

        void Start()
        {
            FillTiles();
            StartCoroutine(TileSelectionSequence(true));
        }

        void FillTiles()
        {
            tiles = new Button[transform.childCount];

            for(int i = 0; i < transform.childCount; i++)
            {
                Button tile = transform.GetChild(i).GetComponent<Button>();
                tiles[i] = tile;
                originalTileColors[tile] = tile.colors;
                HandleTilePressedEvent(tile);
            }
        }

        void HandleTilePressedEvent(Button tile)
        {
            EventTrigger trigger = tile.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new();

            entry.eventID = EventTriggerType.PointerDown; 
            entry.callback.AddListener(a => CheckTileSequence(tile));

            trigger.triggers.Add(entry);
        }

        void CheckTileSequence(Button tile)
        {
            if(isSimulatingClick)
            {
                return;
            }

            if(tileSequence.ContainsKey(tile) && tileSequence[tile] == tileSequenceCount)
            {
                tileSequenceCount++;

                if(tileSequenceCount == selectionTimes)
                {
                    tileSequenceCount = 0;
                    StartCoroutine(NewSequenceRoutine());
                }
            }
            else
            {
                tileSequenceCount = 0;
                StartCoroutine(ResetSequenceRoutine());
            }
        }

        IEnumerator NewSequenceRoutine()
        {
            SetTilesInteraction(false);
            SetTilesColor(Color.green);

            yield return new WaitForSeconds(resetDelay);

            ResetTilesColor();

            yield return new WaitForSeconds(showSequenceDelay);
            yield return StartCoroutine(TileSelectionSequence(true));

            SetTilesInteraction(true);
        }

        IEnumerator ResetSequenceRoutine()
        {
            SetTilesInteraction(false);
            SetTilesColor(Color.red);

            yield return new WaitForSeconds(resetDelay);

            ResetTilesColor();

            yield return new WaitForSeconds(showSequenceDelay);
            yield return StartCoroutine(TileSelectionSequence(false));

            SetTilesInteraction(true);
        }

        IEnumerator TileSelectionSequence(bool generateNewSequence)
        {
            SetTilesInteraction(false);

            isSimulatingClick = true;

            if(generateNewSequence)
            {
                tileSequence.Clear();

                for(int i = 0; i < selectionTimes; i++)
                {
                    Button randomTile;

                    do
                    {
                        randomTile = tiles[Random.Range(0, tiles.Length)];
                    } 
                    while(tileSequence.ContainsKey(randomTile));

                    tileSequence[randomTile] = i;
                }
            }
  
            foreach(var key in tileSequence.Keys)
            {
                yield return StartCoroutine(SimulateClickRoutine(key));
            }
    
            isSimulatingClick = false;

            SetTilesInteraction(true);
        }


        void SetTilesInteraction(bool enabled)
        {
            foreach(var tile in tiles)
            {
                tile.GetComponent<Image>().raycastTarget = enabled;
            }
        }

        void SetTilesColor(Color color)
        {
            foreach(var tile in tiles)
            {
                SetTileColor(tile, color);
            }
        }
        
        void SetTileColor(Button tile, Color color)
        {
            ColorBlock colorBlock = tile.colors;
            colorBlock.normalColor = color;
            colorBlock.selectedColor = color;
            colorBlock.pressedColor = color;
            colorBlock.highlightedColor = color;
            tile.colors = colorBlock;
        }

        void ResetTilesColor()
        {
            foreach(var tile in tiles)
            {
                tile.colors = originalTileColors[tile];
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
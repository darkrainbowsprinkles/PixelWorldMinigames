using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PixelWorld.FabulousFred
{
    public class LaneUI : MonoBehaviour
    {
        [SerializeField] LevelData[] levelsData;
        [SerializeField] Button startLineButton;
        [SerializeField] float showSequenceDelay = 1;
        Button[] tiles;
        Dictionary<Button, int> tileSequence = new();
        Dictionary<Button, ColorBlock> originalTileColors = new();
        int tileSequenceCount = 0;
        int currentLevel = 0;
        bool isSimulatingClick = false;
        bool isStartButtonPressed = false;

        [System.Serializable]
        struct LevelData
        {
            public int sequenceLength;
            public float tileSelectionDuration;
        }

        void Start()
        {
            FillTiles();

            HandleButtonPressedEvent(startLineButton, a => isStartButtonPressed = true);

            StartCoroutine(TileSelectionSequence(true));
        }

        void FillTiles()
        {
            Transform[] filteredChildren = GetFilteredChildren().ToArray();

            tiles = new Button[filteredChildren.Count()];

            for(int i = 0; i < filteredChildren.Length; i++)
            {
                Button tile = filteredChildren[i].GetComponent<Button>();
                tiles[i] = tile;
                originalTileColors[tile] = tile.colors;
                HandleButtonPressedEvent(tile, a => CheckTileSequence(tile));
            }
        }

        IEnumerable<Transform> GetFilteredChildren()
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);

                if(child.CompareTag("Tile"))
                {
                    yield return child;
                }
            }
        }

        void HandleButtonPressedEvent(Button button, UnityAction<BaseEventData> action)
        {
            EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new();

            entry.eventID = EventTriggerType.PointerDown; 
            entry.callback.AddListener(action);

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

                if(tileSequenceCount == levelsData[currentLevel].sequenceLength)
                {
                    CycleCurrentLevel();
                    tileSequenceCount = 0;
                    StartCoroutine(NewSequenceRoutine());
                }
            }
            else
            {
                tileSequenceCount = 0;
                StartCoroutine(FailedSequenceRoutine());
            }
        }

        IEnumerator NewSequenceRoutine()
        {
            SetTilesInteraction(false);
            SetTilesColor(Color.green);

            yield return new WaitUntil(() => isStartButtonPressed);

            startLineButton.interactable = false;

            ResetTilesColor();

            yield return new WaitForSeconds(showSequenceDelay);
            yield return StartCoroutine(TileSelectionSequence(true));

            SetTilesInteraction(true);
        }

        IEnumerator FailedSequenceRoutine()
        {
            SetTilesInteraction(false);
            SetTilesColor(Color.red);

            isStartButtonPressed = false;

            yield return new WaitUntil(() => isStartButtonPressed);

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

                for(int i = 0; i < levelsData[currentLevel].sequenceLength; i++)
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

        void CycleCurrentLevel()
        {
            if(currentLevel < levelsData.Length - 1)
            {
                currentLevel++;
            }
        }

        void SetTilesInteraction(bool enabled)
        {
            foreach(var tile in tiles)
            {
                SetButtonInteraction(tile, enabled);
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

        void SetButtonInteraction(Button button, bool enabled)
        {
            button.GetComponent<Image>().raycastTarget = enabled;
        }

        IEnumerator SimulateClickRoutine(Button tile)
        {
            ExecuteEvents.Execute(tile.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
            yield return new WaitForSeconds(levelsData[currentLevel].tileSelectionDuration);
            ExecuteEvents.Execute(tile.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerUpHandler);
        }
    }
}
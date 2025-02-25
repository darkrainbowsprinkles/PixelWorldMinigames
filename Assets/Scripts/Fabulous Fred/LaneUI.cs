using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PixelWorld.FabulousFred
{
    public class LaneUI : MonoBehaviour
    {
        [SerializeField] LevelData[] levelsData;
        [SerializeField] Transform sequencerButtonsContainer;
        [SerializeField] Button startButton;
        [SerializeField] TMP_Text levelText;
        Button[] sequencerButtons;
        Dictionary<int, Dictionary<Button, int>> sequenceLookup = new();
        Dictionary<Button, ColorBlock> originalColorsLookup = new();
        int sequenceCount = 0;
        int currentLevel = 0;
        bool inSelectionSequence = false;

        [System.Serializable]
        struct LevelData
        {
            public int sequenceLength;
            public float buttonSelectionDuration;
        }

        void Start()
        {
            FillSequencerButtons();
            FillSequenceLookup();
            UpdateLevelText();
            HandleButtonPressedEvent(startButton, a => StartCoroutine(ButtonSequenceRoutine()));
        }

        void FillSequencerButtons()
        {
            sequencerButtons = new Button[sequencerButtonsContainer.childCount];

            for(int i = 0; i < sequencerButtonsContainer.childCount; i++)
            {
                Button button = sequencerButtonsContainer.GetChild(i).GetComponent<Button>();
                sequencerButtons[i] = button;
                originalColorsLookup[button] = button.colors;
                HandleButtonPressedEvent(button, a => CheckButtonsSequence(button));
            }
        }

        void FillSequenceLookup()
        {
            for(int i = 0; i < levelsData.Length; i++)
            {
                Dictionary<Button, int> sequenceIndexes = new();

                LevelData levelData = levelsData[i];

                for(int j = 0; j < levelData.sequenceLength; j++)
                {
                    Button randomButton;
                    do
                    {
                        randomButton = sequencerButtons[Random.Range(0, sequencerButtons.Length)];
                    } 
                    while(sequenceIndexes.ContainsKey(randomButton));

                    sequenceIndexes[randomButton] = j;
                }

                sequenceLookup[i] = sequenceIndexes;
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

        void CheckButtonsSequence(Button button)
        {
            if(inSelectionSequence)
            {
                return;
            }

            if(ButtonInSequence(button))
            {
                sequenceCount++;

                if(LevelSucceded())
                {
                    sequenceCount = 0;
                    CycleCurrentLevel();
                    SetAllButtonsInteraction(false);
                    SetAllButtonsColor(Color.green);
                }
            }
            else
            {
                sequenceCount = 0;
                SetAllButtonsInteraction(false);
                SetAllButtonsColor(Color.red);
            }
        }

        bool ButtonInSequence(Button button)
        {
            return sequenceLookup[currentLevel].ContainsKey(button) && sequenceLookup[currentLevel][button] == sequenceCount;
        }

        bool LevelSucceded()
        {
            return sequenceCount == levelsData[currentLevel].sequenceLength;
        }

        IEnumerator ButtonSequenceRoutine()
        {
            inSelectionSequence = true;

            ResetAllButtonsColor();
            SetAllButtonsInteraction(false);
            SetButtonInteraction(startButton, false);
  
            foreach(var key in sequenceLookup[currentLevel].Keys)
            {
                yield return StartCoroutine(SimulateClickRoutine(key));
            }

            SetAllButtonsInteraction(true);
            SetButtonInteraction(startButton, true);
    
            inSelectionSequence = false;
        }

        void CycleCurrentLevel()
        {
            if(currentLevel < levelsData.Length - 1)
            {
                currentLevel++;
                UpdateLevelText();
            }
        }

        void UpdateLevelText()
        {
            levelText.text = $"Lvl:{currentLevel + 1}";
        }

        void SetAllButtonsInteraction(bool enabled)
        {
            foreach(var tile in sequencerButtons)
            {
                SetButtonInteraction(tile, enabled);
            }
        }

        void SetAllButtonsColor(Color color)
        {
            foreach(var tile in sequencerButtons)
            {
                SetButtonColor(tile, color);
            }
        }

        void ResetAllButtonsColor()
        {
            foreach(var tile in sequencerButtons)
            {
                tile.colors = originalColorsLookup[tile];
            }
        }

        void SetButtonColor(Button button, Color color)
        {
            ColorBlock colorBlock = button.colors;
            colorBlock.normalColor = color;
            colorBlock.selectedColor = color;
            colorBlock.pressedColor = color;
            colorBlock.highlightedColor = color;
            button.colors = colorBlock;
        }

        void SetButtonInteraction(Button button, bool enabled)
        {
            button.GetComponent<Image>().raycastTarget = enabled;
        }

        IEnumerator SimulateClickRoutine(Button button)
        {
            ExecuteEvents.Execute(button.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
            yield return new WaitForSeconds(levelsData[currentLevel].buttonSelectionDuration);
            ExecuteEvents.Execute(button.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerUpHandler);
        }
    }
}
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
        [SerializeField] Button startButton;
        [SerializeField] float showSequenceDelay = 1;
        Button[] buttons;
        Dictionary<Button, int> buttonSequence = new();
        Dictionary<Button, ColorBlock> originalButtonColors = new();
        int buttonSequenceCount = 0;
        int currentLevel = 0;
        bool isSimulatingClick = false;
        bool isStartButtonPressed = false;

        [System.Serializable]
        struct LevelData
        {
            public int sequenceLength;
            public float buttonSelectionDuration;
        }

        void Start()
        {
            FillButtons();

            HandleButtonPressedEvent(startButton, a => isStartButtonPressed = true);

            StartCoroutine(ButtonSelectionSequence(true));
        }

        void FillButtons()
        {
            Transform[] filteredChildren = GetFilteredChildren().ToArray();

            buttons = new Button[filteredChildren.Count()];

            for(int i = 0; i < filteredChildren.Length; i++)
            {
                Button button = filteredChildren[i].GetComponent<Button>();
                buttons[i] = button;
                originalButtonColors[button] = button.colors;
                HandleButtonPressedEvent(button, a => CheckButtonsSequence(button));
            }
        }

        IEnumerable<Transform> GetFilteredChildren()
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);

                if(child.CompareTag("Sequencer"))
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

        void CheckButtonsSequence(Button button)
        {
            if(isSimulatingClick)
            {
                return;
            }

            if(buttonSequence.ContainsKey(button) && buttonSequence[button] == buttonSequenceCount)
            {
                buttonSequenceCount++;

                if(buttonSequenceCount == levelsData[currentLevel].sequenceLength)
                {
                    CycleCurrentLevel();
                    buttonSequenceCount = 0;
                    StartCoroutine(NewSequenceRoutine());
                }
            }
            else
            {
                buttonSequenceCount = 0;
                StartCoroutine(FailedSequenceRoutine());
            }
        }

        IEnumerator NewSequenceRoutine()
        {
            SetAllButtonsInteraction(false);
            SetAllButtonsColor(Color.green);
            
            isStartButtonPressed = false;

            yield return new WaitUntil(() => isStartButtonPressed);

            ResetAllButtonsColor();

            yield return new WaitForSeconds(showSequenceDelay);
            yield return StartCoroutine(ButtonSelectionSequence(true));

            SetAllButtonsInteraction(true);
        }

        IEnumerator FailedSequenceRoutine()
        {
            SetAllButtonsInteraction(false);
            SetAllButtonsColor(Color.red);

            isStartButtonPressed = false;

            yield return new WaitUntil(() => isStartButtonPressed);

            ResetAllButtonsColor();

            yield return new WaitForSeconds(showSequenceDelay);
            yield return StartCoroutine(ButtonSelectionSequence(false));

            SetAllButtonsInteraction(true);
        }

        IEnumerator ButtonSelectionSequence(bool generateNewSequence)
        {
            SetAllButtonsInteraction(false);

            isSimulatingClick = true;

            if(generateNewSequence)
            {
                buttonSequence.Clear();

                for(int i = 0; i < levelsData[currentLevel].sequenceLength; i++)
                {
                    Button randomTile;

                    do
                    {
                        randomTile = buttons[Random.Range(0, buttons.Length)];
                    } 
                    while(buttonSequence.ContainsKey(randomTile));

                    buttonSequence[randomTile] = i;
                }
            }
  
            foreach(var key in buttonSequence.Keys)
            {
                yield return StartCoroutine(SimulateClickRoutine(key));
            }
    
            isSimulatingClick = false;

            SetAllButtonsInteraction(true);
        }

        void CycleCurrentLevel()
        {
            if(currentLevel < levelsData.Length - 1)
            {
                currentLevel++;
            }
        }

        void SetAllButtonsInteraction(bool enabled)
        {
            foreach(var tile in buttons)
            {
                SetButtonInteraction(tile, enabled);
            }
        }

        void SetAllButtonsColor(Color color)
        {
            foreach(var tile in buttons)
            {
                SetButtonColor(tile, color);
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

        void ResetAllButtonsColor()
        {
            foreach(var tile in buttons)
            {
                tile.colors = originalButtonColors[tile];
            }
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
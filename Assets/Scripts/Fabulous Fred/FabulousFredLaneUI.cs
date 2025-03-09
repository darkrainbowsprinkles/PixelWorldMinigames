using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PixelWorld.FabulousFred
{
    public class FabulousFredLaneUI : MonoBehaviour
    {
        [SerializeField] int sequenceScorePoints = 10;
        [SerializeField] int sequencePenaltyPoints = 2;
        [SerializeField] float buttonSelectionDuration = 1;
        [SerializeField] float restartSequenceDelay = 5;
        [SerializeField] float walkTime = 5;
        [SerializeField] Transform sequencerButtonsContainer;
        Button[] sequencerButtons;
        Dictionary<Button, int> sequenceLookup = new();
        Dictionary<Button, ColorBlock> originalColorsLookup = new();
        int score = 0;
        int currentSequenceIndex = 0;
        bool selectionSequenceActive = false;
        const int rowSize = 3;

        void Start()
        {
            FillSequencerButtons();
            FillSequenceLookup();
            StartCoroutine(ButtonSequenceRoutine());
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
            int count = 0;

            Button[] reversedButtons = sequencerButtons.Reverse().ToArray();

            for(int i = 0; i < reversedButtons.Length; i += rowSize)
            {
                Button randomButtonInRow = reversedButtons[Random.Range(i, i + rowSize)];
                sequenceLookup[randomButtonInRow] = count;
                count++;
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
            if(selectionSequenceActive)
            {
                return;
            }

            if(ButtonInSequence(button))
            {
                currentSequenceIndex++;
                score += sequenceScorePoints;
            }
            else
            {
                currentSequenceIndex = 0;
                score -= sequencePenaltyPoints;
                StartCoroutine(RestartSequenceRoutine());
            }
        }

        bool ButtonInSequence(Button button)
        {
            return sequenceLookup.ContainsKey(button) && sequenceLookup[button] == currentSequenceIndex;
        }

        IEnumerator RestartSequenceRoutine()
        {
            SetAllButtonsInteraction(false);
            SetAllButtonsColor(Color.red);

            yield return new WaitForSeconds(restartSequenceDelay);

            ResetAllButtonsColor();
        }

        IEnumerator ButtonSequenceRoutine()
        {
            while(true)
            {
                selectionSequenceActive = true;

                SetAllButtonsInteraction(false);
    
                foreach(var key in sequenceLookup.Keys)
                {
                    yield return StartCoroutine(SimulateClickRoutine(key));
                }

                SetAllButtonsInteraction(true);

                selectionSequenceActive = false;

                yield return new WaitForSeconds(walkTime);
            }
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
            yield return new WaitForSeconds(buttonSelectionDuration);
            ExecuteEvents.Execute(button.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerUpHandler);
        }
    } 
}
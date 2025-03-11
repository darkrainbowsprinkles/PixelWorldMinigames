using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

namespace PixelWorld.FabulousFred
{
    public class FabulousFredLaneUI : MonoBehaviour
    {
        [SerializeField] int sequenceScorePoints = 10;
        [SerializeField] int sequencePenaltyPoints = 2;
        [SerializeField] float buttonSelectionDuration = 1;
        [SerializeField] float restartSequenceDelay = 5;
        [SerializeField] float walkTime = 5;
        [SerializeField] float showPointsDelay = 1;
        [SerializeField] Transform sequencerButtonsContainer;
        [SerializeField] TMP_Text scoreText;
        Button[] sequencerButtons;
        Dictionary<Button, int> sequenceLookup = new();
        Dictionary<Button, ColorBlock> originalColorsLookup = new();
        int score = 0;
        int currentSequenceIndex = 0;
        bool selectionSequenceActive = false;
        bool halfReached = false;
        const int rowSize = 3;
        int[] sequenceIndexes;

        public void SetData(int[] sequenceIndexes)
        {
            this.sequenceIndexes = sequenceIndexes;

            FillSequencerButtons();
            FillSequenceLookup();
            StartCoroutine(ButtonSequenceRoutine());
        }

        public int GetButtonCount()
        {
            return sequencerButtonsContainer.childCount;
        }

        public int GetRowSize()
        {
            return rowSize;
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
            Button[] reversedButton = sequencerButtons.Reverse().ToArray();

            for(int i = 0; i < sequenceIndexes.Length; i++)
            {
                int buttonIndex = sequenceIndexes[i];

                sequenceLookup[reversedButton[buttonIndex]] = i;
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

                if(currentSequenceIndex >= sequenceLookup.Count)
                {
                    StopAllCoroutines();
                    StartCoroutine(ShowScoreRoutine());
                }

                if(currentSequenceIndex >= sequenceLookup.Count / 2)
                {
                    TraverseHalfButtons(button => SetButtonColor(button, Color.green));
                    TraverseHalfButtons(button => SetButtonInteraction(button, false));
                    halfReached = true;
                }
            }
            else
            {
                score -= sequencePenaltyPoints;
                StartCoroutine(PenaltyRoutine());
            }
        }

        void TraverseHalfButtons(System.Action<Button> action)
        {
            Button[] reversedButtons = sequencerButtons.Reverse().ToArray();

            for(int i = 0; i < (reversedButtons.Length / 2) - 1; i++)
            {
                action.Invoke(reversedButtons[i]);
            }
        }

        bool ButtonInSequence(Button button)
        {
            return sequenceLookup.ContainsKey(button) && sequenceLookup[button] == currentSequenceIndex;
        }

        IEnumerator PenaltyRoutine()
        {
            SetAllButtonsInteraction(false);
            SetAllButtonsColor(Color.red);

            yield return new WaitForSeconds(restartSequenceDelay);

            currentSequenceIndex = 0;
            ResetAllButtonsColor();

            if(halfReached)
            {
                currentSequenceIndex = sequenceLookup.Count / 2;
                TraverseHalfButtons(button => SetButtonColor(button, Color.green));
            }
        }

        IEnumerator ShowScoreRoutine()
        {
            SetAllButtonsColor(Color.green);
            SetAllButtonsInteraction(false);
            
            yield return new WaitForSeconds(showPointsDelay);

            sequencerButtonsContainer.gameObject.SetActive(false);
            scoreText.gameObject.SetActive(true);
            scoreText.text = $"Puntaje: {score.ToString()}";
        }
        
        IEnumerator ButtonSequenceRoutine()
        {
            bool showHalfSequence = true;

            while(true)
            {
                selectionSequenceActive = true;

                SetAllButtonsInteraction(false);

                int count = 0;
    
                foreach(var key in sequenceLookup.Keys)
                {
                    if(showHalfSequence && count >= sequenceLookup.Count / 2)
                    {
                        break;
                    }

                    count++;

                    yield return StartCoroutine(SimulateClickRoutine(key));
                }

                SetAllButtonsInteraction(true);

                if(halfReached)
                {
                    TraverseHalfButtons(button => SetButtonInteraction(button, false));
                }

                selectionSequenceActive = false;

                yield return new WaitForSeconds(walkTime);

                showHalfSequence = !showHalfSequence;
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
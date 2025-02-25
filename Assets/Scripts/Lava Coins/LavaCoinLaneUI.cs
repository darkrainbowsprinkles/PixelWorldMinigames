using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PixelWorld.FabulousFred
{
    public class LavaCoinLaneUI : MonoBehaviour
    {
        [SerializeField] Transform coinsContainer;
        [SerializeField] TMP_Text scoreText;
        [SerializeField] TMP_Text lavaWarningText;
        [SerializeField] float activateLavaTime = 15;
        [SerializeField] float lavaDuration = 5;
        [SerializeField] float coinReenableDelay = 0.5f; 
        [SerializeField] int coinPoints = 1;
        [SerializeField] int lavaCoinPenalty = 5;
        [SerializeField] float countdownBaseTime = 1;
        [SerializeField] float countdownSpeedMultiplier = 1;
        [SerializeField] float countdownMinTime = 0.2f;
        Button[] coins;
        int score = 0;
        int collectedCoins = 0;
        int totalCoins;
        bool lavaActive = false;
        Button lastSelectedCoin;

        void Start()
        {
            FillLavaCoins();
            HandlePressedEvent(gameObject, a => LavaStep());
            UpdateScore(0);
            StartCoroutine(LavaRoutineLoop());
        }

        void FillLavaCoins()
        {
            totalCoins = coinsContainer.childCount;
            coins = new Button[totalCoins];

            for(int i = 0; i < totalCoins; i++)
            {
                Button coin = coinsContainer.GetChild(i).GetComponent<Button>();
                coins[i] = coin;
                HandlePressedEvent(coin.gameObject, a => OnCoinPressed(coin));
            }
        }

        void HandlePressedEvent(GameObject gameObject, UnityAction<BaseEventData> action)
        {
            EventTrigger trigger = gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new();

            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener(action);

            trigger.triggers.Add(entry);
        }

        void OnCoinPressed(Button coin)
        {
            coin.image.enabled = false;
            collectedCoins++;
            lastSelectedCoin = coin;

            UpdateScore(coinPoints);

            if(collectedCoins >= totalCoins / 3)
            {
                collectedCoins = 0; 
                StartCoroutine(ReenableCoinsRoutine());
            }
        }

        void UpdateScore(int scoreToAdd)
        {
            score += scoreToAdd;
            scoreText.text = score.ToString();
        }

        IEnumerator ReenableCoinsRoutine()
        {
            foreach(var coin in coins)
            {
                if(!coin.image.enabled)
                {
                    if(lavaActive)
                    {
                        DisableUnselectedCoins();
                        yield break;
                    }

                    yield return new WaitForSeconds(coinReenableDelay);
                    coin.image.enabled = true;
                }
            }
        }

        IEnumerator LavaRoutineLoop()
        {
            while(true)
            {
                yield return new WaitForSeconds(activateLavaTime);
                yield return StartCoroutine(ActivateLavaRoutine());
            }
        }

        IEnumerator ActivateLavaRoutine()
        {
            lavaWarningText.gameObject.SetActive(true);

            yield return StartCoroutine(CountDownRoutine());

            lavaActive = true;

            Image image = GetComponent<Image>();
            Color originalColor = image.color;

            image.color = Color.red;
            image.raycastTarget = true;

            DisableUnselectedCoins();

            yield return new WaitForSeconds(lavaDuration);

            EnableAllCoins();

            image.raycastTarget = false;
            image.color = originalColor;

            lavaWarningText.gameObject.SetActive(false);

            lavaActive = false;
        }

        void DisableUnselectedCoins()
        {
            foreach(var coin in coins)
            {
                if(coin == lastSelectedCoin)
                {
                    coin.image.raycastTarget = false;
                    coin.image.enabled = true;
                    continue;
                }

                coin.image.enabled = false;
            }
        }

        void EnableAllCoins()
        {
            foreach(var coin in coins)
            {
                coin.image.raycastTarget = true;
                coin.image.enabled = true;
            }
        }

        IEnumerator CountDownRoutine()
        {
            float waitTime = Mathf.Max(countdownBaseTime / countdownSpeedMultiplier, countdownMinTime);

            lavaWarningText.text = "3!";
            yield return new WaitForSeconds(waitTime);
            lavaWarningText.text = "2!";
            yield return new WaitForSeconds(waitTime);
            lavaWarningText.text = "1!";
            yield return new WaitForSeconds(waitTime);
            lavaWarningText.text = "LAVA!";

            countdownSpeedMultiplier += 0.2f; 
        }

        void LavaStep()
        {
            if(lavaActive)
            {
                UpdateScore(-lavaCoinPenalty);
            }
        }
    }
}
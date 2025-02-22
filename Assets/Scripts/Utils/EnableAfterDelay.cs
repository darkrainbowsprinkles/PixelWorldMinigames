using System.Collections;
using UnityEngine;

namespace PixelWorld.Utils
{
    public class EnableAfterDelay : MonoBehaviour
    {
        [SerializeField] float delaySeconds;
        [SerializeField] GameObject[] enableTargets;

        void Start()
        {
            StartCoroutine(EnableRoutine());
        }

        IEnumerator EnableRoutine()
        {
            SetEnabled(false);
            yield return new WaitForSeconds(delaySeconds);
            SetEnabled(true);
        }

        void SetEnabled(bool enabled)
        {
            foreach(var target in enableTargets)
            {
                target.SetActive(enabled);
            }
        }
    }
}
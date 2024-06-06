using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArthurUtilities
{
    public class CoroutineUtilities : GenericSingletonClass<CoroutineUtilities>
    {

        private Dictionary<Transform, Coroutine> currentShakePositions = new();
        public void ShakePosition(Transform tr, float duration, float intensity, int changePosFrames = 1)
        {
            if (currentShakePositions.Keys.Contains(tr))
            {
                StopCoroutine(currentShakePositions[tr]);
                currentShakePositions[tr] = StartCoroutine(ShakePositionCoroutine(tr, duration, intensity, changePosFrames));
            }
            else
            {
                currentShakePositions.Add(tr, StartCoroutine(ShakePositionCoroutine(tr, duration, intensity, changePosFrames)));
            }
        }

        private IEnumerator ShakePositionCoroutine(Transform tr, float duration, float intensity, int changePosFrames = 1)
        {

            int currentFrameCounter = 0;
            float timer = 0;
            float startIntensity = intensity;
            Vector3 originalPos = tr.position;
            Vector3 currentPos = new Vector3(Random.Range(-intensity, intensity), Random.Range(-intensity, intensity), Random.Range(-intensity, intensity));

            while (timer < duration)
            {
                timer += Time.deltaTime;
                currentFrameCounter--;

                if (currentFrameCounter <= 0)
                {
                    currentFrameCounter = changePosFrames;
                    currentPos = new Vector3(Random.Range(-intensity, intensity), Random.Range(-intensity, intensity), Random.Range(-intensity, intensity));
                }

                intensity = Mathf.Lerp(startIntensity, 0, timer / duration);
                tr.position = Vector3.Lerp(tr.position, originalPos + new Vector3(Random.Range(-intensity, intensity), Random.Range(-intensity, intensity), Random.Range(-intensity, intensity)), Time.deltaTime * 5);

                yield return null;
            }
        }
    }
}

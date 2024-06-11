using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArthurUtilities
{
    public class CoroutineUtilities : GenericSingletonClass<CoroutineUtilities>
    {

        private Dictionary<Transform, Coroutine> currentShakePositions = new();
        public void ShakePosition(Transform tr, float duration, float intensity, float changePosDuration)
        {
            if (currentShakePositions.Keys.Contains(tr))
            {
                StopCoroutine(currentShakePositions[tr]);
                currentShakePositions[tr] = StartCoroutine(ShakePositionCoroutine(tr, duration, intensity, changePosDuration));
            }
            else
            {
                currentShakePositions.Add(tr, StartCoroutine(ShakePositionCoroutine(tr, duration, intensity, changePosDuration)));
            }
        }

        private IEnumerator ShakePositionCoroutine(Transform tr, float duration, float intensity, float changePosDuration)
        {
            float timer = 0;
            float changePosTimer = 0;
            float startIntensity = intensity;
            Vector3 originalPos = tr.position;
            Vector3 currentPos = new Vector3(Random.Range(-intensity, intensity), Random.Range(-intensity, intensity), Random.Range(-intensity, intensity));

            while (timer < duration)
            {
                timer += Time.deltaTime;
                changePosTimer += Time.deltaTime;
                
                intensity = Mathf.Lerp(startIntensity, 0, timer / duration);

                if (changePosTimer >= changePosDuration)
                {
                    changePosTimer = 0;
                    currentPos = new Vector3(Random.Range(-intensity, intensity), Random.Range(-intensity, intensity), Random.Range(-intensity, intensity));
                }
                
                tr.position = Vector3.Lerp(tr.position, 
                    originalPos + currentPos, changePosTimer / changePosDuration);

                yield return null;
            }
        }
    }
}

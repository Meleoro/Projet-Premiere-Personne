using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace ArthurUtilities
{
    public class CoroutineUtilities : GenericSingletonClass<CoroutineUtilities>
    {

        private Dictionary<Transform, Coroutine> currentShakePositions = new();
        public void ShakePosition(Transform tr, float duration, float intensity)
        {
            if (currentShakePositions.Keys.Contains(tr))
            {
                StopCoroutine(currentShakePositions[tr]);
                currentShakePositions[tr] = StartCoroutine(ShakePositionCoroutine(tr, duration, intensity));
            }
            else
            {
                currentShakePositions.Add(tr, StartCoroutine(ShakePositionCoroutine(tr, duration, intensity)));
            }
        }

        private IEnumerator ShakePositionCoroutine(Transform tr, float duration, float intensity)
        {
            float timer = 0;
            float startIntensity = intensity;
            Vector3 originalPos = tr.position;

            while (timer < duration)
            {
                timer += Time.deltaTime;

                intensity = Mathf.Lerp(startIntensity, 0, timer / duration);
                tr.position = originalPos + new Vector3(Random.Range(-intensity, intensity), Random.Range(-intensity, intensity), Random.Range(-intensity, intensity));

                yield return null;
            }
        }
    }
}

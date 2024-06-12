using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArthurUtilities
{
    public class CoroutineUtilities : GenericSingletonClass<CoroutineUtilities>
    {

        private Dictionary<Transform, Coroutine> currentShakePositions = new();
        public void ShakePosition(Transform tr, float duration, float intensity, float changePosDuration, float rotationIntensity)
        {
            if (currentShakePositions.Keys.Contains(tr))
            {
                StopCoroutine(currentShakePositions[tr]);
                currentShakePositions[tr] = StartCoroutine(ShakePositionCoroutine(tr, duration, intensity, changePosDuration, rotationIntensity));
            }
            else
            {
                currentShakePositions.Add(tr, StartCoroutine(ShakePositionCoroutine(tr, duration, intensity, changePosDuration, rotationIntensity)));
            }
        }

        private IEnumerator ShakePositionCoroutine(Transform tr, float duration, float intensity, float changePosDuration, float rotationIntensity)
        {
            float timer = 0;
            float changePosTimer = 0;
            float startIntensity = intensity;
            float startIntensityRot = rotationIntensity;
            Vector3 originalPos = tr.localPosition;
            Vector3 currentPos = new Vector3(Random.Range(-intensity, intensity), Random.Range(-intensity, intensity), Random.Range(-intensity, intensity));
            Quaternion currentRot = Quaternion.Euler(new Vector3(Random.Range(-rotationIntensity, rotationIntensity), 
                Random.Range(-rotationIntensity, rotationIntensity), 
                Random.Range(-rotationIntensity, rotationIntensity)));

            while (timer < duration)
            {
                timer += Time.deltaTime;
                changePosTimer += Time.deltaTime;
                
                intensity = Mathf.Lerp(startIntensity, 0, timer / duration);
                rotationIntensity = Mathf.Lerp(startIntensityRot, 0, timer / duration);

                if (changePosTimer >= changePosDuration)
                {
                    changePosTimer = 0;
                    currentPos = new Vector3(Random.Range(-intensity, intensity), Random.Range(-intensity, intensity), Random.Range(-intensity, intensity));
                    currentRot = Quaternion.Euler(new Vector3(Random.Range(-rotationIntensity, rotationIntensity), 
                        Random.Range(-rotationIntensity, rotationIntensity), 
                        Random.Range(-rotationIntensity, rotationIntensity)));
                }
                
                tr.localPosition = Vector3.Lerp(tr.localPosition, 
                    originalPos + currentPos, changePosTimer / changePosDuration);
                
                tr.localRotation = Quaternion.Lerp(tr.localRotation, currentRot, changePosTimer / changePosDuration);

                yield return null;
            }

            tr.localPosition = originalPos;
        }
    }
}

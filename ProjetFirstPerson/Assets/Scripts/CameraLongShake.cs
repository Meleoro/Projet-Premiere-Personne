using System.Collections;
using System.Collections.Generic;
using ArthurUtilities;
using UnityEngine;

public class CameraLongShake : MonoBehaviour
{
    public float intensity;
    public float changePosDuration;
    public float rotationForce;
    void Start()
    {
        CoroutineUtilities.Instance.LongShakePosition(transform, 1000000, intensity, changePosDuration, rotationForce);
        AudioManager.Instance.PlaySoundContinuous(3,7,1);
    }

    public void PlayStartSound()
    {
        AudioManager.Instance.PlaySoundOneShot(3,4,0);
    }
}

using System.Collections;
using System.Collections.Generic;
using ArthurUtilities;
using UnityEngine;

public class CameraLongShake : MonoBehaviour
{
    void Start()
    {
        CoroutineUtilities.Instance.LongShakePosition(transform, 1000000, 0.3f, 2f, 1f);
    }
}

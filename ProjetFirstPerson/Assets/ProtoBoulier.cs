using System.Collections;
using System.Collections.Generic;
using ArthurUtilities;
using Puzzle;
using UnityEngine;
using UnityEngine.Serialization;

public class ProtoBoulier : MonoBehaviour
{
    public PuzzleInteract interactManager;
    public List<bool> currentBool;
    public List<bool> expectedBool;
    public bool isGood;
    
    public List<ProtoBoulier> allBouliers;
    public Animation anim;
    
    [Header("Parameters Shake")] 
    [SerializeField] private float waitBeforeShake;
    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeAmplitude;
    [SerializeField] private float shakeChangeFrameDuration;
    [SerializeField] private float shakeRotIntensity;
    
    public void CheckIfGood()
    {
        bool good = true;
        for (int i = 0; i < currentBool.Count; i++)
        {
            if (currentBool[i] != expectedBool[i])
                good = false;
        }
        if (good) isGood = true;
        else isGood = false;
        
        Debug.Log(good);
        
        CheckIfAllGood();
    }
    
    public void CheckIfAllGood()
    {
        bool win = true;
        for (int i = 0; i < allBouliers.Count; i++)
        {
            if (!allBouliers[i].isGood)
                win = false;
        }

        if (win)
        {
            AudioManager.Instance.PlaySoundOneShot(3,4,0);
            anim.Play();
            interactManager.GetOutInteraction();
            Debug.Log("win");
            StartCoroutine(PlayDoorEffects());
        }
    }

    public IEnumerator PlayDoorEffects()
    {
        yield return new WaitForSeconds(waitBeforeShake);
        AudioManager.Instance.PlaySoundOneShot(2,10,0);
        CoroutineUtilities.Instance.ShakePosition(CameraManager.Instance.transform.parent.parent, shakeDuration,
            shakeAmplitude, shakeChangeFrameDuration, shakeRotIntensity);
    }
}

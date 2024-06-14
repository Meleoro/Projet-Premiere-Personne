using System.Collections;
using System.Collections.Generic;
using ArthurUtilities;
using Puzzle;
using UnityEngine;

public class PorteCylindre : MonoBehaviour
{
    [SerializeField] private List<CylindreSymbols> wantedOrder = new List<CylindreSymbols>();
    [SerializeField] private List<CylindrePuzzle> cylinders = new List<CylindrePuzzle>();
    public Animation anim;
    public PuzzleInteract interactScript;
    //public string animationName;

    [Header("Parameters Shake")] 
    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeAmplitude;
    [SerializeField] private float shakeChangeFrameDuration;
    [SerializeField] private float shakeRotIntensity;

    public void CheckIfWin()
    {
        bool win = true;
        for (int i = 0; i < cylinders.Count; i++)
        {
            if (wantedOrder[i] != cylinders[i].symbol)
            {
                win = false;
                break;
            }
        }

        if (win) StartCoroutine(Win());
    }

    private IEnumerator Win()
    {
        AudioManager.Instance.PlaySoundOneShot(3,4,0);
        for (int i = 0; i < cylinders.Count; i++)
        {
            cylinders[i].isMoving = true;
        }
        
        yield return new WaitForSeconds(1);
        
        CoroutineUtilities.Instance.ShakePosition(CameraManager.Instance.transform, shakeDuration,
            shakeAmplitude, shakeChangeFrameDuration, shakeRotIntensity);
        
        interactScript.GetOutInteraction();
        //anim.clip = anim["OpenCylindresIntro"].clip;
        anim.Play();
        yield return new WaitForSeconds(0.2f);
        AudioManager.Instance.PlaySoundOneShot(2,10,0);
    }
    
    public enum CylindreSymbols
    {
        Pales,
        Soleil,
        Fleur,
        Feuille,
    }
}

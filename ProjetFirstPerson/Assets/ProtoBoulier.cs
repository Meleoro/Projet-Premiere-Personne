using System.Collections;
using System.Collections.Generic;
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
        }
    }
}

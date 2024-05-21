using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ProtoBoulier : MonoBehaviour
{
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
            anim.Play();
            Debug.Log("win");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Puzzle;
using UnityEngine;

public class PorteCylindre : MonoBehaviour
{
    [SerializeField] private List<CylindreSymbols> wantedOrder = new List<CylindreSymbols>();
    [SerializeField] private List<CylindrePuzzle> cylinders = new List<CylindrePuzzle>();
    public Animation anim;
    public PuzzleInteract interactScript;
    //public string animationName;

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
        for (int i = 0; i < cylinders.Count; i++)
        {
            cylinders[i].isMoving = true;
        }
        yield return new WaitForSeconds(1);
        interactScript.GetOutInteraction();
        //anim.clip = anim["OpenCylindresIntro"].clip;
        anim.Play();
    }
    
    public enum CylindreSymbols
    {
        Pales,
        Soleil,
        Fleur,
        Feuille,
    }
}

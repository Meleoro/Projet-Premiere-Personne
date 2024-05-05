using System.Collections.Generic;
using Puzzle;
using UnityEngine;

public class PorteCylindre : MonoBehaviour
{
    [SerializeField] private List<CylindreSymbols> wantedOrder = new List<CylindreSymbols>();
    [SerializeField] private List<CylindrePuzzle> cylinders = new List<CylindrePuzzle>();
    private Animation anim;
    private PuzzleInteract interactScript;
    void Start()
    {
        anim = GetComponent<Animation>();
        interactScript = GetComponentInChildren<PuzzleInteract>();
    }
    
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

        if (win) Win();
    }

    private void Win()
    {
        interactScript.GetOutInteraction();
        anim.clip = anim["Open2"].clip;
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
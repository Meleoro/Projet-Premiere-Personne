using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TriangleManager : MonoBehaviour
{
    public ProtoPuzzleInteract interactManager;
    public dalleTriangleProto[] dalleOrder = new dalleTriangleProto[5];
    public float moveSpeed;
  
    private bool canMove;
    private int currentIndex;
    private Vector3 wantedDallePosition;
    private Vector3 wantedDallePosition2;
    private void Update()
    {
        if (canMove)
        {
            if (currentIndex > 0)
            {
                dalleOrder[currentIndex].transform.localPosition = Vector3.Lerp(dalleOrder[currentIndex].transform.localPosition,
                    wantedDallePosition, moveSpeed * Time.deltaTime); //Bouge la dalle cliquée
        
                dalleOrder[currentIndex-1].transform.localPosition = Vector3.Lerp(dalleOrder[currentIndex-1].transform.localPosition,
                    wantedDallePosition2, moveSpeed * Time.deltaTime); //Bouge la dalle du dessus
            }
            else
            {
                dalleOrder[currentIndex].transform.localPosition = Vector3.Lerp(dalleOrder[currentIndex].transform.localPosition,
                    wantedDallePosition, moveSpeed * Time.deltaTime); //Bouge la dalle cliquée
        
                dalleOrder[^1].transform.localPosition = Vector3.Lerp(dalleOrder[^1].transform.localPosition,
                    wantedDallePosition2, moveSpeed * Time.deltaTime); //Bouge la dalle du dessus
            }
        }

        if (Input.GetKeyDown(KeyCode.Break))
            CheckIfDone();
    }

    public void ChangeDalleOrder(int index)
    {
        if (index > 0)
        {
            dalleOrder[index - 1].myIndex += 1; //Change l'index de la dalle du dessus
            dalleOrder[index].myIndex -= 1; //Change l'index de la dalle cliquée

            currentIndex = index;
            StartCoroutine(MoveDalles());
        
        
            (dalleOrder[index - 1], dalleOrder[index]) = (dalleOrder[index], dalleOrder[index - 1]); //Change l'ordre dans l'array
          
        }
        else // Si c'est la dalle la plus haute
        {
            dalleOrder[^1].myIndex = 0; //Change l'index de la dalle du dessus
            dalleOrder[index].myIndex = dalleOrder.Length-1; //Change l'index de la dalle cliquée
            
            currentIndex = index;
            StartCoroutine(MoveDalles());
            
            (dalleOrder[^1], dalleOrder[index]) = (dalleOrder[index], dalleOrder[^1]); //Change l'ordre dans l'array
           
        }
    }

    public IEnumerator MoveDalles()
    {
        if (currentIndex > 0)
        {
            wantedDallePosition = dalleOrder[currentIndex].transform.localPosition - new Vector3(0, 0.00025f,0);
            wantedDallePosition2 = dalleOrder[currentIndex-1].transform.localPosition + new Vector3(0, 0.00025f,0);
            canMove = true;
            yield return new WaitForSeconds(3f);
            canMove = false;
        }
        else
        {
            wantedDallePosition = dalleOrder[currentIndex].transform.localPosition - new Vector3(0, 0.00025f,0);
            wantedDallePosition2 = dalleOrder[^1].transform.localPosition + new Vector3(0, 0.00025f,0);
            canMove = true;
            yield return new WaitForSeconds(0.15f);
            canMove = false;
        }
      
    }

    public void CheckIfDone() // Si c'est la bonne combinaison
    {
        string answer = String.Concat(dalleOrder[0].myName, dalleOrder[1].myName, dalleOrder[2].myName, dalleOrder[3].myName, dalleOrder[4].myName);
        if (answer == "VioletBleuBlancOrangeVert") // Mettre les nom des dalles dans le bon ordre
        {
            interactManager.GetOutInteraction();
            interactManager.OpenDoor();
        }
        else
        {
            interactManager.GetOutInteraction();
            // attirer la bête
        }
            
    }
}

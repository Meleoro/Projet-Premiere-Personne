using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Proto
{
    public class TriangleManager : MonoBehaviour
    {
        public ProtoPuzzleInteract interactManager;
        public dalleTriangleProto[] dalleOrder = new dalleTriangleProto[5];
        public float moveSpeed;
        private Animation anim;

        private bool canMove;
        private int currentIndex;
        private Vector3 wantedDallePosition;
        private Vector3 wantedDallePosition2;

        private void Start()
        {
            anim = GetComponent<Animation>();
        }

        private void Update()
        {
            if (canMove)
            {
                if (currentIndex > 0)// Si ce n'est pas la dalle la plus haute
                {
                    dalleOrder[currentIndex].transform.localPosition = Vector3.Lerp(dalleOrder[currentIndex].transform.localPosition,
                        wantedDallePosition, moveSpeed * Time.deltaTime); //Bouge la dalle cliquée

                    dalleOrder[currentIndex - 1].transform.localPosition = Vector3.Lerp(dalleOrder[currentIndex - 1].transform.localPosition,
                        wantedDallePosition2, moveSpeed * Time.deltaTime); //Bouge la dalle du dessus
                }
                else // Si c'est la dalle la plus haute
                {
                    dalleOrder[currentIndex].transform.localPosition = Vector3.Lerp(dalleOrder[currentIndex].transform.localPosition,
                        wantedDallePosition, moveSpeed * Time.deltaTime); //Bouge la dalle cliquée

                    dalleOrder[3].transform.localPosition = Vector3.Lerp(dalleOrder[3].transform.localPosition,
                        wantedDallePosition2, moveSpeed * Time.deltaTime); //Bouge la dalle la plus basse
                }
            }
        }

        public void ChangeDalleOrder(int index)
        {
            if (index > 0) // Si ce n'est pas la dalle la plus haute
            {
                dalleOrder[index - 1].myIndex += 1; //Change l'index de la dalle du dessus
                dalleOrder[index].myIndex -= 1; //Change l'index de la dalle cliquée

                currentIndex = index; // pour le déplacement 
                StartCoroutine(MoveDalles());


                (dalleOrder[index - 1], dalleOrder[index]) = (dalleOrder[index], dalleOrder[index - 1]); //Change l'ordre dans l'array

            }
            else // Si c'est la dalle la plus haute
            {
                dalleOrder[3].myIndex = 0; //Change l'index de la dalle la plus basse
                dalleOrder[index].myIndex = dalleOrder.Length - 2; //Change l'index de la dalle cliquée

                currentIndex = index;
                StartCoroutine(MoveDalles());

                (dalleOrder[3], dalleOrder[index]) = (dalleOrder[index], dalleOrder[3]); //Change l'ordre dans l'array

            }
            CheckIfDone();
        }

        public IEnumerator MoveDalles()
        {
            if (currentIndex > 0) // Si ce n'est pas la dalle la plus haute
            {
                wantedDallePosition = dalleOrder[currentIndex].transform.localPosition - new Vector3(0, 0.00025f, 0);
                wantedDallePosition2 = dalleOrder[currentIndex - 1].transform.localPosition + new Vector3(0, 0.00025f, 0);
                canMove = true;
                yield return new WaitForSeconds(3f);
                canMove = false;
            }
            else // Si c'est la dalle la plus haute
            {
                wantedDallePosition = dalleOrder[currentIndex].transform.localPosition - new Vector3(0, 0.00025f, 0);
                wantedDallePosition2 = dalleOrder[3].transform.localPosition + new Vector3(0, 0.00025f, 0);
                canMove = true;
                yield return new WaitForSeconds(2f);
                canMove = false;
            }

        }

        public void CheckIfDone() // Si c'est la bonne combinaison
        {
            string answer = String.Concat(dalleOrder[0].myName, dalleOrder[1].myName, dalleOrder[2].myName, dalleOrder[3].myName, dalleOrder[4].myName);
            Debug.Log(answer);
            if (answer == "FeuilleEventailFleurPalesSoleil") // Mettre les nom des dalles dans le bon ordre
            {
                interactManager.GetOutInteraction();
                interactManager.OpenDoor();
                anim.clip = anim["Open"].clip;
                anim.Play();
            }
        }
    }

}
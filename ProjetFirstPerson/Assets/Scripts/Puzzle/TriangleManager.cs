using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

namespace Puzzle
{
    public class TriangleManager : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField] DalleSymbols[] wantedOrder = new DalleSymbols[5];
        [SerializeField] private float moveDuration;

        [Header("Private Infos")]
        //private DalleTriangle[] currentDalles = new DalleTriangle[5];
        private bool isMovingDalles;
        private DalleTriangle selectedDalle1;
        private DalleTriangle selectedDalle2;


        [Header("References")]
        [SerializeField] private DalleTriangle[] dalles = new DalleTriangle[5];     // En index 1 doit se trouver la dalle tout en haut du triangle et en dernier index celle tout en bas
        [SerializeField] private Door doorToOpen;
        private Animation anim;



        private void Start()
        {
            anim = GetComponent<Animation>();

            selectedDalle1 = null;
            selectedDalle2 = null;

            InitialiseDalles();
        }

        private void InitialiseDalles()
        {
            for(int i = dalles.Length - 1; i >= 0; i--)
            {
                dalles[i].currentIndex = i;
            }
        }



        private void Update()
        {
            VerifyWin();
        }


        public void SelectDalle(DalleTriangle selectedDalle)
        {
            if (isMovingDalles) return;

            if (selectedDalle1 is null)
                selectedDalle1 = selectedDalle;

            else 
            {
                selectedDalle2 = selectedDalle;
                StartCoroutine(ExchangeTwoDalles(selectedDalle1, selectedDalle2));
            }
        }



        // Coroutine which exchanges two dalles positions
        public IEnumerator ExchangeTwoDalles(DalleTriangle dalle1, DalleTriangle dalle2)
        {
            if(isMovingDalles) yield break;
            isMovingDalles = true;

            float timer = 0;
            dalle1.currentIndex = dalle2.currentIndex;
            dalle2.currentIndex = dalle1.currentIndex;

            Vector3 originalPosDalle1 = dalle1.transform.position;
            Vector3 originalPosDalle2 = dalle2.transform.position;  


            while(timer < moveDuration)
            {
                timer += Time.deltaTime;

                dalle1.transform.position = Vector3.Lerp(originalPosDalle1, originalPosDalle2, timer / moveDuration);
                dalle2.transform.position = Vector3.Lerp(originalPosDalle2, originalPosDalle1, timer / moveDuration);

                yield return null;
            }

            dalle1.transform.position = originalPosDalle2;
            dalle2.transform.position = originalPosDalle1;

            selectedDalle1 = null;
            selectedDalle2 = null;

            isMovingDalles = false;
        }


        private List<DalleSymbols> currentOrder;
        private void VerifyWin()
        {
            bool win = true;

            currentOrder = new List<DalleSymbols>();
            for(int i = 0; i < dalles.Length; i++) 
            {
                currentOrder[dalles[i].currentIndex] = dalles[i].dalleSymbol;
            }

            for (int i = 0; i < wantedOrder.Length; i++)
            {
                if (wantedOrder[i] != currentOrder[i])
                {
                    win = false;
                    break;
                }
            }

            if (win) Win();
        }

        private void Win()
        {
            anim.clip = anim["Open"].clip;
            anim.Play();
        }
    }
}





public enum DalleSymbols
{
    Feuille,
    Eventail,
    Fleur,
    Pales, 
    Soleil
}

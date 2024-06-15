using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ArthurUtilities;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

namespace Puzzle
{
    public class TriangleManager : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField] DalleSymbols[] wantedOrder = new DalleSymbols[5];
        [SerializeField] private float moveDuration;

        [Header("Parameters Shake")] 
        [SerializeField] private float shakeDuration;
        [SerializeField] private float shakeAmplitude;
        [SerializeField] private float shakeChangeFrameDuration;
        [SerializeField] private float shakeRotIntensity;
        
        [Header("Private Infos")]
        private bool isMovingDalles;
        [HideInInspector] public DalleTriangle selectedDalle1;
        private DalleTriangle selectedDalle2;

        [Header("References")]
        [SerializeField] private DalleTriangle[] dalles = new DalleTriangle[5];     // En index 1 doit se trouver la dalle tout en haut du triangle et en dernier index celle tout en bas
        [SerializeField] private Door doorToOpen;
        private Animation anim;
        private PuzzleInteract interactScript;



        private void Start()
        {
            anim = GetComponent<Animation>();
            interactScript = GetComponentInChildren<PuzzleInteract>();

            selectedDalle1 = null;
            selectedDalle2 = null;

            InitialiseDalles();
        }

        private void InitialiseDalles()
        {
            for (int i = dalles.Length - 1; i >= 0; i--)
            {
                dalles[i].currentIndex = i;
            }
        }

        public void SelectDalle(DalleTriangle selectedDalle)
        {
            if (isMovingDalles) return;

            if (selectedDalle1 is null)
            {
                selectedDalle1 = selectedDalle;
                selectedDalle.meshRenderer.material = selectedDalle.MaterialSelected;
                selectedDalle.isSelected = true;
            }

            else
            {
                selectedDalle1.isSelected = false;
                selectedDalle2 = selectedDalle;
                selectedDalle.meshRenderer.material = selectedDalle.MaterialHighlighted;
                StartCoroutine(ExchangeTwoDalles(selectedDalle1, selectedDalle2));
            }
        }



        // Coroutine which exchanges two dalles positions
        public IEnumerator ExchangeTwoDalles(DalleTriangle dalle1, DalleTriangle dalle2)
        {
            if (isMovingDalles) yield break;
            isMovingDalles = true;
            AudioManager.Instance.PlaySoundOneShot(2,7,0);
            float timer = 0;
            (dalle1.currentIndex, dalle2.currentIndex) = (dalle2.currentIndex, dalle1.currentIndex);

            Vector3 originalPosDalle1 = dalle1.transform.position;
            Vector3 originalPosDalle2 = dalle2.transform.position;


            while (timer < moveDuration)
            {
                timer += Time.deltaTime;

                dalle1.transform.position = Vector3.Lerp(originalPosDalle1, originalPosDalle2, timer / moveDuration);
                dalle2.transform.position = Vector3.Lerp(originalPosDalle2, originalPosDalle1, timer / moveDuration);

                yield return null;
            }

            dalle1.transform.position = originalPosDalle2;
            dalle2.transform.position = originalPosDalle1;
            
            selectedDalle1.GetComponent<MeshRenderer>().material = selectedDalle1.MaterialOff;
            selectedDalle1 = null;
            selectedDalle2 = null;

            isMovingDalles = false;
            VerifyWin();
        }


        private DalleSymbols[] currentOrder = new DalleSymbols[5];
        private void VerifyWin()
        {
            bool win = true;

            currentOrder = new DalleSymbols[5];
            for (int i = 0; i < dalles.Length; i++) 
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

            if (win) StartCoroutine(Win());
        }


        private IEnumerator Win()
        {
            AudioManager.Instance.PlaySoundOneShot(3,4,0);
            for (int i = 0; i < dalles.Length; i++)
            {
                dalles[i].canMove = false;
            }
            yield return new WaitForSeconds(1f);

            interactScript.GetOutInteraction();

          
            
            anim.clip = anim["Open"].clip;
            anim.Play();
            yield return new WaitForSeconds(2.51f);
            AudioManager.Instance.PlaySoundOneShot(2,10,0);
            CoroutineUtilities.Instance.ShakePosition(CameraManager.Instance.transform, shakeDuration,
                shakeAmplitude, shakeChangeFrameDuration, shakeRotIntensity);
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

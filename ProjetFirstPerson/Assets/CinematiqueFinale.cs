using System;
using System.Collections;
using System.Collections.Generic;
using Creature;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CinematiqueFinale : MonoBehaviour
{
    public GameObject lightEtape2;
    public GameObject lightEtape1;
    public Animation doorAnim;
    private BoxCollider collider;
    public CameraComponent camera;
    public GameObject pointToLook;
    public float lookSpeed;
    public CreatureMover creatureMover;
    public CreatureManager creatureManager;
    public CreatureReferences creatureReferences;
    public TextMeshProUGUI titre;

    private void Start()
    {
        collider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.Instance.PlaySoundOneShot(2,10,0);
            doorAnim.clip = doorAnim["FermeturePorte"].clip;
            doorAnim.Play();
            collider.enabled = false;
        }
    }
    

    public IEnumerator DoSecondPart()
    {
        creatureMover.agressiveSpeed = 0;
        camera.cinematicLookSpeed = 0;
        camera.isInCinematic = true;
        yield return new WaitForSeconds(0.4f);
        AudioManager.Instance.PlaySoundOneShot(2,10,0);
        lightEtape2.SetActive(true);
        doorAnim.clip = doorAnim["OuverturePorteFinale"].clip;
        doorAnim.Play();
        yield return new WaitForSeconds(1.4f);
        camera.cinematicLookSpeed = lookSpeed;
        camera.LookTowardsCinematic(pointToLook.transform);
        yield return new WaitForSeconds(2.17f);
        creatureReferences.coleretteAnimator.SetBool("IsOpen", true);
        AudioManager.Instance.PlaySoundOneShot(0,1,1);
        yield return new WaitForSeconds(2.3f);
        StartCoroutine(CameraEffects.Instance.FadeScreen(0.01f, 1));
        titre.gameObject.SetActive(true);
        yield return new WaitForSeconds(4f);
        StartCoroutine(FadeOutTitre(5.5f));
        yield return new WaitForSeconds(5.5f);
        SceneManager.LoadScene("MainMenu");
    }

    public IEnumerator FadeOutTitre(float duration)
    {
        float timer = 0;
        while (timer < duration)
        {
            titre.alpha = Mathf.Lerp(titre.alpha, 0, timer / duration);
            timer += Time.deltaTime;
            
            yield return null;
        }
        titre.alpha = 0;
    }
}

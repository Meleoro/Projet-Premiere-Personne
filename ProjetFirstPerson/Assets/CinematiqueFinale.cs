using System;
using System.Collections;
using System.Collections.Generic;
using ArthurUtilities;
using Creature;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public Image titre;
    public RectTransform credits;
    [SerializeField] private float creditDuration;
    [SerializeField] private float creditYToAdd;
    [Header("Parameters Shake")] 
    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeAmplitude;
    [SerializeField] private float shakeChangeFrameDuration;
    [SerializeField] private float shakeRotIntensity;
    [SerializeField] private AudioSource beteAudio;
    [SerializeField] private GameObject notifLog;
    
    private void Start()
    {
        collider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.Instance.PlaySoundOneShot(2,10,0);
            AudioManager.Instance.FadeOutAudioSource(1, 4);
            AudioManager.Instance.FadeOutAudioSource(1, 5);
            AudioManager.Instance.FadeOutAudioSource(1, 6);
            doorAnim.clip = doorAnim["FermeturePorte 1"].clip;
            doorAnim.Play();
            collider.enabled = false;
            
            CoroutineUtilities.Instance.ShakePosition(CameraManager.Instance.transform, shakeDuration,
                shakeAmplitude, shakeChangeFrameDuration, shakeRotIntensity);
        }
    }
    

    public IEnumerator DoSecondPart()
    { 
        notifLog.SetActive(false);
        creatureMover.agressiveSpeed = 0;
        camera.cinematicLookSpeed = 0;
        camera.isInCinematic = true;
        yield return new WaitForSeconds(0.4f);
        AudioManager.Instance.PlaySoundOneShot(2,10,0);
        lightEtape2.SetActive(true);
        doorAnim.clip = doorAnim["OuverturePorteFinale 1"].clip;
        doorAnim.Play();
        yield return new WaitForSeconds(2.2f);
        camera.cinematicLookSpeed = lookSpeed;
        camera.LookTowardsCinematic(pointToLook.transform);
        yield return new WaitForSeconds(4f);
        creatureReferences.coleretteAnimator.SetBool("IsOpen", true);
        AudioManager.Instance.PlaySoundOneShot(0,1,1);
        yield return new WaitForSeconds(3f);
        StartCoroutine(CameraEffects.Instance.FadeScreen(0.01f, 1));
        titre.gameObject.SetActive(true);
        yield return new WaitForSeconds(4f);
        beteAudio.enabled = false;
        AudioManager.Instance.PlaySoundFadingIn(1, 3, 8, 7);
        StartCoroutine(FadeOutTitre(6f));
        yield return new WaitForSeconds(6f);
        credits.gameObject.SetActive(true);
        credits.transform.DOMove(credits.transform.position + new Vector3(0, creditYToAdd, 0),creditDuration).SetEase(Ease.Linear);
        yield return new WaitForSeconds(creditDuration);
        AudioManager.Instance.FadeOutAudioSource(3.5f,7);
        yield return new WaitForSeconds(3.5f);
        SceneManager.LoadScene("MainMenu");
    }

    public IEnumerator FadeOutTitre(float duration)
    {
        float alpha = 1;
        float timer = 0;
        while (timer < duration)
        {
            alpha = Mathf.Lerp(1, 0, timer / duration);
            titre.color = new Color(1,1,1,alpha);
            timer += Time.deltaTime;
            
            yield return null;
        }
        alpha = 0;
        titre.color = new Color(1,1,1,0);
    }
}

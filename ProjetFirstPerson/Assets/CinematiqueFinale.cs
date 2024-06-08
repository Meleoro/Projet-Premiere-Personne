using System;
using System.Collections;
using System.Collections.Generic;
using Creature;
using UnityEngine;

public class CinematiqueFinale : MonoBehaviour
{
    public GameObject light;
    public Animation doorAnim;
    private BoxCollider collider;
    public CameraComponent camera;
    public GameObject pointToLook;
    public float lookSpeed;
    public CreatureMover creatureMover;
    public CreatureManager creatureManager;
    public CreatureReferences creatureReferences;

    private void Start()
    {
        collider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.Instance.PlaySoundOneShot(2,2,0);
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
        light.SetActive(true);
        
        yield return new WaitForSeconds(0.4f);
        AudioManager.Instance.PlaySoundOneShot(2,2,0);
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
    }
}

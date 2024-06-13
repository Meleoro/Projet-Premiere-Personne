using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematiqueIntro : MonoBehaviour
{
    public bool doCinematique;
    public MoveComponent move;
    public CameraComponent cam;
    public MeshRenderer capsule;
    public Animation camAnim;
    public Animation eyesAnim;
    public CameraTestEthan tablettePhoto;
    public GameObject triggerLog;
    public GameObject HUD;
    public GameObject player;
    
    void Start()
    {
        if (doCinematique)
        {
            UIManager.Instance.canMenu = false;
            tablettePhoto.canInOut = false;
            capsule.enabled = false;
            player.transform.position =  new Vector3(0.232999995f, 88.007225f, -106.825996f);
            eyesAnim.gameObject.SetActive(true);
            StartCoroutine(CameraEffects.Instance.FadeScreen(0.01f, 1));
            triggerLog.SetActive(false);
            StartCoroutine(Cinematique());
            HUD.SetActive(false);
        }
        else
        {
            UIManager.Instance.canMenu = true;
        }
    }

    public IEnumerator Cinematique()
    {
        move.canMove = false;
        cam.isInCinematicIntro = true;
        camAnim.clip = camAnim["CinematiqueIntro"].clip;
        camAnim.Play();
        eyesAnim.Play();
        StartCoroutine(CameraEffects.Instance.FadeScreen(1, 0));
        
        yield return new WaitForSeconds(9.8f);
        
        /*cam.transform.GetChild(2).transform.localPosition = new Vector3(0, 0.8f, 0);
        cam.characterCamera.transform.localEulerAngles = new Vector3(348.700012f, 152.01001f, 0);*/
        
        yield return new WaitForSeconds(0.2f);
        
        eyesAnim.gameObject.SetActive(false);
        HUD.SetActive(true);
        triggerLog.SetActive(true);
        
        //yield return new WaitForSeconds(0.25f);
        
        /*cam.transform.GetChild(2).transform.localPosition = new Vector3(0, 0.8f, 0);
        cam.characterCamera.transform.localEulerAngles = new Vector3(348.700012f, 152.01001f, 0);*/
        cam.ResetCamera();
        move.canMove = true;
        cam.isInCinematicIntro = false;
        cam.canRotate = true;
        cam.canMove = true;
        tablettePhoto.canInOut = true;
        UIManager.Instance.canMenu = true;
    }
}

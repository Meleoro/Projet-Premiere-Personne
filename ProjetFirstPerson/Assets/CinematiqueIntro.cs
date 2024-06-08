using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematiqueIntro : MonoBehaviour
{
    public bool doCinematique;
    public MoveComponent move;
    public CameraComponent cam;
    public GameObject capsule;
    public Animation camAnim;
    void Start()
    {
        if (doCinematique)
            StartCoroutine(Cinematique());
    }

    public IEnumerator Cinematique()
    {
        StartCoroutine(CameraEffects.Instance.FadeScreen(0, 1));
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(CameraEffects.Instance.FadeScreen(1, 0));
        yield return new WaitForSeconds(1f);
        camAnim.clip = camAnim["CinematiqueIntro"].clip;
        camAnim.Play();
        capsule.SetActive(false);
        move.canMove = false;
        cam.canRotate = false;
        cam.canMove = false;
        yield return new WaitForSeconds(2);
        move.canMove = true;
        cam.canRotate = true;
        cam.canMove = true;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour, ICharacterComponent
{
    [Header("Parameters")] 
    [SerializeField] private float recoveryTime;

    [Header("Public Infos")] 
    public Action DieAction;
    public bool isHurted;

    [Header("Private Infos")] 
    private bool isDying;
    private float hurtTimer;
    private Animation anim;
    private Vector3 lastCheckPoint;
    public CameraComponent cam;
    public MoveComponent move;


    private void Start()
    {
        anim = GetComponent<Animation>();
        lastCheckPoint = transform.position;
    }


    public void ComponentUpdate()
    {
        if(isHurted)
        {
            hurtTimer -= Time.deltaTime;

            if(hurtTimer <= 0)
            {
                isHurted = false;
            }
        }
    }

    public void ComponentFixedUpdate()
    {
    }

    public void ComponentLateUpdate()
    {
    }


    public void SavePos()
    {
        lastCheckPoint = transform.position;
    }


    public void TakeDamage()
    {
        if(isHurted && !isDying)
            StartCoroutine(Die());

        GetComponent<StaminaComponent>().RegainStamina();

        isHurted = true;
        hurtTimer = recoveryTime;

        StartCoroutine(CameraEffects.Instance.TakeDamage(0.8f));
        StartCoroutine(CameraEffects.Instance.HurtEffect(recoveryTime));
    }

    private IEnumerator Die()
    {
        DieAction.Invoke();
        
        isDying = true;
        anim.clip = anim["Death"].clip;
        anim.Play();
        cam.canRotate = false;
        move.canMove = false;
        yield return new WaitForSeconds(3);
        StartCoroutine(CameraEffects.Instance.FadeScreen(2f, 1));

        yield return new WaitForSeconds(2.5f);

        transform.position = lastCheckPoint;
        isHurted = false;
        cam.transform.GetChild(2).transform.localPosition = new Vector3(0, 0.8f, 0);
        cam.transform.GetChild(2).transform.localEulerAngles = Vector3.zero;
        
        StartCoroutine(CameraEffects.Instance.FadeScreen(0.75f, 0));
        yield return new WaitForSeconds(0.5f);
        cam.canMove = true;
        cam.canRotate = true;
        move.canMove = true;
        isDying = false;
    }
}

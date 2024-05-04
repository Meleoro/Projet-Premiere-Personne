using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour, ICharacterComponent
{
    [Header("Parameters")] 
    [SerializeField] private float recoveryTime;

    [Header("Private Infos")] 
    public bool isHurted;
    private float hurtTimer;
    private Animation anim;
    private Vector3 lastCheckPoint;


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
        if(isHurted)
            StartCoroutine(Die());

        GetComponent<StaminaComponent>().RegainStamina();

        isHurted = true;
        hurtTimer = recoveryTime;

        StartCoroutine(CameraEffects.Instance.TakeDamage(0.8f));
        StartCoroutine(CameraEffects.Instance.HurtEffect(recoveryTime));
    }

    private IEnumerator Die()
    {
        //anim.clip = anim["Death"].clip;
        //anim.Play();
        yield return new WaitForSeconds(1);
        GetComponent<MoveComponent>().canMove = false;
        GetComponent<CameraComponent>().canRotate = false;        
        StartCoroutine(CameraEffects.Instance.FadeScreen(0.75f, 1));

        yield return new WaitForSeconds(1);

        transform.position = lastCheckPoint;
        isHurted = false;
        GetComponent<MoveComponent>().canMove = true;
        GetComponent<CameraComponent>().canRotate = true;       
        StartCoroutine(CameraEffects.Instance.FadeScreen(0.75f, 0));
    }
}

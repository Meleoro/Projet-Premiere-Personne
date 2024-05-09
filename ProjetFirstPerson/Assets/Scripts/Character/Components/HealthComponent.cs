using ArthurUtilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour, ICharacterComponent
{
    [Header("Parameters")] 
    [SerializeField] private float recoveryTime;
    [SerializeField] private float cameraShakeIntensity;
    [SerializeField] private float cameraShakeDuration;
    [SerializeField] private float fallMaxHeight;
    [SerializeField] private float fallRecovery;

    [Header("Public Infos")] 
    [HideInInspector] public Action DieAction;
    [HideInInspector] public bool isHurted;

    [Header("Private Infos")] 
    private bool isDying;
    private bool isInvincible;
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


    public void VerifyFall(float fallDist)
    {
        if (fallDist > fallMaxHeight)
        {
            StartCoroutine(SlowCharacter(fallRecovery, 0.1f));
            StartCoroutine(CameraEffects.Instance.TakeDamage(0.8f));
            CoroutineUtilities.Instance.ShakePosition(CameraManager.Instance.transform.parent, cameraShakeDuration, cameraShakeIntensity);
        }
    }


    public void SavePos()
    {
        lastCheckPoint = transform.position;
    }


    public void TakeDamage()
    {
        if (isInvincible) return;

        if(isHurted && !isDying)
            StartCoroutine(Die());

        CoroutineUtilities.Instance.ShakePosition(CameraManager.Instance.transform.parent, cameraShakeDuration, cameraShakeIntensity);

        GetComponent<StaminaComponent>().RegainStamina();

        isHurted = true;
        hurtTimer = recoveryTime;

        StartCoroutine(InvincibleTime());
        StartCoroutine(SlowCharacter(2, 0.5f));
        StartCoroutine(CameraEffects.Instance.TakeDamage(0.8f));
        StartCoroutine(CameraEffects.Instance.HurtEffect(recoveryTime));
    }

    private IEnumerator SlowCharacter(float duration, float slowRatio)
    {
        GetComponent<MoveComponent>().currentSpeedModifier = slowRatio;

        yield return new WaitForSeconds(duration);

        GetComponent<MoveComponent>().currentSpeedModifier = 1;
    }

    private IEnumerator InvincibleTime()
    {
        isInvincible = true;

        yield return new WaitForSeconds(1);

        isInvincible = false;
    }


    private IEnumerator Die()
    {
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

        DieAction.Invoke();
    }
}

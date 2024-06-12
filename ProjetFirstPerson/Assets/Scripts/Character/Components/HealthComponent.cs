using ArthurUtilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour, ICharacterComponent
{
    [Header("Parameters")] 
    [SerializeField] private float recoveryTime;
    [SerializeField] private float fallMaxHeight;
    [SerializeField] private float fallRecovery;
    [SerializeField] private float knockbackStrength;
    [SerializeField] private float knockbackDuration;
    
    [Header("Shake Parameters")]
    [SerializeField] private float cameraShakeIntensity;
    [SerializeField] private float cameraShakeDuration;
    [SerializeField] private float cameraShakeChangePosDuration;  
    [SerializeField] private float cameraShakeRotationIntensity;  
    
    [Header("Shake Fall Parameters")]
    [SerializeField] private float cameraShakeIntensityFall;
    [SerializeField] private float cameraShakeDurationFall;
    [SerializeField] private float cameraShakeChangePosDurationFall;    
    [SerializeField] private float cameraShakeRotationIntensityFall;    

    [Header("Public Infos")] 
    [HideInInspector] public Action DieAction;
    public bool isHurted;
    public TriggerPoursuiteFinale currentTriggerPoursuiteF;

    [Header("Private Infos")] 
    public bool isDying;
    private bool isInvincible;
    private float hurtTimer;
    private Animation anim;
    public Vector3 lastCheckPoint;
    public CameraComponent cam;
    public MoveComponent move;
    private Coroutine hurtCoroutine;


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
            AudioManager.Instance.PlaySoundOneShot(1,1,0);
            StartCoroutine(SlowCharacter(fallRecovery, 0.1f));
            StartCoroutine(CameraEffects.Instance.TakeDamage(0.8f));
            CoroutineUtilities.Instance.ShakePosition(CameraManager.Instance.transform.parent, cameraShakeIntensityFall, cameraShakeDurationFall, 
                cameraShakeChangePosDurationFall, cameraShakeRotationIntensityFall);
        }
    }


    public void SavePos()
    {
        lastCheckPoint = transform.position;
    }


    public void TakeDamage(Vector3 attackDir)
    {
        if (isInvincible || isDying) return;

        if(isHurted && !isDying)
            StartCoroutine(Die());
        else 
        {
            AudioManager.Instance.PlaySoundOneShot(1,0,0);
            StartCoroutine(move.AddKnockback(attackDir, knockbackStrength, knockbackDuration));
            //anim.clip = anim["TakeDamage"].clip;
            //anim.Play();
        }

        isInvincible = true;

        CoroutineUtilities.Instance.ShakePosition(CameraManager.Instance.transform, cameraShakeDuration, cameraShakeIntensity, 
            cameraShakeChangePosDuration, cameraShakeRotationIntensity);

        GetComponent<StaminaComponent>().RegainStamina();

        isHurted = true;
        hurtTimer = recoveryTime;

        StartCoroutine(InvincibleTime());
        StartCoroutine(SlowCharacter(1, 0.5f));
        
        StartCoroutine(CameraEffects.Instance.TakeDamage(1.2f));
        
        if(!isDying)
            hurtCoroutine = StartCoroutine(CameraEffects.Instance.HurtEffect(recoveryTime));
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

        yield return new WaitForSeconds(2.4f);

        isInvincible = false;
    }


    private IEnumerator Die()
    {
        AudioManager.Instance.PlaySoundOneShot(1,4,0);
        isDying = true;
        anim.clip = anim["Death"].clip;
        anim.Play();
        cam.canRotate = false;
        move.canMove = false;
        move.rb.isKinematic = true;
        yield return new WaitForSeconds(3);
        StartCoroutine(CameraEffects.Instance.FadeScreen(2f, 1));

        yield return new WaitForSeconds(2.5f);

        transform.position = lastCheckPoint;
        isHurted = false;
        cam.transform.GetChild(2).transform.localPosition = new Vector3(0, 0.8f, 0);
        cam.transform.GetChild(2).transform.localEulerAngles = Vector3.zero;
        
        if(currentTriggerPoursuiteF != null)
            currentTriggerPoursuiteF.ActivateTrigger();
        
        StopCoroutine(hurtCoroutine);
        CameraEffects.Instance.hurtVolume.weight = 0;
        DieAction.Invoke();

        StartCoroutine(CameraEffects.Instance.FadeScreen(0.75f, 0));
        yield return new WaitForSeconds(0.5f);
        cam.canMove = true;
        cam.canRotate = true;
        move.rb.isKinematic = false;
        move.canMove = true;
        isDying = false;
        
        DieAction.Invoke();
    }
}

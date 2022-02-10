using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]

public class PierreActor : MonoBehaviour
{
    // Yup it's GOD
    public PierreManagerAI MAI;

    public Camera camera;

    // ---------- Audio -------------
    public AudioClip audioMort;
    public AudioClip[] audioHit;
    public AudioClip[] audioMove;
    // -------------------------------

    // movement
    private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;


    public float speed = 3.0f;

    // Components
    private AudioSource audioPlayer;
    private CharacterController characterController;
    private Animator animator;

    // ----------- Factions ---------------
    private string factions = "";
    // Editor helper
    public bool catFaction = false;
    public bool dogFaction = false;
    public bool lizardFaction = false;
    // ------------------------------------

    // health
    private bool dead = false;
    private int health = 100;
    public PierreBar healthBar;
    public bool activated = false;

    // target (public is more easier)
    public PierreActor target = null;

    public AudioSource GetAudioPlayer() { return audioPlayer; }
    public CharacterController GetCharacterController() { return characterController; }

    public void MoveCharacter(Vector3 force)
    {
        if(characterController.enabled)
            characterController.Move(force * Time.deltaTime);
    }

    public void RotateTowards(Vector3 direction)
    {
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    // ---------------------------- Animator controls --------------------------------
    public void AnimPlayIdle()
    {
        animator.SetBool("attacking", false);
        animator.SetBool("moving", false);
        animator.SetBool("dying", false);
    }
    public void AnimPlayAttack()
    {
        animator.SetBool("attacking", true);
        animator.SetBool("moving", false);
        animator.SetBool("dying", false);
    }

    public void AnimPlayMoving()
    {
        animator.SetBool("attacking", false);
        animator.SetBool("moving", true);
        animator.SetBool("dying", false);
    }

    public void AnimPlayDying()
    {
        animator.SetBool("attacking", false);
        animator.SetBool("moving", false);
        animator.SetBool("dying", true);
    }
    public bool AnimIsAttacking() { return animator.GetBool("attacking"); }
    public bool AnimIsDying() { return animator.GetBool("dying"); }
    public bool AnimIsMoving() { return animator.GetBool("moving"); }

    // ------------------------------------------------------------------------------
    public string GetFactions() { return factions;}
    public bool IsDead() { return dead; }
    

    public void MoveSound()
    {
        if (!audioPlayer.isPlaying)
        {
            audioPlayer.volume = 0.01f;
            audioPlayer.clip = audioMove[Random.Range(0, 3)];
            audioPlayer.Play();
        }
    }

    // hurt this actor
    public void Hurt(int damage)
    {
        // already dead dude
        if (dead) return;
        health -= damage;
        

        // it should dies now
        if(health <= 0)
        {
            health = 0;
            Die();
        }
        healthBar.SetValue(health);
    }

    public virtual void Die()
    {
        healthBar.SetVisible(false);
        dead = true;
        AnimPlayDying();
        audioPlayer.Stop();
        audioPlayer.volume = 0.1f;
        audioPlayer.clip = audioMort;
        audioPlayer.Play();
        if (target != null)
            MAI.Untarget(target);
        target = null;

        characterController.detectCollisions = false;
        characterController.enabled = false;

        if (catFaction) MAI.DecreaseCatsNumber();
        if (dogFaction) MAI.DecreaseDogsNumber();
        if (lizardFaction) MAI.DecreaseLizardsNumber();
    }

    public virtual void Resurrect()
    {
        healthBar.SetVisible(true);
        healthBar.SetValue(100);
        AnimPlayIdle();
        dead = false;
        health = 100;
        target = null;
        characterController.detectCollisions = true;
        characterController.enabled = true;
        if (catFaction) MAI.IncreaseCatsNumber();
        if (dogFaction) MAI.IncreaseDogsNumber();
        if (lizardFaction) MAI.IncreaseLizardsNumber();
    }

    // Damage someone (can override)
    public virtual void DoDamage(int damage)
    {
        if (!audioPlayer.isPlaying)
        {
            audioPlayer.volume = 0.05f;
            audioPlayer.clip = audioHit[Random.Range(0, 3)];
            audioPlayer.Play();
        }
        if (AnimIsAttacking() && target != null)
            target.Hurt(damage);
    }

    // -------------------------------- Unity methods --------------------------
    protected virtual void Start()
    {
        // Components
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        audioPlayer = GetComponent<AudioSource>();
        audioPlayer.maxDistance = 20;
        audioPlayer.volume = 0.1f;

        // Factions
        if (catFaction)
        {
            factions += PierreFactionType.catFaction;
            MAI.IncreaseCatsNumber();
        }
        if (dogFaction) { 
            factions += PierreFactionType.dogFaction;
            MAI.IncreaseDogsNumber();
        }
        if (lizardFaction)
        {
            factions += PierreFactionType.lizardFaction;
            MAI.IncreaseLizardsNumber();
        }
        MAI.AddActor(this);
        healthBar.SetMaxValue(100);
        healthBar.SetValue(100);
    }
}

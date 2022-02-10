using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PierrePlayer : PierreActor
{

    // --------Inputs Keys -------------------
    public string keyForward;
    public string keyBackward;
    public string keyRight;
    public string keyLeft;
    public string keyAttack;
    //public string keyHelp;
    public string keySprint;
    // ---------------------------------------

    private bool isGrounded = false;
    private float vy = 0;
    private float gravity = 10;
    public LayerMask groundMask;

    public float sprintTime = 3.0f;
    private bool sprinting = false;
    private bool canSprint = true;
    private float baseSpeed;

    public PierreBar sprintBar;
    private float sprintUpdateTime = 3.0f;
    public AudioClip sprintAudio;
    public AudioClip sprintReadyAudio;
    public AudioClip helpingAudio;


    private float resurectTime = 2.0f;
    private float resurectTimer = 0.0f;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        baseSpeed = speed;
        sprintBar.SetMaxValue(100);
        sprintBar.SetValue(100);
        sprintBar.SetVisible(false);
    }

    public override void Die()
    {
        base.Die();
        resurectTimer = 0.0f;
        speed = baseSpeed;
        canSprint = false;
        sprinting = false;
        sprintBar.SetVisible(true);
        sprintBar.SetValue(0);
    }

    public override void Resurrect()
    {
        base.Resurrect();
        sprinting = false;
        sprintBar.SetVisible(false);
        GetAudioPlayer().volume = 0.3f;
        GetAudioPlayer().clip = sprintReadyAudio;
        GetAudioPlayer().Play();
        canSprint = true;
        speed = baseSpeed;
        resurectTimer = 0.0f;
        sprintUpdateTime = 3.0f;
    }

    private IEnumerator StartSprint()
    {
        GetAudioPlayer().volume = 0.3f;
        GetAudioPlayer().clip = sprintAudio;
        GetAudioPlayer().Play();
        sprintBar.SetVisible(true);
        canSprint = false;
        sprinting = true;
        speed = baseSpeed * 1.5f;
        yield return new WaitForSeconds(sprintTime);
        if(!IsDead())
            StartCoroutine(RecoverSprint());
    }
    private IEnumerator RecoverSprint()
    {
        speed = baseSpeed;
        sprinting = false;
        yield return new WaitForSeconds(sprintTime);
        if(!IsDead())
        {
            canSprint = true;
            sprintBar.SetVisible(false);
            GetAudioPlayer().volume = 0.3f;
            GetAudioPlayer().clip = sprintReadyAudio;
            GetAudioPlayer().Play();
        }
    }

    public void Help()
    {
        resurectTimer += Time.deltaTime;
        if(resurectTimer >= resurectTime)
        {
            resurectTimer = 0.0f;
            Resurrect();
        }
    }

    public override void DoDamage(int damage)
    {
        base.DoDamage(damage*2);
    }


    // Update is called once per frame
    void Update()
    {
        // Gravity
        isGrounded = Physics.CheckSphere(transform.position, 0.2f, groundMask);
        if (isGrounded && vy < 0)
        {
            vy = -2;
        }
        vy += gravity * Time.deltaTime;
        MoveCharacter(Vector3.down * vy);

        if (!activated)
        {
            AnimPlayIdle();
            return;
        }

        // handle death
        if (IsDead())
        {
            resurectTimer -= Time.deltaTime * 0.25f;
            if (resurectTimer <= 0) resurectTimer = 0;
            sprintBar.SetValue((int)(100.0f * (resurectTimer / resurectTime)));
            return;
        }

        // update sprint bar
        if (sprinting)
        {
            sprintUpdateTime -= Time.deltaTime;
            if (sprintUpdateTime < 0) sprintUpdateTime = 0;
            sprintBar.SetValue((int)(100.0f * (sprintUpdateTime / 3.0f)));
        }
        else
        {
            sprintUpdateTime += Time.deltaTime;
            if (sprintUpdateTime > 3.0f) sprintUpdateTime = 3.0f;
            sprintBar.SetValue((int)(100.0f * (sprintUpdateTime / 3.0f)));
        }

        // Help the first players in need
        bool helping = false;
        foreach (PierrePlayer p in MAI.players){
            if (!p.IsDead()) continue;
            float dd = (p.transform.position - transform.position).sqrMagnitude;
            if(dd <= 1.5 * 1.5)
            {
                if (!GetAudioPlayer().isPlaying)
                {
                    GetAudioPlayer().volume = 0.3f;
                    GetAudioPlayer().clip = helpingAudio;
                    GetAudioPlayer().Play();
                }
                p.Help();
                helping = true;
            } 
        }

        if (!helping)
        {
            if (GetAudioPlayer().clip == helpingAudio)
                GetAudioPlayer().Stop();
        }

        // Handle attack
        if (Input.GetKey(keyAttack) && !sprinting)
        {
            // No target
            target = null;

            // Get all enemis
            List<PierreActor> enemies = MAI.GetEnemies(GetFactions());
            enemies.RemoveAll(e => e.IsDead());

            if (!(enemies.Count == 0))
            {
                // Find the closest enemy
                PierreActor closest = enemies[0];
                float ddmin = (closest.transform.position - transform.position).sqrMagnitude;
                foreach (PierreActor e in enemies)
                {
                    float dd = (e.transform.position - transform.position).sqrMagnitude;
                    if (dd <= ddmin)
                    {
                        closest = e;
                        ddmin = dd;
                    }
                }
                // if it's close enough, it become the target
                if (ddmin <= 1.5 * 1.5)
                    target = closest;
            }
            if(target != null)
            {
                Vector3 dir = (target.transform.position - transform.position);
                dir.y = 0;
                RotateTowards(dir.normalized);
            }
            AnimPlayAttack();
            return;
        }

        // Handle Sprint
        if (Input.GetKeyDown(keySprint))
        {
            if (canSprint)
                StartCoroutine(StartSprint());
        }

        // Handle movement
        Vector3 direction = Vector3.zero;
        if (Input.GetKey(keyForward))
            direction += Vector3.forward;
        if (Input.GetKey(keyBackward))
            direction += Vector3.back;
        if (Input.GetKey(keyLeft))
            direction += Vector3.left;
        if (Input.GetKey(keyRight))
            direction += Vector3.right;
        direction.Normalize();

        if(direction.sqrMagnitude > 0.01)
        {
            AnimPlayMoving();
            RotateTowards(direction);
            MoveCharacter(direction * speed);
        } else
        {
            AnimPlayIdle();
        }
    }
}

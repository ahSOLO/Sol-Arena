using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Spore : Enemy
{
    // Variables
    private enum State { idle, running, attack1, attack2 };
    private State state = State.idle;
    private float attackTimer1 = 2f;
    private AudioSource audioSource;
    [SerializeField] private float attackTimer1Set = 1.8f;
    [SerializeField] private Transform attackPoint1;
    [SerializeField] private float attackRange1 = 0;
    [SerializeField] private int attackDamage1 = 0;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private AIPath path;

    // call Enemy.Start()
    protected override void Start()
    {
        audioSource = GetComponent<AudioSource>();
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        // Set animation state and facing distance based on path direction
        path.canMove = true;
        if (path.desiredVelocity.x < -0.5f)
        {
            transform.localScale = new Vector3(-1, 1); // facing left if moving left
            if (attacking == false) state = State.running;
        }
        else if (path.desiredVelocity.x > 0.5f)
        {
            transform.localScale = new Vector3(1, 1); // facing right if moving right
            if (attacking == false) state = State.running;
        }
        else
        {
            if (attacking == false) state = State.idle;
        }

        // Attack when reached path destination  - attack checks occur after turn checks to allow for enemy to track player during attacks
        attackTimer1 -= Time.deltaTime;

        if (path.reachedEndOfPath && attackTimer1 < 0)
        {
            if (GetComponent<AIDestinationSetter>().target != null) // only attack if player object exists as a target
            {
                state = State.attack1;
                attacking = true;
                attackTimer1 = attackTimer1Set; // set delay between each attack
            }
        }

        if (attacking == true) path.canMove = false; // prevent movement during attack 

        // Update animation state
        anim.SetInteger("state", (int)state);
    }

    // Ordinary attack
    void SporeHit1()
    {
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint1.position, attackRange1, playerLayer); // detect enemies in range of attack
        if (hitPlayer != null) hitPlayer.GetComponent<PlayerController>().DamagePlayer(attackDamage1);
        AttackSound();
    }

    void OnDrawGizmosSelected() // draw gizmos to show range of attack
    {
        if (attackPoint1 != null)
        {
            Gizmos.DrawWireSphere(attackPoint1.position, attackRange1);
        }

    }

    // Sound Effects
    void AttackSound()
    {
        SoundController.sC.Play(audioSource, SoundController.sC.sporeAttack, 0.9f);
    }

    void WalkSound()
    {
        SoundController.sC.Play(audioSource, SoundController.sC.sporeWalk, 0.2f);
    }

    void HurtSound()
    {
        SoundController.sC.Play(audioSource, SoundController.sC.sporeHurt, 1);
    }
    void DeathSound()
    {
        SoundController.sC.Play(audioSource, SoundController.sC.sporeDeath, 1f);
    }
}

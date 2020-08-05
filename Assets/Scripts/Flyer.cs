using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Flyer : Enemy
{
    // Variables
    private enum State { idle, unused, attack1, attack2} // using same FSM states as skeleton to for animation override controller
    private State state = State.idle;
    private float attackTimer1 = 2f;
    private AudioSource audioSource;
    [SerializeField] private float attackTimer1Set = 2f;
    [SerializeField] private Transform attackPoint1;
    [SerializeField] private float attackRange1 = 0;
    [SerializeField] private int attackDamage1 = 0;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private AIPath path;

    // call Enemy.Start()
    protected override void Start()
    {
        base.Start();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // Set animation state and facing distance based on path direction
        if (path.desiredVelocity.x < -1f)
        {
            transform.localScale = new Vector3(-1, 1); // facing left if moving left
        }
        else if (path.desiredVelocity.x > 1f)
        {
            transform.localScale = new Vector3(1, 1); // facing right if moving right
        }

        if (attacking == false) state = State.idle;

        // Attack when reached path destination
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

        // Update animation state
        anim.SetInteger("state", (int)state);
    }
    // Ordinary attack
    void FlyerHit1()
    {
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint1.position, attackRange1, playerLayer); // detect enemies in range of attack
        if (hitPlayer != null) hitPlayer.GetComponent<PlayerController>().DamagePlayer(attackDamage1);
        AttackSound();
    }

    void OnDrawGizmosSelected() // draw gizmos to show attack range
    {
        if (attackPoint1 != null)
        {
            Gizmos.DrawWireSphere(attackPoint1.position, attackRange1);
        }

    }

    // Sound Effects
    void AttackSound()
    {
        SoundController.sC.Play(audioSource, SoundController.sC.flyerAttack, 1f);
    }

    void FlapSound()
    {
        SoundController.sC.Play(audioSource, SoundController.sC.flyerFlap, 0.2f);
    }

    void HurtSound()
    {
        SoundController.sC.Play(audioSource, SoundController.sC.flyerHurt, 0.8f);
    }
    void DeathSound()
    {
        SoundController.sC.Play(audioSource, SoundController.sC.flyerDeath, 1f);
    }
}

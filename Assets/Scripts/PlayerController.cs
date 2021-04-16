using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{    
    // Movement variables
    [SerializeField] private float hSpeed = 5f;
    [SerializeField] private float jumpVelocity = 10f;
    [SerializeField] private LayerMask collisionLayer;
    [SerializeField] private GameObject dust;
    private float coyoteTime = 0f;
    private float hDirection;
    private int jumpsLeft = 2;

    // Component variables
    private Rigidbody2D rigidBody;
    private Collider2D col;
    private Animator animator;
    
    // FSM variables
    private enum State { idle, running, jump, fall, attack1, attack2, dead };
    private State state = State.idle;
    private bool attacking = false;
    
    // Health variables
    public int playerMaxHealth = 100;
    public int playerCurrentHealth;
    public HealthBar healthBar;

    // Attack variables
    private float attack1Timer;
    public LayerMask enemyLayer;
    [SerializeField] private Transform attackPoint1;
    [SerializeField] private float attackRange1 = 0.5f;
    [SerializeField] private int attackDamage1 = 25;
    [SerializeField] private Transform attackPoint2a;
    [SerializeField] private Transform attackPoint2b;
    [SerializeField] private int attackDamage2 = 50;

    // Color variables
    private Color initColor;
    private float colorTimer = 0f;

    // Sound Variables
    private AudioSource audioSource;

    // Static Variable
    public static PlayerController pC; // referenced in Spawner script to assign enemy pathfinding target to player transform

    // Initialization
    void Start()
    {
        pC = this;
        rigidBody = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        playerCurrentHealth = playerMaxHealth;
        healthBar.SetMaxHealth(playerMaxHealth);
        initColor = new Color(.44f, .9f, 1f, 1f); // These are values for the default player spotlight color
        audioSource = GetComponent<AudioSource>();
        attack1Timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // **Player Movement**
        if (state != State.dead) // only move if alive
        {
            // Get Input
            float hDirection = Input.GetAxisRaw("Horizontal");
            // Set coyote time - downwards raycast from left and right bounds of collision box
            SetCoyoteTime(0.15f);
            // Disable horizontal ground movement, jumping ability, and FSM transitions when attacking
            if (attacking == false)
            {
                HoriMovement(hDirection); // Horizontal ground movement
                Jump(); // check for jump
                // ** Other Animation FSM **
                if (state == State.jump) // transition from jump to fall
                {
                    if (rigidBody.velocity.y <= 0)
                    {
                        state = State.fall;
                    }
                }
                else if (state == State.fall) // transition from fall to idle
                {
                    if (coyoteTime > 0.05f) // prevents game from treating certain jumps into low ceilings as landings
                    {
                        state = State.idle;
                        // Generate Dust
                        GenDust();
                        // Landing sound effect
                        SoundController.sC.Play(audioSource, SoundController.sC.pLand, 1f);
                    }
                }
                else if (Mathf.Abs(hDirection) > 0.1f && coyoteTime > 0f) // transition from any other state to running (.1 controller deadzone)
                {
                    state = State.running;
                }
                else state = State.idle; // If no conditions met, default to idle
            }
            AirMovement(hDirection); // Aerial movement -- half speed; does not change sprite direction

            // Attack check - overrides movement animations + animation cancels each other
            Attack();
        }

        // Advance player off-color animation timer - determines when to revert to default color after player is hit by an attack / picks up a potion.
        colorTimer -= Time.deltaTime;
        if (colorTimer < 0) GetComponentInChildren<Light2D>().color = initColor;

        // Sets animation state
        animator.SetInteger("state", (int)state);
    }

    private void SetCoyoteTime(float cTime) // Uses two downward raycasts to set coyote time for use with movement functions.
    {
        RaycastHit2D hit1 = Physics2D.Raycast(new Vector2(rigidBody.position.x - 0.26f, rigidBody.position.y), Vector2.down, 0.995f, collisionLayer); // raycast down distance dependent on character height
        RaycastHit2D hit2 = Physics2D.Raycast(new Vector2(rigidBody.position.x + 0.26f, rigidBody.position.y), Vector2.down, 0.995f, collisionLayer); // raycast x pos dependent on character width
        if (hit1.collider != null | hit2.collider != null)
        {
            coyoteTime = cTime;
            if (state != State.jump) jumpsLeft = 2;
        }
        coyoteTime -= Time.deltaTime;
    }

    private void HoriMovement(float hDirection) // checks for horizontal input while on the ground, changes facing distance
    {
        if (coyoteTime > 0f)
        {
            if (hDirection > 0.2f)
            {
                rigidBody.velocity = new Vector2(hSpeed, rigidBody.velocity.y);
                transform.localScale = new Vector2(1, 1);
            }
            else if (hDirection < -0.2f)
            {
                rigidBody.velocity = new Vector2(-hSpeed, rigidBody.velocity.y);
                transform.localScale = new Vector2(-1, 1);
            }
            else rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
        }
    }

    private void AirMovement(float hDirection) // Checks for horizontal input while in the air, does not change facing distance
    {
        if (coyoteTime < 0.10f)
        {
            if (hDirection > 0.2f && rigidBody.velocity.x <= 0)
            {
                rigidBody.velocity = new Vector2(hSpeed / 2, rigidBody.velocity.y);
            }
            else if (hDirection < -0.2f && rigidBody.velocity.x >= 0)
            {
                rigidBody.velocity = new Vector2(-hSpeed / 2, rigidBody.velocity.y);
            }
            else rigidBody.velocity = new Vector2(rigidBody.velocity.x, rigidBody.velocity.y);
        }
    }

    private void Jump() // checks for jump ability. Only possible if there are jumps remaining, determined by # of previous jumps + when player last touched the ground
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (jumpsLeft > 0)
            {
                if (coyoteTime > 0f | jumpsLeft == 1) rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpVelocity);
                state = State.jump;
                jumpsLeft--;
                // Sound effect
                SoundController.sC.Play(audioSource, SoundController.sC.pJump, 1f);
            }
        }
    }

    private void Attack() // Attack check - overrides movement animations + animation cancels each other
    {
        attack1Timer -= Time.deltaTime;

        if (Input.GetButtonDown("Fire1"))
        {
            if (attack1Timer < 0)
            {
                state = State.attack1;
                attacking = true;
                // Place a 1 second pause between fast attacks to prevent player from being able to stun lock enemies
                attack1Timer = 1f;
            }
        }

        if (Input.GetButtonDown("Fire2"))
        {
            state = State.attack2;
            attacking = true;
        }
    }

    public void DamagePlayer(int damage) // called when player is hit by enemy attack
    {
        if (state != State.dead) // only call if player is alive
        {
            playerCurrentHealth -= damage;
            healthBar.SetHealth(playerCurrentHealth);
            // Change spotlight color to red for 0.15 seconds
            GetComponentInChildren<Light2D>().color = Color.red;
            colorTimer = 0.15f;
            // Sound effect
            SoundController.sC.Play(audioSource, SoundController.sC.pHurt, 1f);
            // Check for player death
            if (playerCurrentHealth <= 0)
            {
                col.enabled = false; // prevent further damage
                rigidBody.isKinematic = true; // prevent gravity from applying to corpse
                rigidBody.velocity = new Vector2(0, 0);
                state = State.dead;
                // Sound effect
                SoundController.sC.Play(SoundController.sC.sEffectSource, transform.position, SoundController.sC.pDeath, 1f);
            }
        }
    }
    // Generate dust particles when landing from a jump (the fall state)
    private void GenDust() 
    {
        GameObject _dust = Instantiate(dust, transform.position, UnityEngine.Random.rotation);
        _dust.transform.Translate(UnityEngine.Random.Range(-0.5f, 0f), -1.05f, 0f, Space.World);
        _dust = Instantiate(dust, transform.position, UnityEngine.Random.rotation);
        _dust.transform.Translate(UnityEngine.Random.Range(-0.25f, 0.25f), -1.05f, 0f, Space.World);
        _dust = Instantiate(dust, transform.position, UnityEngine.Random.rotation);
        _dust.transform.Translate(UnityEngine.Random.Range(0f, 0.5f), -1.05f, 0f, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision) // Picking up potions heals health
    {
        if (collision.tag == "Potion")
        {
            playerCurrentHealth += 25;
            if (playerCurrentHealth > 100) playerCurrentHealth = 100;
            healthBar.SetHealth(playerCurrentHealth);
            GetComponentInChildren<Light2D>().color = Color.white; // Temporarily changes player color; timer counts down in update function
            colorTimer = 0.15f;
            // Play sound effect
            SoundController.sC.Play(audioSource, SoundController.sC.drinkPotion, 1f);
        }
    }

    private void EndAttack() // Used at the end of attack animations
    {
        attacking = false;
    }

    private void Hit1() // Player's primary (fast) attack - use in attack1 animation
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint1.position, attackRange1, enemyLayer); // detect enemies in range of attack
        foreach(Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().takeDamage(attackDamage1);
        }
        // Sound effect
        SoundController.sC.Play(audioSource, SoundController.sC.pAtk1, 1f);
        // Camera shake
        CameraShake.cS.ShakeCamera(1f, 0.25f);
    }

    private void Hit2() // Player's secondary (slow) attack - use in attack2 animation
    {
        Collider2D[] hitEnemies = Physics2D.OverlapAreaAll(attackPoint2a.position, attackPoint2b.position, enemyLayer); // detect enemies in range of attack
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().takeDamage(attackDamage2);
        }
        // Sound effect
        SoundController.sC.Play(audioSource, SoundController.sC.pAtk2, 1f);
        // Camera shake
        CameraShake.cS.ShakeCamera(2f, 0.35f);
    }

        private void OnDrawGizmosSelected() // Draw gizmos to visually depict attack distance and range
    {
        if (attackPoint1 != null && attackPoint2a != null && attackPoint2b != null)
        {
            Gizmos.DrawWireSphere(attackPoint1.position, attackRange1);
            Gizmos.DrawLine(attackPoint2a.position, attackPoint2b.position);
        }
    }

    public void Destroy() // Destroys player - runs at end of death animation
    {
        GameController.gC.Invoke("GameOver", 3.5f); // moves to Gameover screen after 3.5 seconds
        Destroy(gameObject); // destroys player objects so monsters no longer attack after player death
    }

    void Footstep() // Footstep Audio
    {
        SoundController.sC.Play(audioSource, SoundController.sC.pFootsteps, 0.8f);
    }
}
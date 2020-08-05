using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Health Variables
    [SerializeField] protected int maxHealth = 100;
    protected int currentHealth;
    
    // Layer Variables
    [SerializeField] protected LayerMask enemiesLayer;
    [SerializeField] protected LayerMask collisionLayer;
    [SerializeField] protected int corpseLayerInt;
    
    // Component Variables
    protected Rigidbody2D rb;
    protected Animator anim;
    protected Collider2D col;
    
    // Other Variables
    protected bool attacking = false;
    protected Transform playerTransform;
    
    // Initialize variables
    protected virtual void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        if ((playerTransform = GameObject.FindGameObjectWithTag("Player").transform) != null) GetComponent<AIDestinationSetter>().target = playerTransform; // Sets pathfinding target to player transform
    }

    protected void EndAttack() // called at end of attack animation
    {
        attacking = false;
    }

    public void takeDamage(int damage) // called when receiving damage from player - see Hit1() and Hit2() in player controller
    {
        if (currentHealth > 0)
        {
            attacking = false; // allows player to interrupt enemy attacks
            currentHealth -= damage;
            anim.SetTrigger("Hurt"); // triggers hurt animation
        }
    }

    public void checkDeath() // checks if enemy is dead - called at end of hurt animation
    {
        if (currentHealth <= 0) // if enemy has no hit point left...
        {
            GameController.gC.AdvanceScore(maxHealth); // advance score by enemy's max health
            gameObject.layer = corpseLayerInt; // change layer to prevent player attack and living enemy collisions
            anim.SetBool("Death", true); // play death animation
            // disable pathfinding
            GetComponent<AIPath>().enabled = false;
            GetComponent<Seeker>().enabled = false;
            GetComponent<AIDestinationSetter>().enabled = false; 
            // apply gravity to corpse (for flyer enemies) and remove non-gravity velocity
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.velocity = new Vector2(0, 0);
            rb.gravityScale = 5;
            col.isTrigger = false; // allow corpse to collide with ground
            Destroy(this.gameObject,3f); // remove corpse in 3 seconds
        }
    }
    // Prevent enemy overlap on the same pixel
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy" && other.gameObject.layer == 9) // if colliding with other living enemy...
        {
            if (other.gameObject.transform.position.x < transform.position.x)
            {
                transform.position = new Vector2(transform.position.x + 0.01f, transform.position.y); // ...move 0.01 left if left of other enemy
            }
            if (other.gameObject.transform.position.x == transform.position.x)
            {
                transform.position = new Vector2(transform.position.x + (UnityEngine.Random.Range(-1, 1) / 100), transform.position.y); // randomly move 0.01 if on same pixel as other enemy
            }
            else transform.position = new Vector2(transform.position.x - 0.01f, transform.position.y); // ...move 0.01 right if right of other enemy
        }
    }
}

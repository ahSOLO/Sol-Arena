using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    Animator anim;
    ParticleSystem part;
    Collider2D col;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        part = GetComponentInChildren<ParticleSystem>();
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // On contact with player, play animation + particle system and destroy after 1.2 seconds;
        if (collision.tag == "Player")
        {
            anim.SetTrigger("potion");
            part.Play();
            col.enabled = false;
            Destroy(gameObject, 1.2f);
        }
    }
}

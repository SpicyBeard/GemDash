using UnityEngine;
using System.Collections;

public class LaserSpikes : MonoBehaviour
{

    [Header("Respawn Location")]
    public Transform playerTransform;
    public Vector2 respawnLocation; 
    
    [Header("Audio")]
    public AudioClip hitSound;
    private AudioSource audioSource;
    private bool soundPlayed = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    // Detect collision with the edge collider
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player1"))
        {
            StartCoroutine(PlayerHitActions(collision.collider));
        }
    }


    IEnumerator PlayerHitActions(Collider2D player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if(rb != null)
        {
            rb.simulated = false;
        }
        SpriteRenderer spriteRenderer = player.GetComponentInChildren<SpriteRenderer>();
        if(spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        ParticleSystem particleSystem = player.GetComponentInChildren<ParticleSystem>();
        if(particleSystem != null)
        {
            particleSystem.Play();
        }

        if (hitSound != null && !soundPlayed)
        {
            audioSource.PlayOneShot(hitSound);
            soundPlayed = true;
        }

        yield return new WaitForSeconds(2.5f);

        player.transform.position = respawnLocation;

        if(spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }
        if(particleSystem != null)
        {
            particleSystem.Stop();
            particleSystem.Clear();
        }
        if(rb != null)
        {
            rb.simulated = true;
            player.transform.localScale = new Vector3(1, 1, 1);
        }

        soundPlayed = false;
    }
}

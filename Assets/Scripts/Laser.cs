using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour
{
    public LayerMask layersToHit;
    private LineRenderer lineRenderer;
    public Transform playerTransform; // Assign this in the inspector with the player's Transform
    public Vector2 respawnLocation; // Set this to where you want the player to respawn
    public AudioClip hitSound; // Assign this in the inspector
    private AudioSource audioSource; // To play the sound
    private bool soundPlayed = false;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        ShootLaser();
    }

    void ShootLaser()
    {
        float angle = transform.eulerAngles.z * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 100f, layersToHit);
        lineRenderer.SetPosition(0, transform.position);

        if (hit.collider != null)
        {
            lineRenderer.SetPosition(1, hit.point);
            if (hit.collider.CompareTag("Player1"))
            {
                StartCoroutine(PlayerHitActions(hit.collider));
            }
        }
        else
        {
            lineRenderer.SetPosition(1, transform.position + (Vector3)direction * 100f);
            soundPlayed = false; // Reset the flag when the laser is not hitting the player
        }
    }

    IEnumerator PlayerHitActions(Collider2D player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if(rb != null)
        {
            rb.simulated = false;
        }
        // Disable the player's SpriteRenderer
        SpriteRenderer spriteRenderer = player.GetComponentInChildren<SpriteRenderer>();
        if(spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        // Enable the player's ParticleSystem
        ParticleSystem particleSystem = player.GetComponentInChildren<ParticleSystem>();
        if(particleSystem != null)
        {
            particleSystem.Play();
        }

        // Play a sound effect
        if (hitSound != null && !soundPlayed)
        {
            audioSource.PlayOneShot(hitSound);
            soundPlayed = true;
        }

        // Wait for 2 seconds
        yield return new WaitForSeconds(2.5f);

        // Move the player to a specific location
        player.transform.position = respawnLocation;

        // Re-enable the SpriteRenderer if desired
        if(spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        // Optionally stop the ParticleSystem if needed
        if(particleSystem != null)
        {
            particleSystem.Stop();
            particleSystem.Clear();
        }

        // Reset the Rigidbody simulation if it was disabled
        if(rb != null)
        {
            rb.simulated = true;
        }

        soundPlayed = false;
    }
}

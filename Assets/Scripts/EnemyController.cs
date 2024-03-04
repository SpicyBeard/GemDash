using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [Header("Respawn Location")]

        public Transform playerTransform;
        public Vector2 respawnLocation; 

    [Header("Enemy shit")]

        private bool isInitiallyVisible = false;
        private bool isVisible = false; // Current visibility state of the enemy
        public float bounceForce = 5f;
        public float bounceVelocity = 5f;

    [Header("Camera Shit")]

        public Camera cameraUsed;
        private Renderer renderer;

    [Header("Audio")]

        public AudioClip slimeSound;
        public AudioClip eatSound;
        private bool eatSoundPlayed = false;
        private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        renderer = GetComponent<Renderer>();
        UpdateVisibility();
        isInitiallyVisible = isVisible; // Set the initial visibility based on the first check
    }

    void Update()
    {
        UpdateVisibility();

        // If the enemy has just become visible and it was not initially visible, play the audio
        if (isVisible && !isInitiallyVisible)
        {
            audioSource.PlayOneShot(slimeSound);
            isInitiallyVisible = true; // Prevent the audio from playing again
        }
        else if (!isVisible)
        {
            // When the enemy is no longer visible, allow for the audio to be played again when it becomes visible
            isInitiallyVisible = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player1"))
        {
            StartCoroutine(PlayerHitActions(collision.collider));
        }
    }

    IEnumerator PlayerHitActions(Collider2D player)
    {
        // Play the eat sound when the player is hit
        if (eatSound != null && !eatSoundPlayed)
        {
            audioSource.PlayOneShot(eatSound);
            eatSoundPlayed = true;
        }

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = false;
        }

        SpriteRenderer spriteRenderer = player.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        ParticleSystem particleSystem = player.GetComponentInChildren<ParticleSystem>();
        if (particleSystem != null)
        {
            particleSystem.Play();
        }

        yield return new WaitForSeconds(1.5f); // Adjust the delay as needed

        player.transform.position = respawnLocation;
        Debug.Log("Respawning player at: " + respawnLocation);

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }
        if (particleSystem != null)
        {
            particleSystem.Stop();
            particleSystem.Clear();
        }
        if (rb != null)
        {
            rb.simulated = true;
            player.transform.localScale = new Vector3(1, 1, 1);
        }

        eatSoundPlayed = false; // Reset the flag for the next collision
    }


    void UpdateVisibility()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cameraUsed);
        isVisible = GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
}
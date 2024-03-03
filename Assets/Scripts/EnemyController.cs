using UnityEngine;
using System.Collections;
using AGDDPlatformer;
using UnityEditor.Experimental.GraphView;

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
        public AudioClip hitSound;
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
            audioSource.Play();
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
            StartCoroutine(HandleCollision(collision)); // Start the coroutine
        }
    }

    IEnumerator HandleCollision(Collision2D collision)
    {
        Vector2 normal = collision.contacts[0].normal;
        float angle = Vector2.Angle(normal, Vector2.up);

        if (angle < 45)
        {
            // boing haha
            yield break; // Exit the coroutine early if the angle is less than 45
        }
        else
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.simulated = false;
            }

            SpriteRenderer spriteRenderer = collision.gameObject.GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
            }

            ParticleSystem particleSystem = collision.gameObject.GetComponentInChildren<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Play();
            }

            if (hitSound != null && !eatSoundPlayed)
            {
                audioSource.PlayOneShot(hitSound);
                eatSoundPlayed = true;
            }

            yield return new WaitForSeconds(1.5f); // Wait for 1.5 seconds

            collision.transform.position = respawnLocation;

            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true;
            }

            if (particleSystem != null)
            {
                particleSystem.Stop();
                particleSystem.Clear();
            }

            if (playerRb != null)
            {
                playerRb.simulated = true;
                playerRb.transform.localScale = new Vector3(1, 1, 1);
            }

            eatSoundPlayed = false;
        }
    }


    void UpdateVisibility()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cameraUsed);
        isVisible = GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
}
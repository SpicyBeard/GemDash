using UnityEngine;
using System.Collections;
using AGDDPlatformer;

public class EnemyController : MonoBehaviour
{
    [Header("Respawn Location")]

        public Transform playerTransform;
        public Vector2 respawnLocation; 

    [Header("Enemy Visibility")]

        private bool isInitiallyVisible = false;
        private bool isVisible = false; // Current visibility state of the enemy

    [Header("Camera Shit")]

        public Camera cameraUsed;
        private Renderer renderer;

    [Header("Audio")]

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
        // Check if the colliding object is the player
        if (collision.gameObject.CompareTag("Player")) // Make sure your player GameObject is tagged as "Player"
        {
            // Access the PlayerController component on the player GameObject
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            collision.transform.position = respawnLocation; // Ensure respawnLocation is accessible here, possibly by making it public/static or referencing it from another script
        }
    }

    void UpdateVisibility()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cameraUsed);
        isVisible = GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
}
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private AudioSource audioSource;
    private bool isInitiallyVisible = false;
    private bool isVisible = false; // Current visibility state of the enemy
    public Camera cameraUsed;
    private Renderer renderer;

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

    void UpdateVisibility()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cameraUsed);
        isVisible = GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
}
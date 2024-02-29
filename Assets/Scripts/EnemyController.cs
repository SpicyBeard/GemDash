using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private AudioSource audioSource;
    private bool isInitiallyVisible = false; // Assume the enemy is not visible at the start

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // Initially assume the enemy is not visible
        isInitiallyVisible = GetComponent<Renderer>().isVisible;
    }

    void OnBecameVisible()
    {
        // Only play audio if the enemy was not initially visible
        if (!isInitiallyVisible)
        {
            audioSource.Play();
            // After the first play, consider the enemy as initially visible to prevent repeated plays
            isInitiallyVisible = true;
        }
    }

    void OnBecameInvisible()
    {
        // When the enemy becomes invisible, reset the isInitiallyVisible flag
        // This allows the audio to play again next time the enemy becomes visible,
        // simulating the enemy entering the viewport again
        isInitiallyVisible = false;
    }
}

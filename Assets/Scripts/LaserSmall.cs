using UnityEngine;
using System.Collections;

public class LaserSmall : MonoBehaviour
{
    [Header("Layers that laser hits")]
    public LayerMask layersToHit;
    private LineRenderer lineRenderer;
    public Transform playerTransform;
    [Header("Audio")]
    public AudioClip hitSound;
    private AudioSource audioSource;
    private bool soundPlayed = false;
    [Header("Player Size Adjustment")]
    public float newScale = 0.5f; // The new scale you want to apply to the player

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
            if (hit.collider.CompareTag("Player1") && !soundPlayed)
            {
                hit.collider.transform.localScale = new Vector3(newScale, newScale, newScale);
                audioSource.PlayOneShot(hitSound);
                soundPlayed = true;
            }
        }
        else
        {
            lineRenderer.SetPosition(1, transform.position + (Vector3)direction * 100f);
            soundPlayed = false; // Reset soundPlayed flag when not hitting player
        }
    }

}

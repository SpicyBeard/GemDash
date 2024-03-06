using UnityEngine;
using System.Collections;
using AGDDPlatformer;

public class Laser : MonoBehaviour
{
    [Header("Layers that laser hits")]
    public LayerMask layersToHit;
    private LineRenderer lineRenderer;
    [Header("Respawn Location")]
    public Transform playerTransform;
    public Vector2 respawnLocation; 
    [Header("Audio")]
    public AudioClip hitSound;
    private AudioSource audioSource;
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
            soundPlayed = false;
        }
    }

    IEnumerator PlayerHitActions(Collider2D player)
    {
        if(player != null){
            player.GetComponentInParent<PlayerController>().canSlam = false;

        }
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

        yield return new WaitForSeconds(1.5f);

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

using UnityEngine;

public class Laser : MonoBehaviour
{
    public LayerMask layersToHit;
    private LineRenderer lineRenderer;

    void Start()
    {
        // Get the LineRenderer component attached to this GameObject
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        ShootLaser();
    }


void ShootLaser()
{
    // Calculate the direction based on the laser's orientation
    float angle = transform.eulerAngles.z * Mathf.Deg2Rad;
    Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

    // Perform the raycast
    RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 100f, layersToHit);

    // Always set the first position of the line to be the laser's starting point
    lineRenderer.SetPosition(0, transform.position);

    if (hit.collider != null)
    {
        // If the raycast hits something, set the end of the laser to the hit point
        lineRenderer.SetPosition(1, hit.point);

        // Check if the hit object has the tag "Player"
        if (hit.collider.CompareTag("Player1"))
        {
            // Perform your action here
            Debug.Log("Player Hit");
            
            // Example actions:
            // - Reduce player health
            // - Trigger a hit animation on the player
            // - Play a sound effect
        }
    }
    else
    {
        // If nothing is hit, extend the laser to its maximum length in the direction it's facing
        lineRenderer.SetPosition(1, transform.position + (Vector3)direction * 100f); // Adjust the length as needed
    }
}

    }


using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform startPoint;  // Starting point of the movement
    public Transform endPoint;    // End point of the movement
    public float moveSpeed = 5f;  // Speed at which the enemy moves

    private Transform currentTarget; // Current target the enemy is moving towards
    private SpriteRenderer spriteRenderer; // To flip the sprite based on direction

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentTarget = endPoint; // Start by moving towards the end point
    }

    void Update()
    {
        MoveTowardsTarget();
        CheckSwitchTarget();
    }

    void MoveTowardsTarget()
    {
        if (currentTarget != null)
        {
            // Move the enemy towards the current target
            transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, moveSpeed * Time.deltaTime);

            // Flip the sprite based on the direction of movement
            if (transform.position.x < currentTarget.position.x)
            {
                // Moving right
                spriteRenderer.flipX = false;
            }
            else if (transform.position.x > currentTarget.position.x)
            {
                // Moving left
                spriteRenderer.flipX = true;
            }
        }
    }

    void CheckSwitchTarget()
    {
        // Switch the current target if the enemy reaches it
        if (transform.position == currentTarget.position)
        {
            currentTarget = currentTarget == startPoint ? endPoint : startPoint;
        }
    }
}

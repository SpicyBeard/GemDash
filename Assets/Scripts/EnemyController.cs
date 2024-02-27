using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AGDDPlatformer
{
public class EnemyController : KinematicObject
    {
        public float moveSpeed = 1f; // enemy speed
        private Rigidbody2D rb; // get rigidbody
        private bool isFacingRight = true;
        float defaultGravityModifier;

        void Awake()
        {
            defaultGravityModifier = gravityModifier;
        }

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            rb.velocity = new Vector2((isFacingRight ? 1 : -1) * moveSpeed, rb.velocity.y); // move according to direction faced * speed
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Wall")) // flip on wall collide
            {
                Flip();
            }
        }

        void Flip()
        {
            if(isFacingRight)
            {
                isFacingRight = false;
                transform.Rotate(0f, 180f, 0f);
            }
            else if (!isFacingRight)
            {
                isFacingRight = true;
                transform.Rotate(0f, 180f, 0f);
            }
        }
    }
}
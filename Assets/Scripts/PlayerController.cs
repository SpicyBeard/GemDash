using System.Collections;
using UnityEngine;

namespace AGDDPlatformer
{
    public class PlayerController : KinematicObject
    {
        [Header("Movement")]
        public float maxSpeed = 7;
        public float jumpSpeed = 7;
        public float jumpDeceleration = 0.5f; // Upwards slow after releasing jump button
        public float cayoteTime = 0.1f; // Lets player jump just after leaving ground
        public float jumpBufferTime = 0.1f; // Lets the player input a jump just before becoming grounded

        [Header("Dash")]
        public float dashSpeed;
        public float dashTime;
        public float dashCooldown;
        public Color canDashColor;
        public Color cantDashColor;
        float lastDashTime;
        Vector2 dashDirection;
        bool isDashing;
        public bool canDash;
        bool wantsToDash;

        [Header("Slam")]
        Vector2 slamDirection;
        public bool canSlam;
        public bool isSlaming;
        bool wantsToSlam;
        public Color canSlamColor;
        public bool canBounce;
        public float startSlam;


        [Header("Audio")]
        public AudioSource source;
        public AudioClip jumpSound;
        public AudioClip dashSound;
        public AudioClip slamSound;

        Vector2 startPosition;
        bool startOrientation;

        float lastJumpTime;
        float lastGroundedTime;
        bool canJump;
        bool jumpReleased;
        Vector2 move;
        float defaultGravityModifier;

        SpriteRenderer spriteRenderer;

        Vector2 jumpBoost;


        void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            lastJumpTime = -jumpBufferTime * 2;

            startPosition = transform.position;
            startOrientation = spriteRenderer.flipX;

            defaultGravityModifier = gravityModifier;
        }
        
        IEnumerator SlamDelay(){
            gravityModifier = 0;

            Debug.Log("Slam Delay");
            yield return new WaitForSeconds(1.0f);
            
        }

        void Update()
        {
            isFrozen = GameManager.instance.timeStopped;

            /* --- Read Input --- */

            move.x = Input.GetAxisRaw("Horizontal");
            if (gravityModifier < 0)
            {
                move.x *= -1;
            }

            move.y = Input.GetAxisRaw("Vertical");

            if (Input.GetButtonDown("Jump"))
            {
                // Store jump time so that we can buffer the input
                lastJumpTime = Time.time;
            }

            if (Input.GetButtonUp("Jump"))
            {
                jumpReleased = true;
            }

            // Clamp directional input to 8 directions for dash
            Vector2 desiredDashDirection = new Vector2(
                move.x == 0 ? 0 : (move.x > 0 ? 1 : -1),
                move.y == 0 ? 0 : (move.y > 0 ? 1 : -1));
            if (desiredDashDirection == Vector2.zero)
            {
                // Dash in facing direction if there is no directional input;
                desiredDashDirection = spriteRenderer.flipX ? -Vector2.right : Vector2.right;
            }
            desiredDashDirection = desiredDashDirection.normalized;
            if (Input.GetButtonDown("Dash"))
            {
                wantsToDash = true;
            }
            // Clamp directional input to downward for slam
            Vector2 desiredSlamDirection = new Vector2();
            if (desiredSlamDirection == Vector2.zero)
            {
                // Slam downward 
                desiredSlamDirection = Vector2.down;

            }
            desiredSlamDirection = desiredSlamDirection.normalized;
            if (Input.GetButtonDown("Slam"))
            {
                wantsToSlam = true;
            }
            // Debug.Log(wantsToSlam);
            if (canSlam && wantsToSlam)
            {
                isSlaming = true;
                slamDirection = desiredSlamDirection;
                canSlam = false;
                gravityModifier = 0;
                source.PlayOneShot(dashSound);
            }

            wantsToSlam = false;
            GetComponent<TrailRenderer>().enabled = false;
            if(isSlaming)
            {   
                startSlam = Time.time;
                isFrozen = true;
                StartCoroutine(SlamDelay());
                isFrozen = false;
                GetComponent<TrailRenderer>().enabled = true;
                velocity = slamDirection * dashSpeed;
                canBounce = true;

                    gravityModifier = defaultGravityModifier;
                    if ((gravityModifier >= 0 && velocity.y > 0) ||
                        (gravityModifier < 0 && velocity.y < 0))
                    {
                        velocity.y /= 4* jumpDeceleration;
                    }
                spriteRenderer.color = canSlamColor;
                Debug.Log("Slamming");
                //if player is grounded, reset slam
                isSlaming = false;
                                    
                
                //make player slam down at a faster speed
            }

            /* --- Compute Velocity --- */

            if (canDash && wantsToDash)
            {
                isDashing = true;
                dashDirection = desiredDashDirection;
                lastDashTime = Time.time;
                canDash = false;
                gravityModifier = 0;

                source.PlayOneShot(dashSound);
            }
            wantsToDash = false;

            if (isDashing)
            {
                velocity = dashDirection * dashSpeed;
                if (Time.time - lastDashTime >= dashTime)
                {
                    isDashing = false;

                    gravityModifier = defaultGravityModifier;
                    if ((gravityModifier >= 0 && velocity.y > 0) ||
                        (gravityModifier < 0 && velocity.y < 0))
                    {
                        velocity.y *= jumpDeceleration;
                    }
                }
            }
            else
            {
                if (isGrounded)
                {   
                    canBounce = false;

                    // Store grounded time to allow for late jumps
                    lastGroundedTime = Time.time;
                    canJump = true;
                    if (!isDashing && Time.time - lastDashTime >= dashCooldown)
                        canDash = true;
                }

                // Check time for buffered jumps and late jumps
                float timeSinceJumpInput = Time.time - lastJumpTime;
                float timeSinceLastGrounded = Time.time - lastGroundedTime;

                if (canJump && timeSinceJumpInput <= jumpBufferTime && timeSinceLastGrounded <= cayoteTime)
                {
                    velocity.y = Mathf.Sign(gravityModifier) * jumpSpeed;
                    canJump = false;
                    isGrounded = false;

                    source.PlayOneShot(jumpSound);
                }
                else if (jumpReleased)
                {
                    // Decelerate upwards velocity when jump button is released
                    if ((gravityModifier >= 0 && velocity.y > 0) ||
                        (gravityModifier < 0 && velocity.y < 0))
                    {
                        velocity.y *= jumpDeceleration;
                    }
                    jumpReleased = false;
                }

                velocity.x = move.x * maxSpeed;

                if (isGrounded || (velocity + jumpBoost).magnitude < velocity.magnitude)
                {
                    jumpBoost = Vector2.zero;
                }
                else
                {
                    velocity += jumpBoost;
                    jumpBoost -= jumpBoost * Mathf.Min(1f, Time.deltaTime);
                }
            }

            /* --- Adjust Sprite --- */

            // Assume the sprite is facing right, flip it if moving left
            if (move.x > 0.01f)
            {
                spriteRenderer.flipX = false;
            }
            else if (move.x < -0.01f)
            {
                spriteRenderer.flipX = true;
            }
            if(canSlam){
                Debug.Log("I can slam color");
                // spriteRenderer.color = canSlamColor;
                spriteRenderer.color = canSlam ? canSlamColor : cantDashColor;
            }
            else{
                spriteRenderer.color = canDash ? canDashColor : cantDashColor;

            }
        }

        public void ResetPlayer()
        {
            transform.position = startPosition;
            spriteRenderer.flipX = startOrientation;

            lastJumpTime = -jumpBufferTime * 2;

            velocity = Vector2.zero;
        }

        public void ResetDash()
        {   
            canSlam = false;
            canBounce = false;
            canDash = true;
        }

        public void ResetSlam()
        {   
            canDash = false;
            canSlam = true;

        }

        //Add a short mid-air boost to the player (unrelated to dash). Will be reset upon landing.
        public void SetJumpBoost(Vector2 jumpBoost)
        {
            this.jumpBoost = jumpBoost;
        }
    }

    
}

using System.Collections;
using System.Collections.Generic;
using AGDDPlatformer;
using UnityEngine;
using UnityEngine.UI;

public class Bouncy : MonoBehaviour
{   
        public float cooldown = 1;
        private AudioSource audioSource;
        public AudioClip bounceSound;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {

        }
           void OnTriggerEnter2D(Collider2D other)
        {
            
         PlayerController playerController = other.GetComponentInParent<PlayerController>();
            if (playerController != null)
            {   
                Debug.Log("Bouncy");
                if(playerController.canBounce)
                {
                Debug.Log("Bouncing!");
                playerController.canDash = false;
                Vector2 desiredBounceDirection = new Vector2();
                //bounce upward
                desiredBounceDirection = Vector2.up;
                playerController.velocity = ((desiredBounceDirection * playerController.dashSpeed)/2); 
                playerController.velocity.y += playerController.jumpSpeed;
                playerController.velocity.y -= Time.deltaTime;
                playerController.canBounce= false;
                audioSource.PlayOneShot(bounceSound);
                }
            }
            }
        }


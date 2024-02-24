using System.Collections;
using System.Collections.Generic;
using AGDDPlatformer;
using UnityEngine;

public class Bouncy : MonoBehaviour
{   
        public float cooldown = 1;
        public AudioSource source;


    // Start is called before the first frame update
    void Start()
    {
        //get the player controller
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
                playerController.ResetSlam();
                playerController.ResetDash();
                // playerController.
                }
                
            //if player is slaming and collides with the bouncy object
            if (playerController!= null && playerController.isSlaming )
            {
                //bounce the player
                if(this.gameObject.GetComponent<BoxCollider2D>().IsTouching(playerController.GetComponent<BoxCollider2D>()))
                {
                playerController.velocity.y += playerController.jumpSpeed * 3;
                source.Play();
                }
            }
        }

}

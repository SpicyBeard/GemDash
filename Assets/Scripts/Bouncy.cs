using System.Collections;
using System.Collections.Generic;
using AGDDPlatformer;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UI;

public class Bouncy : MonoBehaviour
{   
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

        void OnTriggerStay2D(Collider2D other)
        {   
            PlayerController playerController = other.GetComponentInParent<PlayerController>();
            if (playerController != null)
            {
                if (playerController.canBounce)
                {
                    playerController.canDash = false;
                    StartCoroutine(DisableBounce(playerController, 1f)); // Disable bouncing after a delay
                    audioSource.PlayOneShot(bounceSound);
                        }
            }
        }
        IEnumerator DisableBounce(PlayerController playerController, float delay)
        {
            playerController.Bounce(10); // Call the Bounce method
            yield return new WaitForSeconds(delay);
            playerController.canBounce = false;
        }
           void OnTriggerEnter2D(Collider2D other)
        {
            
         PlayerController playerController = other.GetComponentInParent<PlayerController>();
            if (playerController != null)
            {   
                Debug.Log("Bouncy");
                if(playerController.canBounce )
                {
                playerController.canDash = false;
                playerController.Bounce(playerController.dashSpeed ); // Call the Bounce method
                audioSource.PlayOneShot(bounceSound);
                }
            }
            }
        }


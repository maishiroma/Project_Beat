using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers {

    public class PlayerController : MonoBehaviour {

        [Header("State Variables")]
        public bool isGrounded = false;
        public bool isJumping = false;

        [Header("Physics References")]
        [Tooltip("Character Controller component of player")]
        public CharacterController playerPhysics;
        [Tooltip("The GameObject that the ground checker will refer to")]
        public Transform groundCheckPos;
        [Tooltip("The GameObject that the wall checker will refer to")]
        public Transform sideWallCheckPos;
        [Tooltip("The GameObject that the ceiling checker will refer to")]
        public Transform ceilingCheckPos;
        [Tooltip("The LayerMask that the controller will deem as solid ground")]
        public LayerMask groundLayer;
        [Tooltip("The LayerMask that the controller will deem as solid wall")]
        public LayerMask wallLayer;

        [Header("Physics Variables")]
        [Tooltip("The move speed the character has when moving side to side")]
        [Range(1f, 25f)]
        public float horizSpeed = 10f;
        [Tooltip("The amount of force gravity has on the player")]
        [Range(1f, 25f)]
        public float gravityForce = 10f;
        [Tooltip("The amount of speed the player's jump movement has")]
        [Range(1f, 75f)]
        public float jumpForce = 10f;
        [Tooltip("The duration of the player's jump")]
        [Range(1f, 25f)]
        public float jumpTime = 1f;
        [Tooltip("How large the player's ground checker is")]
        [Range(0.1f, 3f)]
        public float groundCheckRadius = 1.85f;
        [Tooltip("How large the player's wall checker is")]
        [Range(0.1f, 3f)]
        public float sideWallCheckRadius = 1.85f;
        [Tooltip("How large the player's ceiling checker is")]
        [Range(0.1f, 3f)]
        public float ceilingCheckRadius = 1.85f;
        [Tooltip("The max gravity speed the player will acheive")]
        [Range(0.1f, 1f)]
        public float maxGravityForce = 0.5f;
       
        // Private variables
        private Collider2D[] collisionBoxes = new Collider2D[3];
        private float horizInput;
        private float jumpInput;
        private float currentGravityForce;

        // Gets the player inputs
		private void Update()
		{
            horizInput = Input.GetAxis("Horizontal") * horizSpeed * Time.deltaTime;
            jumpInput = Input.GetAxisRaw("Jump");
		}

        // Handles player movement physics
		private void FixedUpdate()
		{
            // Handles the collision for being grounded
            GroundGravityLogic();

            // Invokes the jump movement if we meet the specific requirements
            if(isGrounded && jumpInput > 0f && !isJumping)
            {
                StartCoroutine(JumpLogic());
            }

            // Handles the player movement 
            MovementLogic();
		}

        // Shows the grounded and wall collision boxes
		private void OnDrawGizmos()
		{
            if(groundCheckPos != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(groundCheckPos.position, groundCheckRadius);
            }

            if(sideWallCheckPos != null)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(sideWallCheckPos.position, sideWallCheckRadius);
            }

            if(ceilingCheckPos != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(ceilingCheckPos.position, ceilingCheckRadius);
            }
		}

        // Determines if the player is grounded and labels it as such.
        // Also handles logic for dealing with gravity
        private void GroundGravityLogic()
        {
            int collisionCount = Physics2D.OverlapCircleNonAlloc(groundCheckPos.position, groundCheckRadius, collisionBoxes, groundLayer);
            if(collisionCount > 0)
            {
                // The player is on solid ground and will not fall down
                isGrounded = true;
                currentGravityForce = 0; 
            }
            else
            {
                // If you are airborn, the player is considered airborn
                isGrounded = false;
            }

            // Logic for gravity force acceleration
            if(!isGrounded)
            {
                if(Mathf.Abs(currentGravityForce) > maxGravityForce)
                {
                    currentGravityForce = -maxGravityForce;
                }
                else
                {
                    currentGravityForce += -gravityForce * Time.deltaTime;
                }
            }
        }

        // Handles the player movement, which should only be called in the Fixed Update
        private void MovementLogic()
        {
            // Saves the previous X coord for later
            float prevXPos = playerPhysics.transform.position.x;
            float prevYPos = playerPhysics.transform.position.y;

            // Invokes the move function according to other calculated parameters
            if(isJumping)
            {
                playerPhysics.Move(new Vector3(0,jumpForce * Time.deltaTime));
            }
            playerPhysics.Move(new Vector3(horizInput,currentGravityForce));

            // If we collide into a wall, we move the player back to the last spot where it wasn't colliding with a wall
            int colliderCount = Physics2D.OverlapCircleNonAlloc(sideWallCheckPos.transform.position, sideWallCheckRadius, collisionBoxes, wallLayer);
            if(colliderCount > 0)
            {
                playerPhysics.transform.position = new Vector3(prevXPos, playerPhysics.transform.position.y);
            }

            // If we collide into a wall on top of the player, we make the player bounce off of the ceiling. 
            colliderCount = Physics2D.OverlapCircleNonAlloc(ceilingCheckPos.position, ceilingCheckRadius, collisionBoxes, wallLayer);
            if(colliderCount > 0)
            {
                if(isJumping)
                {
                    StopCoroutine(JumpLogic());
                    isJumping = false;
                }
                playerPhysics.transform.position = new Vector3(playerPhysics.transform.position.x, prevYPos);
            }
        }

        // Handles the jump movement of the player
        private IEnumerator JumpLogic()
        {
            isGrounded = false;
            isJumping = true;

            yield return new WaitForSeconds(jumpTime);

            isJumping = false;
            yield return null;
        }
	}
}
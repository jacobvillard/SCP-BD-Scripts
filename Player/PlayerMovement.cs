using Interfaces;
using UnityEngine;

// This namespace is for player-related classes
namespace Player {
    /// <summary>
    /// Player movement handles the player's movement, including walking, sprinting, crouching, and jumping.
    /// </summary>
    public class PlayerMovement : MonoBehaviour {
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 4f;                              //The speed of the player when walking
        [SerializeField] private float sprintSpeed = 7f;                            //The speed of the player when sprinting
        [SerializeField] private float crouchSpeed = 2f;                            //The speed of the player when crouching
        [SerializeField] private float jumpForce = 1f;                              //The force of the player's jump
        [SerializeField] private float gravity = -9.81f;                            //The gravity applied to the player
        [SerializeField] private float slideBoost = 10f;                            //The speed boost applied to the player when sliding
        [SerializeField] private float standingHeight = 2f;                         //The height of the player when standing
        [SerializeField] private float crouchingHeight = 1f;                        //The height of the player when crouching
        [SerializeField] private Vector3 standingCenter = new Vector3(0, 0f, 0);    //The center of the player when standing
        [SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0); //The center of the player when crouching
        [SerializeField] private float slideDuration = 1f;                          // The duration of the slide
        
        [Header("State")]
        [SerializeField] private PlayerState currentState = PlayerState.Idle;       // The current state of the player
        
        /// <summary>
        /// The current state of the player. 
        /// </summary>
        public PlayerState CurrentState => currentState;
        
        private CharacterController _controller;                                    // The character controller component
        private Vector3 _velocity;                                                  // The velocity of the player
        private Vector3 slideDirection;                                             // The direction of the slide
        private bool _isGrounded;                                                   // Flag to check if the player is on the ground
        private bool slideJustStarted = false;                                      // Flag to track if the slide just started
        private bool isSlidingActive = false;                                       // Flag to track if the player is sliding
        private bool isCrouchingActive = false;                                     // Flag to track if the player is crouching
        private float slideTimer = 0f;                                              // Timer for the slide duration
        
        /// <summary>
        /// On awake, gets the character controller component.
        /// </summary>
        private void Awake() {
            _controller = GetComponent<CharacterController>();
        }

        /// <summary>
        /// Handles player movement
        /// </summary>
        /// <param name="input">The player's input</param>
        /// <param name="playerHUD">The player's HUD</param>
        public void HandleInput(PlayerInput input, PlayerHUD playerHUD) {
            //Check if the player is on the ground
            GroundCheck();

            //Handle movement
            var move = HandleMovement(input);

            //Handle rotation
            HandleRotation(input);
            
            // Handle jumping
            HandleJump(input);
            
            // Handle crouching
            HandleCrouch(input, playerHUD);
            
            // Handle sliding
            HandleSlide(input);
            
            // Update the player's state based on input and movement
            UpdateState(input, move);
        }

        /// <summary>
        /// Handles the player's movement based on input.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Returns the move vector</returns>
        private Vector3 HandleMovement(PlayerInput input) {
            //Get the input direction
            var move = transform.right * input.moveInput.x + transform.forward * input.moveInput.y;
            
            //Get the speed based on input and movement
            var speed = GetSpeed(input);
            
            //Apply the movement to the character controller
            _controller.Move(move * speed * Time.deltaTime);
            
            //Return the move vector
            return move;
        }

        /// <summary>
        /// Rotates the player based on horizontal mouse movement (yaw).
        /// </summary>
        /// <param name="input">The player input</param>
        private void HandleRotation(PlayerInput input) {
            //Yaw rotation based on mouse input
            var yaw = input.lookInput.x * input.TurnSensitivity;
            
            //Apply the rotation to the player
            transform.Rotate(Vector3.up * yaw);
        }
        
        
        /// <summary>
        /// Handles the player's jump.
        /// </summary>
        /// <param name="input">The player input</param>
        private void HandleJump(PlayerInput input) {
            // If the player is on the ground and the jump button is pressed, jump
            if (input.jumpState == InputState.Pressed  && _isGrounded && !isSlidingActive && !isCrouchingActive) {
                _velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
                currentState = PlayerState.Jumping;
            }
            
            // Apply gravity
            _velocity.y += gravity * Time.deltaTime;
            
            //Move the controller
            _controller.Move(_velocity * Time.deltaTime);
        }

        /// <summary>
        /// Handles crouching and sliding based on player input.
        /// </summary>
        /// <param name="input">The player input</param>
        /// <param name="playerHUD">The player HUD</param>
        private void HandleCrouch(PlayerInput input, PlayerHUD playerHUD) {
            // Return if the player is not on the ground
            if(!_isGrounded)return;
            
            // If the player is crouches and not sliding, toggle crouch state
            if (input.crouchState == InputState.Pressed && !isSlidingActive && !input.isSprinting) {
                // Toggle crouch state
                isCrouchingActive = !isCrouchingActive;
                playerHUD.SetCrouchVignette(isCrouchingActive);
                
                // Apply height based on crouch state
                ApplyHeight(isCrouchingActive);
            }
            
        }

       

        /// <summary>
        /// Handles sliding based on player input.
        /// </summary>
        /// <param name="input">The player input</param>
        private void HandleSlide(PlayerInput input) {
            // Return if the player is not on the ground
            if(!_isGrounded)return;
            
            // If the player is sprinting and crouches, start sliding
            if (input.isSprinting && input.crouchState == InputState.Pressed  && !isSlidingActive && !isCrouchingActive) {
                StartSlide(input);
            }
            
            // If the player releases crouch and is sliding, stop sliding
            if (input.crouchState == InputState.Released  && isSlidingActive) {
                StopSlide();
            }
            
            // If the player is sliding, apply sliding movement
            if (isSlidingActive) {
                SlidingMovement();
                
                // Update the slide timer
                slideTimer += Time.deltaTime;
                
                // If the slide timer exceeds the slide duration, stop sliding
                if (slideTimer >= slideDuration) {
                    StopSlide();
                }
            }
            
        }
        
        /// <summary>
        /// Starts the slide by applying height and setting the sliding state.
        /// </summary>
        /// <param name="input">The player input</param>
        private void StartSlide(PlayerInput input) {
            // Apply height for sliding
            ApplyHeight(true);
            
            // Set the sliding state
            isSlidingActive = true;
            
            // Reset the slide timer
            slideTimer = 0f;
            
            // Calculate the initial movement direction at slide start
            slideDirection = (transform.forward * input.moveInput.y + transform.right * input.moveInput.x).normalized;
        }

        /// <summary>
        /// Slides the player in the direction of movement.
        /// </summary>
        private void SlidingMovement() {
            // If the player is sliding, apply sliding movement
            if (slideDirection.sqrMagnitude > 0.1f) {
               
                // Calculate the movement direction based on slide direction and speed
                var movement = slideDirection * slideBoost * Time.deltaTime;
                
                // Add small downward force to stay grounded
                movement.y += gravity * Time.deltaTime; 
                
                // Move the character controller
                _controller.Move(movement);
            }
        }
        
        /// <summary>
        /// Stops the slide by applying height and resetting the sliding state.
        /// </summary>
        private void StopSlide() {
            // Apply height for standing
            ApplyHeight(false);
            
            // Reset the sliding state
            isSlidingActive = false;
            
            // Reset the slide timer
            slideTimer = 0f;
            
        }

        /// <summary>
        /// Applies the height and center of the character controller based on whether the player is crouching or standing.
        /// </summary>
        /// <param name="isShort">If the player is short</param>
        private void ApplyHeight(bool isShort) {
            // Set the controller height based on crouch state
            _controller.height = isShort ? crouchingHeight : standingHeight;
            
            // Set the controller center based on crouch state
            _controller.center = isShort ? crouchingCenter : standingCenter;
        }
        
        /// <summary>
        /// Sets the player's speed based on input and movement.
        /// </summary>
        /// <param name="input">The player input</param>
        /// <returns>The player speed</returns>
        private float GetSpeed(PlayerInput input) {
            
            // If the player is sliding, return sprint speed + slide boost
            if (isSlidingActive) return sprintSpeed + slideBoost; 
            
            // If the player is crouching, return crouch speed
            if (isCrouchingActive) return crouchSpeed;
            
            // If the player is sprinting, return sprint speed
            if (input.isSprinting) return sprintSpeed;
            
            // Else the player is walking, return walk speed
            return walkSpeed;
        }
        /// <summary>
        /// Updates the player's state based on input and movement.
        /// </summary>
        /// <param name="input">The player's input</param>
        /// <param name="move">The move vector of player</param>
        private void UpdateState(PlayerInput input, Vector3 move) {
            
            // If the player is not on the ground, set the state to jumping
            if (!_isGrounded)
                currentState = PlayerState.Jumping;
            // If the player is sliding, set the state to sliding
            else if (isSlidingActive)
                currentState = PlayerState.Sliding;
            // If the player is crouching, set the state to crouching
            else if (isCrouchingActive)
                currentState = PlayerState.Crouching;
            // If the player is moving, set the state to walking or sprinting
            else if (move.magnitude > 0.1f)
                currentState = input.isSprinting ? PlayerState.Sprinting : PlayerState.Walking;
            // Else the player is not moving, set the state to idle
            else
                currentState = PlayerState.Idle;
        }

        /// <summary>
        /// Checks if the player is on the ground.
        /// </summary>
        private void GroundCheck() {
            // Set the grounded state based on the character controller
            _isGrounded = _controller.isGrounded;
        }

     
    }
}

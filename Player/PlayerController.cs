using UnityEngine;

// This namespace is for player-related classes
namespace Player {
    
    /// <summary>
    /// This class is responsible for controlling the player character.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerCamera))]
    [RequireComponent(typeof(PlayerAnimator))]
    [RequireComponent(typeof(PlayerHUD))]
    public class PlayerController : MonoBehaviour {
        
        [Header("Player Components")]
        [SerializeField]private PlayerMovement movement;    // The PlayerMovement component
        [SerializeField]private PlayerInput input;          // The PlayerInput component
        [SerializeField]private PlayerCamera cam;           // The PlayerCamera component
        [SerializeField]private PlayerAnimator animator;    // The PlayerAnimator component
        [SerializeField]private PlayerHUD playerHUD;        // The PlayerHUD component

        /// <summary>
        /// On Awake, we get references to the PlayerMovement, PlayerInput, PlayerCamera, and PlayerAnimator components.
        /// </summary>
        private void Awake() {
            movement = GetComponent<PlayerMovement>();
            input = GetComponent<PlayerInput>();
            cam = GetComponent<PlayerCamera>();
            animator = GetComponent<PlayerAnimator>();
            playerHUD = GetComponent<PlayerHUD>();
            
            // Set the camera recoil interface
            animator.CameraRecoil = cam;
        }

        /// <summary>
        /// Here we call the Update methods of the input, movement, camera, and animator components.
        /// </summary>
        private void Update() {
            input.ProcessInput();
            movement.HandleInput(input, playerHUD);
            cam.HandleInput(input);
            animator.UpdateAnimation(input, movement);
        }
    }
}

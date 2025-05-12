using UnityEngine;
using UnityEngine.InputSystem;

// This namespace is for player-related classes
namespace Player {
    
    /// <summary>
    /// The InputState enum represents the different states of input.
    /// </summary>
    public enum InputState {
        None,    // No input
        Pressed, // Input was pressed
        Held,    // Input is being held
        Released // Input was released
    }

    /// <summary>
    /// This class handles player input for various actions such as movement, aiming, and firing.
    /// </summary>
    public class PlayerInput : MonoBehaviour {
        
        /// <summary>
        /// Fire state for the player.
        /// </summary>
        [Header("Input States")]
        public InputState fireState;
        
        /// <summary>
        /// Aim state for the player.
        /// </summary>
        public InputState aimState;
        
        /// <summary>
        /// Reload state for the player.
        /// </summary>
        public InputState reloadState;
        
        /// <summary>
        /// Inspect state for the player.
        /// </summary>
        public InputState inspectState;
        
        /// <summary>
        /// Crouch state for the player.
        /// </summary>
        public InputState crouchState;
        
        /// <summary>
        /// Toggle fire mode state for the player.
        /// </summary>
        public InputState toggleFireModeState;
        
        /// <summary>
        /// Jump state for the player.
        /// </summary>
        public InputState jumpState;

        /// <summary>
        /// Movement input for the player.
        /// </summary>
        public Vector2 moveInput;
        
        /// <summary>
        /// Look input for the player.
        /// </summary>
        public Vector2 lookInput;
        
        /// <summary>
        /// Flag to indicate if the player is sprinting.
        /// </summary>
        public bool isSprinting;
        
        [Header("Sensitivity")]
        [SerializeField] private float turnSensitivity = 0.1f;                          // Sensitivity for turning the player
        
        /// <summary>
        /// The sensitivity for turning the player.
        /// </summary>
        public float TurnSensitivity => turnSensitivity;

        private PlayerInputAction inputActions;                                         // Reference to the input actions
        private bool fireHeld, aimHeld, reloadHeld, inspectHeld, crouchHeld, jumpHeld, toggleFireModeHeld;  // Flags for held button presses

        /// <summary>
        /// On Awake, we set up the input actions and their corresponding events.
        /// </summary>
        private void Awake() {
            // Initialize input actions
            inputActions = new PlayerInputAction();

            // Movement input setup
            inputActions.PlayerInputActions.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
            inputActions.PlayerInputActions.Move.canceled += ctx => moveInput = Vector2.zero;

            // Look input setup
            inputActions.PlayerInputActions.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
            inputActions.PlayerInputActions.Look.canceled += ctx => lookInput = Vector2.zero;

            // Sprint (bool only)
            inputActions.PlayerInputActions.Sprint.performed += ctx => isSprinting = true;
            inputActions.PlayerInputActions.Sprint.canceled += ctx => isSprinting = false;

            // Button press detection setup
            SetupButton(inputActions.PlayerInputActions.Fire, val => fireHeld = val);
            SetupButton(inputActions.PlayerInputActions.Aim, val => aimHeld = val);
            SetupButton(inputActions.PlayerInputActions.Reload, val => reloadHeld = val);
            SetupButton(inputActions.PlayerInputActions.Inspect, val => inspectHeld = val);
            SetupButton(inputActions.PlayerInputActions.Crouch, val => crouchHeld = val);
            SetupButton(inputActions.PlayerInputActions.Jump, val => jumpHeld = val);
            SetupButton(inputActions.PlayerInputActions.ToggleFireMode, val => toggleFireModeHeld = val);
        }

        /// <summary>
        /// Sets up the input action for a button press.
        /// </summary>
        /// <param name="action">The action to set up</param>
        /// <param name="setHeldFlag">The flag to set</param>
        private void SetupButton(InputAction action, System.Action<bool> setHeldFlag) {
            action.performed += ctx => setHeldFlag(true);
            action.canceled += ctx => setHeldFlag(false);
        }

        /// <summary>
        /// Updates the input state based on whether the button is held or not.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="heldFlag"></param>
        private void UpdateInputState(ref InputState state, bool heldFlag) {
            switch (state) {
                case InputState.None:
                    if (heldFlag) state = InputState.Pressed;
                    break;
                case InputState.Pressed:
                    state = heldFlag ? InputState.Held : InputState.Released;
                    break;
                case InputState.Held:
                    if (!heldFlag) state = InputState.Released;
                    break;
                case InputState.Released:
                    state = InputState.None;
                    break;
            }
        }

        /// <summary>
        /// Main function to process input states.
        /// </summary>
        public void ProcessInput() {
            UpdateInputState(ref fireState, fireHeld);
            UpdateInputState(ref aimState, aimHeld);
            UpdateInputState(ref reloadState, reloadHeld);
            UpdateInputState(ref inspectState, inspectHeld);
            UpdateInputState(ref crouchState, crouchHeld);
            UpdateInputState(ref jumpState, jumpHeld);
            UpdateInputState(ref toggleFireModeState, toggleFireModeHeld);
        }

        /// <summary>
        /// OnEnable, we enable the input actions.
        /// </summary>
        private void OnEnable() => inputActions.Enable();
        
        /// <summary>
        /// OnDisable, we disable the input actions.
        /// </summary>
        private void OnDisable() => inputActions.Disable();
    }
}

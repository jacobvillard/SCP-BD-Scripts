using Interfaces;
using Unity.Cinemachine;
using UnityEngine;

// This namespace is for player-related classes
namespace Player {
    /// <summary>
    /// This class is responsible for handling the player's camera view.
    /// </summary>
    public class PlayerCamera : MonoBehaviour, IHandleInput, ICameraRecoil {
        [Header("Cinemachine")]
        [SerializeField] private Transform pitchTarget;                 // The PitchHolder
        [SerializeField] private CinemachineCamera cinemachineCamera;   // The CinemachineCamera
        [SerializeField] private CinemachineImpulseSource impulseSource;// The CinemachineImpulseSource
        
        [Header("Settings")]
        [SerializeField] private float minPitch = -90f;                 // The minimum pitch angle
        [SerializeField] private float maxPitch = 90f;                  // The maximum pitch angle

        [Header("FOV Settings")]
        [SerializeField] private float baseFOV = 90f;                   // The base field of view             
        [SerializeField] private float sprintFOV = 100f;                // The field of view when sprinting
        [SerializeField] private float fovLerpSpeed = 10f;              // The speed at which the field of view changes
        
        [Header("Camera Shake")]
        [SerializeField] private bool swayEnabled = true;               // Whether to enable sway
        [SerializeField] private bool shakeEnabled = true;              // Whether to enable shake
        [SerializeField] private float swayAmt = 0.25f;                 // The amount of sway
        [SerializeField] private float swayFreq = 1.5f;                 // The frequency of sway
        [SerializeField] private float recoilRecoverySpeed = 10f;       // The speed at which the camera recovers from recoil
        
        private float _currentPitch = 0f;                               // The current pitch angle
        private float _swayPitch;                                       // The current sway pitch angle
        private float _swayYaw;                                         // The current sway yaw angle
        

        /// <summary>
        /// On start, we get references to the CinemachineCamera and PitchHolder if they are not set in the inspector.
        /// </summary>
        private void Start() {
            // If the CinemachineCamera and PitchHolder are not set in the inspector, we get them from the children
            if (cinemachineCamera == null)
                cinemachineCamera = GetComponentInChildren<CinemachineCamera>();
            if (pitchTarget == null)
                pitchTarget = cinemachineCamera.transform;
            
        }

        /// <summary>
        /// Updates the camera view based on player input and speed.
        /// </summary>
        /// <param name="input">The input component</param>
        public void HandleInput(PlayerInput input) {
            // Mouse input
            var lookDelta = input.lookInput * input.TurnSensitivity;
            
            // Set the current pitch based on the mouse input
            _currentPitch -= Mathf.Clamp(lookDelta.y, -90f, 90f); ;
            
            // Compute final camera rotation
            var finalPitch = Mathf.Clamp(_currentPitch, minPitch, maxPitch);

            // Apply sway (if enabled)
            if (swayEnabled) {
                _swayPitch = Mathf.Cos(Time.time * swayFreq * 0.75f) * swayAmt;
                _swayYaw = Mathf.Sin(Time.time * swayFreq) * swayAmt;
            } else {
                _swayPitch = 0f;
                _swayYaw = 0f;
            }

            // Handle camera rotation
            pitchTarget.localRotation = Quaternion.Euler(finalPitch + _swayPitch, _swayYaw, 0f);
            
            //Handle FOV boost if sprinting
            var targetFOV = input.isSprinting ? sprintFOV : baseFOV;

            // Smoothly interpolate the FOV
            if (cinemachineCamera != null)
                cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(cinemachineCamera.Lens.FieldOfView, targetFOV, Time.deltaTime * fovLerpSpeed);
            
            
            
        }
        
        /// <summary>
        /// Impulses the camera to create a shake effect.
        /// </summary>
        public void TriggerShake(float recoilAmt) {
            impulseSource.GenerateImpulseWithVelocity(new Vector3(0, recoilAmt, 0));
        }

    }
}

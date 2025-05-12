using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Player {
    /// <summary>
    /// This class represents the player's HUD (Heads-Up Display).
    /// </summary>
    public class PlayerHUD : MonoBehaviour {
        [Header("Crouch Overlay")]
        [SerializeField] private Image crouchVignette;
        [SerializeField] private float crouchedAlpha = 0.5f;
        [SerializeField] private float uncrouchedAlpha = 0f;
        
        [Header("Ammo Display")]
        [SerializeField] private TextMeshProUGUI ammoText;
        [SerializeField] private TextMeshProUGUI ammoReserveText;
        [SerializeField] private TextMeshProUGUI ammoTypeText;
        [SerializeField] private TextMeshProUGUI fireModeText;
        [SerializeField] private GameObject ammoParent;
        [SerializeField] private Color normalColor;
        [SerializeField] private Color lowAmmoColour;

        /// <summary>
        /// Sets the crouch vignette overlay.
        /// </summary>
        /// <param name="active"></param>
        public void SetCrouchVignette(bool active) {
#if UNITY_EDITOR
            if (crouchVignette == null) {
                Debug.LogWarning("Crouch vignette Image is not assigned.");
                return;
            }
#endif
            var color = crouchVignette.color;
            color.a = active ? crouchedAlpha : uncrouchedAlpha;
            crouchVignette.color = color;
        }

        /// <summary>
        /// Updates the ammo display with the current and reserve ammo count, ammo type, and fire mode.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="reserve"></param>
        /// <param name="ammoCalibre"></param>
        /// <param name="ammoType"></param>
        /// <param name="fireMode"></param>
        private void UpdateAmmo(int current, int reserve, string ammoCalibre, string ammoType, string fireMode, int lowAmmoThreshold) {
            ammoText.text = $"{current}";
            ammoReserveText.text = $"{reserve}";
            ammoTypeText.text = $"{ammoCalibre.ToUpper()} {ammoType.ToUpper()}" ;
            fireModeText.text = fireMode.ToUpper();

            if (current < lowAmmoThreshold) {
                ammoText.color = lowAmmoColour;
                ammoReserveText.color = lowAmmoColour;
            }
            else {
                ammoText.color = normalColor;
                ammoReserveText.color = normalColor;
            }
            
        }

     
        /// <summary>
        /// Enables or disables the ammo text.
        /// </summary>
        /// <param name="visible"></param>
        public void SetAmmoVisible(bool visible) {
            ammoParent.SetActive(visible);
        }
    
        private void OnEnable() => GlobalEvents.OnAmmoChanged += UpdateAmmo;
        private void OnDisable() => GlobalEvents.OnAmmoChanged -= UpdateAmmo;

    
    }
}

using Player;
using Unity.Cinemachine;
using UnityEngine;

namespace Interfaces {
    
    /// <summary>
    /// Handles the input for the player.
    /// </summary>
    public interface IHandleInput {
        void HandleInput(PlayerInput input);
    }
    
    /// <summary>
    /// Handles the input for the player, returning a boolean value.
    /// </summary>
    public interface IAnimHandleInput {
        bool HandleInput(PlayerInput input);
    }
    
    
    /// <summary>
    /// Handles the camera shake effect.
    /// </summary>
    public interface ICameraRecoil {
        void TriggerShake(float recoilAmt);
    }

}

using System;
using Interfaces;
using Items;
using Player;
using UnityEngine;

/// <summary>
/// This class represents an item in the FPS game.
/// </summary>
public class FPSItem: MonoBehaviour, IAnimHandleInput {

    #region Cached Properties

    protected static readonly int Fire = Animator.StringToHash("Fire");
    protected static readonly int Inspect = Animator.StringToHash("Inspect");
    private static readonly int Disable = Animator.StringToHash("Disable");
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Walk = Animator.StringToHash("Walk");
    private static readonly int Run = Animator.StringToHash("Run");
    private static readonly int Enable = Animator.StringToHash("Enable");

    #endregion

    [Header("Item Settings")]
    [SerializeField] private string _itemName;                  // The name of the item
    [SerializeField] private Animator _animator;                // The animator component
    [SerializeField] private Item _itemType;                    // The type of item
    [SerializeField] private float _equipTime;                  // The time it takes to equip the item
    [SerializeField] protected float _FireCooldownTime = 0.1f;  // The cooldown time for firing
    [SerializeField] protected bool inspectable;                // Flag to check if the item is inspectable
    
    /// <summary>
    /// The name of the item.
    /// </summary>
    public string ItemName => _itemName;
    
    /// <summary>
    /// The type of item.
    /// </summary>
    public Item ItemType => _itemType;
    
    /// <summary>
    /// The animator component.
    /// </summary>
    public Animator Animator => _animator;
    
    /// <summary>
    /// Equips the item.
    /// </summary>
    public float EquipTime => _equipTime;

    protected float _CurFireCooldownTime; // The current fire cooldown time
    protected ICameraRecoil cameraEffects; // The camera effects interface
    
    /// <summary>
    /// On Awake, we get the animator component if it is not set in the inspector.
    /// </summary>
    private void Awake() {
        if(_animator == null) {
            _animator = GetComponent<Animator>();
        }
    }
    

    public void Initialize(ICameraRecoil camEffects) {
        cameraEffects = camEffects;
    }
    

    /// <summary>
    /// OnEnable, we set the current fire cooldown time to 0.
    /// </summary>
    protected void OnEnable() {
        _CurFireCooldownTime = 0f;
        
        _animator.SetTrigger(Enable);
        _animator.SetBool(Idle, false);
        _animator.SetBool(Walk, false);
        _animator.SetBool(Run, false);
    }

    /// <summary>
    /// Handles the input for the item.
    /// </summary>
    /// <param name="input"></param>
    public virtual bool HandleInput(PlayerInput input) {
        var returnValue = false;
        if (input.fireState == InputState.Pressed) {
            if (_CurFireCooldownTime > 0f) {
                _CurFireCooldownTime -= Time.deltaTime;
            }
            else {
                _animator.SetTrigger(Fire);
                _CurFireCooldownTime = _FireCooldownTime;
                return true;
            }
            
        }
        
        if (inspectable) {
            if (input.inspectState == InputState.Pressed) {
                _animator.SetTrigger(Inspect);
                return true;
            }
        }
        
        return returnValue;
    }
    
 
    
    private void OnDisable() {
        _animator.SetTrigger(Disable);
        _animator.SetBool(Idle, false);
        _animator.SetBool(Walk, false);
        _animator.SetBool(Run, false);
    }
    
  

    
}



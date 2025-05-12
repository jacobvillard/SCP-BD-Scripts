using UnityEngine;

/// <summary>
/// A weapon scriptable object that holds the weapon stats.
/// </summary>
[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "Scriptable Objects/WeaponScriptableObject")]
public class Weapon : ItemScriptableObject { 
    public float damage;
    public float range;
    public float firerate;
    public float recoilRate;
    public bool firemodes;
    public string ammoCalibre;
}
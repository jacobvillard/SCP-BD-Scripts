using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class Resize : MonoBehaviour
{
    [SerializeField] private RectTransform Transform;
     
    private void Awake() {
        if (Transform == null) {
            Transform = GetComponent<RectTransform>();
        }
    }
    
    private void OnEnable() {
        UpdateSize();
    }

    private void UpdateSize() {
        var height = Transform.Cast<RectTransform>()
            .Where(child => child.gameObject.activeSelf)
            .Sum(child => child.sizeDelta.y + 130f) + 400f;

        Transform.sizeDelta = new Vector2(Transform.sizeDelta.x, height);
    }
}

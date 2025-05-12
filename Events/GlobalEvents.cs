using System;


/// <summary>
/// This class is responsible for handling global events.
/// </summary>
public static class GlobalEvents {
    
    public static event Action<int, int, string, string, string, int> OnAmmoChanged;

    public static void RaiseAmmoChanged(int current, int reserve, string calibre, string type, string fireMode, int lowAmmoThreshold) {
        OnAmmoChanged?.Invoke(current, reserve, calibre, type, fireMode, lowAmmoThreshold);
    }
}


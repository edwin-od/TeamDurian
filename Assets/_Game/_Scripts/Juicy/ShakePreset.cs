using UnityEngine;

[CreateAssetMenu(menuName = "Shake Preset")]
public class ShakePreset : ScriptableObject
{
    public Vector2 seed;

    [Range(0.01f, 50f)]
    public float speed = 20f;

    [Range(0.01f, 10f)]
    public float maxMagnitude = 0.3f;
    [Range(0f, 3f)]
    public float noiseMagnitude = 0.3f;

    public Vector2 direction = Vector2.up;
}

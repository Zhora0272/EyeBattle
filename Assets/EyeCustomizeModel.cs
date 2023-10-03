using UnityEngine;

public class EyeCustomizeModel
{
    public float? _eyeSize;
    public float? _eyeBibeSize;
    public int? _eyeType;
    public Color? _eyeColor;
    public Color? _eyeBackColor;
    public Texture _eyeTexture;
    public GameObject _eyeDecor;

    public EyeCustomizeModel
    (
        float? eyeBibeSize = null,
        float? eyeSize = null,
        int? eyeType = null,
        Color? eyeColor = null,
        Color? eyeBackColor = null,
        Texture eyeTexture = null,
        GameObject eyeDecor = null
    )
    {
        _eyeBibeSize = eyeBibeSize;
        _eyeSize = eyeSize;
        _eyeColor = eyeColor;
        _eyeType = eyeType;
        _eyeColor = eyeColor;
        _eyeBackColor = eyeBackColor;
        _eyeTexture = eyeTexture;
        _eyeDecor = eyeDecor;
    }
}
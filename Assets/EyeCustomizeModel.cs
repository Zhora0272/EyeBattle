using System;

[Serializable]
public class EyeCustomizeModel
{
    public float _eyeSize;
    public float _eyeBibeSize;
    public int _eyeType;
    public int _eyeColor;
    public int _eyeBackColor;
    public int _eyeTexture;
    public int _eyeDecor;

    public EyeCustomizeModel
    (
        float eyeBibeSize = -1,
        float eyeSize = -1,
        int eyeType = -1,
        int eyeColor = default,
        int eyeBackColor = default,
        int eyeTexture = -1,
        int eyeDecor = -1
    )
    {
        _eyeBibeSize = eyeBibeSize;
        _eyeSize = eyeSize;
        _eyeColor = eyeColor;
        _eyeType = eyeType;
        _eyeBackColor = eyeBackColor;
        _eyeTexture = eyeTexture;
        _eyeDecor = eyeDecor;
    }
}
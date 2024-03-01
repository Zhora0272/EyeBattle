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
    public int _eyeHeadDecor;
    public int _eyeBodyDecor;

    public EyeCustomizeModel
    (
        float eyeBibeSize = -1,
        float eyeSize = -1,
        int eyeType = -1,
        int eyeColor = -1,
        int eyeBackColor = -1,
        int eyeTexture = -1,
        int eyeHeadDecor = -1,
        int eyeBodyDecor = -1
    )
    {
        _eyeBibeSize = eyeBibeSize;
        _eyeSize = eyeSize;
        _eyeColor = eyeColor;
        _eyeType = eyeType;
        _eyeBackColor = eyeBackColor;
        _eyeTexture = eyeTexture;
        _eyeHeadDecor = eyeHeadDecor;
        _eyeBodyDecor = eyeBodyDecor;
    }
}
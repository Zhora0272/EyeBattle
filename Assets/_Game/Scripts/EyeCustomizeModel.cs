using System;
using UnityEngine;

[Serializable]
public class EyeCustomizeModel
{
    public float _eyeSize;
    public float _eyeBibeSize;
    public Texture _eyeType;
    public int _eyeColor;
    public int _eyeBackColor;
    public int _eyeTextureIndex;
    public int _eyeHeadDecor;
    public int _eyeBodyDecor;

    public EyeCustomizeModel
    (
        float eyeBibeSize = -1,
        float eyeSize = -1,
        Texture eyeType = null,
        int eyeColor = -1,
        int eyeBackColor = -1,
        int eyeTextureIndex = -1,
        int eyeHeadDecor = -1,
        int eyeBodyDecor = -1
    )
    {
        _eyeBibeSize = eyeBibeSize;
        _eyeSize = eyeSize;
        _eyeColor = eyeColor;
        _eyeType = eyeType;
        _eyeBackColor = eyeBackColor;
        _eyeTextureIndex = eyeTextureIndex;
        _eyeHeadDecor = eyeHeadDecor;
        _eyeBodyDecor = eyeBodyDecor;
    }
}
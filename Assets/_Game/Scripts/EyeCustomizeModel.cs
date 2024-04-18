using System;
using UnityEngine;

[Serializable]
public class EyeCustomizeModel
{
    public int _eyeSize;
    public int _eyeBibeSize;
    public int _eyeType;
    public int _eyeColor;
    public int _eyeBackColor;
    public int _eyeHeadDecor;
    public int _eyeBodyDecor;

    public EyeCustomizeModel
    (
        int eyeBibeSize = -1,
        int eyeSize = -1,
        int eyeType = -1,
        int eyeColor = -1,
        int eyeBackColor = -1,
        int eyeHeadDecor = -1,
        int eyeBodyDecor = -1
    )
    {
        _eyeBibeSize = eyeBibeSize;
        
        _eyeSize = eyeSize;
        _eyeType = eyeType;
        
        _eyeColor = eyeColor;
        _eyeBackColor = eyeBackColor;
        
        _eyeHeadDecor = eyeHeadDecor;
        _eyeBodyDecor = eyeBodyDecor;
    }
}
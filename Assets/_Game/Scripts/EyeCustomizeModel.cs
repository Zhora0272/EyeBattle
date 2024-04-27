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
    public int _eyeDecor;

    public EyeCustomizeModel
    (
        int eyeBibeSize = -1,
        int eyeSize = -1,
        int eyeType = -1,
        int eyeColor = -1,
        int eyeBackColor = -1,
        int eyeDecor = -1
    )
    {
        _eyeBibeSize = eyeBibeSize;
        
        _eyeSize = eyeSize;
        _eyeType = eyeType;
        
        _eyeColor = eyeColor;
        _eyeBackColor = eyeBackColor;
        
        _eyeDecor = eyeDecor;
    }
}
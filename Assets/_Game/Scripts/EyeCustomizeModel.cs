using System;

[Serializable]
public class EyeCustomizeModel
{
    public int _eyePupilSize;
    public int _eyeType;
    public int _eyeColor;
    public int _eyeBackColor;
    public int _eyeDecor;

    public EyeCustomizeModel
    (
        int eyePupilSize = -1,
        int eyeType = -1,
        int eyeColor = -1,
        int eyeBackColor = -1,
        int eyeDecor = -1
    )
    {
        this._eyePupilSize = eyePupilSize;
        
        _eyeType = eyeType;
        
        _eyeColor = eyeColor;
        _eyeBackColor = eyeBackColor;
        
        _eyeDecor = eyeDecor;
    }
}
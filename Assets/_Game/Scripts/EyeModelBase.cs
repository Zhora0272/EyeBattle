using System;

[Serializable]
public class EyeModelBase
{
    public UpdateElement UpdateType;
    public float Speed = -1;
    public float UpdateTime = -1; // if time is (< 0) update is always
}
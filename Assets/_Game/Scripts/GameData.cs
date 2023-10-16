using System;

[Serializable]
public class GameData
{
    public int Money;
    public int Gem;
    public int[] ContainerConfigIndexes;
    public EyeCustomizeModel EyeConfigModel;
    public BaseEyeItemParameters[] EyeItemParameters;
}
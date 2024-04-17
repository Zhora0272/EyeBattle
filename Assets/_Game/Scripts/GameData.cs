using System;

[Serializable]
public class GameData
{
    public int Money;
    public int Gem;
    public int[] ContainerConfigIndexes;
    public EyeCustomizeModel EyeCustomizeModel;
    public EyeItemCollection[] EyeItemParameters;
    public string SaveTime;
}


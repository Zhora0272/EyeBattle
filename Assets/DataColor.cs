using UnityEngine;
using System;

[Serializable]
public struct DataColor
{
    public static implicit operator Color(DataColor dataColor)
    {
        return new Color(dataColor.r,dataColor.g, dataColor.b, dataColor.a);
    }

    public static implicit operator DataColor(Color color)
    {
        return new DataColor()
        {
            r = color.r,
            g = color.g,
            b = color.b,
            
            initState = true,
        };
    }
    
    public float r;
    public float g;
    public float b;
    public float a;
    public bool initState;
}
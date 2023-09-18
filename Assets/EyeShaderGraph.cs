using UnityEngine;

public static class EyeShaderGraph
{
    /// <summary>
    /// this method get EyeCustomizeModel parameters
    /// </summary>
    /// <param name="model"></param> model its a materials parameters
    /// <returns></returns>
    public static Material GetMaterial(EyeCustomizeModel model)
    {
        var mat = new Material("EyeShaderGraph");
        
        mat.SetFloat("EyeSize", (float)model._eyeSize);
        mat.SetInt("EyeType", (int)model._eyeType);
        mat.SetColor("EyeColor", (Color)model._eyeColor);
        mat.SetColor("EyeBackColor", (Color)model._eyeBackColor);
        mat.SetTexture("EyeTexture", model._eyeTexture);

        return mat;
    }

    public static Material ChangeMaterial(EyeCustomizeModel model, Material oldMaterial)
    {
        if(model._eyeSize != null) oldMaterial.SetFloat("EyeSize", (float)model._eyeSize);
        if(model._eyeType != null) oldMaterial.SetInt("EyeType", (int)model._eyeType);
        if(model._eyeColor != null) oldMaterial.SetColor("EyeColor", (Color)model._eyeColor);
        if(model._eyeBackColor != null) oldMaterial.SetColor("EyeBackColor", (Color)model._eyeBackColor);
        if(model._eyeTexture != null) oldMaterial.SetTexture("EyeTexture", model._eyeTexture);
        
        return oldMaterial;
    }

    public enum EyeParameterType
    {
        
    }
}
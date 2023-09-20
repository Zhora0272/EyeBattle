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
        var shader = Shader.Find("Shader Graphs/EyeShaderGraph");
        var material = new Material(shader);

        ChangeParameters(model, material);

        //mat.SetTexture("EyeTexture", model._eyeTexture);
        //mat.SetInt("EyeType", (int)model._eyeType);

        return material;
    }

    public static void ChangeMaterial(EyeCustomizeModel model, Material oldMaterial)
    {
        ChangeParameters(model, oldMaterial);
    }

    private static void ChangeParameters(EyeCustomizeModel model, Material material)
    {
        if (model._eyeSize != null) material.SetFloat("_EyeSize", (float) model._eyeSize);
        if (model._eyeType != null) material.SetInt("_EyeType", (int) model._eyeType);
        if (model._eyeColor != null) material.SetColor("_EyeColor", (Color) model._eyeColor);
        if (model._eyeBackColor != null) material.SetColor("_EyeBackColor", (Color) model._eyeBackColor);
        if (model._eyeTexture != null) material.SetTexture("_EyeTexture", model._eyeTexture);
    }

    public enum EyeParameterType
    {
        
    }
}
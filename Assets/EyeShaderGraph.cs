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

        return material;
    }

    public static Material ChangeMaterial(EyeCustomizeModel model, Material oldMaterial)
    {
        return ChangeParameters(model, oldMaterial);
    }

    private static Material ChangeParameters(EyeCustomizeModel model, Material material)
    {
        if (model._eyeSize != null) material.SetFloat("_EyeSize", (float) model._eyeSize);
        if (model._eyeBibeSize != null) material.SetFloat("_BibeSize", (float) model._eyeBibeSize);
        if (model._eyeType != null) material.SetInt("_EyeType", (int) model._eyeType);
        if (model._eyeColor != null) material.SetColor("_EyeColor", (Color) model._eyeColor);
        if (model._eyeBackColor != null) material.SetColor("_EyeBackColor", (Color) model._eyeBackColor);
        if (model._eyeTexture != null) material.SetTexture("_EyeTexture", model._eyeTexture);

        return material;
    }
}
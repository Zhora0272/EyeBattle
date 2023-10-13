using UnityEngine;

public static class EyeShaderGraph
{
    private static DataManager _data;
    static EyeShaderGraph()
    {
        _data = MainManager.GetManager<DataManager>();
    }
    
    public static Material GetMaterial
    (
        EyeCustomizeModel model
    )
    {
        var shader = Shader.Find("Shader Graphs/EyeShaderGraph");
        var material = new Material(shader);

        ChangeParameters(model, material);

        return material;
    }

    public static Material ChangeMaterial(
        EyeCustomizeModel model,
        Material oldMaterial
    )
    {
        return ChangeParameters(model, oldMaterial);
    }

    private static Material ChangeParameters
    (
        EyeCustomizeModel model,
        Material material
    )
    {
        if (model._eyeSize != -1) material.SetFloat("_EyeSize", model._eyeSize);
        if (model._eyeBibeSize != -1) material.SetFloat("_EyeBibeSize", model._eyeBibeSize);
        if (model._eyeType != -1) material.SetInt("_EyeType", model._eyeType);
        
        if (model._eyeColor != -1) material.SetColor("_EyeColor", _data.EyeColor.Colors[model._eyeColor].Color);
        if (model._eyeBackColor != -1) material.SetColor("_EyeBackColor", _data.EyeColor.Colors[model._eyeBackColor].Color);
        
        return material;
    }
}
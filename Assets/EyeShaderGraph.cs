using Shop;
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
        if (model._eyeBackColor != -1)
            material.SetColor("_EyeBackColor", _data.EyeBackColor.Colors[model._eyeBackColor].Color);

        return material;
    }

    public static EyeCustomizeModel ConvertMaterialToModel(Material material)
    {
        return new EyeCustomizeModel()
        {
            _eyeSize = material.GetFloat("_EyeSize"),
            _eyeBibeSize = material.GetFloat("_EyeBibeSize"),
            //_eyeType = material.GetInt("_EyeType"),

            _eyeColor = FindTheColorIndex(_data.EyeColor, material.GetColor("_EyeColor")),
            _eyeBackColor = FindTheColorIndex(_data.EyeColor, material.GetColor("_EyeBackColor")),
        };
    }

    public static int FindTheColorIndex(ShopEyeColorScriptable data, Color color)
    {
        for (int i = 0; i < data.Colors.Length; i++)
        {
            if (color == data.Colors[i].Color)
            {
                return i;
            }
        }

        return 0;
    }
}
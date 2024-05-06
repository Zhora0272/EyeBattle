using UnityEngine;
using Data;

public static class EyeShaderGraph
{
    private static readonly DataManager Data;

    private enum EyeMaterialConfig
    {
        _EyeSize,
        _EyeBibeSize,
        _EyeColor,
        _EyeBackColor,
        _EyeTypeTexture
    }


    static EyeShaderGraph()
    {
        Data = MainManager.GetManager<DataManager>();
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
        /*//size
        if (model._eyeSize >= 0)
        {
            SetMaterialValue(Data.EyeSize.SizeParameters[model._eyeSize].EyeSize,
                material, EyeMaterialConfig._EyeSize);
        }*/

        if (model._eyePupilSize >= 0)
        {
            SetMaterialValue(Data.EyePupilSize.SizeParameters[model._eyePupilSize].EyeSize,
                material, EyeMaterialConfig._EyeBibeSize);
        }

        //type
        if (model._eyeType >= 0)
        {
            var texture = Data.EyeTexture.TextureParameters[model._eyeType].Texture;
            SetMaterialValue(texture, material, EyeMaterialConfig._EyeTypeTexture);
        }

        //color
        if (model._eyeColor >= 0)
        {
            var color = Data.EyeColor.Colors[model._eyeColor].Color;
            SetMaterialValue(color, material, EyeMaterialConfig._EyeColor);
        }

        if (model._eyeBackColor >= 0)
        {
            var color = Data.EyeBackColor.Colors[model._eyeBackColor].Color;
            SetMaterialValue(color, material, EyeMaterialConfig._EyeBackColor);
        }

        return material;
    }

    private static void SetMaterialValue
    (
        object value,
        Material material,
        EyeMaterialConfig config
    )
    {
        switch (value)
        {
            case int intValue:
                material.SetInt(config.ToString(), intValue);
                break;
            case float floatValue:
                material.SetFloat(config.ToString(), floatValue);
                break;
            case Color colorValue:
                material.SetColor(config.ToString(), colorValue);
                break;
            case Texture textureValue:
                material.SetTexture(config.ToString(), textureValue);
                break;
        }
    }
}
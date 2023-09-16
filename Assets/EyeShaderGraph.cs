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
        
        mat.SetFloat("EyeSize", model._eyeSize);
        mat.SetInt("EyeType", model._eyeType);
        mat.SetColor("EyeColor", model._eyeColor);
        mat.SetColor("EyeBackColor", model._eyeBackColor);
        mat.SetTexture("EyeTexture", model._eyeTexture);

        return mat;
    }
}
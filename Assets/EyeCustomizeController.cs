using UnityEngine;

public class EyeCustomizeController : MonoBehaviour
{
    [SerializeField] private Material _eyeMaterial;
    [SerializeField] int _textureSize;
    [SerializeField] private Texture _newTexture;

    [ContextMenu("update")]
    private void UpdateInfo()
    {
        _eyeMaterial.mainTexture = (Texture)ScaleTexture((Texture2D)_newTexture, _textureSize, _textureSize);
    }

    private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight);
        Color[] rpixels = result.GetPixels(0);
        float incX = (1.0f / (float)targetWidth);
        float incY = (1.0f / (float)targetHeight);

        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
        }

        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }
}

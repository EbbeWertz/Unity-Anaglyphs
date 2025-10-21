using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class AnaglyphEffect : MonoBehaviour
{
    [Header("Anaglyph Settings")]
    public Material material;
    public Camera rightEyeCam;

    private RenderTexture rightEyeRT;

    private void OnEnable()
    {
        if (material == null || rightEyeCam == null)
            return;

        int w = Screen.width;
        int h = Screen.height;

        rightEyeRT = new RenderTexture(w, h, 24, RenderTextureFormat.Default);
        rightEyeCam.targetTexture = rightEyeRT;
    }

    private void OnDisable()
    {
        if (rightEyeCam != null)
            rightEyeCam.targetTexture = null;

        if (rightEyeRT != null)
        {
            rightEyeRT.Release();
            rightEyeRT = null;
        }
    }

    private void OnRenderImage(RenderTexture leftEyeTex, RenderTexture destination)
    {
        if (material == null || rightEyeCam == null)
        {
            Graphics.Blit(leftEyeTex, destination);
            return;
        }

        rightEyeCam.Render();

        material.SetTexture("_MainTex2", rightEyeRT);

        Graphics.Blit(leftEyeTex, destination, material);
    }
}

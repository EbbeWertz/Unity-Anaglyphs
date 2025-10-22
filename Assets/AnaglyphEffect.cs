using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class AnaglyphEffect : MonoBehaviour
{
    [Header("Anaglyph Settings")]
    public Material material;
    public Camera rightEyeCam;

    [Range(0f, 0.1f)]
    public float eyeSeparation = 0.03f;

    [Range(0.1f, 50f)]
    public float convergenceDistance = 10f;

    [Range(0f, 1f)]
    public float colorBlend = 0.5f; // 0 = pure anaglyph, 1 = full color blend

    private RenderTexture rightEyeRT;
    private Camera leftEyeCam;

    private void OnEnable()
    {
        leftEyeCam = GetComponent<Camera>();
        if (material == null || rightEyeCam == null) return;

        rightEyeRT = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.Default);
        rightEyeCam.targetTexture = rightEyeRT;
    }

    private void OnDisable()
    {
        if (rightEyeCam) rightEyeCam.targetTexture = null;
        if (rightEyeRT) { rightEyeRT.Release(); rightEyeRT = null; }
    }

    private void Update()
    {
        if (rightEyeCam == null || leftEyeCam == null) return;

        // Offset right eye
        rightEyeCam.transform.position = leftEyeCam.transform.position + leftEyeCam.transform.right * eyeSeparation;
        rightEyeCam.transform.rotation = leftEyeCam.transform.rotation;

        // --- Asymmetric frustum calculation ---
        ApplyStereoProjection(leftEyeCam, -eyeSeparation * 0.5f, convergenceDistance);
        ApplyStereoProjection(rightEyeCam, eyeSeparation * 0.5f, convergenceDistance);

        // Pass color-blend strength to shader
        material.SetFloat("_ColorBlend", colorBlend);
    }

    private static void ApplyStereoProjection(Camera cam, float eyeOffset, float convergence)
    {
        float near = cam.nearClipPlane;
        float far = cam.farClipPlane;
        float fov = cam.fieldOfView * Mathf.Deg2Rad;
        float aspect = cam.aspect;

        float top = near * Mathf.Tan(fov * 0.5f);
        float right = top * aspect;
        float frustumShift = eyeOffset * near / convergence;

        cam.projectionMatrix = Matrix4x4.Frustum(
            -right + frustumShift, right + frustumShift,
            -top, top, near, far
        );
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

using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class AnaglyphEffect : MonoBehaviour
{
    public Material material;
    public Camera cam2;

    private RenderTexture rt;


    private void OnEnable()
    {
        if (material == null)
        {
            return;
        }

        //cam2.enabled = false;
        int w = Screen.width, h = Screen.height;
        rt = RenderTexture.GetTemporary(w, h, 8, RenderTextureFormat.Default);
        //cam2.targetTexture = rt;


    }

    private void OnDisable()
    {
        if (rt != null) { rt.Release(); }
        //cam2.targetTexture = null;
        
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material == null)
        {
            Graphics.Blit(source, destination);
            return;
        }


       
        //cam2.Render();

        material.SetTexture("_MainTex2", rt);
        Graphics.Blit(source, destination, material);


    }
}
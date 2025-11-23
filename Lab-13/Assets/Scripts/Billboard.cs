using UnityEngine;

public class Billboard : MonoBehaviour
{
    [Header("Image URL for this billboard")]
    public string imageUrl;                // paste the URL you want to use

    [Header("Renderer of the billboard")]
    public Renderer billboardRenderer;     // assign your cube/plane Renderer

    private void Start()
    {
        // Find the ImageManager in the scene
        ImageManager manager = FindObjectOfType<ImageManager>();

        // Ask the manager for the image
        manager.GetWebImage(imageUrl, (tex) =>
        {
            // Apply the texture to this billboard's material
            billboardRenderer.material.mainTexture = tex;
        });
    }
}
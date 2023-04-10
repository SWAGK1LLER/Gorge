using UnityEngine;

public class WaterTexModif : MonoBehaviour
{
    [SerializeField] private Vector2 tiling;
    [SerializeField] private Vector4 Wave1;
    [SerializeField] private Vector4 Wave2;
    [SerializeField] private Vector4 Wave3;
    [SerializeField] private Vector4 Wave4;

    //The material property block we pass to the GPU
    private MaterialPropertyBlock propertyBlock;

    // OnValidate is called in the editor after the component is edited
    void Awake()
    {
        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();

        Renderer renderer = GetComponentInChildren<Renderer>();

        propertyBlock.SetVector("_WaveA", Wave1);
        propertyBlock.SetVector("_WaveB", Wave2);
        propertyBlock.SetVector("_WaveC", Wave3);
        propertyBlock.SetVector("_WaveD", Wave4);

        renderer.SetPropertyBlock(propertyBlock);
        renderer.material.SetTextureScale("_MainTex", tiling);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorProperty : MonoBehaviour
{
    [SerializeField] private Color MaterialColor;
    [SerializeField] private Color EchoColor;

    //The material property block we pass to the GPU
    private MaterialPropertyBlock propertyBlock;

    // OnValidate is called in the editor after the component is edited
    void Awake()
    {
        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();

        Renderer renderer = GetComponentInChildren<Renderer>();

        propertyBlock.SetColor("_Color", MaterialColor);
        propertyBlock.SetColor("_EchoColor", EchoColor);

        renderer.SetPropertyBlock(propertyBlock);
    }
}

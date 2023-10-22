using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tiles : MonoBehaviour
{
    // [SerializeField] private Material material;
    [SerializeField] private Sprite frontTile;
    private void Start()
    {
        // Create a new Material from the Sprite
        Material material = new Material(Shader.Find("Standard"));
        material.mainTexture = frontTile.texture; // Assign the Sprite's texture to the material

        // Assign the material to the Mesh Renderer
        GetComponent<MeshRenderer>().material = material;
    }

    public Sprite GetFrontTile() => frontTile;
}

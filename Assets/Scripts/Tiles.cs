using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tiles : MonoBehaviour
{
    [SerializeField] private Sprite backTile;
    [SerializeField] private Sprite frontTile;
    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = frontTile;
    }
}

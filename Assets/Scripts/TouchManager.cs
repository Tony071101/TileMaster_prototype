using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TouchManager : MonoBehaviour
{
   private PlayerInput playerInput;

   private InputAction touchInputAction;

   private GameObject hitObject;
   private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>();
    private Vector3[] arrayHolderTilePositions = new Vector3[3];
    private GameObject tileHolder;
    private GameObject arrayHolder;
   private void Awake() {
        playerInput = GetComponent<PlayerInput>();
        touchInputAction = playerInput.actions["TouchInput"];
        tileHolder = GameObject.Find("TileHolder");
        arrayHolder = GameObject.Find("ArrayHolder");
   }

   private void OnEnable() {
        touchInputAction.performed += TouchInput;
   }

   private void OnDisable() {
        touchInputAction.performed -= TouchInput;
   }

   private void TouchInput(InputAction.CallbackContext context){
        Vector2 touchPosition = context.ReadValue<Vector2>();

        // Create a ray from the touch/click position
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);

        // RaycastHit to store information about what the ray hits
        RaycastHit hit;

        // Check if the ray hits a GameObject
        if (Physics.Raycast(ray, out hit))
        {
            hitObject = hit.collider.gameObject;
            if (Physics.Raycast(ray, out hit))
            {
                hitObject = hit.collider.gameObject;
                
                if (arrayHolder != null && hitObject.transform.IsChildOf(arrayHolder.transform))
                {
                    // Clicked a tile in ArrayHolder, move it back to TileHolder
                    MoveTileToTileHolder(hitObject);
                }
                else if (tileHolder != null && hitObject.CompareTag("Tiles"))
                {
                    // Clicked a tile in TileHolder, move it to ArrayHolder
                    MoveTileToArrayHolder(hitObject);
                }
            }
        }
   }

   private void MoveTileToArrayHolder(GameObject tile){
        if (arrayHolder != null && arrayHolder.transform.childCount < 3)
        {
            // Store the original position of the tile
            originalPositions[tile] = tile.transform.localPosition;

            tile.transform.SetParent(arrayHolder.transform);

            // Calculate the new position for the tile based on index and desired spacing
            int childIndex = arrayHolder.transform.childCount - 1;
            Debug.LogError(childIndex);
            float spacing = 25.0f; // Adjust this value as needed
            Vector3 newPosition = Vector3.right * spacing * childIndex;

            // Set the new position for the tile
            tile.transform.localPosition = newPosition;
        }
        else
        {
            Debug.LogError("ArrayHolder is full. It can only contain up to 3 GameObjects.");
        }
   }

   private void MoveTileToTileHolder(GameObject tile)
    {
        // Move the tile to TileHolder
        tile.transform.SetParent(tileHolder.transform);

        // Reset the local position to the original position
        if (originalPositions.ContainsKey(tile))
        {
            tile.transform.localPosition = originalPositions[tile];
        }
    }
}

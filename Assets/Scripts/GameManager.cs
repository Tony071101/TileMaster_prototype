using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance { get; private set; }
    private GameObject arrayHolder;
    private GameObject tileHolder;
    private TouchManager touchManager;
    private int level = 1;
    private bool canGoNextLvl;

     private void Awake() {
        if (_instance != null)
        {
            Destroy(this.gameObject);
        } else{
            _instance = this;
        }
    }

    private void OnDestroy() {
        if(_instance == this){
            _instance = null;
        }        
    }

    private void Start() {
        canGoNextLvl = false;
        arrayHolder = GameObject.Find("ArrayHolder");
        touchManager = FindObjectOfType<TouchManager>();
        tileHolder = GameObject.Find("TileHolder");
    } 


    private void Update() {
        CheckForTilesMatched();

        CheckTile();
        
        if(canGoNextLvl == true){
            Invoke("GoNextLevelEvent", 1.5f);
        }
    }

    private void CheckForTilesMatched() {
        // Get all the children of the ArrayHolder.
        Transform[] children = arrayHolder.GetComponentsInChildren<Transform>();
        // Create a dictionary to store the frontTiles and their corresponding Tile GameObjects.
        Dictionary<Sprite, List<GameObject>> tileSpriteMap = new Dictionary<Sprite, List<GameObject>>();

        foreach (Transform child in children)
        {
            if (child.CompareTag("Tiles"))
            {
                Tiles tilesScript = child.GetComponentInChildren<Tiles>();
                if (tilesScript != null)
                {
                    Sprite tileSprite = tilesScript.GetFrontTile();

                    // Check if the frontTile is already in the dictionary.
                    if (tileSpriteMap.ContainsKey(tileSprite))
                    {
                        // Add the current tile to the list of matching tiles.
                        tileSpriteMap[tileSprite].Add(child.gameObject);
                    }
                    else
                    {
                        // Create a new list with the current tile and add it to the dictionary.
                        tileSpriteMap.Add(tileSprite, new List<GameObject> { child.gameObject });
                    }
                }
            }
        }

        // Iterate through the dictionary to destroy matching tiles.
        foreach (var pair in tileSpriteMap)
        {
            if (pair.Value.Count >= 3)
            {
                // Matching tiles found, destroy them.
                foreach (GameObject matchingTile in pair.Value)
                {
                    // Destroy the matching tile GameObject.
                    Destroy(matchingTile, 0.5f);
                }
            }
        }
    }

    private void CheckTile(){
        if(tileHolder.transform.childCount == 0){
            canGoNextLvl = true;
        }
    }

    private void GoNextLevelEvent(){
        Debug.LogError("Next Level");
    }

    public void NewGame(){
        LoadLevel(level);
    }

    private void LoadLevel(int level){
        this.level = level;
        SceneManager.LoadScene($"Stage-{level}");
    }

    public void NextLevel(){
        LoadLevel(level + 1);
    }

     private void GameOver(){
        NewGame();
    }

    public void Quit(){
        Application.Quit();
    }

}

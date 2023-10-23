using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public class GameData {
        public int levelData;
    }

    public event EventHandler nextLevel;
    public event EventHandler countinueBtnShow;
    public static GameManager _instance { get; private set; }
    private GameObject arrayHolder;
    private GameObject tileHolder;
    private TouchManager touchManager;
    [SerializeField] private int level;
    private byte[] encryptionKey = new byte[32]; // Replace with a strong, secret key
    private byte[] initializationVector = new byte[16]; // Replace with a secure IV
     private void Awake() {
        if (_instance != null)
        {
            Destroy(gameObject);
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
        touchManager = FindObjectOfType<TouchManager>();
        tileHolder = GameObject.Find("TileHolder");
        arrayHolder = GameObject.Find("ArrayHolder");
    } 


    private void Update() {
        CheckForTilesMatched();
        CheckTile();
        CheckSaveData();
    }

    private void CheckForTilesMatched() {
        if (arrayHolder == null){
            return;
        }
        Transform[] children = arrayHolder.GetComponentsInChildren<Transform>();
        Dictionary<Sprite, List<GameObject>> tileSpriteMap = new Dictionary<Sprite, List<GameObject>>();

        foreach (Transform child in children)
        {
            if (child.CompareTag("Tiles"))
            {
                SpriteRenderer spriteRenderer = child.GetComponentInChildren<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    Sprite tileSprite = spriteRenderer.sprite;

                    if (tileSpriteMap.ContainsKey(tileSprite))
                    {
                        tileSpriteMap[tileSprite].Add(child.gameObject);
                    }
                    else
                    {
                        tileSpriteMap.Add(tileSprite, new List<GameObject> { child.gameObject });
                    }
                }
            }
        }

        foreach (var pair in tileSpriteMap)
        {
            if (pair.Value.Count >= 3)
            {
                foreach (GameObject matchingTile in pair.Value)
                {
                    Destroy(matchingTile, 0.5f);
                }
            }
        }
    }

    private void CheckTile(){
        if (tileHolder == null){
            return;
        }
        else if(tileHolder.transform.childCount == 0){
            float delay = 0.5f; 
            Invoke("InvokeNextLevel", delay);
        }
    }

    private void InvokeNextLevel()
    {
        nextLevel?.Invoke(this, EventArgs.Empty);
    }

    public void NewGame(){
        LoadLevel(level);
    }

    private void LoadLevel(int level){
        this.level = level;
        SceneManager.LoadScene($"Level-{level}");
    }

    public void NextLevel(){
        SaveGameData();
        LoadLevel(level);
    }

    public void Quit(){
        Application.Quit();
    }

    public void SaveGameData()
    {
        GameData gameData = new GameData
        {
            levelData = level
        };
        string json = JsonUtility.ToJson(gameData);
        string encryptedData = Encrypt(json);
        File.WriteAllText("saveData.txt", encryptedData);
    }

    public void LoadGameData()
    {
        if (File.Exists("saveData.txt"))
        {
            string encryptedData = File.ReadAllText("saveData.txt");
            string json = Decrypt(encryptedData);
            GameData gameData = JsonUtility.FromJson<GameData>(json);
            level = gameData.levelData;
            LoadLevel(level);
        }
    }

    private void CheckSaveData(){
        if(File.Exists("saveData.txt")){
            countinueBtnShow?.Invoke(this, EventArgs.Empty);
        }
    }

    private string Encrypt(string data)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = encryptionKey;
            aesAlg.IV = initializationVector;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(data);
                    }
                }
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    private string Decrypt(string encryptedData)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = encryptionKey;
            aesAlg.IV = initializationVector;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedData)))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }
}

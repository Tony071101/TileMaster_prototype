using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public event EventHandler disableTouch;
    public event EventHandler enableTouch;
    [SerializeField] private GameObject nextLvlPannel;
    [SerializeField] private GameObject pausePannel;
    [SerializeField] private GameObject countinueBtn;
    [SerializeField] private GameObject nextLvlBtn;
    private void Start() {
        GameManager._instance.nextLevel += ShowPannelNextLevel;
        GameManager._instance.countinueBtnShow += LoadDataBtn;
    }

    private void ShowPannelNextLevel(object sender, EventArgs e){
        nextLvlPannel.SetActive(true);
        if(SceneManager.GetActiveScene().name == "Level-3"){
            nextLvlBtn.SetActive(false);
        }
    }

    public void GoToNextLvLBtn(){
        GameManager._instance.NextLevel();
    }

    public void PauseBtn(){
        Time.timeScale = 0;
        disableTouch?.Invoke(this, EventArgs.Empty);
        pausePannel.SetActive(true);
    }

    public void Countinue(){
        Time.timeScale = 1;
        enableTouch?.Invoke(this, EventArgs.Empty);
        pausePannel.SetActive(false);
    }

    public void Quit() {
        //Save right here
        GameManager._instance.Quit();
    }

    public void StartGameBtn() {
        GameManager._instance.NewGame();
    }

    private void LoadDataBtn(object sender, EventArgs e){
        if(countinueBtn == null){
            return;
        }
        countinueBtn.SetActive(true);
    }

    public void LoadGameData(){
        GameManager._instance.LoadGameData();
    }
}

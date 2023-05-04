using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public UIDocument uIDocument;
    public Button playButton;
    //private Button optionsButton;
    //private Button creditsButton;
    public Button quitButton;

    void Start()
    {
        var rootVE = GetComponent<UIDocument>().rootVisualElement;

        playButton = rootVE.Q<Button>("PlayButton");
        quitButton = rootVE.Q<Button>("QuitButton");

        playButton.clicked += PlayGame;
        quitButton.clicked += QuitGame;
        playButton.RegisterCallback<ClickEvent>(ev => PlayGame());
        quitButton.RegisterCallback<ClickEvent>(ev => QuitGame());
    }

    private void PlayGame()
    {
        SceneManager.LoadScene("Level");
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
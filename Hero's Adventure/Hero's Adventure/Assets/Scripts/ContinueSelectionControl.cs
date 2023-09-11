using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinueSelectionControl : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI continueGame;
    [SerializeField] private TextMeshProUGUI newGame;
    [SerializeField] private TextMeshProUGUI setting;
    [SerializeField] private TextMeshProUGUI exit;
    [SerializeField] private TMP_ColorGradient defaultColor;
    [SerializeField] private TMP_ColorGradient selectedColor;
    [SerializeField] private string newGameSceneName;
    [SerializeField] private string settingSceneName;
    [SerializeField] private AudioSource pressAudio;
    private int currentSelection = 0;
    private enum selection { continueGame, newGame, setting, exit }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            pressAudio.Play();
            currentSelection++;
            if (currentSelection > (int)selection.exit)
            {
                currentSelection = 0;
            }
            if (currentSelection == (int)selection.newGame)
            {
                continueGame.colorGradientPreset = defaultColor;
                newGame.colorGradientPreset = selectedColor;
            }
            else if (currentSelection == (int)selection.setting)
            {
                newGame.colorGradientPreset = defaultColor;
                setting.colorGradientPreset = selectedColor;
            }
            else if (currentSelection == (int)selection.exit)
            {
                setting.colorGradientPreset = defaultColor;
                exit.colorGradientPreset = selectedColor;
            }
            else
            {
                exit.colorGradientPreset = defaultColor;
                continueGame.colorGradientPreset = selectedColor;
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            pressAudio.Play();
            currentSelection--;
            if (currentSelection < (int)selection.continueGame)
            {
                currentSelection = (int)selection.exit;
            }
            if (currentSelection == (int)selection.continueGame)
            {
                newGame.colorGradientPreset = defaultColor;
                continueGame.colorGradientPreset = selectedColor;
            }
            else if (currentSelection == (int)selection.newGame)
            {
                setting.colorGradientPreset = defaultColor;
                newGame.colorGradientPreset = selectedColor;
            }
            else if (currentSelection == (int)selection.setting)
            {
                exit.colorGradientPreset = defaultColor;
                setting.colorGradientPreset = selectedColor;
            }
            else
            {
                continueGame.colorGradientPreset = defaultColor;
                exit.colorGradientPreset = selectedColor;
            }
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            AccessSelection();
        }
    }
    public void AccessSelection()
    {
        pressAudio.Play();
        if (currentSelection == (int)selection.continueGame)
        {
            Debug.Log("Continue");
        }
        else if (currentSelection == (int)selection.newGame)
        {
            Debug.Log("New game");
        }
        else if (currentSelection == (int)selection.setting)
        {
            SceneManager.LoadScene(settingSceneName);
        }
        else
        {
            Application.Quit();
        }
    }
    public void SetCurrentSelection(int index)
    {
        currentSelection = index;
        if(currentSelection == (int)selection.continueGame)
        {
            continueGame.colorGradientPreset = selectedColor;
            newGame.colorGradientPreset = defaultColor;
            setting.colorGradientPreset = defaultColor;
            exit.colorGradientPreset = defaultColor;
        }else if(currentSelection == (int)selection.newGame)
        {
            newGame.colorGradientPreset = selectedColor;
            continueGame.colorGradientPreset = defaultColor;
            setting.colorGradientPreset = defaultColor;
            exit.colorGradientPreset = defaultColor;
        }else if(currentSelection == (int)selection.setting)
        {
            setting.colorGradientPreset = selectedColor;
            continueGame.colorGradientPreset = defaultColor;
            newGame.colorGradientPreset = defaultColor;
            exit.colorGradientPreset = defaultColor;
        }
        else
        {
            exit.colorGradientPreset = selectedColor;
            continueGame.colorGradientPreset = defaultColor;
            newGame.colorGradientPreset = defaultColor;
            setting.colorGradientPreset = defaultColor;
        }
    }
}

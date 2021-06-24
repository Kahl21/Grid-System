using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    bool _paused = false;
    public bool IsPaused { get { return _paused; } }

    public void Init()
    {
        gameObject.SetActive(false);
        GameUpdate.UISubscribe += PauseInput;
    }

    public void PauseInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (_paused)
        {
            gameObject.SetActive(false);
            _paused = false;
        }
        else
        {
            gameObject.SetActive(true);
            _paused = true;
        }
    }

    public void ResetPauseMenu()
    {
        GameUpdate.UISubscribe -= PauseInput;
        gameObject.SetActive(false);
    }
    
    public void Resume()
    {
        PauseGame();
    }

    public void GoToCharacterSelect()
    {
        UIHolder.UIInstance.GetBattleUI.ForceEndBattle();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

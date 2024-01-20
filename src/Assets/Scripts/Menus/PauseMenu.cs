using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject optionsMenu;
    public InputManager inputManager;
    
    private bool _isPaused, _prevMenuState;
    
    private void Update()
    {
        HandleMenu();
    }

    private void HandleMenu()
    {
        bool isPaused = inputManager.Menu;
        
        if (isPaused && !_prevMenuState)
        {
            if (_isPaused)
            {
                Resume();
                _isPaused = false;
            }
            else
            {
                Pause();
                _isPaused = true;
            }
        }
        _prevMenuState = isPaused;
    }
    
    public void BtnResume()
    {
        Resume();
    }
    
    public void BtnOptions()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }
    
    public void BtnMainMenu()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        _isPaused = false;
        SceneManager.LoadScene("MainMenu");
    }

    private void Pause()
    {
        Time.timeScale = 0;
        AudioListener.pause = true;
        pauseMenu.SetActive(true);
        _isPaused = true;
        ChangeCursorState();
    }

    private void Resume()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        pauseMenu.SetActive(false);
        _isPaused = false;
        ChangeCursorState();
    }

    private void ChangeCursorState()
    {
        Cursor.visible = !Cursor.visible;
        Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
    }
}

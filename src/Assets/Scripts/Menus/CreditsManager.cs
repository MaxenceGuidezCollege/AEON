using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{
    
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitCredits();
        }
    }

    public void OnEndCredits()
    {
        QuitCredits();
    }

    private void QuitCredits()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("MainMenu");
    }
}

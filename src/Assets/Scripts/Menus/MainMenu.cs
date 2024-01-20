using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject optionsMenu;
    public Slider loader;

    private const string keyLevel = "LEVEL";
    
    public void Start()
    {
        loader.gameObject.SetActive(false);
        optionsMenu.SetActive(false);
        PlayerPrefs.SetInt(keyLevel, 0);
        
        // In case of bugs.
        // Screen.SetResolution(1920, 1080, FullScreenMode.MaximizedWindow);
    }

    public void BtnPlay()
    {
        gameObject.SetActive(false);
        
        gameObject.transform.parent.GetComponent<MonoBehaviour>().StartCoroutine(LoadMainScene());
    }
    
    public void BtnOptions()
    {
        gameObject.SetActive(false);
        optionsMenu.SetActive(true);
    }
    
    public void BtnCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void BtnQuit()
    {
        Application.Quit();
    }

    IEnumerator LoadMainScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        
        loader.gameObject.SetActive(true);

        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);

            loader.value = progressValue;

            yield return null;
        }
    }
}

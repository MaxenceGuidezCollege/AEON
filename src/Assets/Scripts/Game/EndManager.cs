using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndManager : MonoBehaviour
{
    public EndElevator endElevator;
    public BoxCollider doorBlocker;
    public AudioClip soundOnDoorMoving;

    private const string keyLevel = "LEVEL";
    private bool _areDoorMoving;

    private void Start()
    {
        doorBlocker.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_areDoorMoving) return;
        
        doorBlocker.enabled = true;
        AudioManager.instance.PlayClipAt(soundOnDoorMoving, transform.position);
        endElevator.AnimateLastClosing();
        StartCoroutine(LoadNextLevel());
        _areDoorMoving = true;
    }

    IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(3f);

        int actualLevel = PlayerPrefs.GetInt(keyLevel, 0);
        if (actualLevel + 1 >= 2)
        {
            SceneManager.LoadScene("Credits");
        }
        else
        {
            doorBlocker.enabled = false;
            PlayerController.instance.UnblockPlayer();
            PlayerPrefs.SetInt(keyLevel, actualLevel + 1);
            SceneManager.LoadScene("MainScene");
        }
    }
}

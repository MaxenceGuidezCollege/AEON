using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject kronos;
    
    private const string keyLevel = "LEVEL";
    private int _actualLevel;

    private void Start()
    {
        _actualLevel = PlayerPrefs.GetInt(keyLevel, 0);
        MapManager.instance.LoadLevel(_actualLevel);
    }

    private void Update()
    {
        kronos.SetActive(TimeController.instance.GetStopState());
    }
}

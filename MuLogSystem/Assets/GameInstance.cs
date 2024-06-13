using UnityEngine;
using Logger = LogSystem.Logger;

public class GameInstance : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Logger.Initialize();
        Logger.LogDisplay("This is a display log message.");
        Logger.LogWarning("This is a warning log message.");
        Logger.LogError("This is an error log message.");
    }

    // Update is called once per frame
    void Update()
    {
    }
}
using System;
using UnityEngine;
using Logger = LogSystem.Logger;

public class GameInstance : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Logger.Initialize();
    }

    public void OnBeginTestClick()
    {
        LoggerPerformanceTest test = gameObject.AddComponent<LoggerPerformanceTest>();
        test.TestLogWritePerformance();
    }

    public void OnApplicationQuit()
    {
        Logger.Uninitialize();
    }
}
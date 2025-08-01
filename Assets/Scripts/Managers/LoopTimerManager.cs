using System;
using UnityEngine;

public class LoopTimerManager : MonoSingleton<LoopTimerManager>
{
    public static event Action OnLoopComplete;
    private const float LoopTimeS = 60;
    private float _loopTimer;
    private float _loopCompletionPercentage;

    void Start()
    {
        _loopTimer = Time.time + LoopTimeS;
    }
    void Update()
    {
        if (_loopTimer <= Time.time)
        {
            _loopTimer = Time.time + LoopTimeS;
            OnLoopComplete?.Invoke();
        }

        _loopCompletionPercentage = (Time.time - (_loopTimer - LoopTimeS)) / LoopTimeS;
        UIManager.Singleton.UpdateLoopCompletion(_loopCompletionPercentage);
        // Debug.Log($"Loop Completion Percentage: {_loopCompletionPercentage}");
    }
}

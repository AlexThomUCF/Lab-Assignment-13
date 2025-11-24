using UnityEngine;
using GameAnalyticsSDK;

public class GameAnalyticsBootstrap : MonoBehaviour
{
    private void Awake()
    {
        // Initialize GameAnalytics at the very start
        GameAnalytics.Initialize();
    }
}

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using GameAnalyticsSDK; // added for analytics

public class WeatherManager : MonoBehaviour
{
    [Header("Weather Settings")]
    public string city = "Orlando,us";
    public string apiKey = "02e48361c0a96be0784a983f024752d5";

    [Header("Scene References")]
    public Light sunLight;
    public Material clearSkybox;
    public Material cloudySkybox;
    public Material rainySkybox;
    public Material nightSkyBox;

    private const string API =
        "https://api.openweathermap.org/data/2.5/weather?q={0}&appid={1}&units=metric";

    private void Start()
    {
        RefreshWeather(); // call once at start
    }

    public void RefreshWeather()
    {
        StartCoroutine(GetWeatherForCity(city));
    }

    // ---------------- API CALL ---------------------

    IEnumerator GetWeatherForCity(string cityName)
    {
        string url = string.Format(API, cityName, apiKey);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Weather API Error: " + request.error);
                yield break;
            }

            WeatherData data = ParseJSON(request.downloadHandler.text);
            ApplyWeatherToScene(data);
        }
    }

    // ---------------- PARSE JSON ---------------------

    WeatherData ParseJSON(string json)
    {
        WeatherJSON root = JsonUtility.FromJson<WeatherJSON>(json);

        WeatherData data = new WeatherData();
        data.cityName = root.name;
        data.description = root.weather[0].description;

        // city timezone offset (seconds from UTC)
        DateTime utc = DateTime.UtcNow;
        data.localTime = utc.AddSeconds(root.timezone);

        return data;
    }

    [Serializable]
    public class WeatherJSON
    {
        public WeatherItem[] weather;
        public string name;
        public int timezone;
    }

    [Serializable]
    public class WeatherItem
    {
        public string description;
    }

    // ---------------- APPLY WEATHER ---------------------

    void ApplyWeatherToScene(WeatherData data)
    {
        Debug.Log($"Weather: {data.description} | City Local Time: {data.localTime}");

        UpdateSkybox(data.description);
        UpdateLighting(data.description, data.localTime);
        DynamicGI.UpdateEnvironment();

        // --- Analytics ---
        GameAnalytics.NewDesignEvent($"CitySelected:{data.cityName}");
        GameAnalytics.NewDesignEvent($"WeatherApplied:{data.description}");
    }

    // ---------------- SKYBOX ---------------------

    void UpdateSkybox(string desc)
    {
        desc = desc.ToLower();

        if (desc.Contains("rain"))
            RenderSettings.skybox = rainySkybox;
        else if (desc.Contains("cloud"))
            RenderSettings.skybox = cloudySkybox;
        else
            RenderSettings.skybox = clearSkybox;
    }

    // ---------------- LIGHTING ---------------------

    void UpdateLighting(string desc, DateTime localTime)
    {
        int hour = localTime.Hour;
        bool isNight = (hour < 6 || hour > 19);

        if (isNight)
        {
            sunLight.intensity = 0.1f;
            sunLight.color = new Color(0.2f, 0.3f, 0.6f); // night blue tint
            return;
        }

        // Daytime default
        sunLight.intensity = 1.0f;
        sunLight.color = Color.white;

        desc = desc.ToLower();

        if (desc.Contains("rain"))
        {
            sunLight.intensity = 0.3f;
            sunLight.color = new Color(0.7f, 0.7f, 0.8f);
        }
        else if (desc.Contains("cloud"))
        {
            sunLight.intensity = 0.6f;
            sunLight.color = new Color(0.9f, 0.9f, 0.95f);
        }
    }
}
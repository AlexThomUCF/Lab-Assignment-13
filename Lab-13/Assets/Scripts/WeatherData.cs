using System;

[System.Serializable]
public class WeatherData
{
    public string description;   // "clear sky", "rain", "cloudy", etc.
    public float tempC;          // Celsius
    public string cityName;
    public DateTime localTime;   // remote city local time
}

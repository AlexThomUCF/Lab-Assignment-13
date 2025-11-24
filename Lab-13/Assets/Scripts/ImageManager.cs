using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GameAnalyticsSDK; // added for analytics

public class ImageManager : MonoBehaviour
{
    // Add your 3 public domain URLs here:
    public string[] webImages = new string[]
    {
        "https://upload.wikimedia.org/wikipedia/commons/thumb/1/15/Cat_August_2010-4.jpg/2560px-Cat_August_2010-4.jpg",
        "https://upload.wikimedia.org/wikipedia/commons/3/36/Hopetoun_falls.jpg",
        "https://upload.wikimedia.org/wikipedia/commons/0/02/Otter_in_Southwold.jpg"
    };

    // Cache for already-downloaded images
    private Dictionary<string, Texture2D> imageCache = new Dictionary<string, Texture2D>();

    // Called by billboards
    public void GetWebImage(string url, Action<Texture2D> callback)
    {
        if (imageCache.ContainsKey(url))
        {
            Debug.Log($"[CACHE USED] Returning cached image for: {url}");
            callback(imageCache[url]);

            // Analytics
            GameAnalytics.NewDesignEvent($"ImageCached:{url}");
        }
        else
        {
            Debug.Log($"[DOWNLOADING] Fetching new image from: {url}");
            StartCoroutine(DownloadImage(url, callback));
        }
    }

    private IEnumerator DownloadImage(string url, Action<Texture2D> callback)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[ERROR] Failed to download image: {url}\n{request.error}");
            yield break;
        }

        Texture2D tex = DownloadHandlerTexture.GetContent(request);

        // Store in cache for future use
        imageCache[url] = tex;

        Debug.Log($"[DOWNLOADED SUCCESS] Image downloaded and cached: {url}");

        // Analytics
        GameAnalytics.NewDesignEvent($"ImageDownloaded:{url}");

        callback(tex);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using Newtonsoft.Json;
using System;
using UnityEngine.UIElements;
public class AppManager : MonoBehaviour
{
    private const string API_KEY_PREF_KEY = "ApiKey";
    public static AppManager Instance { get; private set; }

    public static event Action OnCurrencyCollectionUpdated;
    public string ApiKey { get; private set; } = string.Empty;
    public Dictionary<string, float> CurrencyCollection { get; private set; } = new();
    public UIDocument UIDocument;
    private string _currencyCollectionSavePath => Path.Combine(Application.persistentDataPath, "currency_collection.json");
    [SerializeField] private string _apiUrl = "https://v6.api.exchangerate-api.com/v6/";
    public string ApiUrl { get => _apiUrl; }
    //===================================================================//
    private void Awake() {
        if (Instance == null) {
            Instance = this;    
        } else {
            Destroy(gameObject);
        }
    }
    private void Start() {
        Init();
    }
    //===================================================================//
    public void SetApiKey(string apiKey) {
        ApiKey = apiKey;
        PlayerPrefs.SetString(API_KEY_PREF_KEY, ApiKey);
        PopulateCurrencyCollection();
    }
    public void Init() {
        Application.targetFrameRate = 60;

        CheckAndPopulateCurrencyCollection();
    }
    private void CheckAndPopulateCurrencyCollection() {
        if (PlayerPrefs.HasKey(API_KEY_PREF_KEY)) {
            ApiKey = PlayerPrefs.GetString(API_KEY_PREF_KEY);
        }
        // Check if the currency collection file exists
        if (File.Exists(_currencyCollectionSavePath)) {
            CurrencyCollection = JsonConvert.DeserializeObject<CurrencyData>(File.ReadAllText(_currencyCollectionSavePath)).conversion_rates;
            OnCurrencyCollectionUpdated?.Invoke();
            print("Currency collection loaded from file.");
        } else {
            if (!string.IsNullOrEmpty(ApiKey)) {
                PopulateCurrencyCollection();
            } else {
                Debug.LogError("API Key is not set. Please set the API Key first.");
            }
        }
    }
    private async Task PopulateCurrencyCollection() {
        CurrencyData currencyData = await GetCurrencyDataAsync(_apiUrl + ApiKey + "/latest/USD");
        if (currencyData != null && currencyData.conversion_rates != null) {
            CurrencyCollection = currencyData.conversion_rates;
            // Save the currency collection to a file
            string json = JsonConvert.SerializeObject(currencyData, Formatting.Indented);
            File.WriteAllText(_currencyCollectionSavePath, json);
            Debug.Log("Currency collection populated successfully.");
            OnCurrencyCollectionUpdated?.Invoke();
        } else {
            Debug.LogError("Failed to populate currency collection.");
        }
    }
    private async Task<CurrencyData> GetCurrencyDataAsync(string url) {
        using (UnityWebRequest request = UnityWebRequest.Get(url)) {
            var operation = request.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();  // Await next frame to avoid blocking

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError) {
                Debug.LogError("Error: " + request.error);
                return null;
            } else {
                return JsonConvert.DeserializeObject<CurrencyData>(request.downloadHandler.text);
            }
        }
    }
}
public class CurrencyData {
    public string result;
    public string documentation;
    public string terms_of_use;
    public int time_last_update_unix;
    public string time_last_update_utc;
    public int time_next_update_unix;
    public string time_next_update_utc;
    public string base_code;
    public Dictionary<string, float> conversion_rates;
}
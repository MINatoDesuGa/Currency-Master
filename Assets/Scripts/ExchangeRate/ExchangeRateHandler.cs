using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
public class ExchangeRateHandler : MonoBehaviour
{
    public static event Action<float> OnAmountConverted;

    private string _currentFromCurrency, _currentToCurrency;
    private float _currentAmount;

    private Dictionary<string, float> _currencyConversionCache = new();
    //===================================================================//
    private void Awake() {
        ExchangeRateUIManager.OnFromCurrencyUpdated += OnFromCurrencyUpdated;
        ExchangeRateUIManager.OnToCurrencyUpdated += OnToCurrencyUpdated;
        ExchangeRateUIManager.OnAmountInputUpdated += OnAmountInputUpdated;
    }
    private void OnDestroy() {
        ExchangeRateUIManager.OnFromCurrencyUpdated -= OnFromCurrencyUpdated;
        ExchangeRateUIManager.OnToCurrencyUpdated -= OnToCurrencyUpdated;
        ExchangeRateUIManager.OnAmountInputUpdated -= OnAmountInputUpdated;
    }
    //===================================================================//
    private void OnFromCurrencyUpdated(string fromCurrency) {
        _currentFromCurrency = fromCurrency;
        OnAmountInputUpdated(_currentAmount); // Recalculate conversion when from currency changes
    }
    private void OnToCurrencyUpdated(string toCurrency) {
        _currentToCurrency = toCurrency;
        OnAmountInputUpdated(_currentAmount); // Recalculate conversion when to currency changes
    }
    private void OnAmountInputUpdated(float amount) {
        _currentAmount = amount;
        if(_currentFromCurrency == _currentToCurrency) {
            OnAmountConverted?.Invoke(amount); // No conversion needed if currencies are the same
            return;
        }
        if (string.IsNullOrEmpty(_currentFromCurrency) || string.IsNullOrEmpty(_currentToCurrency)) {
            Debug.LogWarning("From or To currency is not set.");
            return;
        }
        string cacheKey = $"{_currentFromCurrency}_{_currentToCurrency}";
        if (!_currencyConversionCache.TryGetValue(cacheKey, out float conversionRate)) {
            HandleConversionRate(amount);
        } else {
            UpdateConversionResult(amount, conversionRate);
        }
    }
    private async Task HandleConversionRate(float amount) {
        if (string.IsNullOrEmpty(AppManager.Instance.ApiKey)) {
            Debug.LogError("API Key is not set. Please set the API Key first.");
            return;
        }
        string conversionUrl = $"{AppManager.Instance.ApiUrl}{AppManager.Instance.ApiKey}/pair/{_currentFromCurrency}/{_currentToCurrency}";
        ExchangeRateConversionApiResponse exchangeRateConversionData = await GetExchangeRateAsync(conversionUrl);
        if (exchangeRateConversionData == null) {
            Debug.LogError("Failed to retrieve exchange rate data.");
            return;
        }
        if (exchangeRateConversionData.result != "success") {
            Debug.LogError($"Error in exchange rate conversion: {exchangeRateConversionData.result}");
            return;
        }
        float conversionRate = exchangeRateConversionData.conversion_rate;
        UpdateConversionResult(amount, conversionRate);
        string cacheKey = $"{_currentFromCurrency}_{_currentToCurrency}";
        _currencyConversionCache[cacheKey] = conversionRate;
    }
    private async Task<ExchangeRateConversionApiResponse> GetExchangeRateAsync(string url) {
        using (UnityWebRequest request = UnityWebRequest.Get(url)) {
            var operation = request.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();  // Await next frame to avoid blocking

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError) {
                Debug.LogError("Error: " + request.error);
                return null;
            } else {
                return JsonConvert.DeserializeObject<ExchangeRateConversionApiResponse>(request.downloadHandler.text);
            }
        }
    }
    private void UpdateConversionResult(float amount, float conversionRate) {
        OnAmountConverted?.Invoke(amount * conversionRate);
    }
}
public class ExchangeRateConversionApiResponse {
    public string result;
    public string documentation;
    public string terms_of_use;
    public long time_last_update_unix;
    public string time_last_update_utc;
    public long time_next_update_unix;
    public string time_next_update_utc;
    public string base_code;
    public string target_code;
    public float conversion_rate;
}
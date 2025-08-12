using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
public class ExchangeRateUIManager : MonoBehaviour
{
    public static event Action<string> OnFromCurrencyUpdated;
    public static event Action<string> OnToCurrencyUpdated;
    public static event Action<float> OnAmountInputUpdated;

    private const string EXCHANGE_RATE_PANEL_NAME = "exchangeRatePanel";
    private const string FROM_CURRENCY_DROPDOWN_NAME = "fromCurrencyDropdown";
    private const string TO_CURRENCY_DROPDOWN_NAME = "toCurrencyDropdown";
    private const string AMOUNT_INPUT_NAME = "amountInput";
    private const string EXCHANGE_RATE_RESULT_LABEL_NAME = "conversionResultTxt";
    private const string EXCHANGE_RATE_CLOSE_BUTTON_NAME = "exchangeRateCloseBtn";

    private UIDocument _uiDocument;
    private VisualElement _exchangeRatePanel;
    private DropdownField _fromCurrencyDropdown;
    private DropdownField _toCurrencyDropdown;
    private FloatField _amountInput;
    private Label _exchangeRateResultLabel;
    private Button _exchangeRateCloseButton;
    //==================================================================//
    private void Awake() {
        AppManager.OnCurrencyCollectionUpdated += OnCurrencyCollectionUpdated;
        MenuUiManager.OnExchangeRateButtonClickedEvent += OnExchangeRateButtonClicked;
        ExchangeRateHandler.OnAmountConverted += OnAmountConverted;
        Init();
    }
    private void OnDestroy() {
        AppManager.OnCurrencyCollectionUpdated -= OnCurrencyCollectionUpdated;
        MenuUiManager.OnExchangeRateButtonClickedEvent -= OnExchangeRateButtonClicked;
        ExchangeRateHandler.OnAmountConverted -= OnAmountConverted;
        if (_exchangeRateCloseButton != null) {
            _exchangeRateCloseButton.clicked -= () => _exchangeRatePanel.style.display = DisplayStyle.None;
        }
        if (_fromCurrencyDropdown != null) {
            _fromCurrencyDropdown.UnregisterValueChangedCallback(evt => OnFromCurrencyUpdated?.Invoke(evt.newValue));
        }
        if (_toCurrencyDropdown != null) {
            _toCurrencyDropdown.UnregisterValueChangedCallback(evt => OnToCurrencyUpdated?.Invoke(evt.newValue));
        }
    }
    //==================================================================//
    private void Init() {
        _uiDocument = AppManager.Instance.UIDocument;
        if (_uiDocument == null) {
            Debug.LogError("Exchange Rate UI Document is not assigned in the inspector.");
            return;
        }
        var root = _uiDocument.rootVisualElement;
        _exchangeRatePanel = root.Q<VisualElement>(EXCHANGE_RATE_PANEL_NAME);
        _fromCurrencyDropdown = _exchangeRatePanel.Q<DropdownField>(FROM_CURRENCY_DROPDOWN_NAME);
        _toCurrencyDropdown = _exchangeRatePanel.Q<DropdownField>(TO_CURRENCY_DROPDOWN_NAME);
        _amountInput = _exchangeRatePanel.Q<FloatField>(AMOUNT_INPUT_NAME);
        _amountInput.RegisterValueChangedCallback(evt => {
            // Handle amount input change if needed
            // For example, you can trigger an exchange rate calculation here
            OnAmountInputUpdated?.Invoke(evt.newValue);
        });
        _exchangeRateResultLabel = _exchangeRatePanel.Q<Label>(EXCHANGE_RATE_RESULT_LABEL_NAME);
        _exchangeRateCloseButton = _exchangeRatePanel.Q<Button>(EXCHANGE_RATE_CLOSE_BUTTON_NAME);
        _exchangeRateCloseButton.clicked += () => _exchangeRatePanel.style.display = DisplayStyle.None;
    }
    private void OnCurrencyCollectionUpdated() {
        // update dropdownpanel with currency collection data
        var currencyChoices = AppManager.Instance.CurrencyCollection.Keys.ToList();
        _fromCurrencyDropdown.choices = currencyChoices;
        _fromCurrencyDropdown.value = currencyChoices.FirstOrDefault() ?? string.Empty;
        OnFromCurrencyUpdated?.Invoke(_fromCurrencyDropdown.value);
        _fromCurrencyDropdown.RegisterValueChangedCallback(evt => {
            // Handle currency selection change if needed
            OnFromCurrencyUpdated?.Invoke(evt.newValue);
            
        });
        _toCurrencyDropdown.choices = currencyChoices;
        _toCurrencyDropdown.value = currencyChoices.FirstOrDefault() ?? string.Empty;
        OnToCurrencyUpdated?.Invoke(_toCurrencyDropdown.value);
        _toCurrencyDropdown.RegisterValueChangedCallback(evt => {
            // Handle currency selection change if needed
            OnToCurrencyUpdated?.Invoke(evt.newValue);
        });
    }
    private void OnAmountConverted(float convertedAmount) {
        // Update the exchange rate result label with the converted amount
        _exchangeRateResultLabel.text = $"{convertedAmount}";
    }
    private void OnExchangeRateButtonClicked() {
        _exchangeRatePanel.style.display = DisplayStyle.Flex;
    }

}

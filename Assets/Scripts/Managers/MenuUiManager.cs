using System;
using UnityEngine;
using UnityEngine.UIElements;
public class MenuUiManager : MonoBehaviour
{
    public static event Action OnExchangeRateButtonClickedEvent;

    private const string EXCHANGE_RATE_BUTTON_NAME = "exchangeRateBtn";
    private const string QUIZ_BUTTON_NAME = "quizBtn";
    private const string WIKI_BUTTON_NAME = "wikiBtn";
    private const string ABOUT_BUTTON_NAME = "aboutBtn";
    private const string API_PANEL_NAME = "apiPanel";
    private const string API_BUTTON_NAME = "apiBtn";
    private const string API_CONFIRM_BUTTON_NAME = "apiConfirmBtn";
    private const string API_KEY_INPUT_NAME = "apiKeyInput";
    private const string API_PANEL_CLOSE_BUTTON_NAME = "apiPanelCloseBtn";

    private UIDocument _uiDocument;

    private VisualElement _apiPanel;

    private Button _exchangeRateButton;
    private Button _quizButton;
    private Button _wikiButton;
    private Button _aboutButton;
    private Button _apiButton;
    private Button _apiConfirmButton;
    private Button _apiPanelCloseButton;
    //=================================================================//
    private void Start() {
        Init();
    }
    private void OnDestroy() {
        if (_exchangeRateButton != null) {
            _exchangeRateButton.clicked -= OnExchangeRateButtonClicked;
        }
        if (_quizButton != null) {
            _quizButton.clicked -= OnQuizButtonClicked;
        }
        if (_wikiButton != null) {
            _wikiButton.clicked -= OnWikiButtonClicked;
        }
        if (_aboutButton != null) {
            _aboutButton.clicked -= OnAboutButtonClicked;
        }
    }
    //=================================================================//
    private void Init() {
        _uiDocument = AppManager.Instance.UIDocument;
        if (_uiDocument == null) {
            Debug.LogError("Menu UI Document is not assigned in the inspector.");
            return;
        }
        var root = _uiDocument.rootVisualElement;
        _exchangeRateButton = root.Q<Button>(EXCHANGE_RATE_BUTTON_NAME);
        _quizButton = root.Q<Button>(QUIZ_BUTTON_NAME);
        _wikiButton = root.Q<Button>(WIKI_BUTTON_NAME);
        _aboutButton = root.Q<Button>(ABOUT_BUTTON_NAME);
        _apiButton = root.Q<Button>(API_BUTTON_NAME);
        _apiConfirmButton = root.Q<Button>(API_CONFIRM_BUTTON_NAME);
        _apiPanel = root.Q<VisualElement>(API_PANEL_NAME);
        _apiPanelCloseButton = _apiPanel.Q<Button>(API_PANEL_CLOSE_BUTTON_NAME);

        if (_exchangeRateButton == null || _quizButton == null || _wikiButton == null || _aboutButton == null) {
            Debug.LogError("One or more menu buttons are not found in the UI Document.");
            return;
        }

        _exchangeRateButton.clicked += OnExchangeRateButtonClicked;
        _quizButton.clicked += OnQuizButtonClicked;
        _wikiButton.clicked += OnWikiButtonClicked;
        _aboutButton.clicked += OnAboutButtonClicked;
        _apiButton.clicked += OnApiButtonClicked;
        _apiConfirmButton.clicked += OnApiConfirmButtonClicked;
        _apiPanelCloseButton.clicked += () => ActivateApiPanel(false);
    }
    private void OnApiConfirmButtonClicked() {
        ActivateApiPanel(false);
        // Here you can add logic to handle the API confirmation, such as saving settings or making an API call.
        AppManager.Instance.SetApiKey(_apiPanel.Q<TextField>(API_KEY_INPUT_NAME).value);
    }
    private void OnApiButtonClicked() {
        ActivateApiPanel(true);
    }
    private void OnAboutButtonClicked() {
    }
    private void OnWikiButtonClicked() {
    }
    private void OnQuizButtonClicked() {
    }
    private void OnExchangeRateButtonClicked() {
        OnExchangeRateButtonClickedEvent?.Invoke();
    }
    private void ActivateApiPanel(bool activate) {
        if (_apiPanel != null) {
            _apiPanel.style.display = activate ? DisplayStyle.Flex : DisplayStyle.None;
        } else {
            Debug.LogError("API Panel is not found in the UI Document.");
        }
    }
}

using System;
using UnityEngine;
using UnityEngine.UIElements;
public class MenuUiManager : MonoBehaviour
{
    private const string EXCHANGE_RATE_BUTTON_NAME = "exchangeRateBtn";
    private const string QUIZ_BUTTON_NAME = "quizBtn";
    private const string WIKI_BUTTON_NAME = "wikiBtn";
    private const string ABOUT_BUTTON_NAME = "aboutBtn";

    [Header("Menu UI Settings")]
    [SerializeField] private UIDocument _menuUiDocument;
    [Space(10)]
    [Header("Menu UI Elements")]
    [SerializeField] private Button _exchangeRateButton;
    [SerializeField] private Button _quizButton;
    [SerializeField] private Button _wikiButton;
    [SerializeField] private Button _aboutButton;
    //=================================================================//
    private void Awake() {
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
        if (_menuUiDocument == null) {
            Debug.LogError("Menu UI Document is not assigned in the inspector.");
            return;
        }
        var root = _menuUiDocument.rootVisualElement;
        _exchangeRateButton = root.Q<Button>(EXCHANGE_RATE_BUTTON_NAME);
        _quizButton = root.Q<Button>(QUIZ_BUTTON_NAME);
        _wikiButton = root.Q<Button>(WIKI_BUTTON_NAME);
        _aboutButton = root.Q<Button>(ABOUT_BUTTON_NAME);

        if (_exchangeRateButton == null || _quizButton == null || _wikiButton == null || _aboutButton == null) {
            Debug.LogError("One or more menu buttons are not found in the UI Document.");
            return;
        }

        _exchangeRateButton.clicked += OnExchangeRateButtonClicked;
        _quizButton.clicked += OnQuizButtonClicked;
        _wikiButton.clicked += OnWikiButtonClicked;
        _aboutButton.clicked += OnAboutButtonClicked;
    }
    private void OnAboutButtonClicked() {
        print("About button clicked!");
    }
    private void OnWikiButtonClicked() {
        print("Wiki button clicked!");
    }
    private void OnQuizButtonClicked() {
        print("Quiz button clicked!");
    }
    private void OnExchangeRateButtonClicked() {
        print("Exchange Rate button clicked!");
    }
}

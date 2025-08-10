using UnityEngine;
public class AppManager : MonoBehaviour
{
    public static AppManager Instance { get; private set; }
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            Init();
            
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
    private void Init() {
        Application.targetFrameRate = 60;
    }
}

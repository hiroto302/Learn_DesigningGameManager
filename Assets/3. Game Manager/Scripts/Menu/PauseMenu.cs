using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button ResumeButton;
    [SerializeField] private Button RestartButton;
    [SerializeField] private Button QuitButton;

    void Start()
    {
        ResumeButton.onClick.AddListener(HandleResumeClicked);
        RestartButton.onClick.AddListener(HandleRestartClicked);
        QuitButton.onClick.AddListener(HandleQuitClicked);
    }

    // 各ボタンを押した時のイベントが発生する。その時,処理する内容を記述。
    void HandleResumeClicked()
    {
        GameManager.Instance.TogglePause();
    }
    void HandleRestartClicked()
    {
        GameManager.Instance.RestartGame();
    }
    void HandleQuitClicked()
    {
        GameManager.Instance.QuitGame();
    }
}

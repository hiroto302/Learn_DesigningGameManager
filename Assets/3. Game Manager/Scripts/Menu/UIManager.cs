using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private MainMenu _mainMenu;
    [SerializeField] private PauseMenu _pauseMenu;
    [SerializeField] private Camera _dummyCamera;  // boot Scene を表示してるカメラ

    public Events.EventFadeComplete OnMainMenuFadeComplete;

    private void Start()
    {
        _mainMenu.onMainMenuFadeComplete.AddListener(HandleMainMenuFadeComplete);
        GameManager.Instance.OnGameStateChange.AddListener(HandleGameStateChange);
    }

    void HandleMainMenuFadeComplete(bool fadeOut)
    {
        OnMainMenuFadeComplete.Invoke(fadeOut);
    }

    void HandleGameStateChange(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        _pauseMenu.gameObject.SetActive(currentState == GameManager.GameState.PAUSED);  //    Trigger method via pause menu
    }

    private void Update()
    {
        // Gameの状態がPREGAME(初期の画面)の時のみ, StartGame()を行い, Main Scene に移行できるように制限する
        if(GameManager.Instance.CurrentGameState != GameManager.GameState.PREGAME)
        {
            return;
        }

        // Game開始 : MainScene の Load
        if(Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.StartGame();
            // _mainMenu.FadeOut();
        }
    }

    // Dummy カメラの Off/On
    public void SetDummyCameraActive(bool active)
    {
        _dummyCamera.gameObject.SetActive(active);
    }
}

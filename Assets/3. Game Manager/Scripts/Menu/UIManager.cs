using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ユーザーインターフェースの制御
public class UIManager : Singleton<UIManager>
{
    [SerializeField] private MainMenu _mainMenu;   // MainMenu を制御
    [SerializeField] private PauseMenu _pauseMenu; // PauseMenu を制御
    [SerializeField] private Camera _dummyCamera;  // boot Scene を表示してるカメラ

    public Events.EventFadeComplete OnMainMenuFadeComplete;

    private void Start()
    {
        _mainMenu.onMainMenuFadeComplete.AddListener(HandleMainMenuFadeComplete);  // MainMenu より event の発生が知らされた時、実行する処理を追加

        // // Listener : GameManagerにより、Gameの状態が更新されたことを聞いた時に応答する処理を追加
        GameManager.Instance.OnGameStateChange.AddListener(HandleGameStateChange);
    }

    void HandleMainMenuFadeComplete(bool fadeOut)
    {
        OnMainMenuFadeComplete.Invoke(fadeOut);     /* MainMenu より event 発生 => UIManagerが聞き取る => UIManager が GameManager Mainmenuで eventが発生したことを知らせる
                                                    (UIManagerが MainMenu と GameManagerを仲介してる)*/
    }

    void HandleGameStateChange(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        // GameManager の状態が PAUSED に変わった時 PauseMenu を表示, PAUSED以外の状態に変わった時 PauseMenu を非表示
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

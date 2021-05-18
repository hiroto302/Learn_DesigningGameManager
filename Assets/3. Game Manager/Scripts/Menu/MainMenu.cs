using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // Track the Animation Component
    // Track the AnimationClips for fade in/out
    // Function that can receive animation events
    // Functions to play fade in/out animations

    [SerializeField] private Animation _mainMenuAnimator;
    [SerializeField] private AnimationClip _fadeOutAnimation;
    [SerializeField] private AnimationClip _fadeInAnimation;

    public Events.EventFadeComplete onMainMenuFadeComplete;  // Fadeが完了した時の event

    private void Start()
    {
        GameManager.Instance.OnGameStateChange.AddListener(HandleGameStateChange);  // Gameの状態が更新される度に発生する event に処理を追加
    }

    // アニメーションイベント : Object のスクリプト内の関数をタイムラインの特定のタイミングで呼びですことができる
    // MainMenuFadeOutのアニメーションイベントで呼び出される関数
    public void OnFadeOutComplete()
    {
        // Debug.LogWarning("FadeOut Complete");
        onMainMenuFadeComplete.Invoke(true);        // event 発生 => UIManagerに知らせる
    }
    // MainMenuFadeInのアニメーションイベントで呼び出される関数
    public void OnFadeInComplete()
    {
        // Debug.LogWarning("FadeIn Complete");
        UIManager.Instance.SetDummyCameraActive(true);  // DummyCameraを再表示
        onMainMenuFadeComplete.Invoke(false);
    }

    // Gameの状態が更新された時に呼ばれるメソッド
    void HandleGameStateChange(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        if (previousState == GameManager.GameState.PREGAME && currentState == GameManager.GameState.RUNNING)
        {
            FadeOut();
        }

        // Pause 画面より、リスタートボタンが押された時、下記の処理が実行される
        if (previousState != GameManager.GameState.PREGAME && currentState == GameManager.GameState.PREGAME)
        {
            FadeIn();
        }
    }

    public void FadeIn()
    {
        _mainMenuAnimator.Stop();
        _mainMenuAnimator.clip = _fadeInAnimation;
        _mainMenuAnimator.Play();
    }
    public void FadeOut()
    {
        UIManager.Instance.SetDummyCameraActive(false); // DummyCamera を非表示

        _mainMenuAnimator.Stop();
        _mainMenuAnimator.clip = _fadeOutAnimation;
        _mainMenuAnimator.Play();
    }
}

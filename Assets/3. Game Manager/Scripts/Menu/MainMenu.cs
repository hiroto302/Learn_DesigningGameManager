﻿using System.Collections;
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

    public Events.EventFadeComplete onMainMenuFadeComplete;

    private void Start()
    {
        GameManager.Instance.OnGameStateChange.AddListener(HandleGameStateChange);
    }

    // アニメーションイベント : Object のスクリプト内の関数をタイムラインの特定のタイミングで呼びですことができる
    // MainMenuFadeOutのアニメーションイベントで呼び出される関数
    public void OnFadeOutComplete()
    {
        // Debug.LogWarning("FadeOut Complete");
        onMainMenuFadeComplete.Invoke(true);
    }
    // MainMenuFadeInのアニメーションイベントで呼び出される関数
    public void OnFadeInComplete()
    {
        // Debug.LogWarning("FadeIn Complete");
        UIManager.Instance.SetDummyCameraActive(true);  // DummyCameraを再表示
        onMainMenuFadeComplete.Invoke(false);
    }

    void HandleGameStateChange(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        if (previousState == GameManager.GameState.PREGAME && currentState == GameManager.GameState.RUNNING)
        {
            FadeOut();
        }

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

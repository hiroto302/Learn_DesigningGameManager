using UnityEngine.Events;

// Event を管理するクラス
public class Events
{
    // Fade In(false) or Out(true) が完了した時に発生するイベント
    [System.Serializable] public class EventFadeComplete : UnityEvent<bool>{};

    // GameManager の State が更新された時に発生する event
    [System.Serializable] public class EventGamState : UnityEvent<GameManager.GameState, GameManager.GameState>{}

}
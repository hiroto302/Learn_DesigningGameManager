using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.Events;
using UnityEngine.SceneManagement;

// 下記の記述は、Event を管理するクラス Events に移行
// [System.Serializable] public class EventGamState : UnityEvent<GameManager.GameState, GameManager.GameState>{} Gameの状態が更新された時に発生する event

/*ToDos: step 2
    Add a method to enter/exit pause
    Trigger method via 'escape' key
    Trigger method via pause menu
    Pause simulation when in pause state
    Modify cursor to use pointer when in pause state
*/

// Built a globally accessible Game Manager & SceneManagement & Managed Game State
// 常にゲーム全般を管理する 「Gamemanager Object」 は最初のシーン（ここでは Boot)でインスタンス化されるように設計している。
// この GameManager と同様に Singleton を継承したクラスのオブジェクトを、この子オブジェクトして最初にインスタンス化することで、シーン間で容易にアクセスできるようにする
public class GameManager : Singleton<GameManager>
{
/*ToDos: step 1
    // what level the game is current in
    // load and Unload game levels
    // keep track of the game state
    // generate other persisten system
*/

    // ゲームの状態
    public enum GameState
    {
        PREGAME,
        RUNNING,
        PAUSED
    }

    public GameObject[] SystemPrefabs;     // GameManager が生成する他の Systemクラス(singleton) を格納
    public Events.EventGamState OnGameStateChange;
    private List<GameObject> _instancedSystemPrefabs;
    GameState _currentGameState = GameState.PREGAME;    // 現在のゲームの状態 : 初期値 PREGAME
    private string _currentLevelName = string.Empty;    // ロードする Scene名
    List<AsyncOperation> _loadOperations;  // Load 時に行う AsyncOperetion を格納

    public GameState CurrentGameState
    {
        get { return _currentGameState; }
        private set { _currentGameState = value; }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);  // Gamemanager を常に存在させる

        // List の初期化は忘れずに
        _loadOperations = new List<AsyncOperation>();

        _instancedSystemPrefabs = new List<GameObject>();


        InstantiateSystemPrefabs(); // 他のシステムクラスの生成

        // MainMenuのfade処理が完了したことを、UIManager が知らせに来た時に行う処理
        UIManager.Instance.OnMainMenuFadeComplete.AddListener(HandleMainMenuFadeComplete);

        // LoadLevel("Main"); メインシーンのロード
    }

    private void Update()
    {
        // PREGAMEの状態でなければPause 画面に Escape key で移行可能
        if(CurrentGameState == GameManager.GameState.PREGAME)
        {
            return;
        }

        // Trigger method via 'escape' key
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // シーンのロード完了時に行う処理
    void OnLoadOperationComplete(AsyncOperation ao)
    {
        if(_loadOperations.Contains(ao))
        {
            // 処理が完了したので、参照を取り除く : メモリーリーク対策
            _loadOperations.Remove(ao);

            // ゲームの状態を RUNNING 更新
            if(_loadOperations.Count == 0)
            {
                UpdateState(GameState.RUNNING);
            }

        }

        Debug.Log("Load Complete.");
    }
    // シーンのアンロード完了時に行う処理
    void OnUnloadOperationComplete(AsyncOperation ao)
    {
        Debug.Log("Unload Complete.");
    }

    // Fade処理が完了した時に行う処理
    void HandleMainMenuFadeComplete(bool fadeOut)
    {
        // FadeInした時
        if(!fadeOut)
        {
            // 現在の読み込んでいたSceneを取り除く
            UnloadLevel(_currentLevelName);
        }
    }

    // GameManagerのState(状態)を更新
    void UpdateState(GameState state)
    {
        GameState previousGameState = _currentGameState; // ロードする前のGameの状態
        _currentGameState = state;                       // ロードした後のGameの状態

        switch (_currentGameState)
        {
            case GameState.PREGAME:
                Time.timeScale = 1.0f;
                break;
            case GameState.RUNNING:
                Time.timeScale = 1.0f;
                break;
            case GameState.PAUSED:
                Time.timeScale = 0f;     // Pause simulation when in pause state
                break;
            default:
                break;
        }

        OnGameStateChange.Invoke(_currentGameState, previousGameState);     // Gameの状態が更新された時の event 発生

        // dispatch message
        // transition between scenes
    }

    // SystemPrefab の生成
    void InstantiateSystemPrefabs()
    {
        GameObject prefabInstance;
        for(int i = 0; i < SystemPrefabs.Length; ++i)
        {
            // 生成
            prefabInstance = Instantiate(SystemPrefabs[i]);
            // 生成したものを追跡・把握するために List に格納
            _instancedSystemPrefabs.Add(prefabInstance);
        }
    }

    // シーンのロード
    public void LoadLevel(string levelName)
    {
        // シーンを非同期でロードし、既存のシーンに追加する（複数のシーンを同時に読み込む）戻り値は, AsyncOperation
        // 非同期操作(ここではロード)が終了したかを判別するには、AsyncOperation を使用
        AsyncOperation ao = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        if(ao == null)
        // levelName のシーンをロード可能かチェック
        {
            Debug.LogError("[GameManager] Unable to load level" + levelName);
            return;
        }
        // AsyncOperation.completed : 操作完了時に起動されるイベント
        ao.completed += OnLoadOperationComplete;
        // ロード時に行う処理を追加
        _loadOperations.Add(ao);

        _currentLevelName = levelName;
    }

    // シーンのアンロード
    public void UnloadLevel(string levelName)
    {
        AsyncOperation ao =  SceneManager.UnloadSceneAsync(levelName);
        if(ao == null)
        {
            Debug.LogError("[GameManager] Unable to unload level" + levelName);
            return;
        }
        ao.completed += OnUnloadOperationComplete;
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();
        for (int i = 0; i < _instancedSystemPrefabs.Count; i++)
        {
            Destroy(_instancedSystemPrefabs[i]);    // Destory
        }
        _instancedSystemPrefabs.Clear();            // Destory したら参照を null にすることを忘れずに (List は参照型) : Clean Up って言ってた
    }

    // Main Secene をロードするメソッド : UIManager で使用
    public void StartGame()
    {
        LoadLevel("Main");
    }

    //  Add a method to enter/exit pause  : Pause Menu へRUNNINGの状態の時のみ移行
    public void TogglePause()
    {
        // if (_currentGameState == GameState.RUNNING)
        // {
        //     UpdateState(GameState.PAUSED);
        // }
        // else
        // {
        //     UpdateState(GameState.RUNNING);
        // }

        // condition ? true : false 上記のコードを一行で書くことができる
        UpdateState(_currentGameState == GameState.RUNNING ? GameState.PAUSED : GameState.RUNNING);
    }

    // 最初のゲーム画面に戻って再スタート
    public void RestartGame()
    {
        UpdateState(GameState.PREGAME);
    }

    // ゲーム終了
    public void QuitGame()
    {
        // implement features for quitting
        Application.Quit();
    }
}



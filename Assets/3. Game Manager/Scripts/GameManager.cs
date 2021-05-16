using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.Events;
using UnityEngine.SceneManagement;

// [System.Serializable] public class EventGamState : UnityEvent<GameManager.GameState, GameManager.GameState>{}

/*ToDos:
    Add a method to enter/exit pause
    Trigger method via 'escape' key
    Trigger method via pause menu
    Pause simulation when in pause state
    Modify cursor to use pointer when in pause state
*/

public class GameManager : Singleton<GameManager>
{
    // what level the game is current in
    // load and Unload game levels
    // keep track of the game state
    // generate other persisten system

    public enum GameState
    {
        PREGAME,
        RUNNING,
        PAUSED
    }

    public GameObject[] SystemPrefabs;
    public Events.EventGamState OnGameStateChange;
    private List<GameObject> _instancedSystemPrefabs;
    GameState _currentGameState = GameState.PREGAME;
    private string _currentLevelName = string.Empty;
    List<AsyncOperation> _loadOperations;

    public GameState CurrentGameState
    {
        get { return _currentGameState; }
        private set { _currentGameState = value; }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        _loadOperations = new List<AsyncOperation>();

        _instancedSystemPrefabs = new List<GameObject>();


        InstantiateSystemPrefabs();

        UIManager.Instance.OnMainMenuFadeComplete.AddListener(HandleMainMenuFadeComplete);

        // LoadLevel("Main");
    }

    private void Update()
    {
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

    void OnLoadOperationComplete(AsyncOperation ao)
    {
        if(_loadOperations.Contains(ao))
        {
            _loadOperations.Remove(ao);

            if(_loadOperations.Count == 0)
            {
                UpdateState(GameState.RUNNING);
            }

            // dispatch message
            // transition between scenes
        }

        Debug.Log("Load Complete.");
    }
    void OnUnloadOperationComplete(AsyncOperation ao)
    {
        Debug.Log("Unload Complete.");
    }

    void HandleMainMenuFadeComplete(bool fadeOut)
    {
        if(!fadeOut)
        {
            UnloadLevel(_currentLevelName);
        }
    }

    void UpdateState(GameState state)
    {
        GameState previousGameState = _currentGameState;
        _currentGameState = state;

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

        OnGameStateChange.Invoke(_currentGameState, previousGameState);
    }

    void InstantiateSystemPrefabs()
    {
        GameObject prefabInstance;
        for(int i = 0; i < SystemPrefabs.Length; ++i)
        {
            prefabInstance = Instantiate(SystemPrefabs[i]);
            _instancedSystemPrefabs.Add(prefabInstance);
        }

        // foreach (var obj in SystemPrefabs)
        // {
        //     GameObject prefabInstance = Instantiate(obj);
        //     _instancedSystemPrefabs.Add(prefabInstance);
        // }
    }

    public void LoadLevel(string levelName)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        if(ao == null)
        {
            Debug.LogError("[GameManager] Unable to load level" + levelName);
            return;
        }
        ao.completed += OnLoadOperationComplete;
        _loadOperations.Add(ao);

        _currentLevelName = levelName;
    }

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
            Destroy(_instancedSystemPrefabs[i]);
        }
        // foreach(var prefab in _instancedSystemPrefabs)
        // {
        //     Destroy(prefab);
        // }
        _instancedSystemPrefabs.Clear();
    }

    public void StartGame()
    {
        LoadLevel("Main");
    }

    //  Add a method to enter/exit pause
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

    public void RestartGame()
    {
        UpdateState(GameState.PREGAME);
    }

    public void QuitGame()
    {
        // implement features for quitting
        Application.Quit();
    }
}



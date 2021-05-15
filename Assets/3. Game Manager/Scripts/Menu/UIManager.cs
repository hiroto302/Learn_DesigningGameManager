using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private MainMenu _mainMenu;
    [SerializeField] private Camera _dummyCamera;

    private void Update()
    {
        if(GameManager.Instance.CurrentGameState != GameManager.GameState.PREGAME)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.StartGame();
            // _mainMenu.FadeOut();
        }
    }

    public void SetDummyCameraActive(bool active)
    {
        _dummyCamera.gameObject.SetActive(active);
    }
}

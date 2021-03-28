using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridSpace : MonoBehaviour
{
    public Button button;
    public Text buttonText;

    private GameController gameController;
    private int index = 0;

    public void SetGameControllerReferenceAndIndex(GameController _gameController, int _index)
    {
        gameController = _gameController;
        index = _index;
    }
    public void SetSpace()
    {
        buttonText.text = gameController.GetPlayerSide();
        button.interactable = false;
        gameController.EndTurn(index);
    }


}

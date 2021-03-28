using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Player
{
    public Image panel;
    public Text text;
    public Button button;

}
[System.Serializable]
public class PlayerColor
{
    public Color panelColor;
    public Color textColor;
}
public class GameController : MonoBehaviour
{
    public Text[] buttonTextList;
    public Button[] buttonList;
    public GameObject gameOverPanel;
    public Text gameOverText;
    public GameObject restartButton;

    private string playSide;
    private string playerSide;
    private string AIside;
    private State state;

    public Player playerX;
    public Player playerO;
    public PlayerColor activePlayerColor;
    public PlayerColor inactivePlayerColor;
    public GameObject startInfo;

    void Awake()
    {
        SetGameControllerRefrenceOnButtons();
        gameOverPanel.SetActive(false);
        restartButton.SetActive(false);
    }
    void SetGameControllerRefrenceOnButtons()
    {
        for (int i = 0; i < buttonTextList.Length; i++)
        {
            buttonTextList[i].GetComponentInParent<GridSpace>().SetGameControllerReferenceAndIndex(this, i);
        }
    }

    public void SetStartingSide(string startingSide)
    {
        playSide = startingSide;
        playerSide = startingSide;
        if (playerSide == "X") AIside = "O";
        else AIside = "X";

        if(playSide == "X")
        {
            SetPlayerColors(playerX, playerO);
        }
        else
        {
            SetPlayerColors(playerO, playerX);
        }
        StartGame();
    }
    void StartGame()
    {
        SetBoardInteractable(true);
        SetPlayerButtons(false);
        startInfo.SetActive(false);
        state = new State(null, null);
    }

    public string GetPlayerSide()
    {
        return playSide;
    }
    public void EndTurn(int cur_action)
    {
        

        state = state.GetNextState(cur_action);
        state.PrintActions();
        Debug.Log("Player Action : " + cur_action);

        if (state.is_done())
        {
            if (!state.is_draw()) GameOver(playerSide);
            else GameOver("draw");
            return;
        }

            ChangeSides();

        //int _action = AISimulator.simulate_mini_max(state);
        int _action = AISimulator.simulate_random(state);
        buttonTextList[_action].text = AIside;
        buttonList[_action].interactable = false;
        
        state = state.GetNextState(_action);
        state.PrintActions();
        Debug.Log("AI Action : " + _action);

        if (state.is_done())
        {
            if (!state.is_draw()) GameOver(AIside);
            else GameOver("draw");
        }
        ChangeSides();

    }
    void ChangeSides()
    {
        playSide = (playSide == "X") ? "O" : "X";
        if(playSide == "X")
        {
            SetPlayerColors(playerX, playerO);
        }
        else
        {
            SetPlayerColors(playerO, playerX);
        }
    }

    void SetPlayerColors(Player newPlayer, Player oldPlayer)
    {
        newPlayer.panel.color = activePlayerColor.panelColor;
        newPlayer.text.color = activePlayerColor.textColor;
        oldPlayer.panel.color = inactivePlayerColor.panelColor;
        oldPlayer.text.color = inactivePlayerColor.textColor;

    }
    void GameOver(string winningPlayer)
    {
        SetBoardInteractable(false);
        if (winningPlayer == "draw")
        {
            SetGameOverText("비겼습니다.");
            SetPlayerColorsInactive();
        }
        else {
            SetGameOverText(winningPlayer + "의 승리~!");
        }
        restartButton.SetActive(true);
    }
    void SetGameOverText(string value)
    {
        gameOverPanel.SetActive(true);
        gameOverText.text = value;
    }
    public void RestartGame()
    {
        gameOverPanel.SetActive(false);
        restartButton.SetActive(false);
        SetPlayerButtons(true);
        SetPlayerColorsInactive();
        startInfo.SetActive(true);

        state = new State(null, null);

        for (int i = 0; i < buttonTextList.Length; i++)
        {
            buttonTextList[i].text = "";
        }
    }
    void SetBoardInteractable(bool is_toggle)
    {
        for(int i = 0; i < buttonTextList.Length; i++)
        {
            buttonTextList[i].GetComponentInParent<Button>().interactable = is_toggle;
        }
    }

    void SetPlayerButtons(bool toggle)
    {
        playerX.button.interactable = toggle;
        playerO.button.interactable = toggle;

    }

    void SetPlayerColorsInactive()
    {
        playerX.panel.color = inactivePlayerColor.panelColor;
        playerX.text.color = inactivePlayerColor.textColor;
        playerO.panel.color = inactivePlayerColor.panelColor;
        playerO.text.color = inactivePlayerColor.textColor;
    }


}

public class State
{
    public List<int> vaild_actions;
    private int[] pieces;
    private int[] enemy_pieces;
    public State(int[] pieces, int[] enemy_pieces)
    {
        if (pieces == null)
        {
            this.pieces = new int[9];
            for (int i = 0; i < 9; i++)
            {
                this.pieces[i] = 0;
            }
        }
        else
        {
            this.pieces = pieces;
        }
        if (enemy_pieces == null)
        {
            this.enemy_pieces = new int[9];
            for (int i = 0; i < 9; i++)
            {
                this.enemy_pieces[i] = 0;
            }
        }
        else
        {
            this.enemy_pieces = enemy_pieces;
        }
    }
    public int GetPieceCount(int[] _pieces)
    {
        int _count = 0;
        foreach (int i in _pieces)
        {
            if (i == 1) _count += 1;
        }
        return _count;
    }
    private bool is_comp(int x, int y, int dx, int dy)
    {
        for (int k = 0; k < 3; k++)
        {
            if (y < 0 || 2 < y || x < 0 || 2 < x || enemy_pieces[x + y * 3] == 0)
            {
                return false;
            }
            x = x + dx;
            y = y + dy;
        }
        return true;
    }
    public bool is_lose()
    {
        if (is_comp(0, 0, 1, 1) || is_comp(0, 2, 1, -1)) return true;
        for (int i = 0; i < 3; i++)
        {
            if (is_comp(0, i, 1, 0) || is_comp(i, 0, 0, 1)) return true;
        }
        return false;
    }
    public bool is_draw()
    {
        return GetPieceCount(pieces) + GetPieceCount(enemy_pieces) == 9;
    }

    public bool is_done()
    {
        return is_lose() || is_draw();
    }



    public State GetNextState(int act_idx)
    {
        int[] _pieces = new int[pieces.Length];
        for(int i = 0; i < _pieces.Length; i++)
        {
            _pieces[i] = pieces[i];
        }
        _pieces[act_idx] = 1;
        State _state = new State(enemy_pieces, _pieces);
        return _state;
    }

    public int[] GetLegalActions()
    {
        List<int> _actions = new List<int>();

        for (int i = 0; i < 9; i++)
        {
            if (pieces[i] == 0 && enemy_pieces[i] == 0)
            {
                _actions.Add(i);
            }
        }
        int[] arr = _actions.ToArray();
        return arr;
    }

    public void PrintActions()
    {
        string act_string = "Actions : {";
        foreach(int i in GetLegalActions())
        {
            act_string = act_string + i.ToString() + ",";
        }
        act_string += "}";

        Debug.Log(act_string);
    }

}

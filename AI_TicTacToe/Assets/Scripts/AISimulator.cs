using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AISimulator
{
    static int mini_max(State _state)
    {
        if (_state.is_lose()) return -1;

        if (_state.is_draw()) return 0;
        int best_score = -100000;

        foreach (int action in _state.GetLegalActions())
        {
            State tmp_state = _state.GetNextState(action);
            int tmp_score = -mini_max(tmp_state);
            if (tmp_score > best_score) best_score = tmp_score;
        }
        return best_score;
    }
    public static int simulate_mini_max(State _state)
    {
        int best_score = -100000;
        int best_action_index = 0;
        foreach (int action in _state.GetLegalActions())
        {
            State tmp_state = _state.GetNextState(action);
            int tmp_score = -mini_max(tmp_state);
            if (tmp_score > best_score)
            {
                best_score = tmp_score;
                best_action_index = action;
            }
        }

        return best_action_index;
    }
    public static int simulate_random(State _state)
    {
        int[] legal_actions = _state.GetLegalActions();
        int random = UnityEngine.Random.Range(0, legal_actions.Length - 1);
        return legal_actions[random];
    }
    public static int simulate_alpha_beta(int _alpha, int _beta, State _state)
    {
        return 0;
    }
}

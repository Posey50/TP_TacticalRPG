using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActiveState : MonoBehaviour, IState
{
    private PlayerStateMachine _state;

    public void OnEnter(PlayerStateMachine playerStateMachine)
    {
        Debug.Log("Player Enters Active State");

        _state = playerStateMachine;
        _state.Main.Pointer.enabled = true;
        _state.Main.Pointer.CursorPress += OnCursorPress;  
    }

    public void OnExit(PlayerStateMachine playerStateMachine)
    {
        Debug.Log("Player Exits Active State");
        _state.Main.Pointer.enabled = false;
        _state.Main.Pointer.CursorPress -= OnCursorPress;
    }

    private void OnCursorPress(Square selectedSquare)
    {
        _state.Main.Move(selectedSquare);
    }
}

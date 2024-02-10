using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActiveState : MonoBehaviour, IState
{
    private PlayerStateMachine _playerStateMachine;
    [SerializeField]
    private Spells _selectedSpells;

    [SerializeField]
    private bool _hasSpellSelected; //TODO : remove 

    public void OnEnter(PlayerStateMachine playerStateMachine)
    {
        Debug.Log("Player Enters Active State");

        _playerStateMachine = playerStateMachine;
        _playerStateMachine.Main.PlayerInput.onActionTriggered += OnAction;
    }

    public void OnExit(PlayerStateMachine playerStateMachine)
    {
        Debug.Log("Player Exits Active State");

        _playerStateMachine.Main.PlayerInput.onActionTriggered -= OnAction;
    }

    private void OnAction(InputAction.CallbackContext context)
    {
        switch (context.action.name)
        {
            case "CursorMove":

                _playerStateMachine.Main.Pointer.SetCurrentSquare(_playerStateMachine.Main.Pointer.GetSquareUnderPosition(context.action.ReadValue<Vector2>()));    //Sets the pointer's current Square to the square below the mouse

                _playerStateMachine.Main.Pointer.UpdateSelectedSquare(); 
                
                break;

            case "CursorPress":
                if (context.started)    //Only on the press of the button
                {
                    if (_playerStateMachine.Main.Pointer.CurrentSquareIsSelectedSquare())
                    {
                        OnCursorPress(_playerStateMachine.Main.Pointer.selectedSquare);
                    }
                }

                break;
        }
    }

    public void SelectSpellByIndex(int index)
    {
        if (_selectedSpells == null)
        {
            //_selectedSpells = spell;
        }
    }

    public void CancelSpell()
    {
        if (_selectedSpells != null)
        {
            _selectedSpells = null;
        }
    }

    /// <summary>
    /// Called when the cursor is pressed. Will either move or attack depending of if a spall has been selected
    /// </summary>
    /// <param name="selectedSquare"></param>
    private void OnCursorPress(Square selectedSquare)
    {
        if (!_hasSpellSelected)  //TODO : _selectedSpells == null
        {
            if (_playerStateMachine.Main.Pointer.path.Count - 1 <= _playerStateMachine.Main.MPs)    //If the player has enough MPs. "- 1" ignores the Square the player is currently standing on
            {
                _playerStateMachine.Main.Move(selectedSquare);

                _playerStateMachine.Main.DecreasePM(_playerStateMachine.Main.Pointer.path.Count - 1);
            }
            else
            {
                Debug.Log($"Not enough MPs to Move {_playerStateMachine.Main.MPs}, needs {_playerStateMachine.Main.Pointer.path.Count - 1}");
            }
        }
        else
        {
            _playerStateMachine.Main.Attack(selectedSquare, _selectedSpells);
        }
    }
}

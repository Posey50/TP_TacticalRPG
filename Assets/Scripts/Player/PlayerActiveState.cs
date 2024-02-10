using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActiveState : MonoBehaviour, IState
{
    private PlayerStateMachine _playerStateMachine;
    
    private int _selectedSpellIndex = -1;
    private SpellsData _selectedSpellData;  

    /// <summary>
    /// Properly sets up the State before its use
    /// </summary>
    /// <param name="playerStateMachine"></param>
    public void OnEnter(PlayerStateMachine playerStateMachine)
    {
        Debug.Log("Player Enters Active State");

        _playerStateMachine = playerStateMachine;
        _playerStateMachine.Main.PlayerInput.onActionTriggered += OnAction;
    }

    /// <summary>
    /// Properly cleans up the State after its use
    /// </summary>
    /// <param name="playerStateMachine"></param>
    public void OnExit(PlayerStateMachine playerStateMachine)
    {
        Debug.Log("Player Exits Active State");

        _playerStateMachine.Main.PlayerInput.onActionTriggered -= OnAction;
    }

    /// <summary>
    /// Called when the player does an Imput
    /// </summary>
    /// <param name="context"></param>
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

    /// <summary>
    /// Selects the index of the Entity's spell list
    /// </summary>
    /// <param name="index"></param>
    public void SelectSpellByIndex(int index)
    {
        if (0 <= index && index < _playerStateMachine.Main.Spells.Count)
        {
            _selectedSpellIndex = index;
        }
    }

    /// <summary>
    /// Unselects a spell
    /// </summary>
    public void CancelSpell()
    {
        if (_selectedSpellIndex >= 0)
        {
            _selectedSpellIndex = -1;
        }
    }

    /// <summary>
    /// Called when the cursor is pressed. Will either move or attack depending of if a spall has been selected
    /// </summary>
    /// <param name="selectedSquare"></param>
    private void OnCursorPress(Square selectedSquare)
    {
        if (_selectedSpellIndex < 0)    //If No spell selected, try to move
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
        else                            //If spell selected, attack
        {
            _selectedSpellData = _playerStateMachine.Main.Spells[_selectedSpellIndex].ActionBase;

            if (_selectedSpellData.PaCost <= _playerStateMachine.Main.APs)
            {
                _playerStateMachine.Main.Attack(selectedSquare, _selectedSpellData);

                _playerStateMachine.Main.DecreasePA(_selectedSpellData.PaCost);

                CancelSpell();
            }
            else
            {
                Debug.Log($"Not enough APs to Attack {_playerStateMachine.Main.APs}, needs {_selectedSpellData.PaCost}");
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActiveState : MonoBehaviour, IState
{
    private PlayerStateMachine _state;
    [SerializeField]
    private Spells _selectedSpells;

    [SerializeField]
    private bool _hasSpellSelected; //TODO : remove 

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

    public void SelectSpell(Spells spell)
    {
        if (_selectedSpells == null)
        {
            _selectedSpells = spell;
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
            _state.Main.Move(selectedSquare);
        }
        else
        {
            _state.Main.Attack(selectedSquare, _selectedSpells);
        }
    }
}

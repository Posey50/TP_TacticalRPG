using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActiveState : IPlayerState
{
    /// <summary>
    /// Player state machine of the playable entity.
    /// </summary>
    private PlayerStateMachine _playerStateMachine;

    private int _selectedSpellIndex = -1;
    private Spell _selectedSpell;

    public void OnEnter(PlayerStateMachine playerStateMachine)
    {
        _playerStateMachine = playerStateMachine;
        _playerStateMachine.Main.PlayerInput.onActionTriggered += OnAction;
    }

    public void OnExit(PlayerStateMachine playerStateMachine)
    {
        _playerStateMachine.Main.PlayerInput.onActionTriggered -= OnAction;
    }

    /// <summary>
    /// Called when an input is trigger.
    /// </summary>
    /// <param name="context"> Informations about the input. </param>
    private void OnAction(InputAction.CallbackContext context)
    {
        switch (context.action.name)
        {
            case "CursorMove":
                _playerStateMachine.Main.Cursor.UpdateSelectedSquare(context.action.ReadValue<Vector2>());
                break;

            case "CursorPress":
                if (context.started)
                {
                    Square selectedSquare = _playerStateMachine.Main.Cursor.SelectedSquare;
                    if (selectedSquare != null)
                    {
                        OnLeftClick(selectedSquare);
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
    private void OnLeftClick(Square selectedSquare)
    {
        PlayerMain playerMain = _playerStateMachine.Main;
        Spell selectedSpell = playerMain.Actions.SelectedSpell;
        Entity entityOnThisSquare = selectedSquare.EntityOnThisSquare;

        // If there is a selected spell and an entity on the selected square then attacks the entity
        if (selectedSpell != null && entityOnThisSquare != null)
        {
            playerMain.Attack(selectedSpell, entityOnThisSquare);
        }
        // If there is no selected spell and no entity on the square selected then moves to the selected square
        else if (selectedSpell == null && entityOnThisSquare == null)
        {
            playerMain.StartFollowPath(AStarManager.Instance.CalculateShortestPathBetween(playerMain.SquareUnderTheEntity, selectedSquare));
        }



        if (_selectedSpellIndex < 0)    //If No spell selected, try to move
        {
            if (_playerStateMachine.Main.Cursor.Path.Count - 1 <= _playerStateMachine.Main.MP)    //If the player has enough MPs. "- 1" ignores the Square the player is currently standing on
            {
                //_playerStateMachine.Main.Move(selectedSquare);

                _playerStateMachine.Main.StartFollowPath(_playerStateMachine.Main.Cursor.Path);

                _playerStateMachine.Main.DecreaseMP(_playerStateMachine.Main.Cursor.Path.Count - 1);
            }
            else
            {
                Debug.Log($"Not enough MPs to Move {_playerStateMachine.Main.MP}, needs {_playerStateMachine.Main.Cursor.Path.Count - 1}");
            }
        }
        else                            //If spell selected, attack
        {
            _selectedSpell = _playerStateMachine.Main.Spells[_selectedSpellIndex];

            if (_selectedSpell.SpellDatas.PaCost <= _playerStateMachine.Main.AP)
            {
                _playerStateMachine.Main.Attack(_selectedSpell, selectedSquare.EntityOnThisSquare);

                _playerStateMachine.Main.DecreaseAP(_selectedSpell.SpellDatas.PaCost);

                CancelSpell();
            }
            else
            {
                Debug.Log($"Not enough APs to Attack {_playerStateMachine.Main.AP}, needs {_selectedSpell.SpellDatas.PaCost}");
            }
        }
    }
}

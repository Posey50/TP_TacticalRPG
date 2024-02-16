using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActiveState : IPlayerState
{
    /// <summary>
    /// State machine of the playable entity.
    /// </summary>
    private PlayerStateMachine _playerStateMachine;

    // Event for the mouse cursor movement
    public delegate void CursorMovementDelegate(Vector2 cursorPosition);

    public event CursorMovementDelegate CursorMove;

    // Event for the right click
    public delegate void RightClickDelegate();

    public event RightClickDelegate RightClickPressed;

    public void OnEnter(PlayerStateMachine playerStateMachine)
    {
        _playerStateMachine = playerStateMachine;
        _playerStateMachine.BattleManager.CurrentActiveEntity = _playerStateMachine.PlayerMain;
        _playerStateMachine.SpellButtonsManager.UpdateButtons(_playerStateMachine.PlayerMain);
        _playerStateMachine.PlayerMain.PlayerInput.onActionTriggered += OnAction;
    }

    public void OnExit(PlayerStateMachine playerStateMachine)
    {
        _playerStateMachine.SpellButtonsManager.HideButtons();
        _playerStateMachine.PlayerMain.PlayerInput.onActionTriggered -= OnAction;
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
                OnCursorMove(context.action.ReadValue<Vector2>());
                break;

            case "MouseLeftClick":
                if (context.canceled)
                {
                    Square selectedSquare = _playerStateMachine.PlayerMain.Cursor.SelectedSquare;
                    if (selectedSquare != null)
                    {
                        OnLeftClick(selectedSquare);
                    }
                }
                break;
            case "MouseRightClick":
                if (context.started)
                {
                    OnRigthClick();
                }
                break;
        }
    }

    /// <summary>
    /// Called when the cursor move.
    /// </summary>
    /// <param name="cursorPosition"> Position of the cursor. </param>
    public void OnCursorMove(Vector2 cursorPosition)
    {
        PlayerMain playerMain = _playerStateMachine.PlayerMain;

        if (!playerMain.IsMoving)
        {
            // Anounces that the cursor has moved
            CursorMove?.Invoke(cursorPosition);
        }
    }

    /// <summary>
    /// Called when the mouse left button is released and checks to move or make attack the playable entity.
    /// </summary>
    /// <param name="selectedSquare"> Selected square when the button is clicked. </param>
    private async void OnLeftClick(Square selectedSquare)
    {
        PlayerMain playerMain = _playerStateMachine.PlayerMain;

        if (!playerMain.IsMoving)
        {
            Spell selectedSpell = playerMain.Actions.SelectedSpell;
            Entity entityOnThisSquare = selectedSquare.EntityOnThisSquare;

            // If there is a selected spell and an entity on the selected square and if the selected square is in the range of the spell then attacks the entity
            if (selectedSpell != null && 
                entityOnThisSquare != null && 
                playerMain.Actions.CurrentRange.Contains(selectedSquare) && 
                selectedSpell.SpellDatas.PaCost <= playerMain.AP)
            {
                playerMain.Attack(selectedSpell, entityOnThisSquare);
            }
            // If there is no selected spell and no entity on the square selected and if the path is less or equal to left MP then moves to the selected square
            else if (selectedSpell == null && 
                entityOnThisSquare == null && 
                playerMain.Cursor.Path.Count <= playerMain.MP)
            {
                await playerMain.FollowThePath(playerMain.Cursor.Path);
            }
        }
    }

    /// <summary>
    /// Called when the mouse right button is pressed in active state.
    /// </summary>
    private void OnRigthClick()
    {
        PlayerMain playerMain = _playerStateMachine.PlayerMain;

        if (!playerMain.IsMoving)
        {
            // Anounces that the right click has been pressed
            RightClickPressed?.Invoke();
        }
    }
}

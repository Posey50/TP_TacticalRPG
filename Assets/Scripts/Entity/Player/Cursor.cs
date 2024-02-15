using System.Collections.Generic;
using System;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    /// <summary>
    /// Current square selected.
    /// </summary>
    public Square SelectedSquare { get; private set; }

    /// <summary>
    /// Current path that the playable entity can follow.
    /// </summary>
    public List<Square> Path { get; private set; }

    /// <summary>
    /// Main component of the playable entity.
    /// </summary>
    private PlayerMain _playerMain;
    
    private void Start()
    {
        _playerMain = GetComponent<PlayerMain>();

        _playerMain.TurnIsEnd += UnselectSquareForPath;
        _playerMain.TurnIsEnd += UnselectSquareForAttack;
    }

    /// <summary>
    /// Changes the selected square if the mouse is pointing at a new square
    /// </summary>
    public void UpdateSelectedSquare(Vector2 mousePosition)
    {
        Square currentSquarePointed = GetSquareUnderPosition(mousePosition);

        if (!_playerMain.IsMoving)
        {
            if (_playerMain.Actions.SelectedSpell != null)
            {
                if (_playerMain.Actions.SelectedSpell.SpellDatas != null)
                {
                    if (currentSquarePointed != null && currentSquarePointed != SelectedSquare)
                    {
                        // Hides the previous square selected
                        UnselectSquareForAttack();

                        // Gets the new one
                        SelectedSquare = currentSquarePointed;

                        // Shows the new one
                        HighlightGroundManager.Instance.HighlightSelectedSquareForAttack(SelectedSquare);
                    }
                    else if (currentSquarePointed == null)
                    {
                        // Hides the previous square selected
                        UnselectSquareForAttack();
                    }
                }
            }
            else
            {
                if (currentSquarePointed != null && currentSquarePointed != SelectedSquare && currentSquarePointed.EntityOnThisSquare == null)
                {
                    // Hides the previous path
                    UnselectSquareForPath();

                    SelectedSquare = currentSquarePointed;

                    // Gets the new one
                    Path = AStarManager.Instance.CalculateShortestPathForAMovement(_playerMain.SquareUnderTheEntity, SelectedSquare);

                    // Shows the new one
                    HighlightGroundManager.Instance.ShowPath(Path);
                }
                else if (currentSquarePointed == null)
                {
                    // Hides the path
                    UnselectSquareForPath();
                }
            }
        }
    }

    /// <summary>
    /// Called to unselect the selected square with the path.
    /// </summary>
    public void UnselectSquareForPath()
    {
        SelectedSquare = null;

        // Hides the previous path
        HighlightGroundManager.Instance.HideCurrentPath();
    }

    /// <summary>
    /// Called to hunselect the selected square when a spell is selected.
    /// </summary>
    public void UnselectSquareForAttack()
    {
        // Hides the selected square
        HighlightGroundManager.Instance.HideSelectedSquareForAttack(SelectedSquare);

        SelectedSquare = null;
    }

    /// <summary>
    /// Returns a Square if the mouse is above a Square. Returns null if not
    /// </summary>
    /// <param name="mousePosition"> Position of the mouse. </param>
    /// <returns></returns>
    private Square GetSquareUnderPosition(Vector2 mousePosition) 
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(mousePosition), out RaycastHit hit, 200f))
        {
            if (hit.transform.CompareTag("Square"))
            {
                return hit.transform.GetComponent<Square>();
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }
}

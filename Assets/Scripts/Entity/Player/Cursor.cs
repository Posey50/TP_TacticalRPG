using System.Collections.Generic;
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

    // Event for the selected square
    public delegate void CurrentSquareDelegate(Square newSquare);

    public event CurrentSquareDelegate SelectedSquareChanged;

    // Event for the path
    public delegate void CurrentPathDelegate(List<Square> newPath);

    public event CurrentPathDelegate PathChanged;

    private void Start()
    {
        _playerMain = GetComponent<PlayerMain>();

        _playerMain.StateMachine.ActiveState.CursorMove += UpdateSelectedSquare;
        _playerMain.TurnIsEnd += UnselectAll;
    }

    /// <summary>
    /// Updates the selected square and the path when mouse moves.
    /// </summary>
    public void UpdateSelectedSquare(Vector2 mousePosition)
    {
        // Gets the square pointed by the mouse
        Square currentSquarePointed = GetSquareUnderPosition(mousePosition);

        if (!_playerMain.IsMoving)
        {
            if (_playerMain.Actions.SelectedSpell != null)
            {
                if (_playerMain.Actions.SelectedSpell.SpellDatas != null)
                {
                    if (currentSquarePointed != null && currentSquarePointed != SelectedSquare)
                    {
                        // Gets the new selected square
                        SelectedSquare = currentSquarePointed;

                        // Anounces that the new square selected has changed
                        SelectedSquareChanged?.Invoke(SelectedSquare);
                    }
                }
            }
            else
            {
                if (currentSquarePointed != null && currentSquarePointed != SelectedSquare && currentSquarePointed.EntityOnThisSquare == null)
                {
                    // Gets the new selected square
                    SelectedSquare = currentSquarePointed;

                    // Anounces that the new square selected has changed
                    SelectedSquareChanged?.Invoke(SelectedSquare);

                    // Gets the new path
                    Path = AStarManager.Instance.CalculateShortestPathForAMovement(_playerMain.SquareUnderTheEntity, SelectedSquare);

                    // Anounces that the path has changed
                    PathChanged?.Invoke(Path);
                }
            }
        }
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

    /// <summary>
    /// Called to unselect all.
    /// </summary>
    private void UnselectAll()
    {
        SelectedSquare = null;
        Path = null;

        SelectedSquareChanged?.Invoke(SelectedSquare);
        PathChanged?.Invoke(Path);
    }
}

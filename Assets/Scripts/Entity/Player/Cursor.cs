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

    void Start()
    {
        _playerMain = GetComponent<PlayerMain>();
    }

    /// <summary>
    /// Changes the selected square if the mouse is pointing at a new square
    /// </summary>
    public void UpdateSelectedSquare(Vector2 mousePosition)
    {
        if (_playerMain.Actions.SelectedSpell == null)
        {
            Square currentSquarePointed = GetSquareUnderPosition(mousePosition);

            if (currentSquarePointed != null && currentSquarePointed != SelectedSquare)
            {
                SelectedSquare = currentSquarePointed;

                // Hides the previous path
                HighlightGroundManager.Instance.HideCurrentPath();

                // Gets the new one
                Path = AStarManager.Instance.CalculateShortestPathBetween(_playerMain.SquareUnderTheEntity, SelectedSquare);

                // Shows the new one
                HighlightGroundManager.Instance.ShowPath(Path);
            }
            else if (currentSquarePointed == null)
            {
                UnselectSquare();
            }
        }
    }

    public void UnselectSquare()
    {
        SelectedSquare = null;

        // Hides the previous path
        HighlightGroundManager.Instance.HideCurrentPath();
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

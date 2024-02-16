using System;
using System.Collections.Generic;
using UnityEngine;

public class HighlightGroundManager : MonoBehaviour
{
    // Singleton
    private static HighlightGroundManager _instance = null;

    public static HighlightGroundManager Instance => _instance;

    /// <summary>
    /// Color to apply if the selected square is not valid.
    /// </summary>
    [field: SerializeField]
    public Color InvalideSquareColor { get; private set; }

    /// <summary>
    /// Color to apply if the selected square is valid.
    /// </summary>
    [field: SerializeField]
    public Color ValideSquareColor { get; private set; }

    /// <summary>
    /// Color to apply if the square is in a path.
    /// </summary>
    [field: SerializeField]
    public Color PathColor { get; private set; }

    /// <summary>
    /// Color to apply if the square is in a range.
    /// </summary>
    [field: SerializeField]
    public Color RangeColor { get; private set; }

    /// <summary>
    /// Current selected square showed on screen.
    /// </summary>
    [field: SerializeField]
    public Square CurrentHighlightSquare { get; private set; }

    /// <summary>
    /// Current path showed on screen.
    /// </summary>
    [field: SerializeField]
    public List<Square> CurrentHighlightPath { get; private set; }

    /// <summary>
    /// Current range showed on screen.
    /// </summary>
    [field: SerializeField]
    public List<Square> CurrentHighlightRange { get; private set; }

    /// <summary>
    /// Event to Update the UI of MP to mmove.
    /// </summary>
    public event Action<int> ShowMPUI;
    public event Action HideMPUI;


    private void Awake()
    {
        // Singleton
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        BattleManager.Instance.AllEntitiesInit += InitialiseHighlightGroundManager;
    }

    /// <summary>
    /// Called to initialise the manager.
    /// </summary>
    private void InitialiseHighlightGroundManager()
    {
        for (int i = 0; i < BattleManager.Instance.PlayableEntitiesInBattle.Count; i++)
        {
            PlayerMain entity = (PlayerMain)BattleManager.Instance.PlayableEntitiesInBattle[i];

            entity.Cursor.SelectedSquareChanged += HighlightSelectedSquare;
            entity.Cursor.PathChanged += HighlightPath;
            entity.Actions.RangeChanged += HighlightRange;
        }
    }

    /// <summary>
    /// Called to highlight the selected square.
    /// </summary>
    /// <param name="newSelectedSquare"> Square to highlight. </param>
    private void HighlightSelectedSquare(Square newSelectedSquare)
    {
        // Sets the previous color on the current highlight square
        if (CurrentHighlightSquare != null)
        {
            CurrentHighlightSquare.SetPreviousColor();
        }

        // Sets the new highlight square
        CurrentHighlightSquare = newSelectedSquare;

        // Checks if the selected square is a square where player can attack or not and highlights it accordingly
        if (CurrentHighlightSquare != null)
        {
            Entity currentActiveEntity = BattleManager.Instance.CurrentActiveEntity;
            Actions actions = currentActiveEntity.GetComponent<Actions>();

            if (actions.CurrentRange != null)
            {
                if (actions.CurrentRange.Contains(CurrentHighlightSquare))
                {
                    if (CurrentHighlightSquare.EntityOnThisSquare != null &&
                        actions.SelectedSpell.SpellDatas.PaCost <= currentActiveEntity.AP)
                    {
                        CurrentHighlightSquare.SetColor(ValideSquareColor);
                    }
                    else
                    {
                        CurrentHighlightSquare.SetColor(InvalideSquareColor);
                    }
                }
                else
                {
                    CurrentHighlightSquare.SetColor(InvalideSquareColor);
                }
            }
        }
    }

    /// <summary>
    /// Called to highlight a path.
    /// </summary>
    /// <param name="newPath"> Path to highlight. </param>
    public void HighlightPath(List<Square> newPath)
    {
        // Sets the original color on each square in the current highlight path
        if (CurrentHighlightPath != null)
        {
            if (CurrentHighlightPath.Count > 0)
            {
                for (int i = 0; i < CurrentHighlightPath.Count; i++)
                {
                    CurrentHighlightPath[i].ResetColor();
                }
            }
        }

        // Sets the new highlight path
        CurrentHighlightPath = newPath;

        // Checks if the new path is too long to move or not and highlight it accordingly
        if (CurrentHighlightPath != null)
        {
            if (CurrentHighlightPath.Count > BattleManager.Instance.CurrentActiveEntity.MP)
            {
                for (int i = 0; i < CurrentHighlightPath.Count; i++)
                {
                    CurrentHighlightPath[i].SetColor(InvalideSquareColor);
                }
            }
            else
            {
                for (int i = 0; i < CurrentHighlightPath.Count; i++)
                {
                    if (i == CurrentHighlightPath.Count - 1)
                    {
                        CurrentHighlightPath[i].SetColor(ValideSquareColor);
                    }
                    else
                    {
                        CurrentHighlightPath[i].SetColor(PathColor);
                    }
                }
            }
        }
    }


    /// <summary>
    /// Called to highlight a range.
    /// </summary>
    /// <param name="newRange"> Range to highlight. </param>
    public void HighlightRange(List<Square> newRange)
    {
        // Sets the original color on each square in the current highlight range
        if (CurrentHighlightRange != null)
        {
            if (CurrentHighlightRange.Count > 0)
            {
                for (int i = 0; i < CurrentHighlightRange.Count; i++)
                {
                    CurrentHighlightRange[i].ResetColor();
                }
            }
        }

        // Sets the new highlight range
        CurrentHighlightRange = newRange;

        // Highlights the new range
        if (CurrentHighlightRange != null)
        {
            for (int i = 0; i < CurrentHighlightRange.Count; i++)
            {
                CurrentHighlightRange[i].SetColor(RangeColor);
            }
        }
    }
}
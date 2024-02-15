using System;
using System.Collections.Generic;
using UnityEngine;

public class HighlightGroundManager : MonoBehaviour
{
    // Singleton
    private static HighlightGroundManager _instance = null;

    public static HighlightGroundManager Instance => _instance;

    /// <summary>
    /// Material to apply if the selected square is not valid.
    /// </summary>
    [field: SerializeField]
    public Material InvalideSquareMaterial { get; private set; }

    /// <summary>
    /// Material to apply if the selected square is valid.
    /// </summary>
    [field: SerializeField]
    public Material ValideSquareMaterial { get; private set; }

    /// <summary>
    /// Material to apply if the square is in a path.
    /// </summary>
    [field: SerializeField]
    public Material PathMaterial { get; private set; }

    /// <summary>
    /// Material to apply if the square is in a range.
    /// </summary>
    [field: SerializeField]
    public Material RangeMaterial { get; private set; }

    /// <summary>
    /// Path to show at screen.
    /// </summary>
    [field: SerializeField]
    public List<Square> CurrentPath { get; private set; }

    /// <summary>
    /// Range to show at screen.
    /// </summary>
    [field: SerializeField]
    public List<Square> CurrentRange { get; private set; }

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

    /// <summary>
    /// Set the material of each square in the path given to show the path on screen.
    /// </summary>
    /// <param name="path"> Path to show. </param>
    public void ShowPath(List<Square> path)
    {
        ShowMPUI?.Invoke(path.Count);
        CurrentPath = path;

        if (CurrentPath != null)
        {
            if (CurrentPath.Count > BattleManager.Instance.CurrentActiveEntity.MP)
            {
                for (int i = 0; i < CurrentPath.Count; i++)
                {
                    CurrentPath[i].SetMaterial(InvalideSquareMaterial);
                }
            }
            else
            {
                for (int i = 0; i < CurrentPath.Count; i++)
                {
                    if (i == CurrentPath.Count - 1)
                    {
                        CurrentPath[i].SetMaterial(ValideSquareMaterial);
                    }
                    else
                    {
                        CurrentPath[i].SetMaterial(PathMaterial);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Restores the original material of each square in the path current path showed.
    /// </summary>
    public void HideCurrentPath()
    {
        HideMPUI?.Invoke();

        if (CurrentPath != null)
        {
            for (int i = 0; i < CurrentPath.Count; i++)
            {
                CurrentPath[i].ResetMaterial();
            }

            CurrentPath.Clear();
        }
    }

    /// <summary>
    /// Called to show a range.
    /// </summary>
    /// <param name="range"> Range to show. </param>
    public void ShowRange(List<Square> range)
    {
        CurrentRange = range;

        if (CurrentRange != null)
        {
            for (int i = 0; i < CurrentRange.Count; i++)
            {
                CurrentRange[i].SetMaterial(RangeMaterial);
            }
        }
    }

    /// <summary>
    /// Called to hide the cuurent range showed.
    /// </summary>
    public void HideRange()
    {
        if (CurrentRange != null)
        {
            for (int i = 0; i < CurrentRange.Count; i++)
            {
                CurrentRange[i].ResetMaterial();
            }

            CurrentRange.Clear();
        }
    }

    /// <summary>
    /// Called to highlight the selected square when a spell is selected.
    /// </summary>
    /// <param name="selectedSquare"> Selected square to highlight. </param>
    public void HighlightSelectedSquareForAttack(Square selectedSquare)
    {
        if (BattleManager.Instance.CurrentActiveEntity.GetComponent<Actions>().CurrentRange.Contains(selectedSquare))
        {
            if (selectedSquare.EntityOnThisSquare != null)
            {
                selectedSquare.SetMaterial(ValideSquareMaterial);
            }
            else
            {
                selectedSquare.SetMaterial(InvalideSquareMaterial);
            }
        }
        else
        {
            selectedSquare.SetMaterial(InvalideSquareMaterial);
        }
    }

    /// <summary>
    /// Called to hide the selected square when a spell is selected.
    /// </summary>
    public void HideSelectedSquareForAttack(Square selectedSquare)
    {
        if (selectedSquare != null)
        {
            selectedSquare.SetPreviousMaterial();
        }
    }
}

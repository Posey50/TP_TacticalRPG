using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pointer : MonoBehaviour
{
    //public Square StartSquare { get; private set; }
    public Square SelectedSquare { get; private set; }
    public List<Square> Path { get; private set; }

    public Material startMaterial;
    public Material selectedMaterial;
    public Material pathMaterial;

    private Square _currentSquare;

    public event Action<Square> CursorPress;

    private PlayerMain _player;

    void Start()
    {
        _player = GetComponent<PlayerMain>();

        _player.SquareUnderTheEntity.GetComponent<MeshRenderer>().material = startMaterial;

        SelectedSquare = _player.SquareUnderTheEntity;
    }

    public void SetCurrentSquare(Square newCurrent)
    {
        _currentSquare = newCurrent;
    }

    /// <summary>
    /// Returns a Square if the mouse is above a Square. Returns null if not
    /// </summary>
    /// <param name="mousePosition"></param>
    /// <returns></returns>
    public Square GetSquareUnderPosition(Vector2 mousePosition) 
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(mousePosition), out RaycastHit hit, 200f))
        {
            if (hit.transform.CompareTag("Square"))
            {
                return hit.transform.GetComponent<Square>();
            }
        }

        return null;
    }

    /// <summary>
    /// Returns if the mouse is pointing at the selected square
    /// </summary>
    /// <returns></returns>
    public bool CurrentSquareIsSelectedSquare()
    {
        return (_currentSquare == SelectedSquare) ? true : false;
    }

    /// <summary>
    /// Changes the selected square if the mouse is pointing at a new square
    /// </summary>
    public void UpdateSelectedSquare()
    {
        if (_currentSquare == null)    //If the Mouse isn't pointing to a Square, return
        {
            HidePath();
            return;
        }

        if (CurrentSquareIsSelectedSquare())      //If the Mouse is pointing at the Square currently selected, return
        {
            return;
        }

        SelectedSquare = _currentSquare;

        ShowPath();

        SelectedSquare.GetComponent<MeshRenderer>().material = selectedMaterial;
    }

    /// <summary>
    /// Draws a path from the starting Square to the Selected Square
    /// </summary>
    private void ShowPath()
    {
        HidePath();

        Path = AStarManager.Instance.CalculateShortestPathBetween(_player.SquareUnderTheEntity, SelectedSquare);

        if (Path != null)
        {
            for (int i = 0; i < Path.Count; i++)
            {
                if (i != 0 && i != Path.Count - 1)  //Excludes the first and the last cube
                {
                    Path[i].GetComponent<MeshRenderer>().material = pathMaterial;
                }
            }
        }
    }

    /// <summary>
    /// Restores the path's cubes to their original material
    /// </summary>
    private void HidePath()
    {
        if (Path != null)
        {
            for (int i = 0; i < Path.Count; i++)
            {
                Path[i].GetComponent<MeshRenderer>().material = Path[i].OriginalMaterial;
            }
        }
    }
}

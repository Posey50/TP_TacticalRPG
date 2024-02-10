using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pointer : MonoBehaviour
{
    public Square startSquare { get; private set; }
    public Square selectedSquare { get; private set; }
    
    public Material startMaterial;
    public Material selectedMaterial;
    public Material pathMaterial;

    private Square _currentSquare;
    private List<Square> path;

    public event Action<Square> CursorPress;

    void Start()
    {
        startSquare.GetComponent<MeshRenderer>().material = startMaterial;
    }

    public void SetStartSquare(Square newStart)
    {
        startSquare = newStart;
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


    public void UpdateSelectedSquare()
    {
        if (_currentSquare == null)    //If the Mouse isn't pointing to a Square, return
        {   
            return;
        }

        if (_currentSquare == selectedSquare)      //If the Mouse is pointing at the Square currently selected, return
        {
            return;
        }

        if (selectedSquare != null)     // If there is currently a square beign selected, restore the square to its original material
        {
            selectedSquare.GetComponent<MeshRenderer>().material = selectedSquare.OriginalMaterial;
        }

        selectedSquare = _currentSquare;

        selectedSquare.GetComponent<MeshRenderer>().material = selectedMaterial;

        ShowPath();
    }

    /// <summary>
    /// Draws a path from the starting Square to the Selected Square
    /// </summary>
    private void ShowPath()
    {
        if (path != null)
        {
            for (int i = 0; i < path.Count; i++)
            {
                path[i].GetComponent<MeshRenderer>().material = path[i].OriginalMaterial;
            }
        }

        path = AStarManager.Instance.CalculateShortestPathBetween(startSquare, selectedSquare);

        if (path != null)
        {
            for (int i = 0; i < path.Count; i++)
            {
                if (i != 0 && i != path.Count - 1)
                {
                    path[i].GetComponent<MeshRenderer>().material = pathMaterial;
                }
            }
        }
    }
}

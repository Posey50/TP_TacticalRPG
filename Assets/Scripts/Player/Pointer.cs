using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pointer : MonoBehaviour
{
    public PlayerInput playerInput;
    public Square startSquare;
    public Square selectedSquare;
    public List<Square> path;
    public Material startMaterial;
    public Material selectedMaterial;
    public Material pathMaterial;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.onActionTriggered += OnAction;

        startSquare.GetComponent<MeshRenderer>().material = startMaterial;
    }

    private void OnAction(InputAction.CallbackContext context)
    {
        switch (context.action.name)
        {
            case "CursorMove":
                CheckSquares(context.action.ReadValue<Vector2>());
                break;
        }
    }

    private void CheckSquares(Vector2 mousePosition) 
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(mousePosition), out RaycastHit hit, 200f))
        {
            if (hit.transform.CompareTag("Square"))
            {
                if (hit.transform.GetComponent<Square>() != selectedSquare)
                {
                    NewSquareSelected(hit.transform.GetComponent<Square>());
                }
            }
        }
    }

    private void NewSquareSelected(Square newSquare)
    {
        if (selectedSquare != null)
        {
            selectedSquare.GetComponent<MeshRenderer>().material = selectedSquare.OriginalMaterial;
        }

        selectedSquare = newSquare;

        newSquare.GetComponent<MeshRenderer>().material = selectedMaterial;

        ShowPath();
    }

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

using UnityEngine;
using System.Collections.Generic;

public class Square : MonoBehaviour
{
    /// <summary>
    /// The Entity currently on the square;
    /// </summary>
    [field: SerializeField]
    public Entity EntityOnThisSquare { get; private set; }

    /// <summary>
    /// List of neighboring squares.
    /// </summary>
    [field: SerializeField]
    public List<Square> Neighbors { get; private set; }

    /// <summary>
    /// Previous square in a path to access at this one.
    /// </summary>
    [field: SerializeField]
    public Square PreviousSquare { get; set; }

    /// <summary>
    /// Number of squares between this one and the departure in a path.
    /// </summary>
    [field: SerializeField]
    public int StepsForAccess { get; set; }

    /// <summary>
    /// A score representating the distance between the arrival and this square in a path.
    /// </summary>
    [field: SerializeField]
    public float F { get; set; }

    /// <summary>
    /// A value indicating if the square is already closed or not.
    /// </summary>
    [field: SerializeField]
    public bool IsClosed { get; set; }

    /// <summary>
    /// Original material of the square.
    /// </summary>
    public Material OriginalMaterial { get; set; }

    void Start()
    {
        OriginalMaterial = GetComponent<MeshRenderer>().material;

        DetecteNeighbors();
        ResetSquare();
    }

    /// <summary>
    /// Sets the list of neighboring squares.
    /// </summary>
    private void DetecteNeighbors()
    {
        float distance = GetComponent<BoxCollider>().bounds.size.x;
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        // For forward and backward
        for (int i = 0; i < 2; i++)
        {
            if (Physics.Raycast(transform.position, forward, out RaycastHit hit, distance))
            {
                if (hit.transform.CompareTag("Square"))
                {
                    Neighbors.Add(hit.transform.GetComponent<Square>());
                }
            }

            forward *= -1f;
        }

        // For right and left
        for (int i = 0; i < 2; i++)
        {
            if (Physics.Raycast(transform.position, right, out RaycastHit hit, distance))
            {
                if (hit.transform.CompareTag("Square"))
                {
                    Neighbors.Add(hit.transform.GetComponent<Square>());
                }
            }

            right *= -1f;
        }
    }

    /// <summary>
    /// Called to remove the entity on this square.
    /// </summary>
    public void LeaveSquare()
    {
        EntityOnThisSquare = null;
    }

    /// <summary>
    /// Called to set the new entity on this square.
    /// </summary>
    /// <param name="newEntity"> New entity on the square. </param>
    public void SetEntity(Entity newEntity)
    {
        if (newEntity != null)
        {
            EntityOnThisSquare = newEntity;
        }
    }

    /// <summary>
    /// Called to set the different values of the square by default.
    /// </summary>
    public void ResetSquare()
    {
        PreviousSquare = null;
        StepsForAccess = 0;
        F = 0f;
        IsClosed = false;
    }
}
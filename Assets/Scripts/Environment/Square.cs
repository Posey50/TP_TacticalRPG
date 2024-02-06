using UnityEngine;
using System.Collections.Generic;

public class Square : MonoBehaviour
{
    /// <summary>
    /// List of neighboring squares.
    /// </summary>
    [field: SerializeField]
    public List<Square> Neighbors { get; private set; }

    void Start()
    {
        DetecteNeighbors();
    }

    /// <summary>
    /// Set the list of neighboring squares.
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
}
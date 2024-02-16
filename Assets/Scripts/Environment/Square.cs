using UnityEngine;
using System.Collections.Generic;

public class Square : MonoBehaviour
{
    /// <summary>
    /// The Entity currently on the square;
    /// </summary>
    public Entity EntityOnThisSquare { get; private set; }

    /// <summary>
    /// List of neighboring squares.
    /// </summary>
    [field: SerializeField]
    public List<Square> Neighbors { get; private set; }

    /// <summary>
    /// Previous square in a path to access at this one.
    /// </summary>
    public Square PreviousSquare { get; set; }

    /// <summary>
    /// Number of squares between this one and the departure in a path.
    /// </summary>
    public int StepsForAccess { get; set; }

    /// <summary>
    /// A score representating the distance between the arrival and this square in a path.
    /// </summary>
    public float F { get; set; }

    /// <summary>
    /// A value indicating if the square is already closed or not.
    /// </summary>
    public bool IsClosed { get; set; }

    /// <summary>
    /// Mesh renderer component of the square.
    /// </summary>
    private MeshRenderer _meshRenderer;

    /// <summary>
    /// Original color of the square.
    /// </summary>
    private Color _originalColor;

    /// <summary>
    /// Original emission color of the square.
    /// </summary>
    private Color _originalEmissionColor;

    /// <summary>
    /// Previous color of the square.
    /// </summary>
    private Color _previousColor;

    /// <summary>
    /// Previous emission color of the square.
    /// </summary>
    private Color _previousEmissionColor;

    void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _originalColor = _meshRenderer.material.color;
        _originalEmissionColor = _meshRenderer.material.GetColor("_EmissionColor");

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
    /// Called to set a new color on the square.
    /// </summary>
    /// <param name="newColor"> Color to set. </param>
    public void SetColor(Color newColor)
    {
        _previousColor = _meshRenderer.material.color;
        _meshRenderer.material.color = CombineColors(_originalColor, newColor);

        // Applies a filter with emission color on the square
        _previousEmissionColor = _meshRenderer.material.GetColor("_EmissionColor");
        newColor *= Mathf.Pow(2f, -2f);
        _meshRenderer.material.SetColor("_EmissionColor", newColor);
    }

    /// <summary>
    /// Called to set the previous color on the square.
    /// </summary>
    public void SetPreviousColor()
    {
        _meshRenderer.material.color = _previousColor;
        _meshRenderer.material.SetColor("_EmissionColor", _previousEmissionColor);
    }

    /// <summary>
    /// Called to reset the color on the square.
    /// </summary>
    public void ResetColor()
    {
        _meshRenderer.material.color = _originalColor;
        _meshRenderer.material.SetColor("_EmissionColor", _originalEmissionColor);
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

    /// <summary>
    /// Called to blend multiple colors.
    /// </summary>
    /// <param name="colors"> Colors to blend. </param>
    /// <returns></returns>
    private Color CombineColors(params Color[] colors)
    {
        Color result = new (0, 0, 0, 0);

        // For each color
        for (int i = 0; i < colors.Length; i++)
        {
            // Adds the color to the result
            result += colors[i];
        }

        // Divides the result by the number of colors
        result /= colors.Length;

        return result;
    }
}
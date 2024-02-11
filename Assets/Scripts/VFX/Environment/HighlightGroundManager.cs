using System.Collections.Generic;
using UnityEngine;

public class HighlightGroundManager : MonoBehaviour
{
    // Singleton
    private static HighlightGroundManager _instance = null;

    public static HighlightGroundManager Instance => _instance;

    /// <summary>
    /// Material to apply if the square is the start of a path.
    /// </summary>
    public Material startMaterial;

    /// <summary>
    /// Material to apply if the square is the current square selected.
    /// </summary>
    public Material selectedMaterial;

    /// <summary>
    /// Material to apply if the square is in a path.
    /// </summary>
    public Material pathMaterial;

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
        if (path != null)
        {
            for (int i = 0; i < path.Count; i++)
            {
                if (i == 0)
                {
                    path[i].GetComponent<MeshRenderer>().material = startMaterial;
                }
                else if (i != path.Count - 1)
                {
                    path[i].GetComponent<MeshRenderer>().material = selectedMaterial;
                }
                else
                {
                    path[i].GetComponent<MeshRenderer>().material = pathMaterial;
                }
            }
        }
    }

    /// <summary>
    /// Restores the original material of each square in the path given.
    /// </summary>
    /// <param name="path"> Path to hide. </param>
    public void HidePath(List<Square> path)
    {
        if (path != null)
        {
            for (int i = 0; i < path.Count; i++)
            {
                path[i].GetComponent<MeshRenderer>().material = path[i].OriginalMaterial;
            }
        }
    }
}

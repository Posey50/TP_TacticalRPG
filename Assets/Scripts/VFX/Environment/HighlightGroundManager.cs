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
    public Material InvalideSquareMaterial {  get; private set; }

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
    public List<Square> CurrentPath { get; private set; }

    /// <summary>
    /// Range to show at screen.
    /// </summary>
    public List<Square> CurrentRange { get; private set; }



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
        CurrentPath = path;

        if (CurrentPath != null)
        {
            if (CurrentPath.Count > BattleManager.Instance.CurrentActiveEntity.MP)
            {
                for (int i = 0; i < CurrentPath.Count; i++)
                {
                    CurrentPath[i].GetComponent<MeshRenderer>().material = InvalideSquareMaterial;
                }
            }
            else
            {
                for (int i = 0; i < CurrentPath.Count; i++)
                {
                    if (i == CurrentPath.Count - 1)
                    {
                        CurrentPath[i].GetComponent<MeshRenderer>().material = ValideSquareMaterial;
                    }
                    else
                    {
                        CurrentPath[i].GetComponent<MeshRenderer>().material = PathMaterial;
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
        if (CurrentPath != null)
        {
            for (int i = 0; i < CurrentPath.Count; i++)
            {
                CurrentPath[i].GetComponent<MeshRenderer>().material = CurrentPath[i].OriginalMaterial;
            }
        }
    }

    /// <summary>
    /// Called to show a range.
    /// </summary>
    /// <param name="range"> Range to show. </param>
    public void ShowRange(List<Square> range)
    {
        CurrentRange = range;

        if (range != null)
        {
            for (int i = 0; i < CurrentRange.Count; i++)
            {
                CurrentRange[i].GetComponent<MeshRenderer>().material = RangeMaterial;
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
                CurrentRange[i].GetComponent<MeshRenderer>().material = CurrentRange[i].OriginalMaterial;
            }
        }
    }
}

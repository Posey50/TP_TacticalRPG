using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MpUseUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMpUse;
    private Cursor _cursor;
    public void Start()
    {
        _cursor = GetComponent<Cursor>();
        _cursor.PathChanged += NotifyShowUI;
        _cursor.SelectedSquareChanged += NotifyHideUI;
    }

    /// <summary>
    /// Update text MP need to Move.
    /// </summary>
    public void NotifyShowUI(List<Square> MPDecrease)
    {
        if (MPDecrease != null)
        {
            _textMpUse.gameObject.SetActive(true);
            _textMpUse.text = "MP : -" + MPDecrease.Count;
        }
    }

    /// <summary>
    /// Hide text MP need to Move.
    /// </summary>
    public void NotifyHideUI(Square square)
    {
        if (square == null)
        {
            _textMpUse.gameObject.SetActive(false);
        }
    }
}

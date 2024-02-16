using TMPro;
using UnityEngine;

public class ApUseUI : MonoBehaviour
{
    private HighlightGroundManager _highlightGroundManager;
    [SerializeField] private TextMeshProUGUI _textApUse;

    void Start()
    {
        _highlightGroundManager = GetComponent<HighlightGroundManager>();
        _highlightGroundManager.ShowApUI += NotifyUpdateApUI;
    }

    /// <summary>
    /// Update text AP need to use this action.
    /// </summary>
    public void NotifyUpdateApUI(int ApUse)
    {
        if (_highlightGroundManager.CurrentHighlightSquare.EntityOnThisSquare != null)
        {
            _textApUse.gameObject.SetActive(true);
            _textApUse.text = "AP : - " + ApUse;
        }
        else
        {
            _textApUse.gameObject.SetActive(false);
        }
    }
}

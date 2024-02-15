using TMPro;
using UnityEngine;

public class MpUseUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMpUse;
    public void Start()
    {
        HighlightGroundManager.Instance.ShowMPUI += NotifyShowUI;
        HighlightGroundManager.Instance.HideMPUI += NotifyHideUI;
    }

    /// <summary>
    /// Update text MP need to Move.
    /// </summary>
    public void NotifyShowUI(int MPDecrease)
    {
        _textMpUse.gameObject.SetActive(true);
        _textMpUse.text = "MP : -" + MPDecrease;
    }

    /// <summary>
    /// Hide text MP need to Move.
    /// </summary>
    public void NotifyHideUI()
    {
        _textMpUse.gameObject.SetActive(false);
    }
}

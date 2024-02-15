using TMPro;
using UnityEngine;

public class MpUseUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TextMpUse;
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
        TextMpUse.gameObject.SetActive(true);
        TextMpUse.text = "MP : -" + MPDecrease;
    }

    /// <summary>
    /// Hide text MP need to Move.
    /// </summary>
    public void NotifyHideUI()
    {
        TextMpUse.gameObject.SetActive(false);
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellButtonsManager : MonoBehaviour
{
    // Singleton
    private static SpellButtonsManager _instance = null;

    public static SpellButtonsManager Instance => _instance;

    /// <summary>
    /// The Buttons Representing the Spells of the player
    /// </summary>
    [field: SerializeField]
    public SpellButton[] SpellButtons { get; private set; }

    /// <summary>
    /// The End Turn Button
    /// </summary>
    [SerializeField]
    private Button _endTurnButton;

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

    private void Start()
    {
        //_textButtons = new TextMeshProUGUI[SpellButtons.Length];

        for (int i = 0; i < SpellButtons.Length; i++)
        {
            //_textButtons[i] = SpellButtons[i].GetComponentInChildren<TextMeshProUGUI>();

            SpellButtons[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Synchronises SpellButtons to the current player's Spells, then display the button. If there is no Spell to synchronise to, keep the button hidden
    /// </summary>
    /// <param name="currentPlayer"></param>
    public void UpdateButtons(PlayerMain currentPlayer)
    {
        for (int i = 0; i < currentPlayer.Spells.Count; i++)
        {
            if (currentPlayer.Spells[i].SpellDatas != null && SpellButtons[i] != null)
            {
                SpellButtons[i].Spell = currentPlayer.Spells[i];
                SpellButtons[i].GetComponent<Button>().image.sprite = currentPlayer.Spells[i].SpellDatas.Sprite;

                //_textButtons[i].SetText(currentPlayer.Spells[i].SpellDatas.Name);

                SpellButtons[i].gameObject.SetActive(true);
            }
        }

        _endTurnButton.interactable = true;
    }

    /// <summary>
    /// Hides every button spell
    /// </summary>
    public void HideButtons()
    {
        for (int i = 0; i < SpellButtons.Length; i++)
        {
            SpellButtons[i].gameObject.SetActive(false);
        }

        _endTurnButton.interactable = false;
    }
}

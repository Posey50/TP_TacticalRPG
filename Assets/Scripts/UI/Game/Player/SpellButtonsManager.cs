using UnityEngine;
using UnityEngine.UI;

public class SpellButtonsManager : MonoBehaviour
{
    /// <summary>
    /// Buttons representing spells of the player.
    /// </summary>
    [SerializeField]
    private SpellButton[] _spellButtons;

    private void Start()
    {
        BattleManager.Instance.AllEntitiesInit += InitialiseSpellButtonsManager;
    }

    /// <summary>
    /// Called to initialise the manager.
    /// </summary>
    private void InitialiseSpellButtonsManager()
    {
        for (int i = 0; i < BattleManager.Instance.PlayableEntitiesInBattle.Count; i++)
        {
            PlayerMain playableEntity = (PlayerMain)BattleManager.Instance.PlayableEntitiesInBattle[i];

            playableEntity.StateMachine.ActiveState.TurnStarted += UpdateSpellButtons;
            playableEntity.StateMachine.ActiveState.TurnEnded += HideButtons;
            playableEntity.StartAttacking += DesactivateButtons;
            playableEntity.StopAttacking += ReactivateButtons;
            playableEntity.StartMoving += DesactivateButtons;
            playableEntity.StopMoving += ReactivateButtons;
        }
    }

    /// <summary>
    /// Synchronises spell buttons to the current player's spells, then displays the button. If there is no spell to synchronise to, keeps the button hidden
    /// </summary>
    private void UpdateSpellButtons()
    {
        Entity currentPlayer = BattleManager.Instance.CurrentActiveEntity;

        for (int i = 0; i < currentPlayer.Spells.Count; i++)
        {
            if (currentPlayer.Spells[i].SpellDatas != null && _spellButtons[i] != null)
            {
                _spellButtons[i].Spell = currentPlayer.Spells[i];
                _spellButtons[i].GetComponent<Button>().image.sprite = currentPlayer.Spells[i].SpellDatas.Sprite;

                _spellButtons[i].gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Called to desactivate buttons.
    /// </summary>
    private void DesactivateButtons()
    {
        for (int i = 0; i < _spellButtons.Length; i++)
        {
            _spellButtons[i].GetComponent<Button>().interactable = false;
        }
    }

    /// <summary>
    /// Called to reactivate buttons.
    /// </summary>
    private void ReactivateButtons()
    {
        for (int i = 0; i < _spellButtons.Length; i++)
        {
            _spellButtons[i].GetComponent<Button>().interactable = true;
        }
    }

    /// <summary>
    /// Called to hide every spell buttons.
    /// </summary>
    private void HideButtons()
    {
        for (int i = 0; i < _spellButtons.Length; i++)
        {
            _spellButtons[i].gameObject.SetActive(false);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndTurnButton : MonoBehaviour
{
    /// <summary>
    /// Buttons to end the turn.
    /// </summary>
    [SerializeField]
    private Button _endTurnButton;

    private void Start()
    {
        BattleManager.Instance.AllEntitiesInit += InitialiseEndTurnButton;
    }

    /// <summary>
    /// Called to initialise the button.
    /// </summary>
    private void InitialiseEndTurnButton()
    {
        for (int i = 0; i < BattleManager.Instance.PlayableEntitiesInBattle.Count; i++)
        {
            PlayerMain playableEntity = (PlayerMain)BattleManager.Instance.PlayableEntitiesInBattle[i];

            playableEntity.StateMachine.ActiveState.TurnStarted += ShowButton;
            playableEntity.StateMachine.ActiveState.TurnEnded += HideButton;
            playableEntity.StartAttacking += DesactivateButton;
            playableEntity.StopAttacking += ReactivateButton;
            playableEntity.StartMoving += DesactivateButton;
            playableEntity.StopMoving += ReactivateButton;
        }
    }

    /// <summary>
    /// Called to desactivate the button.
    /// </summary>
    private void DesactivateButton()
    {
        // Makes text transparent
        TextMeshProUGUI text = _endTurnButton.GetComponentInChildren<TextMeshProUGUI>();
        text.color = new Color(text.color.r, text.color.g, text.color.b, 25f / 255f);

        _endTurnButton.interactable = false;
    }

    /// <summary>
    /// Called to reactivate the button.
    /// </summary>
    private void ReactivateButton()
    {
        // Makes text visible
        TMP_Text text = _endTurnButton.GetComponentInChildren<TMP_Text>();
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1f);

        _endTurnButton.interactable = true;
    }


    /// <summary>
    /// Called to show the button.
    /// </summary>
    private void ShowButton()
    {
        _endTurnButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// Called to hide the button.
    /// </summary>
    private void HideButton()
    {
        _endTurnButton.gameObject.SetActive(false);
    }
}

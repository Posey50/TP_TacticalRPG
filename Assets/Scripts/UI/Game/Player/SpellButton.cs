using UnityEngine;

public class SpellButton : MonoBehaviour
{
    /// <summary>
    /// Spell currently attached to this button.
    /// </summary>
    public Spell Spell { get; set; }

    /// <summary>
    /// called when a spell button is clicked.
    /// </summary>
    public void SetCurrentSpellSelected()
    {
        if (Spell != null)
        {
            if (BattleManager.Instance.CurrentActiveEntity.TryGetComponent<PlayerMain>(out PlayerMain playerMain))
            {
                if (!playerMain.IsInAction)
                {
                    if (playerMain.Actions.SelectedSpell == Spell)
                    {
                        playerMain.Actions.UnselectSpell();
                    }
                    else
                    {
                        playerMain.Actions.UpdateSelectedSpell(Spell);
                    }
                }
            }
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

public class Actions : MonoBehaviour
{
    /// <summary>
    /// Main component of the playable entity.
    /// </summary>
    public PlayerMain PlayerMain {  get; private set; }
    /// <summary>
    /// Spell currently selected.
    /// </summary>
    public Spell SelectedSpell {  get; set; }

    /// <summary>
    /// Current range where the player can attack.
    /// </summary>
    public List<Square> CurrentRange {  get; set; }

    private void Start()
    {
        PlayerMain = GetComponent<PlayerMain>();
    }

    /// <summary>
    /// Called when a new spell is selected.
    /// </summary>
    /// <param name="spell"></param>
    public void UpdateSelectedSpell(Spell spell)
    {
        UnselectSpell();

        SelectedSpell = spell;

        PlayerMain.Cursor.UnselectSquareForPath();

        CurrentRange = AStarManager.Instance.CalculateRange(PlayerMain.SquareUnderTheEntity, spell.SpellDatas.Range);

        HighlightGroundManager.Instance.ShowRange(CurrentRange);
    }

    /// <summary>
    /// Called to unselect the current selected spell.
    /// </summary>
    public void UnselectSpell()
    {
        SelectedSpell = null;

        HighlightGroundManager.Instance.HideRange();
    }
}

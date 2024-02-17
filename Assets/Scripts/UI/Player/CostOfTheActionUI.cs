using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CostOfTheActionUI : MonoBehaviour
{
    /// <summary>
    /// Cost to show on screen.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _textCost;

    /// <summary>
    /// The object that show cost of actions on screen.
    /// </summary>
    [SerializeField]
    private GameObject _costOfActionObject;

    private void Start()
    {
        BattleManager.Instance.AllEntitiesInit += InitialiseCostActionUI;
    }

    /// <summary>
    /// Called to initialise the Cost action UI.
    /// </summary>
    private void InitialiseCostActionUI()
    {
        for (int i = 0; i < BattleManager.Instance.PlayableEntitiesInBattle.Count; i++)
        {
            PlayerMain entity = (PlayerMain)BattleManager.Instance.PlayableEntitiesInBattle[i];

            entity.StateMachine.ActiveState.TurnStarted += ActiveUI;
            entity.StateMachine.ActiveState.TurnEnded += DesactiveUI;
            entity.Cursor.SelectedSquareChanged += UpdateAPCost;
            entity.Cursor.PathChanged += UpdateMPCost;
        }
    }

    /// <summary>
    /// Called to update the cost in AP of a selected attack on an entity
    /// </summary>
    /// <param name="square"> Square selected. </param>
    private void UpdateAPCost(Square square)
    {
        _textCost.text = "";

        if (square != null)
        {
            PlayerMain activeEntity = (PlayerMain)BattleManager.Instance.CurrentActiveEntity;

            if (activeEntity.Actions.SelectedSpell != null)
            {
                if (activeEntity.Actions.SelectedSpell.SpellDatas != null)
                {
                    if (square.EntityOnThisSquare != null)
                    {
                        if (activeEntity.Actions.CurrentRange != null)
                        {
                            if (activeEntity.Actions.CurrentRange.Count > 0)
                            {
                                if (activeEntity.Actions.CurrentRange.Contains(square))
                                {
                                    int APCost = activeEntity.Actions.SelectedSpell.SpellDatas.APCost;
                                    _textCost.text = "- " + APCost.ToString() + " AP";
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Called to update the cost in MP of a path selected.
    /// </summary>
    /// <param name="path"> Path selected. </param>
    private void UpdateMPCost(List<Square> path)
    {
        _textCost.text = "";

        if (path != null)
        {
            if (path.Count > 0)
            {
                int MPCost = path.Count;
                _textCost.text = "- " + MPCost.ToString() + " MP";
            }
        }
    }

    /// <summary>
    /// Called to active UI.
    /// </summary>
    public void ActiveUI()
    {
        _costOfActionObject.SetActive(true);
    }

    /// <summary>
    /// Called to desactive UI.
    /// </summary>
    public void DesactiveUI()
    {
        _costOfActionObject.SetActive(false);
    }
}

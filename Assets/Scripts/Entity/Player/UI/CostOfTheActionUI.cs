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

            entity.Cursor.SelectedSquareChanged += UpdateCost;
            entity.StateMachine.ActiveState.TurnStarted += ActiveUI;
            entity.StateMachine.ActiveState.TurnEnded += DesactiveUI;
        }
    }

    private void UpdateCost(Square square)
    {
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
                                else
                                {
                                    _textCost.text = "";
                                }
                            }
                            else
                            {
                                _textCost.text = "";
                            }
                        }
                        else
                        {
                            _textCost.text = "";
                        }
                    }
                    else
                    {
                        _textCost.text = "";
                    }
                }
                else
                {
                    _textCost.text = "";
                }
            }
            else
            {
                if (activeEntity.Cursor.Path != null)
                {
                    if (activeEntity.Cursor.Path.Count > 0)
                    {
                        int MPCost = activeEntity.Cursor.Path.Count;
                        _textCost.text = "- " + MPCost.ToString() + " MP";
                    }
                    else
                    {
                        _textCost.text = "";
                    }
                }
                else
                {
                    _textCost.text = "";
                }
            }
        }
        else
        {
            _textCost.text = "";
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

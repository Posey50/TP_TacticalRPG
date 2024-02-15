using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIEntitiesActionOrder : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> _listTextEntitesOrder;
    [SerializeField] private List<TextMeshProUGUI> _listTextEntites;
    public void Start()
    {
        BattleManager.Instance.UpadateUIEntitiesActionOrder += Notify;
    }

    /// <summary>
    /// Update text of entities list in their order of action.
    /// </summary>
    public void Notify(List<Entity> entities)
    {
        if (_listTextEntitesOrder.Count <= 1)
        {
            _listTextEntitesOrder = new List<TextMeshProUGUI>();
            _listTextEntitesOrder.AddRange(_listTextEntites);
        }

        for (int i = 0; i < _listTextEntitesOrder.Count; i++)
        {
            if (_listTextEntitesOrder.Count > entities.Count)
            {

                int removeLastText = _listTextEntitesOrder.Count - 1;
                _listTextEntitesOrder[removeLastText].gameObject.SetActive(false);
                _listTextEntitesOrder.Remove(_listTextEntitesOrder[removeLastText]);
            }
            else
            {
                _listTextEntitesOrder[i].gameObject.SetActive(true);
            }

            _listTextEntitesOrder[i].text = (i + 1) + " " + entities[i].Name.ToString();
        }
    }
}
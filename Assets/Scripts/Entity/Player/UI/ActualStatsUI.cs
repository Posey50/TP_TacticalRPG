using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class ActualStatsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textActualMp;
    [SerializeField] private TextMeshProUGUI _textActualAp;
    [SerializeField] private GameObject _dataPlayers;
    private Entity _entity;

    void Start()
    {
        _entity = GetComponent<Entity>();
        _entity.MPChanged += NotifyUdateMP;
        _entity.APChanged += NotifyUdatePA;

        BattleManager.Instance.AllEntitiesInit += CheckPlayableEntity;
    }
    private void CheckPlayableEntity()
    {
        for (int i = 0; i < BattleManager.Instance.PlayableEntitiesInBattle.Count; i++)
        {
            PlayerMain playableEntity = (PlayerMain)BattleManager.Instance.PlayableEntitiesInBattle[i];

            playableEntity.StateMachine.ActiveState.TurnStarted += ActiveUI;
            playableEntity.StateMachine.ActiveState.TurnEnded += DesactiveUI;
        }
    }
    public void NotifyUdatePA(int newAP)
    {
        _textActualAp.text = "AP : " + newAP.ToString();
    }

    public void NotifyUdateMP(int newMP)
    {
        _textActualMp.text = "MP : " + newMP.ToString();
    }

    public void ActiveUI()
    {
        _dataPlayers.SetActive(true);
    }
    public void DesactiveUI()
    {
        _dataPlayers.SetActive(false);
    }
}

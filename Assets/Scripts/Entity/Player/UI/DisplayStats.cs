using TMPro;
using UnityEngine;

public class DisplayStats : MonoBehaviour
{
    // Actual MP showed on screen.
    [SerializeField]
    private TextMeshProUGUI _textActualMp;

    // Actual AP showed on screen.
    [SerializeField] 
    private TextMeshProUGUI _textActualAp;

    // Datas of the player showed on screen.
    [SerializeField] 
    private GameObject _datasPlayer;

    private void Start()
    {
        BattleManager.Instance.AllEntitiesInit += InitialiseDisplayStats;
    }

    /// <summary>
    /// Called to initialise the display.
    /// </summary>
    private void InitialiseDisplayStats()
    {
        for (int i  = 0; i < BattleManager.Instance.PlayableEntitiesInBattle.Count; i++)
        {
            PlayerMain entity = (PlayerMain)BattleManager.Instance.PlayableEntitiesInBattle[i];

            entity.MPChanged += NotifyUdateMP;
            entity.APChanged += NotifyUdateAP;
            entity.StateMachine.ActiveState.TurnStarted += ActiveUI;
            entity.StateMachine.ActiveState.TurnEnded += DesactiveUI;
        }
    }

    /// <summary>
    /// Called to update AP on sceen.
    /// </summary>
    /// <param name="newAP"> New ammount of AP. </param>
    public void NotifyUdateAP(int newAP)
    {
        _textActualAp.text = "AP : " + newAP.ToString();
    }

    /// <summary>
    /// Called to update MP on sceen.
    /// </summary>
    /// <param name="newMP"> New ammount of HP. </param>
    public void NotifyUdateMP(int newMP)
    {
        _textActualMp.text = "MP : " + newMP.ToString();
    }

    /// <summary>
    /// Called to active the UI.
    /// </summary>
    public void ActiveUI()
    {
        _datasPlayer.SetActive(true);
    }

    /// <summary>
    /// Called to desactive the UI.
    /// </summary>
    public void DesactiveUI()
    {
        _datasPlayer.SetActive(false);
    }
}

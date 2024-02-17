using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    /// <summary>
    /// Health gauge.
    /// </summary>
    [SerializeField]
    private Image _frontHealthImage;

    /// <summary>
    /// Back health gauge for game feel.
    /// </summary>
    [SerializeField]
    private Image _backHealthImage;

    /// <summary>
    /// Color of the gauge when entity is heal.
    /// </summary>
    [SerializeField]
    private Color _backHealColor;

    /// <summary>
    /// Default color of the gauge.
    /// </summary>
    [SerializeField]
    private Color _backDmgColor;

    /// <summary>
    /// Time of the effect.
    /// </summary>
    [SerializeField]
    private float _tweenDuration;

    /// <summary>
    /// Delay for the game feel.
    /// </summary>
    [SerializeField]
    private float _tweenInterval;

    /// <summary>
    /// Text to show damages or heal.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _txtDmgHeal;

    /// <summary>
    /// Entity attached to this gauge.
    /// </summary>
    private Entity _entity;

    private void Start()
    {
        _txtDmgHeal.gameObject.SetActive(false);
        _entity = GetComponentInParent<Entity>();
        _entity.DamageReceived += OnDamageTaken;
        _entity.HealReceived += OnHealTaken;
    }

    /// <summary>
    /// Called to empty the HP gauge with an amount of damages.
    /// </summary>
    /// <param name="dmgTaken"> Damages received. </param>
    private void OnDamageTaken(int dmgTaken)
    {
        _backHealthImage.color = _backDmgColor; 
        float targetFillAmount = Mathf.InverseLerp(0, _entity.EntityDatas.MaxHP, _entity.HP);

        StartCoroutine(SetTxtDmg(dmgTaken));

        // Emptys the gauge with damages
        DOTween.Sequence()
            .Append(_frontHealthImage.DOFillAmount(targetFillAmount, _tweenDuration / 2f).SetEase(Ease.OutQuint))
            .AppendInterval(_tweenInterval)
            .Append(_backHealthImage.DOFillAmount(targetFillAmount, _tweenDuration).SetEase(Ease.OutQuint)).WaitForCompletion();
    }
    
    /// <summary>
    /// Called to fill the HP gauge with an amount of heal.
    /// </summary>
    /// <param name="healReceived"> Heal received. </param>
    private void OnHealTaken(int healReceived)
    {
        _backHealthImage.color = _backHealColor;
        float targetFillAmount = Mathf.InverseLerp(0, _entity.EntityDatas.MaxHP, _entity.HP);

        StartCoroutine(SetTxtHeal(healReceived));

        // Fills the gauge with heal
        DOTween.Sequence()
            .Append(_backHealthImage.DOFillAmount(targetFillAmount, _tweenDuration / 2f).SetEase(Ease.OutQuint))
            .AppendInterval(_tweenInterval)
            .Append(_frontHealthImage.DOFillAmount(targetFillAmount, _tweenDuration).SetEase(Ease.OutQuint)).WaitForCompletion();
    }

    /// <summary>
    /// Called to indicate damages received above the gauge.
    /// </summary>
    /// <param name="damagesReceived"> Damages received. </param>
    /// <returns></returns>
    private IEnumerator SetTxtDmg(int damagesReceived)
    {
        // Activates text
        _txtDmgHeal.gameObject.SetActive(true);
        _txtDmgHeal.color = _backDmgColor;
        _txtDmgHeal.SetText("- " + damagesReceived);

        // Waits animation
        yield return new WaitForSecondsRealtime(_txtDmgHeal.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);

        // Desctivates text
        _txtDmgHeal.gameObject.SetActive(false);

        // Indicates to the current entity that the action is ending
        BattleManager.Instance.CurrentActiveEntity.EndOfTheAttack();
    }

    /// <summary>
    /// Called to indicate heal received above the gauge.
    /// </summary>
    /// <param name="heal"> Heal received. </param>
    /// <returns></returns>
    private IEnumerator SetTxtHeal(int heal)
    {
        // Activates text
        _txtDmgHeal.gameObject.SetActive(true);
        _txtDmgHeal.color = _backHealColor;
        _txtDmgHeal.SetText("- " + heal);

        // Waits animation
        yield return new WaitForSecondsRealtime(_txtDmgHeal.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);

        // Desctivates text
        _txtDmgHeal.gameObject.SetActive(false);

        // Indicates to the current entity that the action is ending
        BattleManager.Instance.CurrentActiveEntity.EndOfTheAttack();
    }
}

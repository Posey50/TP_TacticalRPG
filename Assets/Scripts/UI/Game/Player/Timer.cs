using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    /// <summary>
    /// Time in seconds.
    /// </summary>
    [SerializeField]
    private float _time;

    /// <summary>
    /// Seconds on screen.
    /// </summary>
    [SerializeField]
    private TMP_Text _seconds;

    /// <summary>
    /// Value representing seconds lefts.
    /// </summary>
    private float _nbrOfSeconds;

    /// <summary>
    /// Gauge of time.
    /// </summary>
    [SerializeField]
    private Image _timeGauge;

    /// <summary>
    /// Red color of the gauge.
    /// </summary>
    [SerializeField]
    private Color _red;

    /// <summary>
    /// Yellow color of the gauge.
    /// </summary>
    [SerializeField]
    private Color _yellow;

    /// <summary>
    /// Green color of the gauge.
    /// </summary>
    [SerializeField]
    private Color _green;

    /// <summary>
    /// Coroutine that executes itself at each second.
    /// </summary>
    private Coroutine _decrementTimer;

    /// <summary>
    /// The timer game object.
    /// </summary>
    [SerializeField]
    private GameObject _timer;

    // Event for the timer
    public delegate void TimerDelegate();

    public event TimerDelegate TimerStop;

    private void Start()
    {
        BattleManager.Instance.AllEntitiesInit += Initialisetimer;
    }

    /// <summary>
    /// Called to initialise the timer.
    /// </summary>
    private void Initialisetimer()
    {
        for (int i = 0; i < BattleManager.Instance.PlayableEntitiesInBattle.Count; i++)
        {
            PlayerMain playableEntity = (PlayerMain)BattleManager.Instance.PlayableEntitiesInBattle[i];

            playableEntity.StateMachine.ActiveState.TurnStarted += LaunchTimer;
            playableEntity.StateMachine.ActiveState.TurnEnded += DesactivateTimer;
            playableEntity.StartMoving += PauseTimer;
            playableEntity.StartAttacking += PauseTimer;
            playableEntity.StopMoving += RelaunchTimer;
            playableEntity.StopAttacking += RelaunchTimer;
        }
    }

    /// <summary>
    /// Called to launch the timer.
    /// </summary>
    private void LaunchTimer()
    {
        _timer.SetActive(true);

        _nbrOfSeconds = _time;

        _seconds.SetText(ConvertToString(_nbrOfSeconds));
        _timeGauge.fillAmount = ConvertToPercent(_nbrOfSeconds);
        _timeGauge.color = ColorOfTheGauge(_timeGauge.fillAmount);

        _decrementTimer = StartCoroutine(DecrementChrono());
    }

    /// <summary>
    /// Called to pause the timer.
    /// </summary>
    private void PauseTimer()
    {
        if (_decrementTimer != null)
        {
            StopCoroutine(_decrementTimer);
        }
    }

    /// <summary>
    /// Called to relaunch the timer.
    /// </summary>
    private void RelaunchTimer()
    {
        _decrementTimer = StartCoroutine(DecrementChrono());
    }

    /// <summary>
    /// Called to desactivate the timer.
    /// </summary>
    private void DesactivateTimer()
    {
        if (_decrementTimer != null)
        {
            StopCoroutine(_decrementTimer);
        }

        _timer.SetActive(false);
    }

    /// <summary>
    /// Converts time left into string that can be showed on screen.
    /// </summary>
    /// <param name="time"> Time to convert. </param>
    /// <returns></returns>
    private string ConvertToString(float time)
    {
        if (time >= 10f)
        {
            return time.ToString();
        }
        else
        {
            return $"{0}{time}";
        }
    }

    /// <summary>
    /// Converts actual into percents.
    /// </summary>
    /// <param name="actualTime"> Actual time. </param>
    /// <returns></returns>
    private float ConvertToPercent(float actualTime)
    {
        return actualTime / _time;
    }

    /// <summary>
    /// Return the color of the gauge depending of the percentage
    /// </summary>
    private Color ColorOfTheGauge(float percentage)
    {
        if (percentage >= 0.5f)
        {
            return _green;
        }
        else if (percentage < 0.2f)
        {
            return _red;
        }
        else
        {
            return _yellow;
        }
    }

    /// <summary>
    /// Called at eahc second to decrement timer.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DecrementChrono()
    {
        yield return new WaitForSeconds(1f);

        if (_nbrOfSeconds - 1f == -1f)
        {
            _nbrOfSeconds = 0f;

            _seconds.SetText(ConvertToString(_nbrOfSeconds));
            _timeGauge.fillAmount = ConvertToPercent(_nbrOfSeconds);
            _timeGauge.color = ColorOfTheGauge(_timeGauge.fillAmount);

            StopTimer();
        }
        else
        {
            _nbrOfSeconds -= 1f;

            _seconds.SetText(ConvertToString(_nbrOfSeconds));
            _timeGauge.fillAmount = ConvertToPercent(_nbrOfSeconds);
            _timeGauge.color = ColorOfTheGauge(_timeGauge.fillAmount);

            _decrementTimer = StartCoroutine(DecrementChrono());
        }
    }

    /// <summary>
    /// Called to stop the chrono.
    /// </summary>
    private void StopTimer()
    {
        // Anounces that the timer is stoped
        TimerStop?.Invoke();

        // Stops the timer
        StopCoroutine(_decrementTimer);
    }
}

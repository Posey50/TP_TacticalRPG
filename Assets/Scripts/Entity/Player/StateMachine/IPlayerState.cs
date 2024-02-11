public interface IPlayerState
{
    /// <summary>
    /// Called when the playable entity enters in this state.
    /// </summary>
    /// <param name="playerStateMachine"> State machine of the playable entity. </param>
    public void OnEnter(PlayerStateMachine playerStateMachine);

    /// <summary>
    /// Called when the playable entity exits in this state.
    /// </summary>
    /// <param name="playerStateMachine"> State machine of the playable entity. </param>
    public void OnExit(PlayerStateMachine playerStateMachine);
}

using System.Collections.Generic;
using UnityEngine;

public class BattleSteps : MonoBehaviour
{
    /// <summary>
    /// Called at the start of the game to choose random playable entities and place them in the battle.
    /// </summary>
    public void PlacePlayers()
    {
        int teamSize = BattleManager.Instance.TeamsSize;
        List<Entity> playableEntitiesInBattle = BattleManager.Instance.PlayableEntitiesInBattle;
        List<Entity> playableEntitiesInGame = GameManager.Instance.PlayableEntitiesInGame;
        List<Square> playerSquares = BattleManager.Instance.PlayerSquares;

        for (int i = 0; i < teamSize; i++)
        {
            Entity playableEntityToAdd = playableEntitiesInGame[Random.Range(0, playableEntitiesInGame.Count)];

            playableEntitiesInGame.Remove(playableEntityToAdd);
            playableEntitiesInBattle.Add(playableEntityToAdd);

            playableEntityToAdd.transform.position = playerSquares[Random.Range(0, playerSquares.Count)].transform.position;
        }
    }

    /// <summary>
    /// Called at the start of the game to choose random enemies and place them in the battle.
    /// </summary>
    public void PlaceEnnemies()
    {
        int teamSize = BattleManager.Instance.TeamsSize;
        List<Entity> enemiesInBattle = BattleManager.Instance.EnemiesInBattle;
        List<Entity> enemiesInGame = GameManager.Instance.EnemiesInGame;
        List<Square> enemiesSquares = BattleManager.Instance.EnemiesSquares;

        for (int i = 0; i < teamSize; i++)
        {
            Entity enemiesToAdd = enemiesInGame[Random.Range(0, enemiesInGame.Count)];

            enemiesInGame.Remove(enemiesToAdd);
            enemiesInBattle.Add(enemiesToAdd);

            enemiesToAdd.transform.position = enemiesSquares[Random.Range(0, enemiesSquares.Count)].transform.position;
        }
    }

    /// <summary>
    /// Called when entities in the battle are chosen and initialises them.
    /// </summary>
    private void InitEntities(List<Entity> entities)
    {
        for (int i = 0; i < entities.Count; i++)
        {
            entities[i].InitEntity();
        }
    }

    /// <summary>
    /// return the list of entities given.
    /// </summary>
    /// <param name="entitiesInBattle"></param>
    /// <returns></returns>
    public List<Entity> DeterminesOrder(List <Entity> entitiesInBattle)
    {
        return null;
    }
}

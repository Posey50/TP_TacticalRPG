using System.Collections.Generic;
using System.Linq;
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

            // Gets the starting position for the entity
            Square squareForTheEntity = playerSquares[Random.Range(0, playerSquares.Count)];
            playerSquares.Remove(squareForTheEntity);

            // Sets the starting position for the entity
            playableEntityToAdd.SquareUnderTheEntity = squareForTheEntity;
            squareForTheEntity.SetEntity(playableEntityToAdd);
            playableEntityToAdd.transform.position = squareForTheEntity.transform.position;

            // Initialises the entity
            playableEntityToAdd.InitEntity();
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

            // Gets the starting position for the entity
            Square squareForTheEntity = enemiesSquares[Random.Range(0, enemiesSquares.Count)];
            enemiesSquares.Remove(squareForTheEntity);

            // Sets the starting position for the entity
            enemiesToAdd.SquareUnderTheEntity = squareForTheEntity;
            squareForTheEntity.SetEntity(enemiesToAdd);
            enemiesToAdd.transform.position = squareForTheEntity.transform.position;

            // Initialises the entity
            enemiesToAdd.InitEntity();
            enemiesToAdd.GetComponent<EnemyMain>().Brain.InitBrain();
        }
    }

    /// <summary>
    /// return the list of entities given in order of action.
    /// </summary>
    /// <param name="entitiesInBattle"> List of all entities in battle. </param>
    /// <returns></returns>
    public List<Entity> DeterminesOrder(List <Entity> entitiesInBattle)
    {
        return entitiesInBattle.OrderByDescending(entity => entity.Speed).ToList();
    }
}

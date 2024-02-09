using System.Collections.Generic;
using UnityEngine;

public class BattleSteps : MonoBehaviour
{
    /// <summary>
    /// Called at the start of the game to choose random playable entities and place them in the battle.
    /// </summary>
    public void PlacePlayers()
    {

    }

    /// <summary>
    /// Called at the start of the game to choose random ennemies and place them in the battle.
    /// </summary>
    public void PlaceEnnemies()
    {

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

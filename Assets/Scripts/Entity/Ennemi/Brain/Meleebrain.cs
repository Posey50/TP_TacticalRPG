using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeleeBrain : Brain
{
    public override void InitBrain()
    {
        base.InitBrain();

        for (int i = 0; i < _spells.Count; i++)
        {
            if (_spells[i].SpellDatas.Type != Type.melee)
            {
                Debug.LogError(_spells[i].SpellDatas.Name + " is not a valid spell for " + _enemyMain.Name);
            }
        }
    }

    public override IEnumerator EnemyPattern()
    {
        Debug.Log(_enemyMain.Name + " is reflecting");

        // Waits to simulate reflexion 
        yield return new WaitForSeconds(3f);

        // Sorts spells by descending order of their range
        _spells.OrderByDescending(spell => spell.SpellDatas.MaxRange);

        // Gets a copy of the list of spells of the enemy in descending order of their range
        List<Spell> spells = new(_spells);

        // Creates a dictionary which will stocks reachable playable entities with a score of priority calculated with percentage of missing HP
        // and multiplied by a proximity coefficient
        Dictionary<Entity, int> playableEntities = EnemiesByOrderOfPriority(PlayableEntitiesByOrderOfDistance(true));

        if (playableEntities.Count > 0)
        {
            // Gets the playable entity to attack
            Entity playableEntityToAttack = playableEntities.ElementAt(0).Key;

            // Gets the best spell to use on the playable entity to attack
            Spell bestSpellToUse = BestSpellToUse(spells, playableEntityToAttack);

            // Try to attack the playable entity
            yield return StartCoroutine(TryToAttack(playableEntityToAttack, bestSpellToUse));

            // If the melee has enough AP to use the cheapest spell
            if (_enemyMain.AP > _spells[^1].SpellDatas.PaCost)
            {
                // Restarts a patern
                StartCoroutine(EnemyPattern());
            }
            else
            {
                // If the melee has remained MP then he tries to move closer to the weaker playable entity
                if (_enemyMain.MP > 0)
                {
                    // Waits to simulate reflexion 
                    yield return new WaitForSeconds(3f);

                    yield return StartCoroutine(MoveCloserToTheWeakerPlayableEntity());
                }

                // Melee end his turn
                _enemyMain.EndOfTheTurn();
            }
        }
        else if (_enemyMain.MP > 0)
        {
            // Melee tries to move closer to the weaker playable entity
            yield return StartCoroutine(MoveCloserToTheWeakerPlayableEntity());

            // Melee ends his turn
            _enemyMain.EndOfTheTurn();
        }
        else
        {
            // Melee tries to move closer to the weaker playable entity
            _enemyMain.EndOfTheTurn();
        }
    }

    /// <summary>
    /// Called to get a dictionary of playable entities sorted in ascending order of their distance of the melee with a score of priority
    /// </summary>
    /// <returns></returns>
    private Dictionary<Entity, int> PlayableEntitiesByOrderOfDistance(bool itsToAttack)
    {
        // Creates a dictionary which will stocks playable entities with a score of priority
        Dictionary<Entity, int> playableEntities = new();
        List<Entity> playableEntitiesInBattle = new(BattleManager.Instance.PlayableEntitiesInBattle);

        if (playableEntitiesInBattle.Count > 0)
        {
            // For each playable entity
            for (int i = 0; i < playableEntitiesInBattle.Count; i++)
            {
                Entity playableEntity = playableEntitiesInBattle[i];

                // Calculates the shortest path between the melee and the playable entity
                List<Square> shortestPathToThePlayableEntity = AStarManager.Instance.CalculateShortestPathToAnEntity(_enemyMain.SquareUnderTheEntity, playableEntity.SquareUnderTheEntity);

                // If the playable entity is reachable with the attack that has the greatest range or if it's not for an attack
                if (!itsToAttack || shortestPathToThePlayableEntity.Count <= _enemyMain.MP + _spells[0].SpellDatas.MaxRange)
                {
                    // Add the playable entity to the list of playable entities
                    playableEntities.Add(playableEntity, shortestPathToThePlayableEntity.Count);
                }
            }

            if (playableEntities.Count > 1)
            {
                // Sorts playable entities by ascending order of their distance to the melee
                playableEntities = playableEntities.OrderBy(enemies => enemies.Value).ToDictionary(enemies => enemies.Key, enemies => 0);

                // Attributes a distance coefficient for each playable entity
                for (int i = 0; i < playableEntities.Count; i++)
                {
                    playableEntities[playableEntities.ElementAt(i).Key] = playableEntities.Count - i;
                }
            }
            else if (playableEntities.Count == 1)
            {
                // Resets score to 0
                playableEntities[playableEntities.ElementAt(0).Key] = 0;
            }
        }

        return playableEntities;
    }

    /// <summary>
    /// Called to get a dictionary of playable entities sorted in ascending order of their score calculated with percentage of missing HP and multiplied by a proximity coefficient.
    /// </summary>
    /// <param name="playableEntities"> Playable entities to sort. </param>
    /// <returns></returns>
    private Dictionary<Entity, int> EnemiesByOrderOfPriority(Dictionary<Entity, int> playableEntities)
    {
        if (playableEntities.Count > 0)
        {
            // For each playable entity
            for (int i = 0; i < playableEntities.Count; i++)
            {
                Entity playableEntity = playableEntities.ElementAt(i).Key;

                // Calculates the percentage of missing HP and multiplies with the distance coefficient
                playableEntities[playableEntity] = (int)(playableEntity.EntityDatas.MaxHP - playableEntity.HP / playableEntity.EntityDatas.MaxHP * 100f) * playableEntities[playableEntity];
            }

            if (playableEntities.Count > 1)
            {
                // Sorts playable entities by descending order of their score
                playableEntities = playableEntities.OrderByDescending(allies => allies.Value).ToDictionary(allies => allies.Key, allies => allies.Value);
            }
        }

        return playableEntities;
    }

    /// <summary>
    /// Called to get the best spell to use by anticipating a move in advance.
    /// </summary>
    /// <param name="spells"> List of spells. </param>
    /// <param name="playableEntityToAttack"> The playable entity to attack. </param>
    /// <returns></returns>
    private Spell BestSpellToUse(List<Spell> spells, Entity playableEntityToAttack)
    {
        if (spells.Count > 1)
        {
            // Gets a copy of spells
            List<Spell> spellsToCheck = new(spells);

            // Creates the dictionary which will stocks combinations of two spells with the percentage of damages on the enemy to attack
            Dictionary<List<Spell>, int> spellsCombinationInOrderOfDamages = new();

            // For each spell
            for (int i = 0; i < spells.Count; i++)
            {
                Spell currentStartingSpell = spells[i];

                // For each other spell which was not already used to start a combination
                for (int j = 0; j < spellsToCheck.Count; j++)
                {
                    Spell currentSpellToCheck = spellsToCheck[j];

                    // If the combination is a combination of the two same spells
                    if (currentStartingSpell == currentSpellToCheck)
                    {
                        // If the combination is possible
                        if (currentStartingSpell.SpellDatas.PaCost + currentSpellToCheck.SpellDatas.PaCost <= _enemyMain.AP)
                        {
                            // Calculates the percentage of damages that it represents
                            int percentageOfDamages = (int)(((currentStartingSpell.SpellDatas.Damages + currentSpellToCheck.SpellDatas.PaCost) / (playableEntityToAttack.EntityDatas.MaxHP - playableEntityToAttack.HP)) * 100f);

                            // Adds the combination
                            spellsCombinationInOrderOfDamages.Add(new List<Spell>() { currentStartingSpell, currentSpellToCheck }, percentageOfDamages);
                        }
                        // Checks if the spell is possible alone
                        else if (currentStartingSpell.SpellDatas.PaCost <= _enemyMain.AP)
                        {
                            // Calculates the percentage of damages that it represents
                            int percentageOfDamages = (int)((currentStartingSpell.SpellDatas.Damages / (playableEntityToAttack.EntityDatas.MaxHP - playableEntityToAttack.HP)) * 100f);

                            // Adds the spell as a combination
                            spellsCombinationInOrderOfDamages.Add(new List<Spell>() { currentStartingSpell }, percentageOfDamages);
                        }
                    }
                    // If it's not a combination of the two same spells and if the combination is possible
                    else if (currentStartingSpell.SpellDatas.PaCost + currentSpellToCheck.SpellDatas.PaCost <= _enemyMain.AP)
                    {
                        // Calculates the percentage of damages that it represents
                        int percentageOfDamages = (int)(((currentStartingSpell.SpellDatas.Damages + currentSpellToCheck.SpellDatas.Damages) / (playableEntityToAttack.EntityDatas.MaxHP - playableEntityToAttack.HP)) * 100f);

                        // Adds the combination
                        spellsCombinationInOrderOfDamages.Add(new List<Spell>() { currentStartingSpell, currentSpellToCheck }, percentageOfDamages);
                    }
                }

                // Removes the starting spell from the spell to check to prevent duplicates
                spellsToCheck.Remove(currentStartingSpell);
            }

            // Sorts combinations by descending order of their efficacity
            spellsCombinationInOrderOfDamages = spellsCombinationInOrderOfDamages.OrderByDescending(spell => spell.Value).ToDictionary(spell => spell.Key, spell => spell.Value);

            // Returns the best spell to use
            return spellsCombinationInOrderOfDamages.ElementAt(0).Key[0];
        }
        else
        {
            return spells[0];
        }
    }

    /// <summary>
    /// Called to try to attack a playable entity with a spell.
    /// </summary>
    /// <param name="playableEntityToAttack"> Playable entity to attack. </param>
    /// <param name="spellToUse"> Spell to use. </param>
    /// <returns></returns>
    private IEnumerator TryToAttack(Entity playableEntityToAttack, Spell spellToUse)
    {
        // Gets the range of the spell
        List<Square> range = RangeManager.Instance.CalculateRange(_enemyMain.SquareUnderTheEntity, spellToUse.SpellDatas.MinRange, spellToUse.SpellDatas.MaxRange);

        // Checks if the playable entity to attack is in the range
        if (range.Contains(playableEntityToAttack.SquareUnderTheEntity))
        {
            // Attacks the playable entity
            _enemyMain.Attack(spellToUse, playableEntityToAttack);
        }
        // If he is not
        else
        {
            // Calculates the path to go to the playable entity
            Square[] pathToPlayableEntity = AStarManager.Instance.CalculateShortestPathToAnEntity(_enemyMain.SquareUnderTheEntity, playableEntityToAttack.SquareUnderTheEntity).ToArray();

            // Reduces the path by one for the playable entity and by the range of the spell to save MP
            List<Square> pathToGetCloser = pathToPlayableEntity[..^((spellToUse.SpellDatas.MaxRange - 1) + 1)].ToList();

            // Wait until the melee has moved
            UniTask followingThePath = _enemyMain.FollowThePath(pathToGetCloser);
            yield return new WaitUntil(() => followingThePath.Status != UniTaskStatus.Pending);

            // Attacks the playable entity
            _enemyMain.Attack(spellToUse, playableEntityToAttack);
        }
    }

    /// <summary>
    /// Called to move to the weaker playable entity.
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveCloserToTheWeakerPlayableEntity()
    {
        // Creates a dictionary which will stocks reachable playable entities with a score of priority calculated with percentage of remaining HP
        // and multiplied by a proximity coefficient
        Dictionary<Entity, int> playableEntities = EnemiesByOrderOfPriority(PlayableEntitiesByOrderOfDistance(false));

        if (playableEntities.Count > 0)
        {
            // Gets the playable entity to attack
            Entity playableEntityToMoveTo = playableEntities.ElementAt(0).Key;

            // Calculates the path to go to the playable entity
            Square[] pathToPlayableEntity = AStarManager.Instance.CalculateShortestPathToAnEntity(_enemyMain.SquareUnderTheEntity, playableEntityToMoveTo.SquareUnderTheEntity).ToArray();

            // Reduces the path by one for the playable entity
            pathToPlayableEntity = pathToPlayableEntity[..^1];

            // The path to follow
            List<Square> pathToGetCloser;

            // Checks if the path is too long to reach the end
            if (pathToPlayableEntity.Length >= _enemyMain.MP)
            {
                // Reduces the path by the difference between the length of the path and the remaining MP
                pathToGetCloser = pathToPlayableEntity[..^(pathToPlayableEntity.Length - _enemyMain.MP)].ToList();
            }
            else
            {
                pathToGetCloser = pathToPlayableEntity.ToList();
            }

            if (pathToGetCloser.Count > 0)
            {
                // Wait until the melee has moved
                UniTask followingThePath = _enemyMain.FollowThePath(pathToGetCloser);
                yield return new WaitUntil(() => followingThePath.Status != UniTaskStatus.Pending);
            }
        }
    }
}

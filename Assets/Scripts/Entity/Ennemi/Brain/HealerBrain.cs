using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class HealerBrain : Brain
{
    public override void InitBrain()
    {
        base.InitBrain();

        for (int i = 0;  i < _spells.Count; i++)
        {
            if (_spells[i].SpellDatas.Type != Type.heal)
            {
                Debug.LogError(_spells[i].SpellDatas.Name + " is not a valid spell for " + _enemyMain.Name);
            }
        }
    }

    public override IEnumerator EnemyPattern()
    {
        // Waits to simulate reflexion 
        yield return new WaitForSeconds(3f);

        // Sorts spells by descending order of their range
        _spells.OrderByDescending(spell => spell.SpellDatas.MaxRange);

        // Gets a copy of the list of spells of the enemy in descending order of their range
        List<Spell> spells = new(_spells);

        // Creates a dictionary which will stocks reachable allies with a score of priority calculated with percentage of HP to be treated
        // and multiplied by a proximity coefficient
        Dictionary<Entity, int> allies = AlliesByOrderOfPriority(AlliesByOrderOfDistance());

        if (allies.Count > 0)
        {
            // Gets the ally to go help
            Entity allyToGoHelp = allies.ElementAt(0).Key;

            // Gets the best spell to use on the ally to go help
            Spell bestSpellToUse = BestSpellToUse(spells, allyToGoHelp);

            // Try to heal the ally
            yield return StartCoroutine(TryToHeal(allyToGoHelp, bestSpellToUse));

            // If the enemy has enough AP to use the cheapest spell
            if (_enemyMain.AP > _spells[^1].SpellDatas.PaCost)
            {
                // Restart a patern
                StartCoroutine(EnemyPattern());
            }
            else
            {
                // If the enemy has remained MP then he tries to escape as far away from an enemy as possible
                if (_enemyMain.MP > 0)
                {
                    // Waits to simulate reflexion 
                    yield return new WaitForSeconds(3f);

                    // Enemy tries to escape
                    yield return StartCoroutine(TryToEscape());
                }

                // Makes the turn end
                _enemyMain.EndOfTheTurn();
            }
        }
        else if (_enemyMain.MP > 0)
        {
            // Enemy tries to escape
            yield return StartCoroutine(TryToEscape());

            // Healer ends his turn
            _enemyMain.EndOfTheTurn();
        }
        else
        {
            // Healer ends his turn
            _enemyMain.EndOfTheTurn();
        }
    }


    /// <summary>
    /// Called to get a dictionary of allied enemies sorted in descending order of their distance of the enemy with a score of priority seted to 0
    /// </summary>
    /// <returns></returns>
    private Dictionary<Entity, int> AlliesByOrderOfDistance()
    {
        // Creates a dictionary which will stocks allies with a score of priority
        Dictionary<Entity, int> allies = new();
        List<Entity> alliesInBattle = new(BattleManager.Instance.EnemiesInBattle);

        // Removes itself of the enemies
        alliesInBattle.Remove(_enemyMain);

        if (alliesInBattle.Count > 0)
        {
            // For each allied enemy
            for (int i = 0; i < alliesInBattle.Count; i++)
            {
                Entity ally = alliesInBattle[i];

                // Calculate the shortest path between the enemy and his ally
                List<Square> shortestPathToTheAlly = AStarManager.Instance.CalculateShortestPathToAnEntity(_enemyMain.SquareUnderTheEntity, ally.SquareUnderTheEntity);

                // If the ally is reachable with the attack that has the greatest range
                if (shortestPathToTheAlly.Count <= _enemyMain.MP + _spells[0].SpellDatas.MaxRange)
                {
                    // Add the ally to the list of allies
                    allies.Add(ally, shortestPathToTheAlly.Count);
                }
            }

            if (allies.Count > 1)
            {
                // Sorts allies by descending order of their distance to the enemy by reseting score to 0
                allies = allies.OrderByDescending(allies => allies.Value).ToDictionary(allies => allies.Key, allies => 0);
            }
            else if (allies.Count == 1)
            {
                // Resets score to 0
                allies[allies.ElementAt(0).Key] = 0;
            }
        }

        return allies;
    }

    /// <summary>
    /// Called to get a dictionary of allied enemies sorted in descending order of their score calculated with percentage of HP to be treated and multiplied by a proximity coefficient.
    /// </summary>
    /// <param name="allies"> Allies to sort. </param>
    /// <returns></returns>
    private Dictionary<Entity, int> AlliesByOrderOfPriority(Dictionary<Entity, int> allies)
    {
        if (allies.Count > 0)
        {
            // For each allied enemy
            for (int i = 0; i < allies.Count; i++)
            {
                Entity ally = allies.ElementAt(i).Key;

                int missingHP = ally.EntityDatas.MaxHP - ally.HP;

                // Checks if there is hp to heal
                if (missingHP > 0)
                {
                    allies[ally] = (int)(missingHP / ally.EntityDatas.MaxHP * 100f) * i;
                }
                else
                {
                    allies.Remove(ally);
                }
            }

            if (allies.Count > 1)
            {
                // Sorts allies by descending order of their score
                allies = allies.OrderByDescending(allies => allies.Value).ToDictionary(allies => allies.Key, allies => allies.Value);
            }
        }

        return allies;
    }

    /// <summary>
    /// Called to get the best spell to use by anticipating a move in advance.
    /// </summary>
    /// <param name="spells"> List of spells. </param>
    /// <param name="allyToHelp"> The ally to help. </param>
    /// <returns></returns>
    private Spell BestSpellToUse(List<Spell> spells, Entity allyToHelp)
    {
        if (spells.Count > 1)
        {
            // Gets a copy of spells
            List<Spell> spellsToCheck = new(spells);

            // Creates the dictionary which will stocks combinations of two spells with the percentage of heal on the ally to go help
            Dictionary<List<Spell>, int> spellsCombinationInOrderOfHealing = new();

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
                            // Calculates the percentage of HP to heal that it represents
                            int percentageOfHeal = (int)(((currentStartingSpell.SpellDatas.Damages + currentSpellToCheck.SpellDatas.Damages) / (allyToHelp.EntityDatas.MaxHP - allyToHelp.HP)) * 100f);

                            // Adds the combination
                            spellsCombinationInOrderOfHealing.Add(new List<Spell>() { currentStartingSpell, currentSpellToCheck }, percentageOfHeal);
                        }
                        // Checks if the spell is possible alone
                        else if (currentStartingSpell.SpellDatas.PaCost <= _enemyMain.AP)
                        {
                            // Calculates the percentage of HP to heal that it represents
                            int percentageOfHeal = (int)((currentStartingSpell.SpellDatas.Damages / (allyToHelp.EntityDatas.MaxHP - allyToHelp.HP)) * 100f);

                            // Adds the spell as a combination
                            spellsCombinationInOrderOfHealing.Add(new List<Spell>() { currentStartingSpell }, percentageOfHeal);
                        }
                    }
                    // If it's not a combination of the two same spells and if the combination is possible
                    else if (currentStartingSpell.SpellDatas.PaCost + currentSpellToCheck.SpellDatas.PaCost <= _enemyMain.AP)
                    {
                        // Calculates the percentage of HP to heal that it represents
                        int percentageOfHeal = (int)(((currentStartingSpell.SpellDatas.Damages + currentSpellToCheck.SpellDatas.Damages) / (allyToHelp.EntityDatas.MaxHP - allyToHelp.HP)) * 100f);

                        // Adds the combination
                        spellsCombinationInOrderOfHealing.Add(new List<Spell>() { currentStartingSpell, currentSpellToCheck }, percentageOfHeal);
                    }
                }

                // Removes the starting spell from the spell to check to prevent duplicates
                spellsToCheck.Remove(currentStartingSpell);
            }

            // Sorts combinations by descending order of their efficacity
            spellsCombinationInOrderOfHealing = spellsCombinationInOrderOfHealing.OrderByDescending(spell => spell.Value).ToDictionary(spell => spell.Key, spell => spell.Value);

            // Returns the best spell to use
            return spellsCombinationInOrderOfHealing.ElementAt(0).Key[0];
        }
        else
        {
            return spells[0];
        }
    }

    /// <summary>
    /// Called to try to heal an ally with a spell.
    /// </summary>
    /// <param name="allyToHeal"> Ally to heal. </param>
    /// <param name="spellToUse"> Spell to use. </param>
    /// <returns></returns>
    private IEnumerator TryToHeal(Entity allyToHeal, Spell spellToUse)
    {
        // Gets the range of the spell
        List<Square> range = RangeManager.Instance.CalculateSimpleRange(_enemyMain.SquareUnderTheEntity, spellToUse.SpellDatas.MaxRange);

        // Checks if the ally to heal is in the range
        if (range.Contains(allyToHeal.SquareUnderTheEntity))
        {
            // Heals the ally
            _enemyMain.Attack(spellToUse, allyToHeal);
        }
        // If he is not
        else
        {
            // Calculates the path to go to the ally
            Square[] pathToAlly = AStarManager.Instance.CalculateShortestPathToAnEntity(_enemyMain.SquareUnderTheEntity, allyToHeal.SquareUnderTheEntity).ToArray();

            // Reduces the path by one for the ally and by the range of the spell to save MP
            List<Square> pathToGetCloser = pathToAlly[..^((spellToUse.SpellDatas.MaxRange - 1) + 1)].ToList();

            // Wait until the healer has moved
            UniTask followingThePath = _enemyMain.FollowThePath(pathToGetCloser);
            yield return new WaitUntil(() => followingThePath.Status != UniTaskStatus.Pending);

            // Heals the ally
            _enemyMain.Attack(spellToUse, allyToHeal);
        }
    }

    /// <summary>
    /// // Called to try to find a position at a certain distance from any enemy entity
    /// </summary>
    private IEnumerator TryToEscape()
    {
        // All possible positions with remaining MP
        List<Square> possiblePositions = RangeManager.Instance.CalculateSimpleRange(_enemyMain.SquareUnderTheEntity, _enemyMain.MP);

        // All playable entities in battle
        List<Entity> playableEntitiesInBattle = BattleManager.Instance.PlayableEntitiesInBattle;

        // Value indicating that the healer has found a safe place
        bool HasFoundASafePlace = false;

        // Tries to find a safe position from the maximum distance to the minimum
        for (int i = 0; i < _enemyMain.MP; i++)
        {
            int minDistance = _enemyMain.MP - i;

            // For each possible position
            for (int j = 0; j < possiblePositions.Count; j++)
            {
                Square position = possiblePositions[j];
                bool isThisPositionSafe = true;

                // If there is no enemy on it
                if (position.EntityOnThisSquare == null)
                {
                    // For each enemy entity
                    for (int k = 0; k < playableEntitiesInBattle.Count; k++)
                    {                       
                        // Calculates distance from the entity
                        Entity enemyEntity = playableEntitiesInBattle[k];
                        List<Square> path = AStarManager.Instance.CalculateASimpleDistance(position, enemyEntity.SquareUnderTheEntity);
                        int distanceFromThisEnemyEntity = path.Count;

                        // If the distance is lower than the minimum distance then finds another position
                        if (distanceFromThisEnemyEntity < minDistance)
                        {
                            isThisPositionSafe = false;
                            break;
                        }
                    }
                }
                else
                {
                    isThisPositionSafe = false;
                }

                // If the position is safe then go this position
                if (isThisPositionSafe)
                {
                    HasFoundASafePlace = true;
                    List<Square> path = AStarManager.Instance.CalculateShortestPathForAMovement(_enemyMain.SquareUnderTheEntity, position);

                    // Wait until the healer has moved
                    UniTask followingThePath = _enemyMain.FollowThePath(path);
                    yield return new WaitUntil(() => followingThePath.Status != UniTaskStatus.Pending);
                    break;
                }
            }

            // If the healer has found a safe place then stops the calculation
            if (HasFoundASafePlace)
            {
                break;
            }
        }
    }
}

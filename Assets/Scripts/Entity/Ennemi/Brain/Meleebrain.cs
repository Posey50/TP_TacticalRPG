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

        // Creates a dictionary which will stocks reachable enemies with a score of priority calculated with percentage of remaining HP
        // and multiplied by a proximity coefficient
        Dictionary<Entity, int> enemies = EnemiesByOrderOfPriority(EnemiesByOrderOfDistance(true));

        if (enemies.Count > 0)
        {
            // Gets the enemy to attack
            Entity enemyToAttack = enemies.ElementAt(0).Key;

            // Gets the best spell to use on the ally to go help
            Spell bestSpellToUse = BestSpellToUse(spells, enemyToAttack);

            // Try to attack the enemy
            yield return StartCoroutine(TryToAttack(enemyToAttack, bestSpellToUse));

            // If the entity has enough AP to use the cheapest spell
            if (_enemyMain.AP > _spells[^1].SpellDatas.PaCost)
            {
                // Restart a patern
                StartCoroutine(EnemyPattern());
            }
            else
            {
                // If the entity has remained MP then he tries to move to move closer to the weaker player
                if (_enemyMain.MP > 0)
                {
                    // Waits to simulate reflexion 
                    yield return new WaitForSeconds(3f);

                    yield return StartCoroutine(MoveCloserToTheWeaker());
                }

                // Makes the turn end
                _enemyMain.EndOfTheTurn();
            }
        }
        else if (_enemyMain.MP > 0)
        {
            // Enemy tries to move closer to the weaker player
            yield return StartCoroutine(MoveCloserToTheWeaker());

            // Enemy ends his turn
            _enemyMain.EndOfTheTurn();
        }
        else
        {
            // Enemy tries to move closer to the weaker player
            _enemyMain.EndOfTheTurn();
        }
    }

    /// <summary>
    /// Called to get a dictionary of enemies sorted in descending order of their distance of the enemy with a score of priority seted to 0
    /// </summary>
    /// <returns></returns>
    private Dictionary<Entity, int> EnemiesByOrderOfDistance(bool itsToAttack)
    {
        // Creates a dictionary which will stocks enemies with a score of priority
        Dictionary<Entity, int> enemies = new();
        List<Entity> enemiesInBattle = new(BattleManager.Instance.PlayableEntitiesInBattle);

        if (enemiesInBattle.Count > 0)
        {
            // For each enemy
            for (int i = 0; i < enemiesInBattle.Count; i++)
            {
                Entity enemy = enemiesInBattle[i];

                // Calculates the shortest path between the entity and the enemy
                List<Square> shortestPathToTheEnemy = AStarManager.Instance.CalculateShortestPathToAnEntity(_enemyMain.SquareUnderTheEntity, enemy.SquareUnderTheEntity);

                // If the enemy is reachable with the attack that has the greatest range or if it's not for an attack
                if (!itsToAttack || shortestPathToTheEnemy.Count <= _enemyMain.MP + _spells[0].SpellDatas.MaxRange)
                {
                    // Add the enemy to the list of enemies
                    enemies.Add(enemy, shortestPathToTheEnemy.Count);
                }
            }

            if (enemies.Count > 1)
            {
                // Sorts enemies by descending order of their distance to the entity by reseting score to 0
                enemies = enemies.OrderByDescending(enemies => enemies.Value).ToDictionary(enemies => enemies.Key, enemies => 0);
            }
            else if (enemies.Count == 1)
            {
                // Resets score to 0
                enemies[enemies.ElementAt(0).Key] = 0;
            }
        }

        return enemies;
    }

    /// <summary>
    /// Called to get a dictionary of enemies sorted in ascending order of their score calculated with percentage of remaining HP and multiplied by a proximity coefficient.
    /// </summary>
    /// <param name="enemies"> Allies to sort. </param>
    /// <returns></returns>
    private Dictionary<Entity, int> EnemiesByOrderOfPriority(Dictionary<Entity, int> enemies)
    {
        if (enemies.Count > 0)
        {
            // For each enemy
            for (int i = 0; i < enemies.Count; i++)
            {
                Entity enemy = enemies.ElementAt(i).Key;

                // Calculates the percentage of remaining HP
                enemies[enemy] = (int)(enemy.HP / enemy.EntityDatas.MaxHP * 100f) * i;
            }

            if (enemies.Count > 1)
            {
                // Sorts enemies by ascending order of their score
                enemies = enemies.OrderBy(allies => allies.Value).ToDictionary(allies => allies.Key, allies => allies.Value);
            }
        }

        return enemies;
    }

    /// <summary>
    /// Called to get the best spell to use by anticipating a move in advance.
    /// </summary>
    /// <param name="spells"> List of spells. </param>
    /// <param name="enemyToAttack"> The enemy to attack. </param>
    /// <returns></returns>
    private Spell BestSpellToUse(List<Spell> spells, Entity enemyToAttack)
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
                            int percentageOfDamages = (int)(((currentStartingSpell.SpellDatas.Damages + currentSpellToCheck.SpellDatas.PaCost) / (enemyToAttack.EntityDatas.MaxHP - enemyToAttack.HP)) * 100f);

                            // Adds the combination
                            spellsCombinationInOrderOfDamages.Add(new List<Spell>() { currentStartingSpell, currentSpellToCheck }, percentageOfDamages);
                        }
                        // Checks if the spell is possible alone
                        else if (currentStartingSpell.SpellDatas.PaCost <= _enemyMain.AP)
                        {
                            // Calculates the percentage of damages that it represents
                            int percentageOfDamages = (int)((currentStartingSpell.SpellDatas.Damages / (enemyToAttack.EntityDatas.MaxHP - enemyToAttack.HP)) * 100f);

                            // Adds the spell as a combination
                            spellsCombinationInOrderOfDamages.Add(new List<Spell>() { currentStartingSpell }, percentageOfDamages);
                        }
                    }
                    // If it's not a combination of the two same spells and if the combination is possible
                    else if (currentStartingSpell.SpellDatas.PaCost + currentSpellToCheck.SpellDatas.PaCost <= _enemyMain.AP)
                    {
                        // Calculates the percentage of damages that it represents
                        int percentageOfDamages = (int)(((currentStartingSpell.SpellDatas.Damages + currentSpellToCheck.SpellDatas.Damages) / (enemyToAttack.EntityDatas.MaxHP - enemyToAttack.HP)) * 100f);

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
    /// Called to try to attack an enemy with a spell.
    /// </summary>
    /// <param name="enemyToAttack"> Ally to heal. </param>
    /// <param name="spellToUse"> Spell to use. </param>
    /// <returns></returns>
    private IEnumerator TryToAttack(Entity enemyToAttack, Spell spellToUse)
    {
        // Gets the range of the spell
        List<Square> range = RangeManager.Instance.CalculateSimpleRange(_enemyMain.SquareUnderTheEntity, spellToUse.SpellDatas.MaxRange);

        // Checks if the enemy to attack is in the range
        if (range.Contains(enemyToAttack.SquareUnderTheEntity))
        {
            // Attacks the enemy
            _enemyMain.Attack(spellToUse, enemyToAttack);
        }
        // If he is not
        else
        {
            // Calculates the path to go to the enemy
            Square[] pathToEnemy = AStarManager.Instance.CalculateShortestPathToAnEntity(_enemyMain.SquareUnderTheEntity, enemyToAttack.SquareUnderTheEntity).ToArray();

            // Reduces the path by one for the enemy and by the range of the spell to save MP
            List<Square> pathToGetCloser = pathToEnemy[..^((spellToUse.SpellDatas.MaxRange - 1) + 1)].ToList();

            // Wait until the enemy has moved
            UniTask followingThePath = _enemyMain.FollowThePath(pathToGetCloser);
            yield return new WaitUntil(() => followingThePath.Status != UniTaskStatus.Pending);


            // Attacks the enemy
            _enemyMain.Attack(spellToUse, enemyToAttack);
        }
    }

    private IEnumerator MoveCloserToTheWeaker()
    {
        // Creates a dictionary which will stocks reachable enemies with a score of priority calculated with percentage of remaining HP
        // and multiplied by a proximity coefficient
        Dictionary<Entity, int> enemies = EnemiesByOrderOfPriority(EnemiesByOrderOfDistance(false));

        if (enemies.Count > 0)
        {
            // Gets the enemy to attack
            Entity enemyToMoveTo = enemies.ElementAt(0).Key;

            // Calculates the path to go to the enemy
            Square[] pathToEnemy = AStarManager.Instance.CalculateShortestPathToAnEntity(_enemyMain.SquareUnderTheEntity, enemyToMoveTo.SquareUnderTheEntity).ToArray();
            
            // Reduces the path by one for the enemy
            pathToEnemy = pathToEnemy[..^1];

            // The path to follow
            List<Square> pathToGetCloser;

            // Checks if the path is too long to reach the end
            if (pathToEnemy.Length >= _enemyMain.MP)
            {
                // Reduces the path by the difference between the length of the path and the remaining MP
                pathToGetCloser = pathToEnemy[..^(pathToEnemy.Length - _enemyMain.MP)].ToList();
            }
            else
            {
                pathToGetCloser = pathToEnemy.ToList();
            }

            if (pathToGetCloser.Count > 0)
            {
                // Wait until the enemy has moved
                UniTask followingThePath = _enemyMain.FollowThePath(pathToGetCloser);
                yield return new WaitUntil(() => followingThePath.Status != UniTaskStatus.Pending);
            }
        }
    }
}

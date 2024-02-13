using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class HealerBrain : Brain
{
    /// <summary>
    /// Main component of the enemy.
    /// </summary>
    private EnemyMain _enemyMain;

    /// <summary>
    /// List of spells of the enemy in descending order of their range.
    /// </summary>
    private List<Spell> _spells;

    private void Start()
    {
        _enemyMain = GetComponent<EnemyMain>();
    }

    public override IEnumerator EnemyPattern()
    {
        // Initialises spells
        _spells = new(_enemyMain.Spells.OrderByDescending(spell => spell.SpellDatas.Range));

        // Gets a copy of the list of spells of the enemy in descending order of their range
        List<Spell> spells = new(_spells);

        // Creates a dictionary which will stocks reachable allies with a score of priority calculated with percentage of HP to be treated
        // and multiplied by a proximity coefficient
        Dictionary<Entity, int> allies = AlliesByOrderOfPriority(AlliesByOrderOfDistance());

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
            // Makes the turn end
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

        if (BattleManager.Instance.EnemiesInBattle.Count == 1)
        {
            allies.Add(BattleManager.Instance.EnemiesInBattle[0], 0);
        }
        else
        {
            // For each allied enemy
            for (int i = 0; i < BattleManager.Instance.EnemiesInBattle.Count; i++)
            {
                Entity ally = BattleManager.Instance.EnemiesInBattle[i];

                // Calculate the shortest path between the enemy and his ally
                List<Square> shortestPathToTheAlly = AStarManager.Instance.CalculateShortestPathBetween(ally.SquareUnderTheEntity, _enemyMain.SquareUnderTheEntity);

                // If the ally is reachable with the attack that has the greatest range
                if (shortestPathToTheAlly.Count > _enemyMain.MP + _spells[0].SpellDatas.Range)
                {
                    // Add the ally to the list of allies
                    allies.Add(ally, shortestPathToTheAlly.Count);
                }
            }

            // Sorts allies by descending order of their distance to the enemy by reseting score to 0
            allies = allies.OrderByDescending(allies => allies.Value).ToDictionary(allies => allies.Key, allies => 0);
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
        if (allies.Count != 1)
        {
            // For each allied enemy
            for (int i = 0; allies.Count > 0; i++)
            {
                Entity ally = allies.ElementAt(i).Key;

                // Set the score of the ally by calculating the percentage of HP to be treated multiplied by the index which acts as a proximity coefficient
                allies[ally] = (int)(((ally.EntityDatas.MaxHP - ally.HP) / ally.EntityDatas.MaxHP) * 100f) * i;
            }

            // Sorts allies by descending order of their score
            allies = allies.OrderByDescending(allies => allies.Value).ToDictionary(allies => allies.Key, allies => allies.Value);
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
        if (spells.Count != 1)
        {
            // Get a copy of spells
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

                    // If the combination is possible and if it's with itself
                    if (currentStartingSpell.SpellDatas.PaCost + currentSpellToCheck.SpellDatas.PaCost <= _enemyMain.AP && currentStartingSpell == currentSpellToCheck)
                    {
                        // Calculate the percentage of HP to heal that it represents
                        int percentageOfHeal = (int)((currentStartingSpell.SpellDatas.Damages / (allyToHelp.EntityDatas.MaxHP - allyToHelp.HP)) * 100f);

                        // Add the spell as a combination
                        spellsCombinationInOrderOfHealing.Add(new List<Spell>() { currentStartingSpell }, percentageOfHeal);
                    }
                    // If the combination is possible and if it's not with itself
                    else if (currentStartingSpell.SpellDatas.PaCost + currentSpellToCheck.SpellDatas.PaCost <= _enemyMain.AP)
                    {
                        List<Spell> combination = new() { currentStartingSpell, currentSpellToCheck };

                        // Calculate the percentage of HP to heal that it represents
                        int percentageOfHeal = (int)(((currentStartingSpell.SpellDatas.Damages + currentSpellToCheck.SpellDatas.Damages) / (allyToHelp.EntityDatas.MaxHP - allyToHelp.HP)) * 100f);

                        // Add the combination
                        spellsCombinationInOrderOfHealing.Add(combination, percentageOfHeal);
                    }
                }

                // Removes the starting spell from the spell to check to prevent duplicates
                spellsToCheck.Remove(currentStartingSpell);
            }

            // Sorts combinations by descending order of their efficacity
            spellsCombinationInOrderOfHealing = spellsCombinationInOrderOfHealing.OrderByDescending(spell => spell.Value).ToDictionary(spell => spell.Key, spell => spell.Value);

            // Return the best spell to use
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
        // Get the range of the spell
        List<Square> range = AStarManager.Instance.CalculateRange(_enemyMain.SquareUnderTheEntity, spellToUse.SpellDatas.Range);

        // Checks if the ally to heal is in the range
        if (range.Contains(allyToHeal.SquareUnderTheEntity))
        {
            // Heals the ally
            yield return StartCoroutine(_enemyMain.Attack(spellToUse, allyToHeal));
        }
        // If it is not
        else
        {
            // Calculate the path to go to the ally
            Square[] pathToAlly = AStarManager.Instance.CalculateShortestPathBetween(_enemyMain.SquareUnderTheEntity, allyToHeal.SquareUnderTheEntity).ToArray();

            // Reduces the path with the range of the spell to save MP
            List<Square> pathToGetCloser = pathToAlly[..^spellToUse.SpellDatas.Range].ToList();

            // Makes the enemy following the path
            yield return StartCoroutine(_enemyMain.FollowThePath(pathToGetCloser));

            // Heals the ally
            yield return StartCoroutine(_enemyMain.Attack(spellToUse, allyToHeal));
        }
    }
}

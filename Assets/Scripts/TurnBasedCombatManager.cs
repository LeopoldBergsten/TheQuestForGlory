using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class TurnBasedCombatManager : MonoBehaviour
{
    private List<TurnBasedEntity> attackingEntities;
    private List<TurnBasedEntity> victimEntities;

    public void StartCombatScene(TurnBasedEntity attackingEntity, TurnBasedEntity victimEntity)
    {
        attackingEntities.Clear();
        victimEntities.Clear();

        attackingEntity.TurnInCombat = 0;

        attackingEntities.Add(attackingEntity);
        SortEntitiesByDistanceToEntity(attackingEntity.FriendlyEntities, victimEntity, 1, ref attackingEntities);
        victimEntities.Add(victimEntity);
        SortEntitiesByDistanceToEntity(victimEntity.FriendlyEntities, victimEntity, 1, ref victimEntities);

        StartCoroutine(CombatLoop());
    }

    void SortEntitiesByDistanceToEntity(TurnBasedEntity[] entitiesToSort, TurnBasedEntity distanceToEntity, int startIndex, ref List<TurnBasedEntity> sortedList)
    {
        Dictionary<float, TurnBasedEntity> entityDistanceTo = new Dictionary<float, TurnBasedEntity>();

        foreach (var entity in entitiesToSort)
        {
            //Replace with navmesh distance later
            float distanceBetweenEntities = Vector3.Distance(entity.transform.position, distanceToEntity.transform.position);
            entityDistanceTo.Add(distanceBetweenEntities, entity);
        }

        Dictionary<float, TurnBasedEntity> sortedEnemyDistanceTo = entityDistanceTo.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

        for (int sortedEnemy = 0; sortedEnemy < sortedEnemyDistanceTo.Count; sortedEnemy++)
        {
            sortedEnemyDistanceTo[sortedEnemy].TurnInCombat = sortedEnemy + startIndex;
            sortedList.Add(sortedEnemyDistanceTo[sortedEnemy]);
        }
    }

    private IEnumerator CombatLoop()
    {
        bool oppositionEliminated = false;
        while (!oppositionEliminated)
        {
            for (int turn = 0; turn < attackingEntities.Count; turn++)
            {
                yield return attackingEntities[turn].OnAction(victimEntities.ToArray());
            }
            for (int turn = 0; turn < victimEntities.Count; turn++)
            {
                yield return victimEntities[turn].OnAction(victimEntities.ToArray());
            }

            yield return null;
        }
    }
}

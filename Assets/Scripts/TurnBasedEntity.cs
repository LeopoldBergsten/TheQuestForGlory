using System.Collections;
using UnityEngine;

public abstract class TurnBasedEntity : Singleton<TurnBasedEntity>
{
    [SerializeField] private TurnBasedEntity[] friendlyEntities;
    public TurnBasedEntity[] FriendlyEntities => friendlyEntities;

    [SerializeField] private WeaponBase[] weapons;
    public WeaponBase[] Weapons => weapons;

    private int turnInCombat = 0;
    public int TurnInCombat
    {
        get => turnInCombat; set => turnInCombat = value;
    }

    public virtual IEnumerator OnAction(TurnBasedEntity[] enemiesToAttack) 
    {
        yield return null;
    }
}

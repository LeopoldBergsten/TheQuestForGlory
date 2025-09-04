using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField] private WeaponType weaponType;
    [SerializeField] private float weaponTypeValue;

    [System.Serializable]
    enum WeaponType
    {
        Attack,
        Defend,
        Heal
    }

    public virtual void Use(Damageable damageable)
    {
        switch (weaponType)
        {
            case WeaponType.Attack:
                damageable.OnDamaged((int)weaponTypeValue);
                break;
            case WeaponType.Defend:
                damageable.OnDefend(weaponTypeValue);
                break;
            case WeaponType.Heal:
                damageable.OnHealed((int)weaponTypeValue);
                break;
            default:
                break;
        }
    }
}

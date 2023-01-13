using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newWeapon", menuName = "ScriptableObjects/Weapon")]
public class Weapon : ScriptableObject
{
    public enum AttackType { light, heavy, dash};

    [Tooltip("this prefab should only have an 'AnimationEventCoordinator' script on it - all of the functional code for the weapon is in the scriptable object")]
    [SerializeField] GameObject modelPrefab;
    GameObject weaponInstace;
    Animator animator;

    public void Equip(GameObject parent)
    {
        weaponInstace = Instantiate(modelPrefab, parent.transform);
        animator = weaponInstace.GetComponent<Animator>();
    }

    public void DoLightAttack()
    {
        if (weaponInstace == null) { Debug.LogError("Tried to light attack without equipping weapon. Call Equip() first.");  }
    }

    public void DoHeavyAttack()
    {
        if (weaponInstace == null) return;
    }

    public void DoDashAttack()
    {
        if (weaponInstace == null) return;
    }

    void TriggerWeaponAnimation(AttackType type)
    {
        if (animator == null) return; 
    }
}

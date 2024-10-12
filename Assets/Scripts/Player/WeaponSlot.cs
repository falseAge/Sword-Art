using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlot : MonoBehaviour
{
    [SerializeField] private Transform _weapon;
    [SerializeField] private List<GameObject> _weapons;

    private GameObject currentWeapon;

    private void Start()
    {
        PlayerMovementController.Current.OnHotBarChange.AddListener(UpdateHotBar);
    }
    
    private void UpdateHotBar(int slot)
    {
        switch (slot){
            case 1:
                Destroy(currentWeapon);
                break;
            case 2:
                Destroy(currentWeapon);
                currentWeapon = Instantiate(_weapons[0], _weapon);
                break;
            case 3:
                Destroy(currentWeapon);
                currentWeapon = Instantiate(_weapons[1], _weapon);
                break;

        }

    }   
}

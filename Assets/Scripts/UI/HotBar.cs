using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HotBar : MonoBehaviour
{
    [SerializeField] private List<GameObject> _barSlot;

    private void Start()
    {
        PlayerMovementController.Current.OnHotBarChange.AddListener(UpdateHotBar);
    }
    
    private void UpdateHotBar(int slot)
    {
        Debug.Log(slot.ToString());
    }
}

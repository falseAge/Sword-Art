using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    [SerializeField] private Slider _manaSlider;

    private void Start()
    {
        PlayerMovementController.Current.OnUsingMana.AddListener(UpdateManaBar);
    }
    
    private void UpdateManaBar(float newMana)
    {
        _manaSlider.value = newMana;
    }
}

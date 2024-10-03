using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider _healthSlider;

    private void Start()
    {
        PlayerMovementController.Current.OnGetDamage.AddListener(UpdateHealthBar);
    }
    
    private void UpdateHealthBar(float newHealth)
    {
        _healthSlider.value = newHealth;
    }
}

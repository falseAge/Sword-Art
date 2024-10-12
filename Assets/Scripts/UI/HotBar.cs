using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HotBar : MonoBehaviour
{
    [SerializeField] private List<GameObject> _barSlot;
    private Color _defaultColor = Color.white;
    private Color _activeColor = Color.green;
    private int _previousSlot = 0;
    private int _activeSlot = 1;

    private void Start()
    {
        PlayerMovementController.Current.OnHotBarChange.AddListener(UpdateHotBar);
    }
    
    private void UpdateHotBar(int slot)
    {
        _activeSlot = slot - 1;

        var previousImageComponent = _barSlot[_previousSlot].GetComponent<Image>();
        var activeImageComponent = _barSlot[_activeSlot].GetComponent<Image>();
        if (activeImageComponent != null)
        {
            previousImageComponent.color = _defaultColor;
            activeImageComponent.color = _activeColor;
        }

        _previousSlot = _activeSlot;
    }
}

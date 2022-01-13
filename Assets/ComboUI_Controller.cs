using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ComboUI_Controller : MonoBehaviour
{
    public UnityEngine.UI.Slider comboSlider;
    private float _lastValue;

    private void OnEnable()
    {
        PlayerController.OnComboAdd += ComboUpdate;
        PlayerController.OnComboAdd += ComboShake;
        PlayerController.OnComboLost += ComboUpdate;
    }

    private void OnDisable()
    {
        PlayerController.OnComboAdd -= ComboUpdate;
        PlayerController.OnComboAdd -= ComboShake;
        PlayerController.OnComboLost -= ComboUpdate;
    }

    void ComboUpdate()
    {
        float value = (float)PlayerController.Instance.COMBO / (float)PlayerController.Instance.MAX_COMBP;

        //if (value != _lastValue)
        //    UI_Shake.Shake(comboSlider.transform);

        comboSlider.value = value;
        _lastValue = value;
    }

    void ComboShake()
    {
        FindObjectOfType<CameraShake>().FireOnce(CameraShake.ShakeForce.ComboMove);
    }
}

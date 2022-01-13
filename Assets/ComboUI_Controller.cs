using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ComboUI_Controller : MonoBehaviour
{
    public UnityEngine.UI.Slider comboSlider;

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
        comboSlider.value = value;
    }

    void ComboShake()
    {
        FindObjectOfType<CameraShake>().FireOnce(CameraShake.ShakeForce.ComboMove);
    }
}

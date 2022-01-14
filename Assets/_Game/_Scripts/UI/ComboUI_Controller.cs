using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ComboUI_Controller : MonoBehaviour
{
    public UnityEngine.UI.Slider comboSlider;
    private float _lastValue;
    public string scanlinePropertyName;
    //public UnityEngine.UI.Image rend;
    public bool _disableScanline;

    public float scrollDuration = .3f;
    public float valueLerpDuration = .2f;

    public ParticleSystem ps;

    private void OnEnable()
    {
        PlayerController.OnComboAdd += ComboUpdate;
        PlayerController.OnComboLost += ComboUpdate;
        comboSlider.value = 0;
        //rend.material.SetInt(scanlinePropertyName, 0);
    }

    private void OnDisable()
    {
        PlayerController.OnComboAdd -= ComboUpdate;
        PlayerController.OnComboLost -= ComboUpdate;
    }

    void ComboUpdate()
    {
        if (!JuiceManager.Instance.IsComboBarJuiceOn) return;

        float value = (float)PlayerController.Instance.COMBO / (float)PlayerController.Instance.MAX_COMBP;



        if (value != _lastValue)
        {
            //StartCoroutine(UI_Shake.Shake(comboSlider.transform, 0.1f, 0.5f));
            
            if(ps.isPlaying)
                ps.Stop();

            if(!ps.isPlaying)
                ps.Play();

            ps.Emit(100);
            
            if (PlayerController.Instance.COMBO >= PlayerController.Instance.MAX_COMBP)
            {
                DistortionController.instance.FireOnce();
            }
            else
            {
                DistortionController.instance.CloseOnce();
            }
        }

        //rend.material.SetInt(scanlinePropertyName, 1);
        //StartCoroutine(DisableScanline());

        StartCoroutine(UpdateSliderValue(value));
        _lastValue = value;
    }

    IEnumerator DisableScanline()
    {
        yield return new WaitForSeconds(scrollDuration);
        //rend.material.SetInt(scanlinePropertyName, 0);
    }


    public IEnumerator UpdateSliderValue(float endValue)
    {
        float timeElapsed = 0;
        float startValue = comboSlider.value;

        while (timeElapsed < valueLerpDuration)
        {
            comboSlider.value = Mathf.Lerp(startValue, endValue, timeElapsed / valueLerpDuration);
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        comboSlider.value = endValue;
    }
}

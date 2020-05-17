using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilterTab : MonoBehaviour
{
    public Button Btn_ShowFiltet;
    public Button Btn_HideFilter;
    public Button Btn_Spread;
    public Slider slider_Level;
    public Animator filterAnimation;

    public delegate void OnLevelChanged(int level);
    public static OnLevelChanged OnLevelChangedEvent;
    
    // Start is called before the first frame update
    void Start()
    {
        //HideFilter();
        Btn_HideFilter.gameObject.SetActive(false);
        Btn_ShowFiltet.gameObject.SetActive(true);
        slider_Level.gameObject.SetActive(false);

        Btn_HideFilter.onClick.AddListener(HideFilter);
        Btn_ShowFiltet.onClick.AddListener(ShowFilter);
        Btn_Spread.onClick.AddListener(SpreadLevelSliderCliked);

        slider_Level.minValue = 0;
        slider_Level.maxValue = CovidNetworkManager.MAX_LEVEL;
    }

    
    public void ShowFilter()
    {
        Btn_HideFilter.gameObject.SetActive(true);
        Btn_ShowFiltet.gameObject.SetActive(false);
        filterAnimation.Play("Open");
    }

    public void HideFilter()
    {
        Btn_HideFilter.gameObject.SetActive(false);
        Btn_ShowFiltet.gameObject.SetActive(true);
        filterAnimation.Play("Close");
    }

    bool toggleLevelButton = false;
    public void SpreadLevelSliderCliked()
    {
        toggleLevelButton = !toggleLevelButton;
        slider_Level.gameObject.SetActive(toggleLevelButton);
    }

    public void OnLevelSliderChanged(float value)
    {
        CovidNetworkManager.CurrentLevel = (int) value;
        Debug.Log("current level : "+ CovidNetworkManager.CurrentLevel);
        OnLevelChangedEvent((int)value);
    }
}

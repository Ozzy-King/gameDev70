using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class settingMenuScript : MonoBehaviour
{
    public GameObject mouseSensitivitySlider;
    public GameObject mouseSensitivityText;

    public void sliderUpdate() {
        print("updateing text"+ mouseSensitivitySlider.GetComponent<Slider>().value.ToString());
        TextMeshProUGUI test = mouseSensitivityText.GetComponent<TextMeshProUGUI>();
        print(test);
        mouseSensitivityText.GetComponent<TextMeshProUGUI>().text = mouseSensitivitySlider.GetComponent<Slider>().value.ToString();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

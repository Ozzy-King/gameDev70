using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class settingMenuScript : MonoBehaviour
{
    public GameObject mouseSensitivitySlider;
    public GameObject mouseSensitivityText;

    public void sliderUpdate()
    {
        string text = mouseSensitivitySlider.GetComponent<Slider>().value.ToString();
        TextMeshProUGUI test = mouseSensitivityText.GetComponent<TextMeshProUGUI>();
        test.text = text.Substring(0, Math.Min(3, text.Length));

        gameControllerScript gmShortcut = GameObject.Find("GameController").GetComponent<gameControllerScript>();
        
        if (gmShortcut.gameState == gameControllerScript.gamestates.running) {
            GameObject.Find("player").GetComponent<playercameraLook>().mouseSensitivity = float.Parse(test.text);
        }
        else {
            gmShortcut.PlayerMouseSens = float.Parse(test.text);
        }
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

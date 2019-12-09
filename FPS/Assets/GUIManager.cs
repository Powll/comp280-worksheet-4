using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public GameObject gui;
    public GameObject ui_paused;
    public GameObject ui_settings_video;
    public GameObject ui_settings_audio;
    public GameObject ui_settings_gameplay;

    public void ToggleGUI()
    {
        ToggleSingleGameObject(gui);
    }

    public void TogglePaused()
    {
        ToggleSingleGameObject(ui_paused);
    }

    public void ToggleVideoSettings()
    {
        ToggleSingleGameObject(ui_settings_video);
    }

    public void ToggleAudioSettings()
    {
        ToggleSingleGameObject(ui_settings_audio);
    }

    public void ToggleGameplaySettings()
    {
        ToggleSingleGameObject(ui_settings_gameplay);
    }

    private void ToggleSingleGameObject(GameObject obj)
    {
        gui.SetActive(obj == gui ? true : false);
        ui_paused.SetActive(obj == ui_paused ? true : false);
        ui_settings_video.SetActive(obj == ui_settings_video ? true : false);
        ui_settings_audio.SetActive(obj == ui_settings_audio ? true : false);
        ui_settings_gameplay.SetActive(obj == ui_settings_gameplay ? true : false);
    }
}

using Lightshift;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void Exit() 
    {
        gameObject.SetActive(false);
    }
    public void ShowGraphicSettings()
    {
        MainMenu.ShowSettings(SettingsMenu.SettingsType.Graphics);
    }

    public void ShowOptions()
    {
        MainMenu.ShowSettings(SettingsMenu.SettingsType.Options);
    }

    public void ShowSoundSettings()
    {
        MainMenu.ShowSettings(SettingsMenu.SettingsType.Sound);
    }

    public void ShowControlSettings()
    {
        MainMenu.ShowSettings(SettingsMenu.SettingsType.Controls);
    }

    public void ShowCredits() 
    {
        DialogManager.ShowMessage($"# Programming # {Environment.NewLine}" +
            $"Velcer{Environment.NewLine}" +
            $"Voidshard{Environment.NewLine}" +
            $"{Environment.NewLine}" +
            $"# Game Design #{Environment.NewLine}" +
            $"Mildred{Environment.NewLine}" +
            $"Voidshard{Environment.NewLine}" +
            $"Velcer{Environment.NewLine}" +
            $"{Environment.NewLine}" +
            $"# Graphics #{Environment.NewLine}" +
            $"Mildred{Environment.NewLine}" +
            $"Velcer{Environment.NewLine}" +
            $"{Environment.NewLine}" +
            $"# Audio #{Environment.NewLine}" +
            $"Step{Environment.NewLine}" +
            $"Xtruller{Environment.NewLine}" +
            $"Velcer");
        Exit();
    }

    public void ExitGame() 
    {
        Exit();

        DialogManager.ShowDialog("Are you sure you want to Quit? :(", delegate (bool result) 
        {
            if (result)
                Application.Quit();
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(Settings.MenuKey))
            Exit();
    }
}

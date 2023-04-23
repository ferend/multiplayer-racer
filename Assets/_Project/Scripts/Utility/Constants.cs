using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants 
{
    public const float cameraPosXLerpSpeed = 5f;
    
    public static readonly List<string> GameTypes = new() { "Battle Royal", "Capture The Flag", "Creative" };
    public static readonly List<string> Difficulties = new() { "Easy", "Medium", "Hard" };
        
    public const string JoinKey = "j";
    public const string DifficultyKey = "d";
    public const string GameTypeKey = "t";
    #region UI
    public const float defaultTransitionDuration = 0.25f;
    public const float overlayTransitionDuration = 0.5f;
    public const float splashScreenDuration = 2.0f;
    public const float popupOpenDuration = 0.5f;
    public const float popupUnderlayTransitionDuration = 0.5f;
    public const float popupCloseDuration = 0.5f;
    #endregion
}

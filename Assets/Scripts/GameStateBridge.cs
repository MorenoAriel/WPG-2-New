using UnityEngine;

public static class GameStateBridge
{
    public static bool HasSavedState = false;
    public static string MainSceneName = "";

    // Position
    public static Vector2 PlayerPosition;

    // Physics
    public static Vector2 Velocity;
    public static float   FlightAngle;

    // Controller
    public static PlayerController.PlayerState ControllerState;
    public static float RotationAngle;
    public static float SavedTargetAngle;

    // Score
    public static float Score;

    public static void Clear()
    {
        HasSavedState = false;
        MainSceneName = "";
    }
}
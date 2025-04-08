// I didn't want to do this, but I could not find a way to have a method in the SessionManager load the level scene properly.
// Null reference exeptions in Overlay UI components because Overlay loaded before the Gameplay scene despite the simple fact that they were told to load after Gameplay finished.
// So I will rely GameplayOrchestrator in the Gameplay scene to load the Overlay scene for me.
// This makes it a little harder to understand the sequence of events because everything is split between files with no direct references to each other.
// You just have to know.
//
// It's a surprisingly easy fix though.
// Is there a lesson in here? IDK.

public static class SelectedLevelScene
{
    public static string SelectedLevelSceneName { get; set; }
}
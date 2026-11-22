namespace EcoToss.Feedback
{
    /// <summary>
    /// Kinds of in-match NPC reactions shown in the HUD reaction panel.
    /// Victory, defeat and support sprites are handled by the end-game UI, not here.
    /// </summary>
    public enum NPCReactionType
    {
        Positive,
        Negative,
        Streak
    }
}

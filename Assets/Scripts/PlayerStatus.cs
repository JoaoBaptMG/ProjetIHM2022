// The different statuses the player can have.
// Statuses determine the transitions to use to get from one 
// status to another.

public enum PlayerStatus
{
    GROUNDED_IDLE,
    WALK,
    RUN,
    GROUND_SLIDE,
    MIDAIR_IDLE,
    AIR_CONTROL,
    WALL_SLIDE,
    DASH
}
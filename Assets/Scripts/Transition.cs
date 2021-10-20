using UnityEngine;

public abstract class Transition
{
    // The inital value (value of the variable to be changed before the transition begins).
    public float From { protected get; set; }

    // The target value (value of the variable to be changed after the transition has been fully applied to it).
    public float To { protected get; set; }

    // The duration of the transition from From to To (in seconds).
    private float duration = 0.5f;
    public float Duration { 
        protected get { return duration; }
        set { duration = Mathf.Max(0, value); } 
    }

    // The progression of the transition. Its value is 0 when the transition just begins.
    // As soon as the progression exceeds 1, the transition is considered finished.
    public float Progression { protected get; set; }

    // Transition is considered finished when its progression is greater than 1.
    // Note that the first value returned by the transition is associated with a 
    // progression value of Time.deltaTime / Duration (not 0).
    public bool isFinished() { return Progression >= 1; }

    // A transition instance is available when it is not associated with/referenced by
    // any game object whose properties can be animated using transitions.
    // When a transition instance is returned to such game object by the 
    // transition factory, its availability is set to false by the factory.
    // When the game object frees a transition instance (it no longer references it 
    // as the transition is finished or it has been canceled), its availability 
    // has to be set back to true, so that this instance can be reused later 
    // (returned by the transition factory to the demanding game object).
    public bool Available { get; set; }

    public Transition()
    {
        Progression = 0;
        Available = true;
    }

    // Returns the value of the variable to be changed as the transition has
    // progressed by Progression.
    // The implementation is transition-specific.
    public abstract float getValue();
}
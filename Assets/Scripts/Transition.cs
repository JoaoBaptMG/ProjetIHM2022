using UnityEngine;

public abstract class Transition
{
    // The inital value (value of the variable to be changed before the transition begins).
    public float From { protected get; set; }

    // The target value (value of the variable to be changed after the transition has been fully applied to it).
    public float To { protected get; set; }

    // The duration of the transition from From to To (in seconds).
    private float duration = 0.5f;
    public float Duration
    {
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

    public Transition()
    {
        Progression = 0;
    }

    // Returns the value of the variable to be changed as the transition has
    // progressed by Progression.
    // The implementation is transition-specific.
    public abstract float getValue();
}
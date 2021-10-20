using UnityEngine;

public class LinearTransition : Transition
{
    public LinearTransition() : base() {}
    public override float getValue()
    {
        // Updates the transition's progression.
        if (Duration > 0) { Progression += Time.deltaTime / Duration; }
        else { Progression = 1; }

        if (Progression >= 1) { return To; }

        return From * (1 - Progression) + To * Progression;
    }
}
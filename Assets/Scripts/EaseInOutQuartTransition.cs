using UnityEngine;

public class EaseInOutQuartTransition : Transition
{
    public EaseInOutQuartTransition() : base() { }

    public override float getValue()
    {
        // Updates the transition's progression.
        if (Duration > 0) { Progression += Time.deltaTime / Duration; }
        else { Progression = 1; }

        if (Progression >= 1) { return To; }

        float u = Progression < 0.5 ? 8 * Mathf.Pow(Progression, 4) : 1 - Mathf.Pow(-2 * Progression + 2, 4) / 2;

        return From * (1 - u) + To * u;

    }
}
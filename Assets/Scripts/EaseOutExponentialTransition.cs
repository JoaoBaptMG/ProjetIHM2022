using UnityEngine;

public class EaseOutExponentialTransition : Transition
{
    private float param1 = 10f;
    public float Param1
    {
        private get { return param1; }
        set { param1 = Mathf.Max(5, value); }
    }
    public EaseOutExponentialTransition() : base() {}

    public override float getValue()
    {
        // Updates the transition's progression.
        if (Duration > 0) { Progression += Time.deltaTime / Duration; }
        else { Progression = 1; }

        if (Progression >= 1) { return To; }

        float u =  1 - Mathf.Pow(2, -Param1 * Progression);

        return From * (1 - u) + To * u;

    }
}
using UnityEngine;

public class EaseInExponentialTransition : Transition
{
    private float param1 = 10f;
    public float Param1 {
        private get { return param1; }
        set { param1 = Mathf.Max(5, value); }
    }
    public EaseInExponentialTransition() : base() {}

    public override float getValue()
    {
        // Updates the transition's progression.
        if (Duration > 0) { Progression += Time.deltaTime / Duration; }
        else { Progression = 1; }

        if (Progression >= 1) { return To; }

        float u = Mathf.Pow(2, Param1 * Progression - Param1);

        return From * (1 - u) + To * u;
        
    }
}
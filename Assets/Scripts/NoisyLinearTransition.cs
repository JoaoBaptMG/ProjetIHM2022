using UnityEngine;

public class NoisyLinearTransition : Transition
{
    // The noise's maximum value
    public float NoiseMax
    {
        private get;
        set;
    } = 1f;

    // The noise's minimum value
    public float NoiseMin
    {
        private get;
        set;
    } = 1f;

    public NoisyLinearTransition() : base() { }

    public override float getValue()
    {
        // Updates the transition's progression.
        if (Duration > 0) { Progression += Time.deltaTime / Duration; }
        else { Progression = 1; }

        if (Progression >= 1) { return To; }

        return From * (1 - Progression) + To * Progression + Random.Range(NoiseMin, NoiseMax);

    }
}
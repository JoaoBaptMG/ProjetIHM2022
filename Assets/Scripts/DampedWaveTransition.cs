using UnityEngine;

public class DampedWaveTransition : Transition
{
    // The angular frequency of the oscillations
    private float frequency = 15f;
    public float Frequency
    {
        private get { return frequency; }
        set { frequency = Mathf.Max(1, value); }
    }

    // The damping factor
    private float dampingFactor = 2f;

    public float DampingFactor
    {
        private get { return dampingFactor; }
        set { dampingFactor = Mathf.Max(2, value); }
    }

    public DampedWaveTransition() : base() { }

    public override float getValue()
    {
        // Updates the transition's progression.
        if (Duration > 0) { Progression += Time.deltaTime / Duration; }
        else { Progression = 1; }

        if (Progression >= 1) { return To; }

        float u = Mathf.Exp(-dampingFactor * Progression) * Mathf.Cos(frequency * Progression);

        return From * u + To * (1 - u);

    }
}
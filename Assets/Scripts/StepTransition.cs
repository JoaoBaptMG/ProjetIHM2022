using UnityEngine;

public class StepTransition : Transition
{
    // The progression of the transition when the value switches from From to To.
    // 0 <= stepProgression <= 1.
    private float stepProgression;

    // stepTime is the time when the value controlled by the transition switches 
    // from From to To.
    // 0 <= stepTime <= Duration.
    // stepProgression == stepTime / Duration.
    public void setStepTime(float stepTime)
    {
        if(Duration > 0) { stepProgression = Mathf.Clamp(stepTime / Duration, 0, 1); }
        else { stepProgression = 0; }
    }

    public StepTransition() : base() {}

    public override float getValue()
    {
        // Updates the transition's progression.
        if (Duration > 0) { Progression += Time.deltaTime / Duration; }
        else { Progression = 1; }

        return Progression < stepProgression ? From : To;

    }
}
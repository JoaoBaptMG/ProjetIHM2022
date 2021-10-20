using System;
using System.Collections.Generic;

public class TransitionFactory
{
    // The transition factory is a singleton.
    private static TransitionFactory instance;

    private TransitionFactory() {}

    public static TransitionFactory getInstance()
    {
        if (instance == null) {
            instance = new TransitionFactory();

            // Initializing the transition pool.
            instance.transitionPool = new Dictionary<Type, List<Transition>>();
        }
        return instance;
    }

    // To avoid constantly instantiating new transitions, the ones already 
    // created are kept in a pool.
    // transitionPool is a dictionary listing existing transitions by type 
    // as returned by the typeof operator.
    // Thus if key typeof(LinearTransition) exists in transitionPool, 
    // transitionPool[typeof(LinearTransition)] returns the list of all existing 
    // instances of the LinearTransition Transition subclass.
    private Dictionary<Type, List<Transition>> transitionPool;

    // Returns an instance of a Transition subclass of type type from the transition pool.
    // If no transition of that type is found in the pool, or if all transitions of that 
    // type contained within the pool are unavailable (Transition.Available == false), 
    // instanciates a new instance of the Transition subclass of type type and returns it.
    public Transition getTransition(Type type)
    {
        if(!(type is Transition)) { type = typeof(DefaultTransition); }

        Transition transition;

        if (transitionPool.ContainsKey(type))
        {
            transition = transitionPool[type].Find(t => t.Available);

            if(transition == null)
            {
                transition = (Transition)Activator.CreateInstance(type);
                transitionPool[type].Add(transition);
            }
        }
        else
        {
            transition = (Transition)Activator.CreateInstance(type);
            transitionPool.Add(type, new List<Transition>());
            transitionPool[type].Add(transition);
        }

        transition.Progression = 0;
        transition.Available = false;

        return transition;
    }

    // Returns an instance of a Transition subclass of type type from the transition pool.
    // If no transition of that type is found in the pool, or if all transitions of that 
    // type contained within the pool are unavailable (Transition.Available == false), 
    // instanciates a new instance of the Transition subclass of type type and returns it.
    /*
    public Transition getTransition(TransitionType type)
    {
        Transition transition = transitionPool[(int)type].Find(t => t.Available);

        if(transition == null) {
            
            switch(type)
            {
                case TransitionType.LINEAR:
                    transition = new LinearTransition();
                    Type test = typeof(LinearTransition);
                    test.
                    break;
                case TransitionType.EASE_IN_EXPONENTIAL:
                    transition = new EaseInExponentialTransition();
                    break;
                case TransitionType.EASE_OUT_EXPONENTIAL:
                    transition = new EaseOutExponentialTransition();
                    break;
                case TransitionType.STEP:
                    transition = new StepTransition();
                    break;
                default:
                    transition = new DefaultTransition();
                    break;
            }

            // Adding the newly created transition to the pool.
            transitionPool[(int)type].Add(transition);
        }

        // Resetting the transition in all cases.
        transition.Progression = 0;
        transition.Available = true;

        return transition;
    }
    */
}
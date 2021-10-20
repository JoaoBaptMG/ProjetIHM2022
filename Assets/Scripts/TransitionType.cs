// The different types of transitions (the easing functions used within the transitions) 
// are listed here.
// Everytime a new Transition subclass is added, its type must be added too as a value of 
// the following enumeration.
// The names of the transition types are taken from here: https://easings.net/.
public enum TransitionType {
    LINEAR,
    EASE_IN_EXPONENTIAL,
    EASE_OUT_EXPONENTIAL,
    STEP
}
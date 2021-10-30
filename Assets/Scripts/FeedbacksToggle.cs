using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class FeedbacksToggle : MonoBehaviour
    {
        Toggle toggle;

        // Use this for initialization
        void Start()
        {
            toggle = GetComponent<Toggle>();
            toggle.isOn = !Game.FeedbacksActivated;
        }

    }
}
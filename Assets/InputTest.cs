using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTest : MonoBehaviour
{
    public float axisDisplacement = 4f;
    List<int> lastButton = new List<int>();

    // Update is called once per frame
    void Update()
    {
        // Get the inputs
        var x = Input.GetAxisRaw("Horizontal");
        var y = Input.GetAxisRaw("Vertical");
        transform.position = new Vector3(x, y, 0) * axisDisplacement;

        // Get the buttons and add them to the list if necessary
        void CheckButton(string name, int v)
        {
            if (Input.GetButtonDown(name)) lastButton.Add(v);
            else if (Input.GetButtonUp(name)) lastButton.Remove(v);
        }

        CheckButton("A", 0);
        CheckButton("B", 1);
        CheckButton("X", 2);
        CheckButton("Y", 3);

        // Get the last pressed color
        var color = lastButton.Count == 0 ? Color.white : lastButton[lastButton.Count - 1] switch
        {
            0 => Color.green, 1 => Color.red, 2 => Color.blue, 3 => Color.yellow, _ => Color.white
        };

        // Set the color
        var sprite = GetComponent<SpriteRenderer>();
        if (sprite != null) sprite.color = color;
    }
}

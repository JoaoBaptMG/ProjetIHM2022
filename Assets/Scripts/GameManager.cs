using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private GameManager() {}

    public static GameManager getInstance()
    {
        if (instance == null) { instance = new GameManager(); }
        return instance;
    }

    [SerializeField]
    private float gravity;
    public float Gravity { get { return gravity; } }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

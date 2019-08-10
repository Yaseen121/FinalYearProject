// Script attached to player on the attacking scne 
// Used to control the player movement and edit the button text 
// Script written by : Mohammed Yaseen Sultan
 
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovementTest : MonoBehaviour
{
    //5f
    private float distance =20f;
    //Bools and text to control player movement
    private bool forwardPressed;
    public Text forwardText;

    private bool backwardPressed;
    public Text backwardText;

    // Start is called before the first frame update
    void Start()
    {
        forwardPressed = false;
        backwardPressed = false;
    }

    // Update is called once per frame
    //Moves player depending on the bools that have been set
    void Update()
    {
        if (forwardPressed)
        {
            //Change text of buttons then move
            forwardText.text = "Stop";
            //transform.position = transform.position + Camera.main.transform.forward * distance * Time.deltaTime;
            transform.position = transform.position + transform.forward * distance * Time.deltaTime;
        }
        else {
            forwardText.text = "Forward";
            transform.position = transform.position;
        }

        if (backwardPressed)
        {
            //Change text of buttons then move
            backwardText.text = "Stop";
            //transform.position = transform.position + Camera.main.transform.forward * (-distance) * Time.deltaTime;
            transform.position = transform.position + transform.forward * (-distance) * Time.deltaTime;
        }
        else
        {
            backwardText.text = "Backward";
            transform.position = transform.position;
        }
    }

    public void ForwardPressed() {
        forwardPressed = !forwardPressed;
        backwardPressed = false;
    }

    public void BackwardPressed()
    {
        backwardPressed = !backwardPressed;
        forwardPressed = false;
    }

    //Rotates player left 
    public void LeftPressed()
    {
        transform.Rotate(0, -90, 0);
    }

    //Rotates player right
    public void RightPressed()
    {
        transform.Rotate(0, 90, 0);
    }

    //Stops player movement
    public void Stop() {
        forwardPressed = false;
        backwardPressed = false;
    }
}

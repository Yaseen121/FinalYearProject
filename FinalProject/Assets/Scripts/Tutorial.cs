// Script attached to the help panel 
// Used to load the corresponding tutorial image upon button presses
// Script created by: Mohammed Yaseen Sultan

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public GameObject[] images;
    public GameObject nextBtn;
    public GameObject prevBtn;
    private int imgCount;

    // Start is called before the first frame update
    //Deactives all images to hide them except for the first so that the player sees the first image
    void Start()
    {
        imgCount = 0;
        foreach (GameObject img in images) {
            img.SetActive(false);
        }
        images[0].SetActive(true);
    }

    //Load the next image on button press
    public void nextImage() {
        imgCount++;
        images[imgCount].SetActive(true);
        images[imgCount-1].SetActive(false);
        
    }

    //Load the previous image on button press
    public void prevImage() {
        imgCount--;
        images[imgCount].SetActive(true);
        images[imgCount + 1].SetActive(false);
    }

    //When help pressed again reset back to first image 
    private void OnEnable()
    {
        imgCount = 0;
        foreach (GameObject img in images)
        {
            img.SetActive(false);
        }
        images[0].SetActive(true);
    }

    //If on the last image hide next button and if on first image hide previous button 
    private void Update()
    {
        if ((imgCount + 1) == images.Length)
        {
            nextBtn.SetActive(false);
        }
        else {
            nextBtn.SetActive(true);
        }


        if (imgCount == 0)
        {
            prevBtn.SetActive(false);
        }
        else
        {
            prevBtn.SetActive(true);
        }
    }
}

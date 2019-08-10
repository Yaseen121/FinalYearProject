/************************************************************************************************************************************************************************
 * Script used for the shop in the build scene 
 * Script written by : "Brackeys" on YouTube and modified by Mohammed Yaseen Sultan
 * The turoial followed can be found here: https://www.youtube.com/watch?v=uv1zp7aOoOs
 ************************************************************************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    BuildManager buildManager;
    public Button turret1Button;
    public Button turret2Button;
    public Button turret3Button;

    //Sprite for each turret and the alternative sprite for when turret is selected 
    public Sprite tur1;
    public Sprite tur1Selected;
    public Sprite tur2;
    public Sprite tur2Selected;
    public Sprite tur3;
    public Sprite tur3Selected;


    private void Start()
    {
        buildManager = BuildManager.instance;
        turret1Button.GetComponent<Image>().sprite = tur1Selected;
        turret2Button.GetComponent<Image>().sprite = tur2;
        turret3Button.GetComponent<Image>().sprite = tur3;
    }

    //Change the turret that is selected in buildmanager 
    public void SelectTurret1() {
        buildManager.setTurret1();
        //Change UI to show selected turret
        SelectTurret(1);
    }

    public void SelectTurret2() {
        buildManager.setTurret2();
        //Change UI to show selected turret
        SelectTurret(2);
    }

    public void SelectTurret3()
    {
        buildManager.setTurret3();
        //Change UI to show selected turret
        SelectTurret(3);
    }


    //Sets the corresponding images of the seleceted turret 
    public void SelectTurret(int tur) {
        if (tur == 1)
        {
            turret1Button.GetComponent<Image>().sprite = tur1Selected;
            turret2Button.GetComponent<Image>().sprite = tur2;
            turret3Button.GetComponent<Image>().sprite = tur3;
        }
        else if (tur == 2)
        {
            turret2Button.GetComponent<Image>().sprite = tur2Selected;
            turret1Button.GetComponent<Image>().sprite = tur1;
            turret3Button.GetComponent<Image>().sprite = tur3;
        }
        else {
            turret3Button.GetComponent<Image>().sprite = tur3Selected;
            turret2Button.GetComponent<Image>().sprite = tur2;
            turret1Button.GetComponent<Image>().sprite = tur1;
        }
    }

}

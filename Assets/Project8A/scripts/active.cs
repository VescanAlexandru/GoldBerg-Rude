using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class active : MonoBehaviour {
    public SteamVR_TrackedObject trakedObj;
    private SteamVR_Controller.Device device;
    public GameObject Active;



    // Use this for initialization
    void Start ()
    {
		
		
       trakedObj = GetComponent<SteamVR_TrackedObject>();
		Active.SetActive (false);
        


    }

    // Update is called once per frame
    void Update () {
        device = SteamVR_Controller.Input((int)trakedObj.index);
        if (device.GetTouch(SteamVR_Controller.ButtonMask.Touchpad))
        {
            Active.SetActive(true);
        }
        if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            Active.SetActive(false);

        }
    }
}

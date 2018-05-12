using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class ControllerInputManager : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;
    public SteamVR_Controller.Device device;
    public GameObject rightController;
    
    private GameObject ball;
    private BallReset ballReset;
    public Material outsidePlayspaceMaterial;

   
    // ...Teleport...//
    public bool ControllerLeft;
    private LineRenderer laser;
    public GameObject teleportAimerObject;
    public Vector3 teleportLocation;
    public GameObject player;
	public LayerMask laserMask;
	public Material restrictedColor;
	public Material laserPointerColor;
	public float teleportRange;


	//...Dashing..//


	public float dashSpeed = 0.1f;          
	private bool Dashing;                 
	private float lerpTime;                 
	private Vector3 dashStartPosition;      

	   
   //..... Grabbing and throwing.....//
    public float throwForce = 1.5f;

    
    //...ObjectMenu..//
    public bool ControllerRight;
    private float swipeSum;
    private float touchLast;
    private float touchCurrent;
    private float distance;
    private bool hasSwipedLeft;
    private bool hasSwipedRight;
    public ObjectMenuManager objectMenuManager;
	public Text Active;


    //------------------------------------------------------------------------------------------------------

	void Awake () 
	{
		trackedObj = GetComponent<SteamVR_TrackedObject>();

		laser = GetComponentInChildren<LineRenderer>();
        ballReset = GameObject.FindObjectOfType<BallReset>();
	}

    void Start () 
	{
        ball = GameObject.Find("Ball");



        
	}
	
	void Update () {
        device = SteamVR_Controller.Input((int)trackedObj.index);

        if (!isballChild())
        {
            Teleport();
        }
		else if (isballChild() && device.GetPress(SteamVR_Controller.ButtonMask.Touchpad) )
        {
            ball.GetComponent<Renderer>().material = outsidePlayspaceMaterial;
        }
        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            ball.GetComponent<Renderer>().material = ballReset.insideMaterial;
        }
        ObjectMenu();
	}
		
	bool isballChild ()
	{
        return (ball.transform.parent == gameObject.transform || ball.transform.parent == rightController.gameObject.transform);
	}

    /* -------------------------------------------------------------------------------------------------------- //

                                                    Teleporting
    // -------------------------------------------------------------------------------------------------------- */

    void Teleport ()
	{
		if (ControllerLeft) {
			
			if (Dashing) {
				lerpTime += Time.deltaTime * dashSpeed;
				player.transform.position = Vector3.Lerp(dashStartPosition, teleportLocation, lerpTime);

				if (lerpTime >= 1) {
					Dashing = false;
					lerpTime = 0;
				}
			}
			else 
			{
				
				if (device.GetPress(SteamVR_Controller.ButtonMask.Touchpad)) 
				{
					if (laser == null) return;

					laser.gameObject.SetActive(true);
					teleportAimerObject.SetActive(true);
					laser.SetPosition( 0,gameObject.transform.position);

				
					RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, teleportRange).OrderBy(h=>h.distance).ToArray();


					for (int i = 0; i < hits.Length; i++) 
					{   
						if (hits[0].transform.CompareTag("Restricted") || hits[0].transform.CompareTag("Structure")|| hits[0].transform.CompareTag("Camera")|| hits[0].transform.CompareTag("Ground")) 
						{
							laser.SetPosition(1, transform.position + transform.forward * (hits[0].distance));
							laser.material = restrictedColor;

							RaycastHit hitGround;
							if (Physics.Raycast(transform.position, -Vector3.up, out hitGround, 10, laserMask)) {
								teleportAimerObject.transform.position = player.transform.position;
								teleportLocation = player.transform.position;
								dashStartPosition = player.transform.position;
								device.TriggerHapticPulse();
							}
							return;
						}
					}


					RaycastHit hit;
					if (Physics.Raycast(transform.position, transform.forward, out hit, teleportRange, laserMask)) 
					{
						teleportLocation = hit.point;   
						laser.SetPosition(1, teleportLocation);   
						laser.material = laserPointerColor;
						teleportAimerObject.transform.position = teleportLocation;
					}

					else 
					{
						 
						teleportLocation = transform.position + transform.forward * teleportRange;


						RaycastHit groundRay;
						if (Physics.Raycast(teleportLocation, -Vector3.up, out groundRay, 17, laserMask)) 
						{
							teleportLocation = new Vector3(
								transform.position.x + transform.forward.x * teleportRange,
								groundRay.point.y,
								transform.position.z + transform.forward.z * teleportRange);
						}
						laser.SetPosition(1, transform.position + transform.forward * teleportRange);
						laser.material = laserPointerColor;


						teleportAimerObject.transform.position = teleportLocation;
					}
				}


				if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad)) 
				{
					if (laser == null) return;


					laser.gameObject.SetActive(false);
					teleportAimerObject.SetActive(false);


					dashStartPosition = player.transform.position;
					Dashing = true;
				}
			}
			if (Dashing && Time.timeScale <= 1) {
				dashSpeed = 15;
			}
			if(Dashing && Time.timeScale >= 1) {
				dashSpeed = 5;
				
			}
	}
	}

    /* -------------------------------------------------------------------------------------------------------------------------------------------------------- //

                                                                                    Grabbing and Throwing
    // -------------------------------------------------------------------------------------------------------------------------------------------------------- */

    void GrabObject (Collider col)
    {
        col.transform.SetParent(gameObject.transform);
        col.GetComponent<Rigidbody>().isKinematic = true;
        device.TriggerHapticPulse(40);
    }

    void ThrowObject (Collider col)
    {
        col.transform.SetParent(null);
        Rigidbody rigidbody = col.GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        rigidbody.velocity = device.velocity * throwForce;
        rigidbody.angularVelocity = device.angularVelocity;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Throwable"))
        {
            if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                ThrowObject(other);
            }
            else if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                GrabObject(other);
            }
		}
        if (other.gameObject.CompareTag("Structure"))
        {
            if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                other.transform.SetParent(null);
               
            }
            else if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                 GrabObject(other);
            }

        }
			
    }
		

    /* -------------------------------------------------------------------------------------------------------------------------------------------------------- //

                                                                                   Object Menu
   // -------------------------------------------------------------------------------------------------------------------------------------------------------- */

    void ObjectMenu ()
    {
		if (ControllerRight)
        {
            if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad))
            {
                touchLast = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
            }
            if (device.GetTouch(SteamVR_Controller.ButtonMask.Touchpad)) {
                touchCurrent = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
                distance = touchCurrent - touchLast;
                touchLast = touchCurrent;
                swipeSum += distance;

                if (!hasSwipedRight)
                {
                    if (swipeSum > 0.3f)
                    {
                        swipeSum = 0;
                        SwipeRight();
                        hasSwipedRight = true;

                    }
                }

                if (!hasSwipedLeft)
                {
                    if (swipeSum < -0.3f)
                    {
                        swipeSum = 0;
                        SwipeLeft();
                        hasSwipedLeft = true;
						Active.enabled = true;
                    }
                }
            }

            if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad))
            {
				Active.enabled = false;
                int currentObject = objectMenuManager.currentObject;
                objectMenuManager.objectList[currentObject].SetActive(false);

                swipeSum = 0;
                touchCurrent = 0;
                touchLast = 0;
                hasSwipedLeft = false;
                hasSwipedRight = false;
            }
            
            if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) && device.GetTouch(SteamVR_Controller.ButtonMask.Touchpad))
            {
				SpawnObject ();


            }

        }
    }

    void SwipeRight()
    {
        objectMenuManager.MenuRight();
    }

    void SwipeLeft ()
    {
        objectMenuManager.MenuLeft();
    }

    void SpawnObject ()
	{
		objectMenuManager.SpwanCurrentObject ();
	}

		
	}

    


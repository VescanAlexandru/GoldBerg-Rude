using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

public class InstructionManager : MonoBehaviour {

	private SteamVR_TrackedObject trackedObj;
	private SteamVR_Controller.Device device;
	public GameObject canvas;
	public Instructuction instruction;
	public Text tutorialText;
	public GameObject controllerText;
	public bool LeftController;
	public bool RightController;

	void Awake ()
	{
		trackedObj = GetComponentInParent<SteamVR_TrackedObject>();
	}

	void Update ()
	{
		device = SteamVR_Controller.Input((int)trackedObj.index);
		DisplayHints();
	}

	void DisplayHints ()
	{
		if (SceneManager.GetActiveScene().name == "Start")
		{
			if (LeftController)
			{
				//- loads the teleportation hint when user touches the touchpad
				if (device.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
				{
					tutorialText.text = "For teleporting, hold and\nrelease the touchpad";
				}
				if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
				{
					tutorialText.text = "To interact with the ball\nuse one of the\ncontroller and hold the trigger\nto lift it";
					instruction.HasTeleported = true;
				}
				if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
				{
					instruction.BallHasBeenLifted = true;
				}
				if (instruction.BallHasBeenLifted == true && instruction.HasTeleported == true)
				{
					tutorialText.text = "To continue\npress trigger on the right\ncontroller ";
					instruction.LeftControllerTutorialCompleted = true;
				}
			}
			if (RightController)
			{
				//- can only complete right controller tutorial if player has completed left controller tutorial
				if (instruction.LeftControllerTutorialCompleted == true)
				{
					if (device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
					{
						tutorialText.text = "To choose an item,\ntouch the touchpad while\npressing the trigger.\nUse the item like\nyou use the ball";
					}
					if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
					{
						instruction.RightControllerTutorialCompleted = true;
					}
				}
			}
			if (instruction.LeftControllerTutorialCompleted == true && instruction.RightControllerTutorialCompleted == true)
			{
				if (LeftController)
					controllerText.SetActive(false);
				if (RightController)
					controllerText.SetActive(false);
				canvas.SetActive (false);
			}
		}
	}
}

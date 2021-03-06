﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Goal : MonoBehaviour {

    public GameObject[] collectables;
    

	void OnCollisionEnter (Collision other)
	{
		if (other.gameObject.CompareTag("Throwable") && AreStarsAndGoalInActive()) 
		{
				switch (SceneManager.GetActiveScene().name)
				{
			     case "Start":
				    SteamVR_LoadLevel.Begin("level1");
				    break;
				case "Level1":
					SteamVR_LoadLevel.Begin("level2");
					break;
				case "Level2":
					SteamVR_LoadLevel.Begin("level3");
					break;
				case "Level3":
					SteamVR_LoadLevel.Begin("level4");
					break;
				case "Level4":
					SteamVR_LoadLevel.Begin("WinScreen");
					break;
				default: Debug.Log("StartScreen");
					break;
				}
		}
	}

    bool AreStarsAndGoalInActive()
    {
        foreach (GameObject collectable in collectables)
        {
			if (collectable.gameObject.activeInHierarchy)
            {
                return false;
            }
        }
        return true;
    }
}

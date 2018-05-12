using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTeleporter : MonoBehaviour
{

    public Transform exit;
    static Transform last;


    void OnTriggerEnter(Collider other)
    {
        if (exit == last)
            return;
        TeleportToExit(other);
    }
    void OnTriggerExit()
    {
        if (exit == last)
            last = null;
		
    }
    void TeleportToExit(Collider other)
	{if (other.gameObject.CompareTag ("Throwable")){
        last = transform;
        other.transform.position = exit.transform.position;
		Rigidbody rigidbody = other.GetComponent<Rigidbody>();
		rigidbody.isKinematic = false;
    }


}
}

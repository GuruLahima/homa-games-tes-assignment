using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Com.HomaGamesTest.Alek{

	public class CylinderDetector : MonoBehaviour
	{

		void OnTriggerExit(Collider other){
			if(transform.CompareTag("OutsideDetector"))
				if(other.gameObject.layer == LayerMask.NameToLayer("Cylinder") && other.transform.parent != null){
					// turning isKinematic on triggers this event so we are using this as a means to circumvent it
					if(other.GetComponent<CylinderInfo>().justGotUnlocked){
						other.GetComponent<CylinderInfo>().justGotUnlocked = false;
						return;
					}
					other.transform.parent = null;
					LevelManager.instance.UpdateLevelProgress();
					LevelManager.instance.UpdateFloors();
				}
		}

		void OnTriggerEnter(Collider other){
			if(transform.CompareTag("InsideDetector"))
				if(other.gameObject.layer == LayerMask.NameToLayer("Cylinder") && other.transform.parent != null){
					other.transform.parent = null;
					if(other.GetComponent<CylinderInfo>().justGotUnlocked){
						other.GetComponent<CylinderInfo>().justGotUnlocked = false;
						return;
					}
					LevelManager.instance.UpdateLevelProgress();
					LevelManager.instance.UpdateFloors();
				}
		}
	}
}

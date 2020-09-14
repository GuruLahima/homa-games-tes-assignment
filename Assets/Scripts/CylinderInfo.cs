using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.HomaGamesTest.Alek{

	public class CylinderInfo : MonoBehaviour
	{
		public Material assignedColor;
		// this flag is used as a solution to a problem where turning isKinematic on and off triggers OnTriggerExit
		//  which makes tracking destroyed cylinders dificult
		public bool justGotUnlocked; 
	}
}
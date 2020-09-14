using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Com.HomaGamesTest.Alek{

	public class ObjectCollector : MonoBehaviour
	{
	    void OnCollisionEnter(Collision colInfo){
	    	if(colInfo.gameObject.layer == LayerMask.NameToLayer("Cylinder") || colInfo.gameObject.layer == LayerMask.NameToLayer("CylinderDestroyed")){
	    		Destroy(colInfo.collider);
	    		Destroy(colInfo.rigidbody);
	    		Destroy(colInfo.transform.GetComponent<CylinderInfo>());
	    	}
	    	else {
	    		Destroy(colInfo.gameObject);
	    	}

	    }
	}
}

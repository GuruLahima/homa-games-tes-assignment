using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.HomaGamesTest.Alek{

	public class OnHit : MonoBehaviour
	{


		private Rigidbody rgbody;


		void Start(){
			rgbody = GetComponent<Rigidbody>();
		}

		void OnCollisionEnter(Collision colInfo){

			// destroy cylinder if it was same color, if not just bounce off
			Material cylinderMat = colInfo.transform.GetComponent<Renderer>().sharedMaterial;
			Material ballMat = transform.GetComponent<Renderer>().sharedMaterial;
			if(cylinderMat == ballMat){
				// get all same coloured cylinders
				List<Collider> chainOfCylinders = new List<Collider>();
				chainOfCylinders.AddRange(FindChainOfSameColour(colInfo.transform, colInfo.transform.GetComponent<Renderer>().sharedMaterial, LevelManager.instance.proximityDistance));

				// destroy the chain of cylinders, add explision force, and show partile effects
				foreach(Collider item in chainOfCylinders){

					// apply force t surrounding cylinders
					SimulateExplosion(item.transform, LevelManager.instance.explosionForce, LevelManager.instance.explosionRadius);

					// particle effect
					GameObject partEff = Instantiate(LevelManager.instance.popCylinderEffect, item.transform.position, LevelManager.instance.popCylinderEffect.transform.rotation);
					partEff.transform.GetComponent<ParticleSystemRenderer>().material = item.GetComponent<Renderer>().material;

					// keep track of level progress
					if(item.transform.parent != null)
						LevelManager.instance.UpdateLevelProgress();

					// destroy cylinder
					Destroy(item.gameObject);
				}
				// tell LeveManager to unlock new floors if neccessary
				if(LevelManager.instance.chooseTowerType == LevelManager.TowerType.Tetris){
					if(UIScreens.instance.comboGoober.sharedMaterial == transform.GetComponent<Renderer>().sharedMaterial)
					{

						TetrisMode.UpdateFloor(chainOfCylinders);
					}
				}
				else
					LevelManager.instance.UpdateFloors();


				
				// finaly destroy this ball so it doesnt destroy more cylinders
				Destroy(gameObject);
			}
			else{
				// if cylinder was not same color make the ball bounce off somewhere else harmlessly
				rgbody.useGravity = true;
				rgbody.isKinematic = false;
				Vector3 bounceDirection = (transform.position - colInfo.transform.position).normalized;
				transform.GetComponent<Rigidbody>().AddForce(bounceDirection * LevelManager.instance.bounceForce, ForceMode.Impulse);
				gameObject.layer = LayerMask.NameToLayer("Default");
			}
		}



		#region private methods

		private List<Collider> GetNearbyCylinders(Transform origin, float radius){
			List<Collider> cylinders =  new List<Collider>(Physics.OverlapSphere(origin.position, radius, LayerMask.GetMask("Cylinder")));
			if(LevelManager.instance.chooseTowerType == LevelManager.TowerType.Tetris){
				List<Collider> tmpList = cylinders.FindAll(p => (Mathf.Abs(p.transform.position.y - origin.position.y) < 0.1f || (Mathf.Abs(p.transform.position.z - origin.position.z) < 0.1f) && Mathf.Abs(p.transform.position.x - origin.position.x) < 0.1f ));
				cylinders = tmpList;
				
			}
			return cylinders;
		}

		private List<Collider> GetNearbyCylindersOfSameColor(Transform origin, Material mat, float radius){
			List<Collider> cylinders =  new List<Collider>(GetNearbyCylinders(origin, radius));
			cylinders.RemoveAll(a => a.transform.GetComponent<Renderer>().sharedMaterial != mat);
			ExcludeFromConsecutiveSearches(cylinders);
			return cylinders;
		}

		private void ExcludeFromConsecutiveSearches(List<Collider> cylinders){
			foreach(Collider cyl in cylinders){
				cyl.gameObject.layer = LayerMask.NameToLayer("CylinderDestroyed");
			}
		}

		private List<Collider> FindChainOfSameColour(Transform origin, Material mat, float radius){

			List<Collider> cylinders = new List<Collider>();
			List<Collider> chainedCylinders = new List<Collider>();

			cylinders.AddRange(GetNearbyCylindersOfSameColor(origin, mat, radius));

			while(cylinders.Count > 0){
				cylinders.AddRange(GetNearbyCylindersOfSameColor(cylinders[0].transform, mat, radius));
				chainedCylinders.Add(cylinders[0]);
				cylinders.RemoveAt(0);
			}
				
			return chainedCylinders;
		}


		private void SimulateExplosion(Transform origin, float force, float radius){
			List<Collider> affectedCylinders = GetNearbyCylinders(origin, radius);
			foreach(Collider cyl in affectedCylinders){

				// calculate direction of explosion
				Vector3 direction = cyl.transform.position - origin.position;
				direction = direction.normalized;

				// apply force
				cyl.GetComponent<Rigidbody>().AddForce(direction * force, ForceMode.Impulse);
			}

		}

		#endregion private methods


	}
}
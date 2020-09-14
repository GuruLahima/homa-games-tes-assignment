using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.HomaGamesTest.Alek{

	public static class NormalMode{

		public static List<GameObject> InstantiateCircleTower(Vector3 center, int pieceCount, int floorCount, GameObject prefab, float m_radius, bool rotateFloors){
			List<GameObject> generatedFloors = new List<GameObject>();			
			float angle = 360f / (float)pieceCount;
			GameObject floorParent = new GameObject();
			float yOffset = prefab.transform.GetComponent<Renderer>().bounds.size.y/2;
			floorParent.transform.position = (center + new Vector3(0, yOffset, 0));

	    // first generate one floor
			for (int i = 0; i < pieceCount; i++)
			{
				Quaternion rotation = Quaternion.AngleAxis(i * angle, Vector3.up);
				Vector3 direction = rotation * Vector3.forward;

				Vector3 position = floorParent.transform.position + (direction * m_radius);

				GameObject newCyl = MonoBehaviour.Instantiate(prefab, position, rotation);
				newCyl.transform.parent = floorParent.transform;

			}
			generatedFloors.Add(floorParent);

	    // now duplicate, raise, and rotate that floor floorCount amount of times
			Vector3 floorOffset = new Vector3(0, prefab.transform.GetComponent<Renderer>().bounds.size.y, 0);
			for (int i = 1; i < floorCount; i++)
			{
				Quaternion rotation = rotateFloors ? Quaternion.AngleAxis(angle / ((i % 2) + 1), Vector3.up) : Quaternion.identity;
				generatedFloors.Add(MonoBehaviour.Instantiate(floorParent, floorParent.transform.position + (floorOffset * i), rotation));
			}

			return generatedFloors;
		}

		public static List<GameObject> TriangleTower(Vector3 center, int pieceCount, int floorCount, GameObject prefab, float m_radius, bool rotateFloors){
			// stub. to be coded
			List<GameObject> generatedFloors = InstantiateCircleTower(center, pieceCount, floorCount, prefab, m_radius, rotateFloors);			
			return generatedFloors;

		}

		public static List<GameObject> HourGlassTower(Vector3 center, int pieceCount, int floorCount, GameObject prefab, float m_radius, bool rotateFloors){
			// stub. to be coded
			List<GameObject> generatedFloors = InstantiateCircleTower(center, pieceCount, floorCount, prefab, m_radius, rotateFloors);			
			return generatedFloors;

		}
		
		public static void AssignRandomColors(List<GameObject> floors, Material[] colors){
			foreach(GameObject floor in floors){
				Material mat;
				foreach(Transform cylinder in floor.transform){
					mat = colors[Random.Range(0, colors.Length)];
					// record the assigned colors so they can be reused when floors become unlocked
					cylinder.GetComponent<CylinderInfo>().assignedColor = mat;
					cylinder.GetComponent<Renderer>().material = mat;
				}
			}
		}


		public static IEnumerator LockFloors(List<GameObject> floors, List<GameObject> m_lockedFloors, int activeFloors, float delay, Material lockedCylinderColor){
			for(int i = 0; i < floors.Count - activeFloors; i++){
				foreach(Transform cylinder in floors[i].transform){
					// give the locked cylinders that dark material
					cylinder.GetComponent<Renderer>().material = lockedCylinderColor;
					// freeze them in place
					cylinder.GetComponent<CylinderInfo>().justGotUnlocked = false;
					cylinder.GetComponent<Rigidbody>().isKinematic = true;
					// cylinder.GetComponent<Rigidbody>().constraints =  RigidbodyConstraints.FreezeAll;
					// we don't want player to be able to shoot balls at locked cylinders
					cylinder.gameObject.layer = LayerMask.NameToLayer("Inactive");
				}
				// add locked floors to the locked floors list. we will shift floors from this list to the unlockedList as we destroy cylinders
				m_lockedFloors.Add(floors[i]);

				yield return new WaitForSeconds(delay);
			}
		}

		public static void UnlockFloors(List<GameObject> list, int numberOfFloors){
			int floorToReach;
			if(list.Count - 1 - numberOfFloors < 0)
				floorToReach = -1;
			else
				floorToReach = list.Count - 1 - numberOfFloors;

			for(int i = list.Count - 1; i > floorToReach; i--){
				foreach(Transform cylinder in list[i].transform){
					cylinder.GetComponent<Renderer>().material = cylinder.GetComponent<CylinderInfo>().assignedColor;
					cylinder.GetComponent<CylinderInfo>().justGotUnlocked = true;
					cylinder.GetComponent<Rigidbody>().isKinematic = false;
					// cylinder.GetComponent<Rigidbody>().constraints =  RigidbodyConstraints.None;
					cylinder.gameObject.layer = LayerMask.NameToLayer("Cylinder");
				}
			}

		}

	}
}
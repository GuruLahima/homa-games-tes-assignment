using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.HomaGamesTest.Alek{

	public static class TetrisMode{

			public static List<GameObject> TetrisTower(Vector3 center, int pieceCount, int floorCount, 
																								GameObject prefab, GameObject discPrefab, float m_radius, bool rotateFloors){
				List<GameObject> generatedFloors = new List<GameObject>();			
				float angle = 360f / (float)pieceCount;
				GameObject floorParent = new GameObject();
				float yOffset = prefab.transform.GetComponent<Renderer>().bounds.size.y/2;
				floorParent.transform.position = (center + new Vector3(0, yOffset, 0));
				Vector3 floorOffset = new Vector3(0, prefab.transform.GetComponent<Renderer>().bounds.size.y, 0);

				GameObject disc = MonoBehaviour.Instantiate(discPrefab, floorParent.transform.position + (floorOffset * 0), discPrefab.transform.rotation);
				LevelManager.instance.tetrisDisks.Add(disc);

		    // first generate one floor
				for (int i = 0; i < pieceCount; i++)
				{
					Quaternion rotation = Quaternion.AngleAxis(i * angle, Vector3.up);
					Vector3 direction = rotation * Vector3.forward;

					Vector3 position = floorParent.transform.position + floorOffset + (direction * m_radius);

					GameObject newCyl = MonoBehaviour.Instantiate(prefab, position, rotation);
					newCyl.transform.parent = floorParent.transform;

				}
				generatedFloors.Add(floorParent);

		    // now duplicate, raise, and rotate that floor floorCount amount of times
				for (int i = 1; i < floorCount; i++)
				{
					if(i%5 == 0){
						disc = MonoBehaviour.Instantiate(discPrefab, floorParent.transform.position + (floorOffset * (i+1)), discPrefab.transform.rotation);
						LevelManager.instance.tetrisDisks.Add(disc);
					}
					else{
						Quaternion rotation = Quaternion.identity;
						generatedFloors.Add(MonoBehaviour.Instantiate(floorParent, floorParent.transform.position + (floorOffset * i), rotation));
					}
				}

				// we rely on only the top most platform to have the TetrisObstacle script
				disc.AddComponent<TetrisObstacle>();

				return generatedFloors;
			}

			public static void AssignRandomColors(List<GameObject> floors, Material[] colors, List<GameObject> obstacles){
				Material mat;				
				foreach(GameObject floor in floors){
					foreach(Transform cylinder in floor.transform){
						mat = colors[Random.Range(0, colors.Length)];
						// record the assigned colors so they can be reused when floors become unlocked
						cylinder.GetComponent<CylinderInfo>().assignedColor = mat;
						cylinder.GetComponent<Renderer>().material = mat;
					}
				}
				foreach(GameObject obst in obstacles){
						mat = colors[Random.Range(0, colors.Length)];
						obst.GetComponent<Renderer>().material = colors[Random.Range(0, colors.Length)];
				}
				UIScreens.instance.comboGoober.material = obstacles[obstacles.Count-1].transform.GetComponent<Renderer>().sharedMaterial;
			}


			// in tetris mode there are obstacles that can only be destroyed if the player makes a big combo of the same color
			// the destruction of cylinders also only works in horizontal and vertical
			//  "In this mode there are platforms that can be destroyed by making a combo of the same color, just like in tetris!"

			public static void UpdateFloor(List<Collider> combo){
				List<GameObject> trtDsks = LevelManager.instance.tetrisDisks;
				Debug.Log("Updating tetris floor" + trtDsks[trtDsks.Count-1].transform.position);
				if(combo.Count >= LevelManager.instance.obstacleThreshold ){

					Debug.Log("Disk destroyed, continue to disk " + (trtDsks.Count - 1));

					MonoBehaviour.Destroy(trtDsks[trtDsks.Count-1]);
					trtDsks.RemoveAt(trtDsks.Count-1);
					if(trtDsks.Count - 1 < 0){
						//  game won
						Debug.Log("<color=blue> GAME WON </color");
						LevelManager.instance.LevelComplete();
						return;
					}


					LevelManager.instance.obstacleThreshold += LevelManager.instance.obstacleIncrement;
					UIScreens.instance.comboGoober.material = trtDsks[trtDsks.Count-1].GetComponent<Renderer>().sharedMaterial;
					UIScreens.instance.comboCounter.text = LevelManager.instance.obstacleThreshold.ToString();
					trtDsks[trtDsks.Count-1].AddComponent<TetrisObstacle>();
					
					// tell camera to follow unlocked floors
					CameraMovement cam = Camera.main.GetComponent<CameraMovement>();
					if(cam.routine != null){
						cam.StopCoroutine(cam.routine);
					}
					cam.routine = cam.StartCoroutine(cam.SlideToPosition(trtDsks[trtDsks.Count-1].transform.position, 1f));
				}
			}
	}
}
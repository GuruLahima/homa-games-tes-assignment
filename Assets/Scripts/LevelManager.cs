using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Array = System.Array;
using UnityEngine.SceneManagement;

namespace Com.HomaGamesTest.Alek{

	public class LevelManager : MonoBehaviour
	{
		// easy access to this object's public fields
		public static LevelManager instance;

		public enum TowerType{
			Circle = 0,
			Triangle = 1,
			HourGlass = 2,
			Tetris,
		}

		// Properties shown using custom editor script. See Editor folder

		#region public fields
		[Tooltip("How many different colors the cylinders will be")]
		[SerializeField] public Material[] cylinderColors;
		public bool gameWon;
		[Tooltip("Type of tower")]
		[SerializeField] public TowerType chooseTowerType;
		[Tooltip("The platforms that need to be broken to continue with the level. Tetris mode only")]
		[HideInInspector] public List<GameObject> tetrisDisks;
		#endregion public fields

		#region Exposed private fields
		/* Level generation settings */
		[Tooltip("Should this level repeat")]
		[SerializeField] bool infiniteLevel;
		[RangeAttribute(0,100)]
		[Tooltip("How much of the cylinders should be destroyed/knocked over in order to win the game")]
		[SerializeField] float winPercentage;		
		[Tooltip("If this is true it will assume this object's position as the center, otherwise it will use spawnCenter")]
		[SerializeField] bool thisObjectIsSpawnPoint;
		[SerializeField] Transform spawnCenter;
		[Tooltip("Radius of tower")]
		[SerializeField] float radius;
		[Tooltip("Number of cylinders in a floor")]
		[MinAttribute(3)]
		[SerializeField] int pieceCount;
		[Tooltip("Number of floors in the entire tower of cylinders")]
		[MinAttribute(1)]
		[SerializeField] int floorCount;
		[Tooltip("Should every consecutive floor be rotated by little?")]
		[SerializeField] bool rotateFloors;
		[Tooltip("Tetris mode only")]
		[SerializeField] GameObject discPrefab;
		[SerializeField] public int obstacleThreshold;
		[SerializeField] public int obstacleIncrement;

		[SerializeField] Material lockedCylinderColor;
		[Tooltip("The small time between locking the floors on the initial pass that colors them black")]
		[SerializeField] float delayBetweenFloors;

		/* Cylinder settings */
		[SerializeField] GameObject cylinderPrefab;

		/* Camera settings */
		[Tooltip("The time for the camera to move from bottom to top at the beggining of the level")]
		[SerializeField] float moveCameraUpInterval;
		[Tooltip("Hoe much the camera rotates around the tower at start of level as an intro")]
		[SerializeField] float cameraRotationAtStart;
		[Tooltip("how fast the camera falls down to next floor")]
		[SerializeField] float cameraSlideDownSpeed;

		[Tooltip("The collider that checks if a cylinder fell off and should be considered destroyed")]
		[HideInInspector] public  GameObject fallOffDetectorOutside;
		[HideInInspector] public  GameObject fallOffDetectorInside;

		/* explosion settings */
		[SerializeField] public GameObject popCylinderEffect;
		[Tooltip("How close to the destroyed cylinder should other cylinders be to be included in the chain reaction")]
		[SerializeField] public float proximityDistance;
		[SerializeField] public float explosionForce;
		[SerializeField] public float explosionRadius;
		[Tooltip("How much does the ball bounce off of cylinders")]
		[SerializeField] public float bounceForce;

		[SerializeField] GameObject celebrationEffect;
		#endregion Exposed private fields



		#region private fields

		private List<GameObject> allFloors;
		private List<GameObject> lockedFloors;
		private List<GameObject> unlockedFloors;
		private CameraMovement mainCam;
		private Vector3 centerOfTower;
		private float completedPercentage;
		private int cylinderCount;
		private int initialCylinderCount;
		private bool levelComplete;

		#endregion private fields



		#region MonoBehaviour callbacks

		// makes sure we have at least 2 types of cylinders
		void OnValidate()
		{
		    if (cylinderColors.Length < 2)
		    {
		        Array.Resize(ref cylinderColors, 2);
		    }
		}

		void Awake(){
			if(instance == null)
				instance = this;
			else if(instance != this)
				Destroy(gameObject);
		}


	    // Start is called before the first frame update
		void Start()
		{
			if(thisObjectIsSpawnPoint){
				centerOfTower = this.transform.position;
			}
			mainCam = Camera.main.GetComponent<CameraMovement>();
			allFloors = new List<GameObject>();
			lockedFloors = new List<GameObject>();
			fallOffDetectorOutside.SetActive(false);
			fallOffDetectorInside.SetActive(false);

			UIScreens.instance.currentLevel.text = SceneManager.GetActiveScene().buildIndex.ToString();
			UIScreens.instance.nextLevel.text = (SceneManager.GetActiveScene().buildIndex  + 1).ToString();
			UIScreens.instance.levelProgress.fillAmount = 0f;
			UIScreens.instance.ballCounter.gameObject.SetActive(false);

			// generate level
			allFloors.AddRange(GenerateLevel(chooseTowerType));
			initialCylinderCount = allFloors.Count * allFloors[0].transform.childCount;
			if(chooseTowerType != TowerType.Tetris){

				// assign random colors to all cylinders and remember those colors 
				NormalMode.AssignRandomColors(allFloors, cylinderColors);
				// lock them in place
				for(int i = 0; i < allFloors.Count - 8; i++){
					foreach(Transform cylinder in allFloors[i].transform){
						cylinder.GetComponent<Rigidbody>().isKinematic = true;
					}
				}
			}
			else{
				TetrisMode.AssignRandomColors(allFloors, cylinderColors, tetrisDisks);
				UIScreens.instance.comboCounter.text = obstacleThreshold.ToString();

			}

			UIScreens.instance.ShowScreen("start", true);
		}

		void Update(){
			// Debugging
			/*if(Input.GetKeyDown("k"))
				NormalMode.UnlockFloors(lockedFloors, 1);*/
		}
		#endregion MonoBehaviour callbacks



		#region public methods

		public void UpdateLevelProgress(){
			Debug.Log("Updating progress");

			cylinderCount++;

			completedPercentage = ((float)cylinderCount / initialCylinderCount) * 100f;

			UIScreens.instance.levelProgress.fillAmount = (float)cylinderCount / initialCylinderCount;

			if(completedPercentage >= winPercentage && !levelComplete){
				levelComplete = true;
				LevelComplete();
			}
		}

		// rewrite this so it goes hrough al the floors and checks if they are empty
		public void UpdateFloors(){



				for(int i = allFloors.Count-1; i > lockedFloors.Count-1; i--){

					// if all cylinders of the topmost floor have fallen or been destroyed unlock a new floor to maintain an 8 floor minimum
					if(allFloors[i].transform.childCount <= 0){
						if(lockedFloors.Count > 0){

							allFloors.RemoveAt(allFloors.Count - 1);
							
							// unlock a new floor
							NormalMode.UnlockFloors(lockedFloors, 1);

							// tell camera to follow unlocked floors
							if(mainCam.routine != null){
								StopCoroutine(mainCam.routine);
							}
							mainCam.routine = StartCoroutine(mainCam.SlideToPosition(lockedFloors[lockedFloors.Count - 1].transform.position, cameraSlideDownSpeed));

							// remove the unlocked floor from the locked floors list
							lockedFloors.RemoveAt(lockedFloors.Count - 1);
						}
					}
				}
		}

		public void RestartLevel(){
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

		public void StartLevel(){
			// start the sequence for a new level
			StartCoroutine(NewLevelSequence());

			UIScreens.instance.ShowScreen("start", false);
		}

		public void NextLevel(){
			if(infiniteLevel)
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			else
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		}

		#endregion public methods



		#region private methods

		IEnumerator NewLevelSequence(){
			
			// at the same time rotate the camera
			Vector3 targetPos = allFloors[Mathf.Clamp(allFloors.Count - 8, 0, allFloors.Count)].transform.position;
			StartCoroutine(mainCam.RotateObject(targetPos, Vector3.up, cameraRotationAtStart, moveCameraUpInterval));
			// move camera slowly upward to top of tower
			if(chooseTowerType == TowerType.Tetris){
				yield return StartCoroutine(mainCam.SlideToPosition(tetrisDisks[tetrisDisks.Count-1].transform.position, moveCameraUpInterval));
			}
			else{
				yield return StartCoroutine(mainCam.SlideToPosition(targetPos, moveCameraUpInterval));
				// lock all fooors except top 8
				yield return StartCoroutine(NormalMode.LockFloors(allFloors, lockedFloors, 8, delayBetweenFloors, lockedCylinderColor));
			}
			// enable camera movement and shooting
			mainCam.GetComponent<CameraMovement>().canRotate = true;
			mainCam.GetComponent<ShootBall>().canShoot = true;
			// spawn a ball
			mainCam.GetComponent<ShootBall>().SpawnNewBall();
			// adjust the collider sthat detects wehn cylinders fall off the tower
			fallOffDetectorOutside.GetComponent<CapsuleCollider>().radius = radius;
			fallOffDetectorOutside.SetActive(true);
			fallOffDetectorInside.GetComponent<CapsuleCollider>().radius = radius/2;
			fallOffDetectorInside.SetActive(true);

			UIScreens.instance.ballCounter.gameObject.SetActive(true);



			yield return null;
		}

		List<GameObject> GenerateLevel(TowerType type){
			List<GameObject> list;
			switch(type){
				case TowerType.Circle:
				list = NormalMode.InstantiateCircleTower(centerOfTower, pieceCount, floorCount, cylinderPrefab, radius, rotateFloors);
				break;
				case TowerType.Triangle:
				list = NormalMode.TriangleTower(centerOfTower, pieceCount, floorCount, cylinderPrefab, radius, rotateFloors);
				break;
				case TowerType.HourGlass:
				list = NormalMode.HourGlassTower(centerOfTower, pieceCount, floorCount, cylinderPrefab, radius, rotateFloors);
				break;
				case TowerType.Tetris:
				list = TetrisMode.TetrisTower(centerOfTower, pieceCount, floorCount, cylinderPrefab, discPrefab, radius, rotateFloors);
				break;
				default:
				list = NormalMode.InstantiateCircleTower(centerOfTower, pieceCount, floorCount, cylinderPrefab, radius, rotateFloors);
				break;
			}
			return list;
		}



		public void LevelComplete(){
			gameWon = true;
			mainCam.canRotate = false;
			mainCam.GetComponent<ShootBall>().canShoot = false;

			StartCoroutine(mainCam.MoveAway(30f, 0.8f));
			StartCoroutine(mainCam.RotateObject(centerOfTower, Vector3.up, 180f, 20f));

			// celebration effect (confeti)
			Instantiate(celebrationEffect, centerOfTower, celebrationEffect.transform.rotation);

			// show level clear text
			UIScreens.instance.ShowScreen("level_clear", true);

			Invoke("NextLevel", 4f);
		}

		#endregion private methods



	}
}
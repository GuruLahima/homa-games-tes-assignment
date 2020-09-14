using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.HomaGamesTest.Alek{

    public class ShootBall : MonoBehaviour
    {

    	#region exposed private fields

        [SerializeField] GameObject ballPrefab;

        [SerializeField] float delayForNewBall = 0.2f;
        [SerializeField] public int  ballCount = 15;


    	[SerializeField] Transform ballSpawnPosition;

    	#endregion


    	#region public fields

    	[Header("Projectile motion settings")]

        public float firingAngle = 45.0f;

        public float shootSpeed = 9.8f;

        public bool canShoot;

    	#endregion


    	#region Private fields

    	private Rigidbody currentBall;
        private Material prevBallMaterial;
        private CameraMovement cm;

    	#endregion

        // Start is called before the first frame update
        void Start()
        {
            UIScreens.instance.ballCounter.text = ballCount.ToString();

            cm = GetComponent<CameraMovement>();
        }

        
        void Update()
        {
            if(canShoot && !cm.isRotating){
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    if(touch.phase == TouchPhase.Ended)
                      Shoot();        	
                }
    	    }
        }

        void Shoot(){
            


            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
            if(Physics.Raycast(ray, out hitInfo, 100f, LayerMask.GetMask("Cylinder"), QueryTriggerInteraction.UseGlobal)){

                // Debug.DrawLine(currentBall.position, hitInfo.point, Color.cyan, 2f, true);  
                StartCoroutine(SimulateProjectile(hitInfo.point));


                ballCount--;
                UIScreens.instance.ballCounter.text = ballCount.ToString();

                if(ballCount <= 0){
                    canShoot = false;
                    // wait for a while to see if game was won with final shot
                    Invoke("GameOver", 3f);
                    return;
                }

                SpawnNewBall();
            }   
        }


        IEnumerator SimulateProjectile(Vector3 Target)
        {
            Transform Projectile = currentBall.transform;
            Projectile.parent = null;
          
           
            // Calculate distance to target
            float target_Distance = Vector3.Distance(Projectile.position, Target);
        
            // Calculate the velocity needed to throw the object to the target at specified angle.
            float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / shootSpeed);
        
            // Extract the X  Y componenent of the velocity
            float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
            float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);
        
            // Calculate flight time.
            float flightDuration = target_Distance / Vx;
        
            // Rotate projectile to face the target.
            Projectile.rotation = Quaternion.LookRotation(Target - Projectile.position);
           
            float elapse_time = 0;
        
            // move the ball in a parabolic curve towards clicked cylinder
            while (elapse_time < flightDuration)
            {
                if(Projectile != null)
                    Projectile.Translate(0, (Vy - (shootSpeed * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);
                else
                    break;
               
                elapse_time += Time.deltaTime;
        
                yield return null;
            }

            yield return null;       

        } 


        public void SpawnNewBall(){


            // improvement: perform pooling here. and also it needs to not be so instantenious , there should be an animation of creation and it shouldnt have collider right away so it doesnt bother previous ball
            currentBall = Instantiate(ballPrefab, ballSpawnPosition.position, Quaternion.identity).GetComponent<Rigidbody>();
            currentBall.transform.SetParent(ballSpawnPosition, true);


            // assign a material from the materials defined in LevelGenerator, but make sure to not repeat the same material twice in a row
            List<Material> availableColors = new List<Material>(LevelManager.instance.cylinderColors);
            int index = availableColors.FindIndex(a => a == prevBallMaterial);
            if(index != -1)
                availableColors.RemoveAt(index);   //  Remove material already used
            else{
                Debug.LogWarning("This warning should only appear if ball started out with unknown material.");
            }

            currentBall.GetComponent<Renderer>().material = availableColors[Random.Range(0,availableColors.Count)];

            prevBallMaterial = currentBall.GetComponent<Renderer>().sharedMaterial;

        }

        void GameOver(){
            if(!LevelManager.instance.gameWon){
                canShoot = false;
                cm.canRotate = false;
                UIScreens.instance.ShowScreen("restart");
            }
        }


    }
}
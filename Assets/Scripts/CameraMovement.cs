using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.HomaGamesTest.Alek{

    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] Transform centerOfRotation;
        [SerializeField] float turnSpeed;
    	[SerializeField] float slowDownFactor;
        [SerializeField] float offsetFromFloor;
        [SerializeField] float dragThreshold;

        public bool canRotate;
        public bool isRotating;

        public Coroutine routine;

        // Start is called before the first frame update
        void Start()
        {
            Reposition();
        }

        float rot;
        float targetAngle;
        // Update is called once per frame
        void Update()
        {
            if(canRotate){
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);

                    if(touch.phase == TouchPhase.Moved && Mathf.Abs(touch.deltaPosition.x) > dragThreshold) {
                        isRotating = true;
                        
                        targetAngle = touch.deltaPosition.x * slowDownFactor;
                        

                    }
                    else if(touch.phase == TouchPhase.Ended)
                        targetAngle = 0f;


                }
                else {
                    isRotating = false;
                }

                rot = Mathf.Lerp(rot, targetAngle, Time.deltaTime * turnSpeed);
                transform.RotateAround(centerOfRotation.position, Vector3.up, rot);
            }
        }

        public IEnumerator SlideToPosition(Vector3 pos, float lerpTime)
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = startPos;
            endPos.y = pos.y + offsetFromFloor;
            float currentLerpTime = 0f;

            while(currentLerpTime < lerpTime){

                currentLerpTime += Time.deltaTime;
                if (currentLerpTime > lerpTime) {
                    currentLerpTime = lerpTime;
                }

                float t = currentLerpTime / lerpTime;
                t = t*t * (3f - 2f*t);

                // move in y axis                
                Vector3 newPos = transform.position;
                newPos.y = Mathf.Lerp(startPos.y, endPos.y, t);
                transform.position = newPos;

                yield return null;
            }
        }

        public IEnumerator RotateObject(Vector3 point, Vector3 axis, float rotateAmount, float rotateTime) {
            var step = 0.0f; //non-smoothed
            var rate = 1.0f/rotateTime; //amount to increase non-smooth step by
            var smoothStep = 0.0f; //smooth step this time
            var lastStep = 0.0f; //smooth step last time
            while(step < 1.0) { // until we're done
                step += Time.deltaTime * rate; //increase the step
                smoothStep = Mathf.SmoothStep(0.0f, 1.0f, step); //get the smooth step
                transform.RotateAround(point, axis, -rotateAmount * (smoothStep - lastStep));
                lastStep = smoothStep; //store the smooth step
                yield return null;
            }
            //finish any left-over
            if(step > 1.0) transform.RotateAround(point, axis, rotateAmount * (1.0f - lastStep));
        }

        public IEnumerator MoveAway(float distance, float lerpTime) {
            // StopCoroutine(routine);


            Vector3 startPos = transform.position;
            Vector3 endPos = startPos + (-transform.forward * distance);
            float currentLerpTime = 0f;

            while(currentLerpTime < lerpTime){

                currentLerpTime += Time.deltaTime;
                if (currentLerpTime > lerpTime) {
                    currentLerpTime = lerpTime;
                }

                float t = currentLerpTime / lerpTime;
                t = Mathf.Sin(t * Mathf.PI * 0.5f);

                // move in z axis                
                Vector3 newPos = Vector3.Lerp(startPos, endPos, t);
                transform.position = newPos;

                yield return null;
            }

        }

        // reposition camera to the lowest unlocked/active/colored floor. this should eventually be a coroutine that moves the camera smoothly
        void Reposition()
        {
            Vector3 newPos = transform.position;
            newPos.y = centerOfRotation.position.y + offsetFromFloor;
            transform.position = newPos;
        }

    }
}

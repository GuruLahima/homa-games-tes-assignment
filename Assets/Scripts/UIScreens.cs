using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIScreens : MonoBehaviour
{
		[SerializeField] GameObject restartScreen;
		[SerializeField] GameObject levelClearScreen;
		[SerializeField] GameObject startScreen;

		public TextMeshProUGUI  currentLevel;
		public TextMeshProUGUI  nextLevel;
        public TextMeshProUGUI  ballCounter;
		public TextMeshProUGUI  comboCounter;
        public Renderer comboGoober;
		public Image levelProgress;

		public static UIScreens instance;

        private Camera cam;


    // Start is called before the first frame update
    void Awake()
    {
		instance = this;
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(comboCounter != null && comboGoober != null){
            comboCounter.transform.position = cam.WorldToScreenPoint(comboGoober.transform.position);
        }
        
    }

    public void ShowScreen(string screenName, bool show = true){
    	switch(screenName){
    		case "restart":
    			restartScreen.SetActive(show);
    			break;
    		case "level_clear":
    			levelClearScreen.SetActive(show);
    			break;
    		case "start":
    			startScreen.SetActive(show);
    			break;	
    	}

    }
}

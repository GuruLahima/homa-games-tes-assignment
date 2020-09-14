using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Com.HomaGamesTest.Alek{

  [CustomEditor(typeof(LevelManager))]
	public class LevelManagerEditor : Editor
	{
    bool showLevelGenerationSettings = true;
    bool showCameraSettings = true;
    bool showCylinderSettings = true;
    bool explosionSettings = true;


    private SerializedProperty m_infiniteLevel;

    private SerializedProperty m_towerType;
    private SerializedProperty m_winPercentage;
    private SerializedProperty m_cylinderColors;
    private SerializedProperty m_radius;
    private SerializedProperty m_pieceCount;
    private SerializedProperty m_floorCount;
    private SerializedProperty m_rotateFloors;
    private SerializedProperty m_cylinderPrefab;
    private SerializedProperty m_lockedCylinderColor;
    private SerializedProperty m_delayBetweenFloors;
    private SerializedProperty m_fallOffDetectorInside;
    private SerializedProperty m_fallOffDetectorOutside;
    private SerializedProperty m_moveCameraUpInterval;
    private SerializedProperty m_cameraRotationAtStart;
    private SerializedProperty m_cameraSlideDownSpeed;
    private SerializedProperty m_thisObjectIsSpawnPoint;
    private SerializedProperty m_spawnCenter;
    private SerializedProperty m_celebrationEffect;
    private SerializedProperty m_discPrefab;
    private SerializedProperty m_obstacleThreshold;
    private SerializedProperty m_obstacleIncrement;

    private SerializedProperty m_popCylinderEffect;
    private SerializedProperty m_proximityDistance;
    private SerializedProperty m_explosionForce;
    private SerializedProperty m_explosionRadius;
    private SerializedProperty m_bounceForce;

    protected virtual void OnEnable () {
      m_winPercentage = serializedObject.FindProperty ( "winPercentage" );
      m_infiniteLevel = serializedObject.FindProperty ( "infiniteLevel" );
      m_towerType = serializedObject.FindProperty ( "chooseTowerType" );
      m_cylinderColors = serializedObject.FindProperty ( "cylinderColors" );
      m_pieceCount = serializedObject.FindProperty ( "pieceCount" );
      m_rotateFloors = serializedObject.FindProperty ( "rotateFloors" );
      m_floorCount = serializedObject.FindProperty ( "floorCount" );
      m_cylinderPrefab = serializedObject.FindProperty ( "cylinderPrefab" );
      m_radius = serializedObject.FindProperty ( "radius" );
      m_lockedCylinderColor = serializedObject.FindProperty ( "lockedCylinderColor" );
      m_delayBetweenFloors = serializedObject.FindProperty ( "delayBetweenFloors" );
      m_moveCameraUpInterval = serializedObject.FindProperty ( "moveCameraUpInterval" );
      m_fallOffDetectorOutside = serializedObject.FindProperty ( "fallOffDetectorOutside" );
      m_fallOffDetectorInside = serializedObject.FindProperty ( "fallOffDetectorInside" );
      m_thisObjectIsSpawnPoint = serializedObject.FindProperty ( "thisObjectIsSpawnPoint" );
      m_cameraSlideDownSpeed = serializedObject.FindProperty ( "cameraSlideDownSpeed" );
      m_cameraRotationAtStart = serializedObject.FindProperty ( "cameraRotationAtStart" );
      m_spawnCenter = serializedObject.FindProperty ( "spawnCenter" );
      m_celebrationEffect = serializedObject.FindProperty ( "celebrationEffect" );
      m_discPrefab = serializedObject.FindProperty ( "discPrefab" );
      m_obstacleThreshold = serializedObject.FindProperty ( "obstacleThreshold" );
      m_obstacleIncrement = serializedObject.FindProperty ( "obstacleIncrement" );

      m_popCylinderEffect = serializedObject.FindProperty ( "popCylinderEffect" );
      m_proximityDistance = serializedObject.FindProperty ( "proximityDistance" );
      m_explosionForce = serializedObject.FindProperty ( "explosionForce" );
      m_explosionRadius = serializedObject.FindProperty ( "explosionRadius" );
      m_bounceForce = serializedObject.FindProperty ( "bounceForce" );

    }


	  public override void OnInspectorGUI()
	  {
      GUI.enabled = false;
      EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((LevelManager)target), typeof(LevelManager), false);
      GUI.enabled = true;

      showLevelGenerationSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showLevelGenerationSettings, "Level generation settings");
      if (showLevelGenerationSettings){
        EditorGUILayout.PropertyField(m_infiniteLevel);

        EditorGUILayout.PropertyField(m_thisObjectIsSpawnPoint);
        if(!m_thisObjectIsSpawnPoint.boolValue){
          EditorGUI.indentLevel++;
          EditorGUILayout.PropertyField(m_spawnCenter);
          EditorGUI.indentLevel--;
        }

        EditorGUILayout.PropertyField(m_winPercentage);
        EditorGUILayout.PropertyField(m_towerType);
        if(m_towerType.enumValueIndex == 3) // Tetris tower is 3
        {
          EditorGUI.indentLevel++;
          EditorGUILayout.PropertyField(m_discPrefab);
          EditorGUILayout.PropertyField(m_obstacleThreshold);
          EditorGUILayout.PropertyField(m_obstacleIncrement);
          EditorGUI.indentLevel--;
        }
        EditorGUILayout.PropertyField(m_cylinderColors);
        EditorGUILayout.PropertyField(m_radius);
        EditorGUILayout.PropertyField(m_pieceCount);
        EditorGUILayout.PropertyField(m_floorCount);
        EditorGUILayout.PropertyField(m_rotateFloors);
        EditorGUILayout.PropertyField(m_lockedCylinderColor);
        EditorGUILayout.PropertyField(m_delayBetweenFloors);


      }
      EditorGUILayout.EndFoldoutHeaderGroup();

      showCameraSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showCameraSettings, "Camera settings");
      if (showCameraSettings){
        EditorGUILayout.PropertyField(m_moveCameraUpInterval);
        EditorGUILayout.PropertyField(m_cameraRotationAtStart);
        EditorGUILayout.PropertyField(m_cameraSlideDownSpeed);
      }
      EditorGUILayout.EndFoldoutHeaderGroup();

      showCylinderSettings = EditorGUILayout.BeginFoldoutHeaderGroup(showCylinderSettings, "Cylinder settings");
      if (showCylinderSettings){
        EditorGUILayout.PropertyField(m_cylinderPrefab);
        EditorGUILayout.PropertyField(m_fallOffDetectorOutside);
        EditorGUILayout.PropertyField(m_fallOffDetectorInside);
      }
      EditorGUILayout.Space();
      EditorGUILayout.PropertyField(m_celebrationEffect);
      EditorGUILayout.EndFoldoutHeaderGroup();

      explosionSettings = EditorGUILayout.BeginFoldoutHeaderGroup(explosionSettings, "Explosion settings");
      if (explosionSettings){
        EditorGUILayout.PropertyField(m_popCylinderEffect);
        EditorGUILayout.PropertyField(m_proximityDistance);
        EditorGUILayout.PropertyField(m_explosionForce);
        EditorGUILayout.PropertyField(m_explosionRadius);
        EditorGUILayout.PropertyField(m_bounceForce);
      }

      EditorGUILayout.EndFoldoutHeaderGroup();

    	this.serializedObject.ApplyModifiedProperties ();

	  }
	}
}
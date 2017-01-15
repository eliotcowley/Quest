using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(RhythmTool))]
public class RhythmToolEditor : Editor {

	public override void OnInspectorGUI()
	{
		RhythmTool myTarget = (RhythmTool)target;
		
		EditorGUILayout.LabelField("Total frames:", myTarget.totalFrames.ToString());
		EditorGUILayout.LabelField("Last Frame:", myTarget.lastFrame.ToString());
		EditorGUILayout.LabelField("Current Frame:", myTarget.currentFrame.ToString());
		EditorGUILayout.Separator();
	
		EditorGUILayout.LabelField("BPM:", myTarget.bpm.ToString());
		EditorGUILayout.LabelField("Beat Length:", myTarget.beatLength.ToString() + " frames");
		EditorGUILayout.Separator();

		EditorGUI.BeginDisabledGroup(Application.isPlaying);

		SerializedProperty calculateTempo = serializedObject.FindProperty("_calculateTempo");
		EditorGUILayout.PropertyField(calculateTempo);

		SerializedProperty preCalculate = serializedObject.FindProperty("_preCalculate");
		EditorGUILayout.PropertyField(preCalculate);

		if(preCalculate.boolValue){
			SerializedProperty storeAnalyses = serializedObject.FindProperty("_storeAnalyses");
			EditorGUILayout.PropertyField(storeAnalyses);
		}

		EditorGUI.EndDisabledGroup();

		if(!preCalculate.boolValue) {
			SerializedProperty lead = serializedObject.FindProperty("_lead");
			EditorGUILayout.IntSlider(lead,300,10000);
		}

		serializedObject.ApplyModifiedProperties();
		serializedObject.Update();
	}
}

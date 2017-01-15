
using UnityEngine;
using System.Collections;

[System.Serializable]
public class Beat{

	/// <summary>
	/// Length of this beat
	/// </summary>
	public float length;

	/// <summary>
	/// Most probable bpm during this beat
	/// </summary>
	public float bpm;

	/// <summary>
	/// Index of frame at which this beat occurs
	/// </summary>
	public int index;
	
	public Beat(float length, float bpm, int index)
	{
		this.length=length;
		this.bpm=bpm;
		this.index=index;
	}
}

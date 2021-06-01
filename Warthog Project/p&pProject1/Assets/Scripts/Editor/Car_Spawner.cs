using UnityEngine;
using UnityEditor;
using System.Collections;

class Car_Spawner : EditorWindow {
	private int axlesCount = 2;
	private float mass = 1000;
	private float axleStep = 2;
	private float axleWidth = 2;
	private float axleShift = -0.5f;

	[MenuItem ("Vehicles/New Vehicle")]
	public static void  ShowWindow () {
		EditorWindow.GetWindow(typeof(Car_Spawner));
	}

	void OnGUI () {
		axlesCount = EditorGUILayout.IntSlider ("Axles Count: ", axlesCount, 2, 10);
		mass = EditorGUILayout.FloatField ("Mass: ", mass);
		axleStep = EditorGUILayout.FloatField ("Axle step: ", axleStep);
		axleWidth = EditorGUILayout.FloatField ("Axle width: ", axleWidth);
		axleShift = EditorGUILayout.FloatField ("Axle shift: ", axleShift);

		if (GUILayout.Button("Build")) {
			CreateCar ();
		}
	}

	void CreateCar()
	{
		var root = new GameObject ("Car Body");
		var rootBody = root.AddComponent<Rigidbody> ();
		rootBody.mass = mass;

		var body = GameObject.CreatePrimitive (PrimitiveType.Cube);
		body.transform.parent = root.transform;

		float length = (axlesCount - 1) * axleStep;
		float firstOffset = length / 2;

		body.transform.localScale = new Vector3(axleWidth, 1, length);

		for (int i = 0; i < axlesCount; ++i) 
		{
			var leftWheel = new GameObject (string.Format("Wheel_{0}_Left", i));
			var rightWheel = new GameObject (string.Format("Wheel_{0}_Right", i));

			leftWheel.AddComponent<WheelCollider> ();
			rightWheel.AddComponent<WheelCollider> ();

			leftWheel.transform.parent = root.transform;
			rightWheel.transform.parent = root.transform;

			leftWheel.transform.localPosition = new Vector3 (-axleWidth / 2, axleShift, firstOffset - axleStep * i);
			rightWheel.transform.localPosition = new Vector3 (axleWidth / 2, axleShift, firstOffset - axleStep * i);
		}

		root.AddComponent<SuspensionHelper>();
		root.AddComponent<Driver>();
	}
}
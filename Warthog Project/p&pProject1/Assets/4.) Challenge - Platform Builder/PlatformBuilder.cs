using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBuilder : MonoBehaviour
{
    float hornLength = 12.4f;
    float rodLength = 90.0f;
    public bool showRodLines = false;
    public Vector3[] platformPointOrigins; // defined later
    public Vector3[] platformPoints; // defined later
    public Vector3[] basePoints; // defined later
    public float[] inputValues; //Sway Surge Heave Pitch Roll Yaw
    float platformDefaultHeight = 0;


    float actuatorLength; // used to represent the linear distance our servo/actuator can move (in mm)
    Vector3 servoAxis = new Vector3(0, 1, 0); // a unit vector that represents the direction the servo is facing
    LineRenderGroup platformGroup;
    LineRenderGroup baseGroup;
    public Material lineMaterial;

    void Start()
    {
        actuatorLength = hornLength*2; // This value represents how much each actuator can move

        // fill in all 6 values with your values from solidworks
        basePoints = new Vector3[6];
        basePoints[0] = new Vector3(32.16f, 41.66f, 0.00f);
        basePoints[1] = new Vector3(52.16f, 7.02f, 0.00f);
        basePoints[2] = new Vector3(20.00f, -48.68f, 0.00f);
        basePoints[3] = new Vector3(-20.00f, -48.68f, 0.00f);
        basePoints[4] = new Vector3(-52.16f, 7.02f, 0.00f);
        basePoints[5] = new Vector3(-32.16f, 41.66f, 0.00f);

        // fill in all 6 values with your values from solidworks
        platformPoints = new Vector3[6];
        platformPoints[0] = new Vector3(10.00f, 45.33f, 0.00f);
        platformPoints[1] = new Vector3(44.26f, -14.00f, 0.00f);
        platformPoints[2] = new Vector3(34.26f, -31.32f, 0.00f);
        platformPoints[3] = new Vector3(-34.26f, -31.32f, 0.00f);
        platformPoints[4] = new Vector3(-44.26f, -14.00f, 0.00f);
        platformPoints[5] = new Vector3(-10.00f, 45.33f, 0.00f);

        // LineRenderGroup is a (custom class) collection of line renderers, vector references, and parameters.
        baseGroup = new LineRenderGroup(ref basePoints, Color.green, lineMaterial, "BaseGroup", true, true, 1, this.transform);
        platformGroup = new LineRenderGroup(ref platformPoints, Color.cyan, lineMaterial, "PlatformGroup", true, true, 1, this.transform);

        platformPointOrigins = new Vector3[6]; // used to store default/identity values
        System.Array.Copy(platformPoints, platformPointOrigins, platformPoints.Length); // copy the values

        CalculateDefaultHeight();
    }

    void Update()
    {
        // Sway, surge, heave, pitch, roll, yaw
        for (int i = 0; i < platformPoints.Length; i++)
        {
            // Apply the input values to the top platform (rotate, translate, and height)

            // Rotate each point by the input values for pitch, roll, and yaw
            platformPoints[i] = RotatePoint(platformPointOrigins[i], inputValues[3] * Mathf.Deg2Rad, inputValues[5] * Mathf.Deg2Rad, inputValues[4] * Mathf.Deg2Rad); // delete

            // Translate each point by the input values for sway, surge, and heave
            platformPoints[i] += new Vector3(inputValues[0], inputValues[1], inputValues[2]); // delete

            // Add in the default height
            platformPoints[i].z += platformDefaultHeight; // delete

            if (showRodLines)
            {
                // To determine if the newly positioned platform is in a valid position,
                // we compare the virtual vector length to the actual rod/horn length.

                // Measure the distance (arm/rod length)
                float measuredLegnth = (platformPoints[i] - basePoints[i]).magnitude; // delete

                // Check the distance of the requested position compared to the
                // length parameters of our platform (actuator rod or servo/horn)
                if (Mathf.Abs(measuredLegnth - rodLength) < actuatorLength) // delete
                {
                    // Valid length
                    Debug.DrawLine(platformPoints[i], basePoints[i], Color.yellow);
                }
                else
                {
                    // Invalid length
                    Debug.DrawLine(platformPoints[i], basePoints[i], Color.red);
                }
            }
        }

        baseGroup.UpdateLineRenderers();
        platformGroup.UpdateLineRenderers();
    }

    void CalculateDefaultHeight()
    {
        Vector3 hornDirection = Vector3.Cross(servoAxis, new Vector3(0, 0, 1)); // delete
        Vector3 hornEndPoint = basePoints[0] + (hornDirection * hornLength);

        // Calculate default height (Pythagorean theorem)
        // c^2 = a^2 + b^2

        float sideC = rodLength;
        float sideB = (platformPointOrigins[0] - hornEndPoint).magnitude;
        float sideA = Mathf.Sqrt(sideC * sideC - sideB * sideB); // delete

        platformDefaultHeight = sideA;

        // Update the z value (up) with our newly calculated height
        for (int i = 0; i < platformPointOrigins.Length; i++)
        {
            platformPointOrigins[i].z = platformDefaultHeight;
        }
    }


    // Angle and Rotation functions
    Vector3 RotatePoint(Vector3 point, float angRadX, float angRadY, float angRadZ)
    {
        float x = point.x * Mathf.Cos(angRadZ) * Mathf.Cos(angRadY) + point.y * (Mathf.Sin(angRadX) * Mathf.Sin(angRadZ) * Mathf.Cos(angRadZ) - Mathf.Cos(angRadX) * Mathf.Sin(angRadY));
        float y = point.x * Mathf.Cos(angRadZ) * Mathf.Sin(angRadY) + point.y * (Mathf.Cos(angRadX) * Mathf.Cos(angRadY) + Mathf.Sin(angRadX) * Mathf.Sin(angRadZ) * Mathf.Sin(angRadY));
        float z = -point.x * Mathf.Sin(angRadZ) + point.y * Mathf.Sin(angRadX) * Mathf.Cos(angRadZ);
        return new Vector3(x, y, z);
    }

    float GetServoAngle(int i, Vector3 rodVector)
    {
        // Project the end point of the new virtual arm onto the servo's plane of rotation
        float[] angles = new float[] { 300, 120, 180, 0, 60, 240 };

        float L = Mathf.Sqrt(rodVector.magnitude) - Mathf.Sqrt(rodLength) + Mathf.Sqrt(hornLength);
        float servoDiameter = 2 * hornLength;
        float M = servoDiameter * rodVector.z;
        float N = servoDiameter * (Mathf.Cos(Mathf.Deg2Rad * angles[i]) * rodVector.x + Mathf.Sin(Mathf.Deg2Rad * angles[i]) * rodVector.y);

        // Derive the servo angle (in radians)
        return Mathf.Asin(L / Mathf.Sqrt(M * M + N * N)) - Mathf.Atan(N / M);
    }
}
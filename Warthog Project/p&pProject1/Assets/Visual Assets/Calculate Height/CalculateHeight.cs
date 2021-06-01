using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateHeight : MonoBehaviour
{
    public Vector3 basePoint = Vector3.zero;
    public Vector3 platformPoint = Vector3.zero;
    public float hornLength = 0;
    public float rodLength = 0;

    [SerializeField] Color rodColor = Color.yellow;
    [SerializeField] Color hornColor = Color.cyan;
    [SerializeField] Color triangleAdjacentColor = Color.green;
    [SerializeField] Color servoAxisColor = Color.white;
    [SerializeField] Color heightColor = Color.magenta;

    Vector3 servoForwardAxis = Vector3.forward;
    Vector3 hornEndPoint; // we need to caluculate this (using horn length and a cross product)
    float sideC; // sideC is rod length
    float sideB; // sideB is derived from measurements (horn end point and corresponding platform attach point)
    public float platformHeight; // this is the value we want to calculate

    void Update()
    {
        platformHeight = CalculateHeightValue();

        Debug.DrawLine(basePoint, basePoint + servoForwardAxis * 30, servoAxisColor); // draw servo axis (normal)
        Debug.DrawLine(basePoint, hornEndPoint, hornColor); // draw horn line
        Debug.DrawLine(platformPoint + Vector3.up * platformHeight, hornEndPoint, rodColor); // rod (hypotenuse)
        Debug.DrawLine(hornEndPoint, platformPoint, triangleAdjacentColor); // draw adjacent (bottom) side of our "height" triangle
        Debug.DrawLine(platformPoint, platformPoint + Vector3.up * platformHeight, heightColor); // draw height value (the value we are looking for)
    }

    float CalculateHeightValue()
    {
        // The CrossProduct is used to determine the direction vector that defines the servo horn when it's flat (90 degrees).
        // For example: hornDirection = Cross(Vector3 axis1, Vector3 axis2);
        Vector3 hornDirection = Vector3.Cross(servoForwardAxis, Vector3.up);
        hornEndPoint = basePoint + (hornDirection * hornLength);

        // Calculate default height (Pythagorean theorem) c^2 = a^2 + b^2, c^2 -b^2 = a^2, a = sqrt(c*c - b*b)
        sideC = rodLength;
        sideB = (platformPoint - hornEndPoint).sqrMagnitude;
        float sideA = Mathf.Sqrt(sideC * sideC - sideB);  // sideB is already the squared length from the previous caluculation
        return sideA;
    }
}

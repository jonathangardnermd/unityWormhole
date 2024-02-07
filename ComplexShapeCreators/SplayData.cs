using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;

// we assume the curve (aka "splay") from the cylinder's vertical slope to the plane's horizontal slope (a 90-degree difference)
// is the simplest possible curve with a CONSTANT rate of change in the derivative (i.e. a parabola)
// However, since the starting slope along the cylinder is vertical, it's actually infinity/undefined. 
// Therefore, we choose to solve for the curve in a coordinate system where 
// we can model the curve with a symmetric segment of a parabola: the u,v coordinate system that 
// is rotated 45 degrees from the original x,y coordinate system.
// In this coordinate system, the starting slope is -1 and the ending slope is +1 (still a 90 degree change),
// so the math is easier. Once we solve for the curve in the u,v coordinate system, we rotate back to x,y coords.
public class SplayData
{
    // we solve for "numDivisions" equally-spaced points along the parabolic curve and draw straight lines to join them up
    // therefore, the higher numDivisions, the curvier our curve becomes
    public int numDivisions;

    // the totChangeInU is the total distance traveled in the "u" direction, 
    // which is the x direction rotated by 45 degrees.
    public float totChangeInU;

    public const float sqrt2 = 1.4142135f; // Approximation of square root of 2

    // uvChanges are points along the curve in the 45-degree-rotated u,v coordinate system 
    // (where the math is simpler bc the curve starts with slope +1 and ends with slope -1)
    public List<Vector2> uvChanges;

    // xyChanges are points along the curve in the original x,y coordinate system 
    // (where the curve starts with a vertical slope and ends with a horizontal slope)
    public List<Vector2> xyChanges;

    public SplayData(int numDivisions, float totChangeInU)
    {
        uvChanges = new();
        xyChanges = new();
        this.numDivisions = numDivisions;
        this.totChangeInU = totChangeInU;

        // it's easier to calc the curve in the u,v coordinate system, which is rotated 45 degrees to the x,y coordinate system
        // in this coordinate system, we can model the curve as a symmetric segment of a parabola from y'=+1 to y'=-1 where y' is the first deriv
        CalculateUVChanges();

        // we then rotate the u,v coordinates we just calculated back to x,y coords via another 45 degree rotation (in the opposite direction, of course)
        CalculateXYChanges();
    }

    private void CalculateUVChanges()
    {
        // in this 45 degree-rotated coord system, the curved path from a vertical slope to a horizontal slope (90 degrees)
        // is just a movement from slope = +1 to slope = -1 (90 degrees)
        float startDeriv = 1f;
        float endDeriv = -1f;

        // we assume rateOfChangeInVDeriv=constant (i.e. v ~ u^2, a parabola with a constant second derivative); 
        // we can calc the value of it bc we're moving from deriv=+1 to deriv=-1 over a total u distance of totChangeInU
        float rateOfChangeInVDeriv = (endDeriv - startDeriv) / totChangeInU;

        // if we divide the total u distance into "numDivisions" equal pieces, each piece has length deltaU
        float deltaU = totChangeInU / numDivisions;

        for (int i = 0; i <= numDivisions; i++)
        {
            // uChange = u distance from the BEGINNING to the end of this piece
            float uChange = i * deltaU;

            // the change in the derivative from the BEGINNING to the end of this piece is 
            // rateOfChangeInVDeriv (the second deriv, v'') times the total u distance traveled, which is uChange
            float changeInDeriv = uChange * rateOfChangeInVDeriv;

            // the deriv at the end of this piece is startDeriv + changeInDeriv
            float vDeriv = startDeriv + changeInDeriv;

            // bc the rateOfChangeInVDeriv is constant, the avgDeriv over a segment is just the average of the start and end derivs
            float avgDeriv = (startDeriv + vDeriv) / 2;

            // the change in v over the segment is avgDeriv * uChange (bc avgDeriv = vChange/uChange = rise/run)
            float vChange = avgDeriv * uChange;

            // uChanges holds points along the curve assuming the curve started at 0,0
            uvChanges.Add(new Vector2(uChange, vChange));
        }
    }

    private void CalculateXYChanges()
    {
        // the conversion from uv-coords to xy-coords is a 45-degree rotation
        xyChanges = uvChanges.Select(uvChange =>
            new Vector2(
                CalculateDeltaX(uvChange.x, uvChange.y),
                CalculateDeltaY(uvChange.x, uvChange.y)
            )
        ).ToList();
    }

    private float CalculateDeltaX(float deltaU, float deltaV)
    {
        return (deltaU - deltaV) / sqrt2;
    }

    private float CalculateDeltaY(float deltaU, float deltaV)
    {
        return (deltaU + deltaV) / sqrt2;
    }

    public float GetTotalChangeInY()
    {
        // as a consequence a choosing a symmetric (parabolic) segment in 45-degree-rotated uv-space with totChangeInV=0,
        // the total change in x and y form the smaller sides of a right triangle with hypotenuse equal to totChangeInU
        // therefore, totChangeInY = totChangeInX = totChangeInU / sqrt(2)
        return totChangeInU / sqrt2;
    }

    public void SaveToCSV(string filePath)
    {
        // save the uv and xy points we've found for the curve to a file
        System.Text.StringBuilder csvContent = new System.Text.StringBuilder();
        csvContent.AppendLine("uChange,vChange,xChange,yChange");
        for (int i = 0; i < uvChanges.Count; i++)
        {
            var line = string.Format("{0},{1},{2},{3}",
                uvChanges[i].x,
                uvChanges[i].y,
                xyChanges[i].x,
                xyChanges[i].y);
            csvContent.AppendLine(line);
        }
        File.WriteAllText(filePath, csvContent.ToString());
    }
}

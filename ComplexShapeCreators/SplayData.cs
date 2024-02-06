using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;

public class SplayData
{
    public int numDivisions;
    public float totChangeInU;
    public float startDeriv;
    public float endDeriv;
    public const float sqrt2 = 1.4142135f; // Approximation of square root of 2

    public List<Vector2> uvChanges;
    public List<Vector2> xyChanges;

    public SplayData(int numDivisions, float totChangeInU)
    {
        uvChanges = new();
        xyChanges = new();

        this.numDivisions = numDivisions;
        this.totChangeInU = totChangeInU;

        startDeriv = 1f;
        endDeriv = -1f;

        CalculateUVChanges();
        CalculateXYChanges();
    }

    private void CalculateUVChanges()
    {
        float rateOfChangeInVDeriv = (endDeriv - startDeriv) / totChangeInU;
        float deltaU = totChangeInU / numDivisions;

        for (int i = 0; i <= numDivisions; i++)
        {
            float uChange = i * deltaU;
            float vDeriv = startDeriv + (uChange * rateOfChangeInVDeriv);
            float avgDeriv = (startDeriv + vDeriv) / 2;
            float vChange = avgDeriv * uChange;
            uvChanges.Add(new Vector2(uChange, vChange));
        }
    }
    private void CalculateXYChanges()
    {
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
        return totChangeInU / sqrt2;
    }

    public void SaveToCSV(string filePath)
    {
        // Create a StringBuilder to build the CSV content
        System.Text.StringBuilder csvContent = new System.Text.StringBuilder();

        // Add header row
        csvContent.AppendLine("uChange,vChange,xChange,yChange");

        // Add data rows
        for (int i = 0; i < uvChanges.Count; i++)
        {
            var line = string.Format("{0},{1},{2},{3}",
                uvChanges[i].x,
                uvChanges[i].y,
                xyChanges[i].x,
                xyChanges[i].y);
            csvContent.AppendLine(line);
        }

        // Write the CSV content to the file
        File.WriteAllText(filePath, csvContent.ToString());
    }
}

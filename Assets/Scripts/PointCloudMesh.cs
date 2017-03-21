using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PointCloudMesh : MonoBehaviour
{
    public List<Vector4> originalContainedParticles = null;
    public List<Vector4> containedParticles = null;

    private Color[] colorArray =
    {
        Color.red, Color.blue, Color.black, Color.cyan, Color.green, Color.magenta,
        Color.yellow, Color.white, Color.gray, Color.clear
    };
    public void UpdateContainedPointDisplay()
    {
        List<Vector3> positions = new List<Vector3>();
        List<Color> colors = new List<Color>();
        float max = 0f;
        foreach (
            Vector4 particle in containedParticles) {
            if (particle.w > max)
                max = particle.w;
        }
        foreach (
            Vector4 particle in containedParticles)
        {
            positions.Add(new Vector3(particle.x, particle.y, particle.z));
            try {
                
                colors.Add(IntToColor((int)particle.w,(int)max));
            }
            catch(Exception e)
            {
                colors.Add(Color.blue);
            }
        }
        SetPoints(positions.ToArray(), colors.ToArray());
    }
    public static Color IntToColor(int i, int max)
    {
        Color[] colors = new Color[] { Color.black, Color.blue, Color.cyan, Color.green, Color.yellow, Color.magenta, Color.red, Color.white, Color.grey };
        float scaled = (float)(i) / max * 7;
        Color color0 = colors[(int)scaled];
        Color color1 = colors[(int)scaled + 1];
        float fraction = scaled - (int)scaled;
        Color result = new Color();
        result.r = (byte)((1 - fraction) * (float)color0.r + fraction * (float)color1.r);
        result.g = (byte)((1 - fraction) * (float)color0.g + fraction * (float)color1.g);
        result.b = (byte)((1 - fraction) * (float)color0.b + fraction * (float)color1.b);
        result.a = 1;
        return result;
    }
    public void SetPoints(Vector3[] positions, Color[] colors)
    {
        GetComponent<PointCloudManager>().LoadDynamic(positions, colors);
    }

    public void UpdateContainedPointDisplay(Vector3 vector3)
    {
        GetComponent<PointCloudManager>().UpdatePointCloudPosition(vector3);
    }
}
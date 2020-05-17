using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCountries
{
    public string Name;
    public Color32 Color;
    public int NumberOfAffected;
    public Material mat;

    public NetworkCountries(string name, Color32 color, int count)
    {
        Name = name;
        Color = color;
        NumberOfAffected = count;
        mat = new Material(Shader.Find("Legacy Shaders/Particles/Additive"));
        mat.color = Color;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CovidNode 
{
    public string name;
    public float branch_length;
    public List<CovidNode> children;
}

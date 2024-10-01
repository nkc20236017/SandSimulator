using MackySoft.Navigathena.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public readonly struct ProgressData
{
    public float Progress { get; }
    public string ProgressText { get; }
    public string ProgressMesage { get; }

    public ProgressData(float progress, string progressText,string progressMesage)
    {
        this.Progress = progress;
        this.ProgressText = progressText;
        this.ProgressMesage = progressMesage;
    }

}

public class SceneData : ISceneData
{
    public string Name { get; }

    public SceneData(string name)
    {
        this.Name = name;
    }

}

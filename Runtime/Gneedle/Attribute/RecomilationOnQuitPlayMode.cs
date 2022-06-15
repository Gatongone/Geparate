using System;
using UnityEditor.Compilation;
using UnityEngine;

public class RecomilationOnQuitPlayMode : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
        gameObject.hideFlags = HideFlags.HideAndDontSave;
    }

    private void OnApplicationQuit()
    {
        CompilationPipeline.RequestScriptCompilation();
    }
}

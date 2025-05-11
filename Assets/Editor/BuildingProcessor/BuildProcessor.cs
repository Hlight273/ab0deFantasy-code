using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Reporting;
using UnityEditor.Build;
using UnityEngine;
using HFantasy.Editor.ShaderManagement;

namespace HFantasy.Editor.BuildingProcessor
{
    class BuildProcessor : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            Debug.Log("����ǰ��鲢���Shader...");
            ShaderIncludeProcessor.AddRequiredShaders();
        }
    }
}

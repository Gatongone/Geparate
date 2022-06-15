#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Geparate.Gneedle.Editor
{

    public class AssembliesInclude : EditorWindow
    {
        [MenuItem("Geparate/Gneedle/Include Assemblies")]
        public static void ShowExample()
        {
            AssembliesInclude wnd = GetWindow<AssembliesInclude>();
            wnd.titleContent = new GUIContent("AssembliesInclude");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.gatongone.geparate/Editor/Gneedle/AssembliesInclude.uxml");
            VisualElement labelFromUXML = visualTree.CloneTree();
            root.Add(labelFromUXML);


        }
    }
}
#endif
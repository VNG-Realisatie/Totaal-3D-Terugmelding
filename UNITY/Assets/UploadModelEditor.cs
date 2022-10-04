#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using WebGLFileUploaderExample;

[CustomEditor( typeof(UploadModel)  ), CanEditMultipleObjects()]
public class UploadModelEditor : Editor
{

    public override void OnInspectorGUI()
    {        
        base.OnInspectorGUI();

        UploadModel up = (UploadModel)target;


        if (GUILayout.Button("Converteer lokaal IFC naar CityJson"))
        {
            up.TestConvert();
        }

        


    }

}

#endif

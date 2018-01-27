using UnityEngine;
using UnityEditor;
using System.Collections;

[@CustomEditor(typeof(CameraController))]
public class CameraControllerEditor : Editor
{

    void OnSceneGUI()
    {
        CameraController controller = (CameraController)target;

        if (controller != null && controller.cameraBoundsRestriction)
        {

            Vector3 min = controller.minimumBounds = Handles.PositionHandle(controller.minimumBounds, Quaternion.identity);
            Vector3 max = controller.maximumBounds = Handles.PositionHandle(controller.maximumBounds, Quaternion.identity);

            Handles.color = Color.red;

            // draw the camera bounds (in red)

            Handles.DrawLine(new Vector3(min.x, min.y, min.z), new Vector3(max.x, min.y, min.z));
            Handles.DrawLine(new Vector3(min.x, min.y, min.z), new Vector3(min.x, min.y, max.z));
            Handles.DrawLine(new Vector3(max.x, min.y, min.z), new Vector3(max.x, min.y, max.z));
            Handles.DrawLine(new Vector3(min.x, min.y, max.z), new Vector3(max.x, min.y, max.z));

            Handles.DrawLine(new Vector3(min.x, min.y, min.z), new Vector3(min.x, max.y, min.z));
            Handles.DrawLine(new Vector3(max.x, min.y, max.z), new Vector3(max.x, max.y, max.z));
            Handles.DrawLine(new Vector3(max.x, min.y, min.z), new Vector3(max.x, max.y, min.z));
            Handles.DrawLine(new Vector3(min.x, min.y, max.z), new Vector3(min.x, max.y, max.z));

            Handles.DrawLine(new Vector3(min.x, max.y, min.z), new Vector3(max.x, max.y, min.z));
            Handles.DrawLine(new Vector3(min.x, max.y, min.z), new Vector3(min.x, max.y, max.z));
            Handles.DrawLine(new Vector3(max.x, max.y, min.z), new Vector3(max.x, max.y, max.z));
            Handles.DrawLine(new Vector3(min.x, max.y, max.z), new Vector3(max.x, max.y, max.z));

            // draw the camera zoom range (in blue)

            Handles.color = Color.blue;

            Vector3 zoomedIn = controller.transform.position;
            zoomedIn.y = controller.zoomedInHeight;
            controller.zoomedInHeight = Handles.ScaleValueHandle(controller.zoomedInHeight, zoomedIn, Quaternion.identity, HandleUtility.GetHandleSize(zoomedIn), Handles.SphereCap, 0);

            Vector3 zoomedOut = controller.transform.position;
            zoomedOut.y = controller.zoomedOutHeight;
            controller.zoomedOutHeight = Handles.ScaleValueHandle(controller.zoomedOutHeight, zoomedOut, Quaternion.identity, HandleUtility.GetHandleSize(zoomedOut), Handles.SphereCap, 0);

            Handles.DrawLine(zoomedIn, zoomedOut);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }

    }

}

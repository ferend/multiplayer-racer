using System;
using System.Collections.Generic;
using _Project.Scripts.Car;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Toolkit
{
    public class CarIndicator : MonoBehaviour
    {
        public static List<ArcadeVehicleController> allCarControllers = new();

        private void OnDrawGizmos()
        {
            
            foreach (ArcadeVehicleController segments in allCarControllers)
            {
                Vector3 controller = transform.position;
                Vector3 position = segments.transform.position;
                float halfHeight = (controller.y - position.y) * .5F;
                // Offset position of the 4 points that we have. Vertically halfway between them.
                Vector3 tangentOffset = Vector3.up * halfHeight;

#if UNITY_EDITOR

                Handles.DrawBezier(
                    controller,
                    position,
                    controller - tangentOffset,
                    position + tangentOffset,
                    Color.white,
                    EditorGUIUtility.whiteTexture,
                    1F
                );

#endif

            }
        }
    }
}
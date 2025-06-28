using UnityEditor;
using UnityEngine;

namespace SLC.RetroHorror.Core
{
    public class TestingIndicators : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Handles.color = Color.red;
            Handles.DrawLine(Vector3.zero, Vector3.left * 5f, 3f);
        }
    }
}

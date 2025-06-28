using UnityEngine;

namespace SLC.RetroHorror.Core
{
    [RequireComponent(typeof(Canvas))]
    public class WorldCanvasFaceCamera : MonoBehaviour
    {
        private RectTransform worldCanvas;

        //Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            worldCanvas = GetComponent<RectTransform>();
        }

        //Update is called once per frame
        void Update()
        {
            worldCanvas.forward = Camera.main.transform.forward;
        }
    }
}

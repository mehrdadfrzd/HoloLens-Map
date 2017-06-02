/*     INFINITY CODE 2013-2017      */
/*   http://www.infinity-code.com   */

using UnityEngine;
using UnityEngine.UI;

namespace InfinityCode.OnlineMapsExamples
{

  
    /// <summary>
    /// Example of how to override the input, and use the center of screen as the cursor, and Z key as a left mouse button.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/InterceptInputExample")]
    public class InterceptInputExample : MonoBehaviour
    {

        public Text textHolder;

        private Vector2 OnGetInputPosition()
        {

#if !UNITY_EDITOR
            // On the device returns center of screen.
            return Camera.main.ViewportToScreenPoint(new Vector3(0.5F, 0.5F, 0));
#else
            // In the editor returns the coordinates of the mouse cursor.
            textHolder.text = string.Format("Position:{0:0.00},{1:0.00},{2:0.00}\n Velocity: -,-,-", Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
            return Input.mousePosition;
#endif
        }

        private int OnGetTouchCount()
        {
            // If pressed Z, then it will work like the left mouse button.
            return Input.GetKey(KeyCode.Z) ? 1 : 0;
        }

        private void Start()
        {
            textHolder = GameObject.Find("HoloLensCamera/Canvas/Text").GetComponent<Text>();

            // Intercepts getting the cursor coordinates.
            OnlineMapsControlBase.instance.OnGetInputPosition += OnGetInputPosition;

            // Intercepts getting the number of touches.
            //OnlineMapsControlBase.instance.OnGetTouchCount += OnGetTouchCount;
        }
    }
}
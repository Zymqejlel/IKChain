using UnityEngine;

public class MouseDragTarget : MonoBehaviour
{
    public Camera mainCam;
    private bool isDragging = false;

    void Update()
    {
        // 鼠标按下，拾取小球
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    isDragging = true;
                }
            }
        }
        // 松开鼠标停止拖拽
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // 拖拽的时候更新小球位置
        if (isDragging)
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, transform.position);
            if (plane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                transform.position = hitPoint;
            }
        }
    }
}
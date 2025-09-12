using UnityEngine;

public class AutoAim : MonoBehaviour
{
    public float maxDistance = 100f;    // how far the ray goes
    private Outline currentOutline;     // currently highlighted target

    void Update()
    {
        // Create a ray from screen center (crosshair)
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            Outline outline = hit.collider.GetComponent<Outline>();

            if (outline != null)
            {
                // If we're aiming at a new target
                if (currentOutline != outline)
                {
                    ClearTarget();
                    SetTarget(outline);
                }
            }
            else
            {
                ClearTarget();
            }
        }
        else
        {
            ClearTarget();
        }
    }

    void SetTarget(Outline outline)
    {
        currentOutline = outline;
        currentOutline.enabled = true; // turn on glow
    }

    void ClearTarget()
    {
        if (currentOutline != null)
        {
            currentOutline.enabled = false; // turn off glow
            currentOutline = null;
        }
    }
}

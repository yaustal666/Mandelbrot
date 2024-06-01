using UnityEngine;
using UnityEngine.UI;

public class MandelbrotExplorer : MonoBehaviour
{
    [SerializeField] private Material mat;
    [SerializeField] private Vector2 pos;

    [SerializeField] private int maxIter;

    [SerializeField] private float scale;
    [SerializeField] private float angle;

    [SerializeField] private float repeat;
    [SerializeField] private float color;
    [SerializeField] private float speed;

    [SerializeField] private RawImage img;

    [SerializeField] private Vector2 smoothPos;
    [SerializeField] private float smoothScale, smoothAngle;

    private bool isSmooth = false;
    private bool isAnimationOn = false;

    [SerializeField] private float scaleInSpeed = 0.99f;
    [SerializeField] private float scaleOutSpeed = 1.01f;
    [SerializeField] private float rotationSpeed = 0.01f;
    [SerializeField] private float movingSpeed = 0.01f;

    private void Start()
    {
        UpdateShader();
    }

    private void FixedUpdate()
    {
        HandleInputs();
        UpdateShader();
    }

    private void UpdateShader()
    {
        float aspect = (float)img.rectTransform.rect.width / (float)img.rectTransform.rect.height;

        if(isSmooth)
        {
            smoothPos = Vector2.Lerp(smoothPos, pos, 0.03f);
            smoothScale = Mathf.Lerp(smoothScale, scale, 0.03f);
            smoothAngle = Mathf.Lerp(smoothAngle, angle, 0.03f);
        } else if (isAnimationOn)
        {
            smoothPos = new Vector2(pos.x, pos.y);
            smoothScale = 5.0f;
            smoothAngle = 0.0f;
        } else
        {
            smoothPos = new Vector2(pos.x, pos.y);
            smoothScale = scale;
            smoothAngle = angle;
        }

        float scaleX = isSmooth ? smoothScale : scale;
        float scaleY = isSmooth ? smoothScale : scale;

        if (aspect > 1f) scaleY /= aspect;
        else scaleX *= aspect;

        mat.SetFloat("_MaxIter", maxIter);

        if (isSmooth)
        {
            mat.SetVector("_Area", new Vector4(smoothPos.x, smoothPos.y, scaleX, scaleY));
            mat.SetFloat("_Angle", smoothAngle);
        } else
        {
            mat.SetVector("_Area", new Vector4(pos.x, pos.y, scaleX, scaleY));
            mat.SetFloat("_Angle", angle);
        }

        mat.SetFloat("_Color", color);
        mat.SetFloat("_Repeat", repeat);
        mat.SetFloat("_Speed", speed);
    }

    private void HandleInputs()
    {
        // zooming
        if(Input.GetKey(KeyCode.Z))
        {
            scale *= scaleInSpeed;
        }

        if (Input.GetKey(KeyCode.X))
        {
            scale *= scaleOutSpeed;
        }

        //rotating
        if (Input.GetKey(KeyCode.E))
        {
            angle += rotationSpeed;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            angle -= rotationSpeed;
        }

        // moving
        Vector2 dir = new Vector2(scale * movingSpeed, 0); // making moving speed aware of scale
        float s = Mathf.Sin(angle);
        float c = Mathf.Cos(angle);
        dir = new Vector2(dir.x * c, dir.x * s); // rotating our direction

        if (Input.GetKey(KeyCode.A))
        {
            pos -= dir;
        }

        if (Input.GetKey(KeyCode.D))
        {
            pos += dir;
        }

        dir = new Vector2(-dir.y, dir.x); // rotate dir 90 degrees left for up-down direction
        if (Input.GetKey(KeyCode.W))
        {
            pos += dir;
        }

        if (Input.GetKey(KeyCode.S))
        {
            pos -= dir;
        }

    }

    public void setMaxIterations(float val)
    {
        maxIter = (int)val;
    }
    public void setRepeat(float val)
    {
        repeat = val;
    }
    public void setColor(float val)
    {
        color = val;
    }
    public void setSpeed(float val)
    {
        speed = val / 100.0f;
    }
    public void toggleSmoothMoving(bool isOn)
    {
        isSmooth = isOn;
    }
    public void toggleAnimation(bool isOn)
    {
        isAnimationOn = isOn;
    }
}


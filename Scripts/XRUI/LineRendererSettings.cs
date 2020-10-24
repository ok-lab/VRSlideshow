using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineRendererSettings : MonoBehaviour
{
    public Launcher Launcher;

    public GameObject panel;

    private Button btn;

    public GameObject LeftHand;

    public Text DisplayName;

    public string Select { get; private set; }


    readonly private string Sub = "You select : ";

    public int distance = 50; // rayの飛ばせる距離

    //Declare a LineRenderer to store the component attached to the GameObject. 
    [SerializeField] LineRenderer rend;

    //Settings for the LineRenderer are stored as a Vector3 array of points. Set up a V3 array to //initialize in Start. 
    Vector3[] points;

    private void Awake()
    {
        this.Select = "UnityChan"; // Initialize select

        Launcher = GameObject.Find("GameManager").GetComponent<Launcher>();
    }

    //Start is called before the first frame update
    void Start()
    {
        //get the LineRenderer attached to the gameobject. 
        rend = gameObject.GetComponent<LineRenderer>();
    
        //initialize the LineRenderer
        points = new Vector3[2];

        //set the start point of the linerenderer to the position of the gameObject. 
        //points[0] = Vector3.zero;
        points[0] = LeftHand.transform.position;

        //set the end point 20 units away from the GO on the Z axis (pointing forward)
        points[1] = LeftHand.transform.forward * 10;

        //finally set the positions array on the LineRenderer to our new values
        rend.SetPositions(points);

        rend.enabled = false;
        
    }

    public LayerMask layerMask;

    public bool AlignLineRenderer(LineRenderer rend)
    {
        Ray ray = new Ray(LeftHand.transform.position, LeftHand.transform.forward);
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction*distance);
        bool hitBtn = false;

        points[0] = LeftHand.transform.position;

        // もしRayにオブジェクトが衝突したら　
        if (Physics.Raycast(ray, out hit, layerMask))
        {
            rend.enabled = true;
            //Debug.Log(hit.collider.gameObject.tag);//手もマスク
            if (hit.collider.gameObject.tag == "Button")
            {
                points[1] = hit.point;
                rend.startColor = Color.red;
                rend.endColor = Color.red;
                btn = hit.collider.gameObject.GetComponent<Button>();
                hitBtn = true;
            }
            else if (hit.collider.gameObject.tag == "BaseUI")
            {
                points[1] = hit.point;
                rend.startColor = Color.green;
                rend.endColor = Color.green;
                hitBtn = false;
            }
            else
            {
                points[1] = hit.point;
                rend.startColor = Color.blue;
                rend.endColor = Color.blue;
                hitBtn = false;
            }
        }
        else
        {
            rend.enabled = false;
            points[1] = LeftHand.transform.forward * 10;
            hitBtn = false;
        }

        rend.SetPositions(points);
        rend.material.color = rend.startColor;
        return hitBtn;
    }

    public void CharacterSelectOnClick()
    {
        if (btn != null)
        {
            if (btn.name == "unitychan_btn")
            {
                Select = "unity_chan";
            }
            else if (btn.name == "robot_btn")
            {
                Select = "robot";
                Debug.Log("robot");
            }
            else if (btn.name == "paladin_btn")
            {
                Select = "Paladin";
                Debug.Log("Paladin");
            }
            else if (btn.name == "rin_btn")
            {
                Select = "Rin";
                Debug.Log("Rin");
            }
            else if (btn.name == "misaki_btn")
            {
                Select = "Misaki";
                Debug.Log("Misaki");
            }
            else if (btn.name == "FA_unitychan_btn")
            {
                Select = "FA_unitychan_btn";
                Debug.Log("FA_unitychan_btn");
            }
            else if (btn.name == "play_btn")
            {
                ConnectRoom();
            }
            else if (btn.name == "exit_btn")
            {
                QuitBunOnClick();
            }
            DisplayName.text = Sub + Select;
        }
    }

    public void QuitBunOnClick()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_STANDALONE
        UnityEngine.Application.Quit();
        #endif
    }

    public void ConnectRoom()
    {
        Debug.Log("Connet Room");
        Launcher.Connect();
    }

    void Update()
    {
        if (AlignLineRenderer(rend) && Input.GetButtonDown("Oculus_CrossPlatform_Button3"))
        {
            btn.onClick.Invoke();
        }
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Scale : MonoBehaviour
{
    public AudioClip selectsound, scalesound;
    private AudioSource audiosource;
    private Vector3 initialpos;
    private List<Vector3> initialscale;
    public GameObject parentplane;
    Vector3 box;

    private void Start()
    {
        audiosource = GetComponent<AudioSource>();
        initialpos = Vector3.zero;
        initialscale = new List<Vector3>(); // list of children for corners

        if (transform.parent.gameObject.tag == "corner")  // 
        {
            foreach (Transform child in transform.parent)
            {
                initialscale.Add(child.localScale);
            }
        }
        else
        {
            initialscale.Add(transform.localScale);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        audiosource.volume = 0.5f;
        audiosource.clip = selectsound;
        //audiosource.Play();


        if (other.gameObject.tag == "controller" && !other.gameObject.GetComponent<Controller>().holding)
            if (transform.parent.gameObject.tag == "corner")  // highlight both corners
            {
                box = GetComponent<BoxCollider>().size;
                box.y *= 40.0f;
                GetComponent<BoxCollider>().size = box;

                foreach (Transform child in transform.parent)
                {
                    initialscale.Add(child.localScale);
                    child.gameObject.GetComponent<Renderer>().material.color = new Vector4(.117f, 1, .259f, 1);
                    child.localScale *= 1.1f;
                }
            }
            else
            {
                initialscale.Add(transform.localScale);
                this.gameObject.GetComponent<Renderer>().material.color = new Vector4(.117f, 1, .259f, 1);
                transform.localScale *= 1.1f;
            }

    }

    void OnTriggerExit(Collider other)
    {
        initialpos = Vector3.zero;

        if (transform.parent.gameObject.tag == "corner")  // highlight both corners
        {
            box = GetComponent<BoxCollider>().size;
            box.y /= 40.0f;
            GetComponent<BoxCollider>().size = box;
        }
            if (other.gameObject.tag == "controller")
            other.gameObject.GetComponent<Controller>().scaling = false;


        if (audiosource.clip == scalesound && audiosource.isPlaying)
            audiosource.Stop();

        int i = 0;
        if (transform.parent.gameObject.tag == "corner")  // unhighlight both corners
        {

            foreach (Transform child in transform.parent)
            {
                child.gameObject.GetComponent<Renderer>().material.color = new Vector4(0, 1, 1, 1);
                child.localScale = initialscale[i];
                i++;
            }
        }
        else
        {
            this.gameObject.GetComponent<Renderer>().material.color = new Vector4(0, 1, 1, 1);
            transform.localScale = initialscale[i];


        }


    }

    void OnTriggerStay(Collider other)
    {
        Vector3 newscale = parentplane.transform.localScale;

        foreach (Transform child in parentplane.transform)
            child.gameObject.SetActive(true);

        if (other.gameObject.tag == "controller" && !other.gameObject.GetComponent<Controller>().holding)
        {
            if (initialpos == Vector3.zero && other.gameObject.GetComponent<Controller>().ButtonHeld())
            {
                // get controller initial position
                initialpos = other.gameObject.GetComponent<Controller>().GetPosition();

            }

            if (!other.gameObject.GetComponent<Controller>().ButtonHeld())
            {
                // get controller initial position
                initialpos = Vector3.zero;
                other.gameObject.GetComponent<Controller>().scaling = false;
            }



            if (other.gameObject.GetComponent<Controller>().ButtonHeld())
            {
                Vector3 dir = transform.position - parentplane.transform.position; // need corner case!
                Vector3 controllerdelta = other.gameObject.GetComponent<Controller>().GetPosition() - initialpos;
                float dot = Vector3.Dot(dir, controllerdelta);
                float scalefactor = .05f,cornerscale = 1.5f;

     
                other.gameObject.GetComponent<Controller>().scaling = true;
                switch (this.gameObject.tag)
                {

                    case "ul":
                        if (dot >= 0)
                            newscale += new Vector3(1f,0,1f) * controllerdelta.magnitude * scalefactor*cornerscale;
                        else
                            newscale -= new Vector3(1f, 0,1f) * controllerdelta.magnitude * scalefactor* cornerscale;
                        break;
                    case "ur":
                        if (dot >= 0)
                            newscale -= new Vector3(-1f, 0, -1f) * controllerdelta.magnitude * scalefactor * cornerscale;
                        else
                            newscale += new Vector3(-1f, 0, -1f) * controllerdelta.magnitude * scalefactor * cornerscale;
                        break;
                    case "bl":
                        if (dot >= 0)
                            newscale += new Vector3(1f, 0, 1f) * controllerdelta.magnitude * scalefactor * cornerscale;
                        else
                            newscale -= new Vector3(1f, 0, 1f) * controllerdelta.magnitude * scalefactor * cornerscale;
                        break;
                    case "br":
                        if (dot >= 0)
                            newscale -= new Vector3(-1f, 0, -1f) * controllerdelta.magnitude * scalefactor * cornerscale;
                        else
                            newscale += new Vector3(-1f, 0, -1f) * controllerdelta.magnitude * scalefactor * cornerscale;
                        break;
                    case "bottom":
                      case "left":
                        if(dot >=0)
                        newscale += transform.localPosition*controllerdelta.magnitude*scalefactor;
                        else
                            newscale -= transform.localPosition * controllerdelta.magnitude * scalefactor;
                        break;
                    case "top":
                    case "right":
                        if (dot >= 0)
                            newscale -= transform.localPosition * controllerdelta.magnitude * scalefactor;
                        else
                            newscale += transform.localPosition * controllerdelta.magnitude * scalefactor;
                        break;
                    default:
                        break;

                }
                initialpos = other.gameObject.GetComponent<Controller>().GetPosition();

            }

            if (parentplane.transform.localScale != newscale)
            {
                audiosource.volume = 0.5f;
                audiosource.clip = scalesound;

                if (!audiosource.isPlaying)
                    audiosource.Play();
            }
            else
                audiosource.Stop();

            // set position = controller position
            parentplane.transform.localScale = newscale;



        }
        else
            audiosource.Stop();
    }

}
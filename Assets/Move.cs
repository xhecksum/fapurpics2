using UnityEngine;
using System.Collections;
using System.IO;

public class Move : MonoBehaviour
{
    private AudioSource audiosource;
    public AudioClip selectsound, destroysound;
    private GameObject pickup;
    public bool held;

    private void Start()

    {

        held = false;
           audiosource = GetComponent<AudioSource>();
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "controller" && !other.gameObject.GetComponent<Controller>().scalingoff)
        {
            audiosource.clip = selectsound;
            audiosource.Play();
        }


    }
    private void OnTriggerExit(Collider other)
    {
        
        // save state
        string tagx = string.Concat(this.name, "xpos");
        string tagy = string.Concat(this.name, "ypos");
        string tagz = string.Concat(this.name, "zpos");

        PlayerPrefs.SetFloat(tagx, transform.position.x);
        PlayerPrefs.SetFloat(tagy, transform.position.y);
        PlayerPrefs.SetFloat(tagz, transform.position.z);

        tagx = string.Concat(this.name, "xrot");
        tagy = string.Concat(this.name, "yrot");
        tagz = string.Concat(this.name, "zrot");
        string tagw = string.Concat(this.name, "wrot");

        PlayerPrefs.SetFloat(tagx, transform.rotation.x);
        PlayerPrefs.SetFloat(tagy, transform.rotation.y);
        PlayerPrefs.SetFloat(tagz, transform.rotation.z);
        PlayerPrefs.SetFloat(tagw, transform.rotation.w);

        tagx = string.Concat(this.name, "xscale");
        tagy = string.Concat(this.name, "yscale");
        tagz = string.Concat(this.name, "zscale");

        PlayerPrefs.SetFloat(tagx, transform.localScale.x);
        PlayerPrefs.SetFloat(tagy, transform.localScale.y);
        PlayerPrefs.SetFloat(tagz, transform.localScale.z);

    }

    private void Update()
    {

        if (pickup)
        {
            if (pickup.gameObject.GetComponent<Controller>().ButtonHeld() && !pickup.gameObject.GetComponent<Controller>().holding && !pickup.gameObject.GetComponent<Controller>().scaling)
            {
                transform.SetParent(pickup.gameObject.transform);
                pickup.gameObject.GetComponent<Controller>().holding = true;
                held = true;
                pickup.gameObject.GetComponent<Controller>().lastpickup = this.gameObject;
            }

            if (!pickup.gameObject.GetComponent<Controller>().ButtonHeld())
            {
                transform.parent = null;
                pickup.gameObject.GetComponent<Controller>().holding = false;
                pickup = null;
                held = false;
            }


        }

    }

    void OnTriggerStay(Collider other)
    {


        if (other.gameObject.tag == "controller")
        {
            if(!other.gameObject.GetComponent<Controller>().scalingoff)
            foreach (Transform child in transform)  
                child.gameObject.SetActive(true);

            if (other.gameObject.GetComponent<Controller>().ButtonHeld())
            {
                pickup = other.gameObject;

        
            }

            if (other.gameObject.GetComponent<Controller>().gripbutton)
            {
                string destpath = string.Concat(Path.GetDirectoryName(this.gameObject.name),"\\","temp.fapurpics","\\",Path.GetFileName(this.gameObject.name));

              
                File.Move(this.gameObject.name, destpath);

                Destroy(this.gameObject);
                other.gameObject.GetComponent<Controller>().gripbutton = false;
            }


        }
    }



}
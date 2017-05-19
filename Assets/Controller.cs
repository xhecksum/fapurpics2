using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.VR;
using System.IO;

public class Controller : MonoBehaviour
{
    private SteamVR_TrackedObject trackedObj;
    public bool triggerdown, holding, scaling, scalingoff,gripbutton;
    private List<GameObject> images = new List<GameObject>();
    public GameObject lastpickup;
    int fileset;
    private AudioSource audiosource;

    void Awake()
    {
        fileset = 0;

        trackedObj = GetComponent<SteamVR_TrackedObject>();
        triggerdown = false;
        scalingoff = false;
        audiosource = GetComponent<AudioSource>();

        foreach (GameObject fooObj in GameObject.FindGameObjectsWithTag("plane"))
        {
            images.Add(fooObj);
        }
    }

    private void Update()
    {
        if (SteamVR_Controller.Input((int)trackedObj.index).GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            triggerdown = true;
        if (SteamVR_Controller.Input((int)trackedObj.index).GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            triggerdown = false;

        if (SteamVR_Controller.Input((int)trackedObj.index).GetPressDown(SteamVR_Controller.ButtonMask.Grip))
            gripbutton = true;
        if (SteamVR_Controller.Input((int)trackedObj.index).GetPressUp(SteamVR_Controller.ButtonMask.Grip))
            gripbutton = false;

        if (SteamVR_Controller.Input((int)trackedObj.index).GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            GameObject go = GameObject.Find("GameStart"), holdObj = null;
            fileset = (fileset + 1) % go.GetComponent<LoadAllFromFolder>().filesLocation.Count;

            foreach (GameObject fooObj in GameObject.FindGameObjectsWithTag("plane"))
            {
                if (!fooObj.GetComponent<Move>().held)
                    Destroy(fooObj);
                else
                    holdObj = fooObj;


            }

            audiosource.volume = 0.5f;
            audiosource.Play();
            StartCoroutine(go.GetComponent<LoadAllFromFolder>().LoadFiles(fileset));
            foreach (GameObject fooObj in GameObject.FindGameObjectsWithTag("plane"))
            {
                images.Add(fooObj);
            }

            string destpath = go.GetComponent<LoadAllFromFolder>().filesLocation[fileset];

           /* if (holdObj)
            {
                destpath = string.Concat(destpath, "\\", Path.GetFileName(holdObj.name));
                File.Move(holdObj.name, destpath);
                holdObj.name = destpath;
                holdObj = null;
                
            }
            */
        }

        if (SteamVR_Controller.Input((int)trackedObj.index).GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            scalingoff = !scalingoff;

            // toggle scaling
            foreach (GameObject fooObj in GameObject.FindGameObjectsWithTag("plane"))
            {
                images.Add(fooObj);
            }
            foreach (GameObject image in images)
                if(image)
                foreach (Transform child in image.transform)
                    child.gameObject.SetActive(false);

            if(!scalingoff && lastpickup)
                foreach (Transform child in lastpickup.transform)
                    child.gameObject.SetActive(true);

        }


    }


    public bool ButtonHeld()
    {
        return triggerdown;
    }



    public Vector3 GetPosition()
    {
        return transform.position;
    }

  
    void OnTriggerStay(Collider other)
    {


        if (!triggerdown && other.gameObject.tag != "plane" && other.gameObject.tag != "controller")
            SteamVR_Controller.Input((int)trackedObj.index).TriggerHapticPulse(200);



        
    }

}
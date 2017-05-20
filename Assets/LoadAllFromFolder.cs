using UnityEngine;

using System.Collections;

using System.Collections.Generic;

using System.IO;
using UnityEngine.VR;



public class LoadAllFromFolder : MonoBehaviour
{

    public List<string> filesLocation = new List<string>();
    public List<Texture2D> images = new List<Texture2D>();
    public List<string> filenames = new List<string>();
    public GameObject prefab;
    public int i = 0;
    public float radius = 10f;


    public IEnumerator Start()
    {
        //VRSettings.renderScale = 2.0f;

        readTextFile("./filepaths.txt");

        //load first dir
        yield return LoadFiles(0);

    }

    public IEnumerator LoadFiles(int f)
    {
        images = new List<Texture2D>();
        filenames = new List<string>();
        i = 0;

        yield return StartCoroutine(

     "LoadAll",

      Directory.GetFiles(filesLocation[f], "*.PNG", SearchOption.TopDirectoryOnly)

   );

        yield return StartCoroutine(

     "LoadAll",

      Directory.GetFiles(filesLocation[f], "*.jpg", SearchOption.TopDirectoryOnly)

      
   );

        CreateImages();
    }

    public void CreateImages()
    {
        foreach (Texture2D texture in images)
        {
          
            CreateImage(texture, images.Count);
            i++;
        }
    }

    void readTextFile(string file_path)
    {
        StreamReader inp_stm = new StreamReader(file_path);


        while (!inp_stm.EndOfStream)
        {
            string inp_ln = inp_stm.ReadLine();
            filesLocation.Add(inp_ln);

        }

        inp_stm.Close();
    }


    public void CreateImage(Texture2D texture, int numberOfObjects)
    {

        float angle = i * Mathf.PI * 2 / numberOfObjects;
        Vector3 pos = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;

        pos.z += i * .01f;

        Quaternion rot = new Quaternion();
        rot.SetLookRotation(new Vector3(1, 0, 0), new Vector3(-pos.x, pos.y, -pos.z));

        pos.y = 5f;
        GameObject go = Instantiate(prefab, pos * .15f, rot);
        go.name = filenames[i];

        float scalefactor = .00005f;
        // set size of prefab to image size
        go.transform.localScale = new Vector3(scalefactor * texture.width, .01f, scalefactor * texture.height);

        Vector3 savedpos = new Vector3();
        Vector3 savedscale = new Vector3();
        Quaternion savedrot = new Quaternion();

        savedpos.x = PlayerPrefs.GetFloat(string.Concat(go.name, "xpos"));
        savedpos.y = PlayerPrefs.GetFloat(string.Concat(go.name, "ypos"));
        savedpos.z = PlayerPrefs.GetFloat(string.Concat(go.name, "zpos"));
        savedrot.x = PlayerPrefs.GetFloat(string.Concat(go.name, "xrot"));
        savedrot.y = PlayerPrefs.GetFloat(string.Concat(go.name, "yrot"));
        savedrot.z = PlayerPrefs.GetFloat(string.Concat(go.name, "zrot"));
        savedrot.w = PlayerPrefs.GetFloat(string.Concat(go.name, "wrot"));
        savedscale.x = PlayerPrefs.GetFloat(string.Concat(go.name, "xscale"));
        savedscale.y = PlayerPrefs.GetFloat(string.Concat(go.name, "yscale"));
        savedscale.z = PlayerPrefs.GetFloat(string.Concat(go.name, "zscale"));

        if (savedpos != Vector3.zero)
        {
            go.transform.position = savedpos;
            go.transform.rotation = savedrot;
            go.transform.localScale = savedscale;
        }

        // set texture or prefab to image 
        go.GetComponent<MeshRenderer>().material.mainTexture = texture;
    }


    public IEnumerator LoadAll(string[] filePaths)
    {

        foreach (string filePath in filePaths)
        {
            System.IO.Directory.CreateDirectory(string.Concat(Path.GetDirectoryName(filePath),"\\","temp.fapurpics"));
  

            WWW load = new WWW("file:///" + filePath);

            yield return load;

            if (!string.IsNullOrEmpty(load.error))
            {

                Debug.LogWarning(filePath + " error");

            }
            else
            {
                images.Add(load.texture);
                filenames.Add(filePath);

            }

        }

    }

}




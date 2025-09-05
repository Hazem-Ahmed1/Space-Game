using UnityEngine;

public class CubeRandomizer : MonoBehaviour
{
    
    public Mesh[] possibleMeshes;


    private MeshFilter meshFilter;
    private Renderer rend;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        rend = GetComponent<Renderer>();

        RandomizeCube();
    }

    void RandomizeCube()
    {
        // dol 3shan n8yer elshakl
        if (possibleMeshes.Length > 0)
        {
            int randomIndex = Random.Range(0, possibleMeshes.Length);
            meshFilter.mesh = possibleMeshes[randomIndex];
        }

        // w dol el 7gm
        float randomScaleX = Random.Range(0.5f, 2.0f);
        float randomScaleY = Random.Range(0.5f, 2.0f);
        float randomScaleZ = Random.Range(0.5f, 2.0f);
        transform.localScale = new Vector3(randomScaleX, randomScaleY, randomScaleZ);

        // w aked dol el lon y3ny msh fadl 8ero 🙂
        Color randomColor = new Color(Random.value, Random.value, Random.value);
        rend.material.color = randomColor;
    }
}

//Mosaab Elrfa3y 😎
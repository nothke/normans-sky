using UnityEngine;

public class Example : MonoBehaviour
{
    // arrays of values
    string[] prefixes;
    string[] suffixes;

    void Start()
    {
        // Fill arrays
        prefixes = SReader.GetLines("read_this.txt", "PREFIXES");
        suffixes = SReader.GetLines("read_this.txt", "SUFFIXES");
    }

    string GetRandomName()
    {
        return SReader.GetRandom(prefixes) + SReader.GetRandom(suffixes);
    }

    void Update()
    {
        // Press space to output a random name
        if (Input.GetKeyDown(KeyCode.Space))
            Debug.Log(GetRandomName());
    }
}
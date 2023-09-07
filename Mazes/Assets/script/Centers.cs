using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Centers : MonoBehaviour
{
    private static Centers mInstance = null;

    void Start()
    {
        isHidden(gameObject);
    }

    private void Awake()
    {
        if (instance == null)
        {
            mInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static Centers instance
    {
        get
        {
           return mInstance;
        }
    }

    // 정사각형 미로의 한 변의 길이

    private uint mazeSize_ = 20;
    public uint mazeSize
    {
        get { return mazeSize_; }
        set
        {
            if (value < 20)
                mazeSize_ = 20;
            else
                mazeSize_ = value;
        }
    }

    public void isHidden(GameObject go)
    {
        if(go.TryGetComponent(out MeshRenderer mr))
        {
            mr.enabled = false;
        }
        
    }

    public void isSeen(GameObject go)
    {
        if (go.TryGetComponent(out MeshRenderer mr))
        {
            mr.enabled = true;
        }
    }

    public void Print1D_Array<T>(T[] data)
    {
        string output = $"1D Array of type {typeof(T).Name}:\n";

        for (int i = 0; i < data.Length; i++)
        {
            output += data[i].ToString();
            output += " ";
        }

        Debug.Log(output);
    }

    public void Print2D_Array<T>(T[,] data)
    {
        string output = $"Array of type {typeof(T).Name}:\n";

        for (int i = 0; i < data.GetLength(0); i++)
        {
            for (int j = 0; j < data.GetLength(1); j++)
            {
                output += data[i, j].ToString();
                output += " ";

                // If this is the last column, add a line break
                if (j == data.GetLength(1) - 1)
                {
                    output += "\n";
                }
            }
        }

        Debug.Log(output);
    }

    public void Print3D_Array<T>(T[,,] data)
    {
        string output = $"3D Array of type {typeof(T).Name}:\n";

        for (int i = 0; i < data.GetLength(0); i++)
        {
            output += $"Layer {i}:\n";

            for (int j = 0; j < data.GetLength(1); j++)
            {
                for (int k = 0; k < data.GetLength(2); k++)
                {
                    output += data[i, j, k].ToString();
                    output += " ";
                }

                output += "\n";
            }
        }

        Debug.Log(output);
    }

    public void PrintArray<T>(System.Array data, int dimensions)
    {
        string output = $"{dimensions}D Array of type {typeof(T).Name}:\n";

        PrintArrayRecursively<T>(data, new int[dimensions], 0, output);

        Debug.Log(output);
    }

    public void PrintArrayRecursively<T>(System.Array array, int[] indices, int dimension, string output)
    {
        if (dimension == indices.Length - 1)
        {
            for (int i = 0; i < array.GetLength(dimension); i++)
            {
                indices[dimension] = i;
                output += array.GetValue(indices).ToString();
                output += " ";
            }

            output += "\n";
        }
        else
        {
            for (int i = 0; i < array.GetLength(dimension); i++)
            {
                indices[dimension] = i;
                PrintArrayRecursively<T>(array, indices, dimension + 1, output);
            }
        }
    }


    public PV personView = PV.none;
    public List<Challanges> challanges = new() { Challanges.none };

    public float trapFrequecy = 0.0f;
    public float distanceForEnd = 0.0f;
    public float bonusFrequency = 0.0f;

    public enum PV
    {
        none,
        FPV,
        TPV
    }

    public enum Challanges
    {
        none,
        HardControl,
        FallDown,
        RandomWalls,
        WallDeath
    }

}

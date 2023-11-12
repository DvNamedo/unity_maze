using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EnumerableExtensions
{
    public static T[,] ToTwoDimensionalArray<T>(this IEnumerable<IEnumerable<T>> source)
    {
        int rowCount = source.Count();
        int colCount = source.First().Count();

        T[,] array2D = new T[rowCount, colCount];

        int i = 0;
        int j = 0;

        foreach (var row in source)
        {
            foreach (var value in row)
            {
                array2D[i, j++] = value;
            }

            i++;
            j = 0;
        }

        return array2D;
    }
} // feat. Chat GPT


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

    // 직사각형 미로의 밑변, 높이 길이 및 정사각형 길이

    private uint mazeSize_ = 12;
    public uint mazeSize
    {
        get { return mazeSize_; }
        set
        {
            if (value <= 12)
                mazeSize_ = 12;
            else
                mazeSize_ = value;
        }
    }

    private uint mazeSizeX_ = 12;
    private uint mazeSizeY_ = 12;

    public uint mazeSizeX
    {
        get { return mazeSizeX_; }
        set
        {
            if (value <= 12)
                mazeSizeX_ = 12;
            else
                mazeSizeX_ = value;
        }
    }

    public uint mazeSizeY
    {
        get { return mazeSizeY_; }
        set
        {
            if (value <= 12)
                mazeSizeY_ = 12;
            else
                mazeSizeY_ = value;
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

    public void Array2D_y_SymmetricTranspositon<T>( ref T[,] data)
    {
        T[,] symmetriced = new T[data.GetLength(0), data.GetLength(1)];

        for (int i = 0; i < data.GetLength(0); i++)
        {
            for (int j = 0; j < data.GetLength(1); j++)
            {
                symmetriced[i, j] = data[i, data.GetLength(1) - j - 1];
            }
        }

        data = symmetriced;
    }

    public void Array2D_clockwise_90_rotationalTransposition<T>(ref T[,] data)
    {
        T[,] rotatedData = new T[data.GetLength(1), data.GetLength(0)];

        for (int i = 0; i < data.GetLength(0); i++)
        {
            for (int j = 0; j < data.GetLength(1); j++)
            {
                rotatedData[j, i] = data[data.GetLength(0) - i - 1,j];
            }
        }

        data = rotatedData;
    }

    //곂쳐지는 부분은 || 로 처리
    // 여기 행열 반대로 생각한듯
    public bool[,] booleanArray2D_merge(bool[,] centerData, bool[,] sideData, direction dir, int overlapedIndexLength)
    {
        int restwardSize = 1;
        overlapedIndexLength = Mathf.Abs(overlapedIndexLength);
        bool[,] combinedData = new bool[1, 1];

        if (dir == direction.Downward)
        {
            if (centerData.GetLength(1) == sideData.GetLength(1))
            {
                restwardSize = centerData.GetLength(0) + centerData.GetLength(0) - overlapedIndexLength;
                combinedData = new bool[restwardSize, centerData.GetLength(1)];
                for (int i = 0; i < sideData.GetLength(0); i++)
                {
                    for (int j = 0; j < centerData.GetLength(1); j++)
                    {
                        combinedData[i, j] = sideData[i, j];
                    }
                }

                for(int i = sideData.GetLength(0) - overlapedIndexLength; i < restwardSize; i++)
                {
                    for (int j = 0; j < centerData.GetLength(1); j++)
                    {
                        combinedData[i, j] = combinedData[i, j] || centerData[i - (sideData.GetLength(0) - overlapedIndexLength), j];
                    }
                }

            }
            else { return null; }

        }
        if (dir == direction.Upward)
        {
            if (centerData.GetLength(1) == sideData.GetLength(1))
            {
                restwardSize = centerData.GetLength(0) + centerData.GetLength(0) - overlapedIndexLength;
                combinedData = new bool[restwardSize, centerData.GetLength(1)];
                for (int i = 0; i < centerData.GetLength(0); i++)
                {
                    for (int j = 0; j < centerData.GetLength(1); j++)
                    {
                        combinedData[i, j] = centerData[i, j];
                    }
                }

                for (int i = centerData.GetLength(0) - overlapedIndexLength; i < restwardSize; i++)
                {
                    for (int j = 0; j < centerData.GetLength(1); j++)
                    {
                        combinedData[i, j] = combinedData[i, j] || sideData[i - (centerData.GetLength(0) - overlapedIndexLength), j];
                    }
                }
            }
            else { return null; }
        }
        if (dir == direction.leftward)
        {
            if (centerData.GetLength(0) == sideData.GetLength(0))
            {
                restwardSize = centerData.GetLength(1) + centerData.GetLength(1) - overlapedIndexLength;
                combinedData = new bool[centerData.GetLength(0), restwardSize];
                for (int i = 0; i < centerData.GetLength(0); i++)
                {
                    for (int j = 0; j < sideData.GetLength(1); j++)
                    {
                        combinedData[i, j] = sideData[i, j];
                    }
                }

                for (int i = 0; i < centerData.GetLength(0); i++)
                {
                    for (int j = sideData.GetLength(1) - overlapedIndexLength; j < restwardSize; j++)
                    {
                        combinedData[i, j] = combinedData[i,j] || centerData[i, j - sideData.GetLength(1) - overlapedIndexLength];
                    }
                }

            }
            else { return null; }
        }
        if (dir == direction.rightward)
        {
            if (centerData.GetLength(0) == sideData.GetLength(0))
            {
                restwardSize = centerData.GetLength(1) + centerData.GetLength(1) - overlapedIndexLength;
                combinedData = new bool[centerData.GetLength(0), restwardSize];
                for (int i = 0; i < centerData.GetLength(0); i++)
                {
                    for (int j = 0; j < centerData.GetLength(1); j++)
                    {
                        combinedData[i, j] = centerData[i, j];
                    }
                }

                for (int i = 0; i < centerData.GetLength(0); i++)
                {
                    for (int j = centerData.GetLength(1) - overlapedIndexLength; j < restwardSize; j++)
                    {
                        combinedData[i, j] = combinedData[i, j] || sideData[i, j - centerData.GetLength(1) - overlapedIndexLength];
                    }
                }

            }
            else { return null; }
        }

        return combinedData;
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

    public enum direction
    {
        Upward,
        Downward,
        leftward,
        rightward
    }

}

/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGenerater : MonoBehaviour
{
//     public Text stateMassage;
//     public Image prograssBar;
//     public GameObject wallPrefap;

//     private byte[,] mapSchematicArray;


//     static List<uint> familyList = new List<uint>();
//     static List<blockBase> blocks = new List<blockBase>();

//     class blockBase
//     {
//         public uint x;
//         public uint y;
//         /// <summary>
//         /// �Ϲ������� uint.MaxValue �� null �ν� ����ؾ� ��
//         /// </summary>
//         public uint family;
//         public byte wallData;
        
//         public blockBase(uint CoordX, uint CoordY, uint _family, byte _wall)
//         {
//             x = CoordX;
//             y = CoordY;
//             family = _family;
//             wallData = _wall;

//             familyAdd(_family);
//         }

//         public void familyAdd(uint number)
//         {
//             if (!familyList.Contains(number))
//             {
//                 familyList.Add(number);
//             }
//             family = number;
//         }

//         public void newFamilyAdd()
//         {
//             uint i = 0;
//             while (familyList.Contains(i)) { i++; }
//             family = i;
//             familyList.Add(i);
//         }

//         public void familychange(uint number)
//         {
//             int count = 0;
//             for(int i = 0; i < blocks.Count; i++)
//             {
//                 if(blocks[i].family == this.family)
//                 {
//                     count++;
//                 }
//             }

//             if(count <= 1)
//             {
//                 familyList.Remove(family);
//                 family = number;
//             }
//         }

//     }

//     private void Start()
//     {
//         mapSchematicArray = new byte[Centers.instance.mazeSize, Centers.instance.mazeSize];
//     }

//     void setSchementics()
//     {

//     }

//     byte[,] formalizatedMazeSchematic(uint size_X, uint size_Y)
//     {
//         uint[,] arrayToGrouping = new uint[size_X, 2]; //{{family , num},{family , num},{family , num}, ... , {family , num}}
//             for (uint i = 0; i < size_X; i++)
//             {
//                 blocks.Add(new blockBase(i, 0, i, 0b0000_1111));
//             }
        
//         for (uint column = 0; column < size_Y - 1; column++) 
//         {

//             // ��ǥ (i,column)�� ���� ���, ��ǥ(i + 1, column)�� ���� �� ������ family ���� wallData ���� �����Ѵ�.
//             for (uint i = 0; i < size_X - 1; i++)
//             {
//                 if(Random.value < 0.75f)
//                 {
//                     // �������� ��и� ���� ���� ��� ���ϵ��� ���� �ڱ� �ڽ��� �йи� ������ �ٲ�� ��.

//                     foreach(int myFamily in listFindAtFamilyValue(blocks[iFindAtCoordinations(i + 1, column)].family))
//                     {
//                         blocks[myFamily].familychange(blocks[iFindAtCoordinations(i, column)].family);
//                     }

//                     blocks[iFindAtCoordinations(i, column)].wallData -= 0b0000_0010; // �캮 ����
//                     blocks[iFindAtCoordinations(i + 1, column)].wallData -= 0b0000_1000; // �º� ����
//                 }
//             }

//             foreach(int fam in familyList)
//             {
//                 List<uint> order = new List<uint>();

//                 for(uint i=0; i < size_X; i++)
//                 {
//                     if (blocks[iFindAtCoordinations(i, column)].family.Equals(fam)) { order.Add(i); }
//                 }

//                 bool isModifiedWall = false;

//                 for(int i = 0; i < order.Count; i++)
//                 {
//                     if(Random.value <= 1/Mathf.Clamp(order.Count, 1, 8))
//                     {
//                         isModifiedWall = true;
//                         blocks[iFindAtCoordinations(order[i], column)].wallData -= 0b0000_0100;
//                         blocks.Add(new blockBase(order[i], column + 1, (uint)fam, 0b0000_1110));
//                     }
//                 }

//                 if (!isModifiedWall)
//                 {
//                     uint randomOrder = order[Random.Range(0, order.Count - 1)];

//                     blocks[iFindAtCoordinations(randomOrder, column)].wallData -= 0b0000_0100;
//                     blocks.Add(new blockBase(randomOrder, column + 1, (uint)fam, 0b0000_1110));
                    
//                 }

//             }

//             // ���� ĭ�� �� ���� ���
//             for(uint i = 0; i < size_X; i++)
//             {
//                 if(iFindAtCoordinations(i,column + 1).Equals(-1))
//                 {
//                     blocks.Add(new blockBase(i, column + 1, uint.MaxValue, 0b0000_1111));
//                     blocks[blocks.Count - 1].newFamilyAdd();
//                 }
//             }

//         }

//         //�� �� ��(n, size_Y - 1) ��� ���� �ձ�
//         for(uint i=0; i<size_X; i++)
//         {
//             if(i == 0) //ù ��
//             {
//                 blocks[iFindAtCoordinations(i, size_Y - 1)].wallData -= 0b0000_0010;
//             }
//             else if(i == size_X - 1) // �� ��
//             {
//                 blocks[iFindAtCoordinations(i, size_Y - 1)].wallData -= 0b0000_1000;
//             }
//             else // ������
//             {
//                 blocks[iFindAtCoordinations(i, size_Y - 1)].wallData -= 0b0000_1010;
//             }

//         }

//         //blocks ����Ʈ �ϼ�(��� ��ǥ�� ���� �˸°� ��ġ��)

//     }

    

//     int iFindAtCoordinations(uint X, uint Y)
//     {
//         for(int i=0; i < blocks.Count; i++)
//         {
//             if(blocks[i].x.Equals(X) && blocks[i].y.Equals(Y))
//             {
//                 return i;
//             }
//         }

//         Debug.LogError("do Not exist coordinations");
//         return -1;

//     }

//     List<int> listFindAtFamilyValue(uint _family)
//     {
//         List<int> foundedValues = new List<int>();

//         for(int i=0;i< blocks.Count; i++)
//         {
//             if (blocks[i].family.Equals(_family))
//             {
//                 foundedValues.Add(i);
//             }
//         }

//         return foundedValues;

//     }

}
*/
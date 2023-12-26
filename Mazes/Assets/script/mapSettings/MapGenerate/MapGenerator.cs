using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System.Linq;

public enum State
{
    none,
    getMazeSchementics0,
    getMazeSchementics1,
    getMazeSchementics2,
    getMazeSchementics3,
    getMazeSchementicsLast,
    getTopWall,
    getLeftWall,
    fillTile,
    setZTrap,
    setBonus,
    setUtilityItem,
    etcResource,

}

namespace try2
{
    public static class Extentions
    {



        public static List<T> AddMultiple<T>(this List<T> source, T value, int Count)
        {
            for (int i = 0; i < Count; i++)
            {
                source.Add(value);
            }

            return source;
        }

    }


    class Maze<T> where T : blockBase, new()
    {
        public float _leftEilminationRate;
        public float _topEilminationRate;

        private string operationDirection = "column";

        public List<T> blocks = new List<T>();
        private (int, int) size;

        /// <summary>
        /// </summary>
        /// <param name="od">default setting is column direction</param>
        public Maze(int X, int Y, string od, bool isVoid = false, float topEilmnationRate = 0.5f, float leftEilminationRate = 0.5f)
        {
            operationDirection = od == "row" ? od : "column";

            _topEilminationRate = topEilmnationRate;
            _leftEilminationRate = leftEilminationRate;

            if(operationDirection == "column" && !isVoid)
                size = (Y , X); // 이거 나중에 회전하면서 바뀜
            else
                size = (X , Y);

            if (isVoid)
                blocks = blocks.AddMultiple(null, X * Y);
            else
                setMaze(X, Y);
        }









        // 오브젝트 까지 표시하기에 너무 길어서 간소화 하고자 쓴 델리게이트. 의미 없는 코드
        private delegate int Coord(int i, int j);

        /// <summary>
        /// it return Normal maze Array that each dimension is x coordination, y coordination and wall direction
        /// </summary>
        /// <param name="size_X">X coordination size</param>
        /// <param name="size_Y">Y coordination size</param>
        /// <returns></returns>
        void setMaze(int size_X, int size_Y)
        {
            /*
            switch (loadState)
            {
                case State.getMazeSchementics0:
                    loadState = State.getMazeSchementics1;
                    break;
                case State.getMazeSchementics1:
                    loadState = State.getMazeSchementics2;
                    break;
                case State.getMazeSchementics2:
                    loadState = State.getMazeSchementics3;
                    break;
                case State.getMazeSchementics3:
                    loadState = State.getMazeSchementicsLast;
                    break;
                default:
                    loadState = State.none;
                    break;
            }
            */

            size_X = Mathf.Abs(size_X);
            size_Y = Mathf.Abs(size_Y);

            // 좌표
            Coordinate coordinate = new Coordinate(size_X);

            Coord coord = new Coord(coordinate.coord);
            bool isModified;


            // blocks 초기화
            // 남겨놓을 테투리 까지 고려해서 (좌표) - 1 이 없어도 됨
            blocks = blocks.AddMultiple(null, (size_X) * (size_Y));

            // 중간 디버깅
            //Debug.Log(debuggingDoubleWallDataMaze(typecast_Blocklization(mazeTypecast(blocks, size_X, size_Y))));

            // Create new blocks where none exist
            // After randomly removing the wall, the family value of all blocks that have the family value of the block to be integrated into the previous block is changed to the family value of the previous block.

            //각 행에 대하여 한번씩 돌되, 마지막 행은 따로 처리해줘야 하므로 마지막 전 행까지 돌아주는 코드
            for (int Y = 0; Y < size_Y - 1; Y++)
            {

                //progress = (column + 1.0f) / size_Y;
                //Random.InitState((int)(Mathf.Sin(DateTime.Now.Ticks) * 100));

                // 행이 정해진 상태에서 각 열에서의 값을 null 이 아닐떄 blockBase 로 초기화 해주는 부분
                for (int X = 0; X < size_X; X++)
                {
                    //Debug.Log($"setblock in ({row},{column})");
                    blocks[coord(X, Y)] = blocks[coord(X, Y)] ?? new T().Init<T>((uint)coord(X, Y), top: true, side: true);

                }

                //중간 디버깅
                //Debug.Log(debuggingDoubleWallDataMaze(typecast_Blocklization(mazeTypecast(blocks, size_X, size_Y))));

                //horizontal wall eliminating

                // 맵 가해부분인 첫번 째 열 부분은 벽이 무조건 존재해야 하므로 건들여도 의미 없는 정보이므로 건들지 않음. (맵의 최외각은 추후 따로 처리됨(이 코드에서는 작성 안된부분))
                for (int i = 1; i < size_X; i++)
                {
                    
                    if (Random.Range(0, int.MaxValue) % 10000 < _leftEilminationRate * 10000)
                    {
                        //벽이 삭제된다면 그 삭제된 쪽의 구역들은 전부 연결되게 되므로 연결되었다는 표시인 family 값이 같은 모든 blocks의 원소에 대하여 연결 시킨 원소의 family 값으로 통일 시키는 코드
                        //수정: 피지배블록과 지배블록을 바꿈.
                        foreach (int myFamily in listFindAtFamilyValue(blocks[coord(i - 1, Y)].family))
                        {
                            blocks[myFamily].family = blocks[coord(i, Y)].family;
                        }

                        //Debug.Log($"Removed between {(i - 1, Y)} and {(i, Y)} , so {blocks[coord(i, Y)].mWallData.sideWall} to {false}");

                        // 왼쪽 벽 없앰을 표기하는 코드
                        blocks[coord(i, Y)].mWallData.sideWall = false;

                        //Debug.Log($"+ Removed between {(i - 1, Y)} and {(i, Y)} , modified to {blocks[coord(i, Y)].mWallData.sideWall}");
                    }
                }


                //중간 디버깅
                //Debug.Log(debuggingDoubleWallDataMaze(typecast_Blocklization(mazeTypecast(blocks, size_X, size_Y))));

                // virtical wall Eliminating ================================================================================================

                // 모든 연결된 각각의 큰 영역(family) 의 값들을 순서 없이 저장 하기 위한 family들의 집합, 즉 마을(town)
                HashSet<uint> townList = new HashSet<uint>();

                // 각각의 영역에 대하여 적어도 하나는 수직한 통로가 존재해야 하므로 한번 수정 되었는가를 표기하기 위한 지역 변수
                //bool isModified;

                // 마을(town) 리스트에 모든 가족(family)을 등재
                for (int X = 0; X < size_X; X++)
                {
                    townList.Add(blocks[coord(X, Y)].family);
                }

                // 마을에 등재된 모든 가족에 대하여 확인함
                foreach (uint eachFamily in townList)
                {
                    // 각각의 가족에 대한 수직통로가 있는가를 초기화
                    isModified = false;

                    // 그 가족구성원들(blocks 의 원소)을 저장하는 리스트
                    List<int> familyList = new List<int>();

                    for (int X = 0; X < size_X; X++)
                    {
                        if (blocks[coord(X, Y)].family.Equals(eachFamily))
                        {
                            familyList.Add(X);
                        }
                    }


                    // 랜덤으로 각 가족구성원들에 대하여 수직통로를 생성
                    foreach (int fam in familyList)
                    {

                        if (Mathf.Clamp(familyList.Count, 1, 8) * Random.Range(0, int.MaxValue) % 10000 < _topEilminationRate * 10000)
                        {
                            //Debug.Log($"randomModified: {(fam,Y)}");
                            isModified = true;

                            //처음부터 같은 가족값을 같도록 생성
                            blocks[coord(fam, Y + 1)] = new T().Init<T>(eachFamily, top: false, side: true);
                        }
                    }

                    // 가족 구성원 중 적어도 하나는 얻도록 강제
                    if (!isModified)
                    {
                        //neccessary modified 
                        
                        int randomFam = familyList[Random.Range(0, int.MaxValue) % familyList.Count];
                        //Debug.Log("nec-Modified{coord(familyList[randomFam])}");
                        //Debug.Log($"nec-Modified{(randomFam, Y)}");
                        blocks[coord(randomFam, Y + 1)] = new T().Init<T>(eachFamily, top: false, side: true);
                    }


                }

                //중간 디버깅
                //Debug.Log(debuggingDoubleWallDataMaze(typecast_Blocklization(mazeTypecast(blocks, size_X, size_Y))));

                #region old
                /*
                       //int[] familySet = new int[size_X];
                       //bool isModified = false;

                       //int confirmingFamily = -1;

                       //bool end = false;


                       //for (int row = 0; row < size_X; row++)
                       //{
                       //    familySet[row] = (int)blocks[coord(row, column)].family;
                       //}

                       //while (!end)
                       //{
                       //    Debug.Log("processed!");

                       //    for (int i = 0; i < size_X; i++)
                       //    {
                       //        if (familySet[i] != -1)
                       //        {
                       //            confirmingFamily = i;
                       //            i = size_X;
                       //        }

                       //        if (i == size_X - 1)
                       //        {
                       //            end = true;
                       //        }
                       //    }

                       //    if (!end)
                       //    {
                       //        int lastRow = -1;

                       //        for (int i = 0; i < size_X; i++)
                       //        {
                       //            if (familySet[i] == confirmingFamily)
                       //            {
                       //                lastRow = i;
                       //                familySet[i] = -1;
                       //                if (Random.value < downWallEilminationRate)
                       //                {
                       //                    Debug.Log("randomModified");
                       //                    isModified = true;
                       //                    blocks[coord(i, column + 1)] = new blockBase(blocks[coord(i, column)].family, top: false, side: true);
                       //                }
                       //            }
                       //        }

                       //        if (!isModified)
                       //        {
                       //            Debug.Log("neccessaryModified");
                       //            isModified = true;
                       //            blocks[coord(lastRow, column + 1)] = new blockBase(blocks[coord(lastRow, column)].family, top: false, side: true);
                       //        }

                       //    }

                       //    //=======================================================================================================================================

                       //} 
                       */
                #endregion
            }

            // 마지막 줄은 전부 연결되어, 모든 가족들이 하나의 가족으로 통일 되도록 해야하므로(즉, 전부 연결되어야 하므로) 특수한 처리를 해줌 (이거 틀림 다름)
            //end process

            //progress = (size_Y - 1.0f) / size_Y;

            for (int X = 0; X < size_X; X++)
            {
                if (blocks[coord(X, size_Y - 1)] == null)
                {
                    blocks[coord(X, size_Y - 1)] = new T().Init<T>((uint)coord(X, size_Y - 1), top: true, side: true); //(uint)coord(row, size_Y - 1), top: true, side: true
                }

                //Debug.Log($"{X} | {blocks.Count}");
            }

            for (int i = 1; i < size_X; i++)
            {
                if (blocks[coord(i, size_Y - 1)].family != blocks[coord(i - 1, size_Y - 1)].family)
                {

                    foreach (int myFamily in listFindAtFamilyValue(blocks[coord(i - 1, size_Y - 1)].family))
                    {
                        blocks[myFamily].family = blocks[coord(i, size_Y - 1)].family;
                    }

                    blocks[coord(i, size_Y - 1)].mWallData.sideWall = false;
                }

            }

            //중간 디버깅
            //Debug.Log(debuggingDoubleWallDataMaze(typecast_Blocklization(mazeTypecast(blocks, size_X, size_Y))));

            // 가해부분 삭제
            for (int i = 0; i < size_X; i++)
            {
                blocks[coord(i, 0)].mWallData.topWall = false;
            }

            for (int i = 0; i < size_Y; i++)
            {
                blocks[coord(0, i)].mWallData.sideWall = false;
            }

            // 혹시모를 비 연결부 예외처리
            uint befFamily = uint.MaxValue;
            for (int X = 0; X < size_X; X++)
            {
                for (int Y = 0; Y < size_Y; Y++)
                {
                    if (befFamily != uint.MaxValue)
                    {
                        if (befFamily != blocks[coord(X, Y)].family)
                        {
                            Debug.LogError("difference!");

                            if (X == 0 && Y != size_Y - 1)
                                blocks[coord(X, Y + 1)].mWallData.topWall = false;
                            else
                                blocks[coord(X, Y)].mWallData.sideWall = false;
                        }
                    }

                            
                    befFamily = blocks[coord(X, Y)].family;
                        
                }
            }

            for (int y = 0; y < size_Y - 1; y++)
            {
                for (int x = 0; x < size_X -1; x++)
                {
                    if (blocks[coord(x + 1, y)].mWallData.sideWall && blocks[coord(x, y)].mWallData.sideWall)
                        if (blocks[coord(x, y)].mWallData.topWall && blocks[coord(x,y+1)].mWallData.topWall)
                        {
                            Debug.LogError($"blocked at {(x,y)}");
                        }
                }
            }


            if(operationDirection == "column")
            {
                this.ForceSetRow(); // 처리할게 있어서 어쩔수 없이..
                this.rotate();
            }

            //this.addBoundary();

            //progress = 1;
            // type Casting
            // 출력부

        }
        //=============================================================




        /// <summary>
        /// 90 degree rotate to O-clock
        /// </summary>
        /// <returns></returns>
        public Maze<T> rotate()
        {
            int sizeX = size.Item1;
            int sizeY = size.Item2;

            Coordinate crd = new(sizeY);
            Coordinate org = new(sizeX);


            List<T> temp = new();
            temp = temp.AddMultiple(new T().Init<T>(uint.MaxValue,false,false), sizeX * sizeY);

            Debug.Log($"{(sizeX, sizeY)}");
            Debug.Log($"{temp.Count} | {blocks.Count}");

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    //old
                    //Debug.Log($"{(x,y)} >>> {(y, sizeX - x - 1)}");
                    //Debug.Log($"{org.coord(x, y)} | {crd.coord(y, sizeX - x - 1)}");
                    //Debug.Log($"{temp[crd.coord(y, sizeX - x - 1)].mWallData.sideWall} | {blocks[org.coord(x, y)].mWallData.topWall}");
                    temp[crd.coord(sizeY - (y + 1), x)].mWallData.topWall = blocks[org.coord(x,y)].mWallData.sideWall;    
                    
                    if(y != 0)
                        temp[crd.coord(sizeY - y, x)].mWallData.sideWall = blocks[org.coord(x,y)].mWallData.topWall;
                }
            }

            size = (sizeY, sizeX);
            blocks = temp;
            switch (operationDirection)
            {
                case "column":
                    operationDirection = "row";
                    break;
                case "row":
                    operationDirection = "column";
                    break;
                default:
                    break;
            }

            return this;

        }

        /// <summary>
        /// 쓰지 마
        /// </summary>
        /// <returns></returns>
        public Maze<T> removeBoundary()
        {
            List<T> core = new List<T>();

            Coordinate cdn = new(size.Item1);
            Coordinate original = new(size.Item1 + 1);

            core = core.AddMultiple(null, size.Item1 * size.Item2);

            for (int X = 0; X < size.Item1; X++)
            {
                for (int Y = 0; Y < size.Item2; Y++)
                {
                    core[cdn.coord(X, Y)] = blocks[original.coord(X, Y)];

                    if (Y == 0)
                        core[cdn.coord(X, Y)].mWallData.topWall = false;
                    if (X == 0)
                        try
                        {
                            core[cdn.coord(X, Y)].mWallData.sideWall = false;
                        }
                        catch(Exception e)
                        {
                            Debug.LogError($"{e} | size: {size} | {cdn.coord(X,Y)} | {(X,Y)}");
                        }
                }
            }

            blocks = core;

            return this;
        }

        /// <summary>
        /// 쓰지 마
        /// </summary>
        /// <returns></returns>
        public Maze<T> addBoundary()
        {
            List<T> result = new List<T>();

            Coordinate cdn = new(size.Item1 + 1);
            Coordinate original = new(size.Item1);

            result = result.AddMultiple(new T().Init<T>(uint.MaxValue, false, false), (size.Item1 + 1) * (size.Item2 + 1));

            for (int X = 0; X < size.Item1; X++)
            {
                for (int Y = 0; Y <= size.Item2; Y++)
                {
                    if (Y != size.Item2)
                        result[cdn.coord(X, Y)] = blocks[original.coord(X, Y)];
                    else
                        result[cdn.coord(X, Y)] = new T().Init<T>(uint.MaxValue, top: false, side: true);
                    

                    if (Y == 0)
                        result[cdn.coord(X, Y)].mWallData.topWall = true;
                    if (X == 0)
                        result[cdn.coord(X, Y)].mWallData.sideWall = true;
                }
                result[cdn.coord(X, size.Item2)] = new T().Init<T>(uint.MaxValue, top: true, side: false);
            }
            result[cdn.coord(size.Item1, size.Item2)] = new T().Init<T>(uint.MaxValue, top: true, side: true);

            blocks = result;
            return this;

        }

        // List<blockBase> 형태를 출력을 위해 bool?[x,y,2] 형태로 변환하는 부분
        //bool?[,,] mazeTypecast(List<blockBase> b, int X, int Y)
        //{
        //    bool?[,,] typeCastedArray = new bool?[X, Y, 2]; // [0]:width [1]: height
        //    Coordinate crd = new Coordinate(X);

        //    for (int j = 0; j < Y; j++)
        //    {
        //        for (int i = 0; i < X; i++)
        //        {
        //            typeCastedArray[i, j, 0] = b[crd.coord(i, j)] is null ? null : b[crd.coord(i, j)].mWallData.topWall;
        //            typeCastedArray[i, j, 1] = b[crd.coord(i, j)] is null ? null : b[crd.coord(i, j)].mWallData.sideWall;
        //            // + 
        //        }
        //    }

        //    // 맵의 가해 부분을 제거하려는 시도였으나 약간의 문제가 있어보여 일시적으로 주석처리
        //    //bool?[,,] sideCutArray = new bool?[X - 1, Y - 1, 2];

        //    //for (int i = 0; i < X-1; i++)
        //    //{
        //    //    for (int j = 0; j < Y-1; j++)
        //    //    {
        //    //        for (int k = 0; k < 2; k++) // non-meaning-practice
        //    //        {
        //    //            sideCutArray[i, j, k] = typeCastedArray[i + 1, j + 1, k];
        //    //        }

        //    //    }
        //    //}


        //    //return sideCutArray;
        //    return typeCastedArray;
        //}

        // 같은 family 값을 같는 모든 blocks 안의 원소들을 인덱스 순서가 빠른 순으로 리스트 형태로 출력하는 함수
        List<int> listFindAtFamilyValue(uint _family)
        {

            List<int> coords = new List<int>();

            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[i] == null)
                    break;

                if (blocks[i].family.Equals(_family))
                    coords.Add(i);
            }

            return coords;
        }


        public string getOperationDirection()
        {
            return operationDirection;
        }

        public (int, int) getSize()
        {
            return size;
        }

        /// <summary>
        /// ForceSetColumn() : The Meaning of "Force" change the "OperationDirection" irrelated to Maze's last point
        /// </summary>
        public void ForceSetColumn()
        {
            operationDirection = "column";
            // 처리해야함
        }

        /// <summary>
        /// ForceSetRow() : The Meaning of "Force" change the "OperationDirection" irrelated to Maze's last point
        /// </summary>
        public void ForceSetRow()
        {
            operationDirection = "row";
            // 처리해야함
        }

        public static Maze<T> operator +(Maze<T> element1, Maze<T> element2)
        {
            if (element1.getOperationDirection() != element2.getOperationDirection())
            {
                Debug.LogError("Elements with different direction CANNOT merge");
                return element1;
            }


            (int, int) resultSize = (1, 1);

            if (element1.getOperationDirection() == "column")
            {
                if (element1.size.Item2 != element2.size.Item2)
                {
                    Debug.LogError("Elements with differnt size on Y axis CANNOT merge");
                    return element1;
                }

                resultSize = (element1.size.Item1 + element2.size.Item1, element1.size.Item2);
            }

            if (element1.getOperationDirection() == "row")
            {
                if (element1.size.Item1 != element2.size.Item1)
                {
                    Debug.LogError("Elements with differnt size on X axis CANNOT merge");
                    return element1;
                }

                resultSize = (element1.size.Item1, element1.size.Item2 + element2.size.Item2);

            }

            Coordinate e1 = new(element1.size.Item1);
            Coordinate e2 = new(element2.size.Item1);
            Coordinate res = new(resultSize.Item1);


            Maze<T> result = new Maze<T>(resultSize.Item1, resultSize.Item2, element1.getOperationDirection(), isVoid:true);

            Debug.Log($"{element1.size} | {element2.size} | {resultSize} or {result.size}");
            Debug.Log($"{element1.blocks.Count} | {element2.blocks.Count} | {result.blocks.Count}");

            if(result.getOperationDirection() == "column")
            {
                for (int x = 0; x < element1.size.Item1; x++)
                {
                    for (int y = 0; y < resultSize.Item2; y++)
                    {
                        result.blocks[res.coord(x, y)] = element1.blocks[e1.coord(x, y)];
                    }
                }

                for (int x = 0; x < element2.size.Item1; x++)
                {
                    for (int y = 0; y < resultSize.Item2; y++)
                    {
                        result.blocks[res.coord(element1.size.Item1 + x, y)] = element2.blocks[e2.coord(x, y)];
                    }
                }


            }

            if(result.getOperationDirection() == "row")
            {
                for (int x = 0; x < resultSize.Item1; x++)
                {
                    for (int y = 0; y < element1.size.Item2; y++)
                    {
                        result.blocks[res.coord(x, y)] = element1.blocks[e1.coord(x, y)];
                    }
                }

                for (int x = 0; x < resultSize.Item1; x++)
                {
                    for (int y = 0; y < element2.size.Item2; y++)
                    {
                        Debug.Log($"{(x, y)}");
                        Debug.Log($"{res.coord(x, element1.size.Item2 + y)} | {e2.coord(x, y)}");
                        result.blocks[res.coord(x, element1.size.Item2 + y)] = element2.blocks[e2.coord(x, y)];
                    }
                }


            }


            return result;
        }


    }


    // 미로 구역을 벽에 따라 나눈 구역 중 한 부분
    public abstract class blockBase
    {
        /// <summary>
        /// uint.MaxValue implies meaning of 'nullValue' in this variable
        /// </summary>

        //별개의 각각의 구역이 벽이 가로막지 않고 연결되어 있음을 표시하는 부분. 
        // 이 family 값이 같은 값의 구역은 서로 연결되어 있어야 한다. 즉, 사이에 topWall 또는 sideWall 값이 거짓이어야 한다.
        // The set to which the block belongs
        public uint family;
        // 구역에서의 벽의 유무에 대한 구조체
        // Wall of the block
        public struct _wallData
        {
            //블록의 윗부분의 벽의 유무
            public bool topWall;
            //블록의 왼쪽 부분의 벽의 유무
            public bool sideWall; //it usually refer to left wall

            // 아래, 오른쪽 부분의 벽의 유무를 저장할 필요가 없는 이유는 인접한 구역에 대한 위쪽, 왼쪽 벽 데이터가 그 정보를 가지고 있기 때문.

            public _wallData(bool t, bool s)
            {
                topWall = t;
                sideWall = s;
            }

        }

        // 구역에서의 벽의 유무에 대한 구조체를 참조하여 선언
        public _wallData mWallData;


        /// <summary>
        /// It's an object for the structuralization of maze walls
        /// </summary>
        /// <param name="_family">Set for data proccessing</param>
        /// <param name="walls">Follow the formalities: "(bool,bool)" , Do Not here allow Blank.</param>
        public blockBase()
        {
            // 값 입력부

        }

        public void Init(uint _family, bool top, bool side)
        {
            // 값 입력부
            mWallData.topWall = top;
            mWallData.sideWall = side;

            family = _family;

        }

        public T Init<T>(uint _family, bool top, bool side) where T : blockBase, new()
        {
            T instance = new T();

            instance.Init(_family, top, side);

            return instance;
        }

    }
    // this Type source
    public class NormalBlock : blockBase
    {
        public NormalBlock()
        {
            
        }




    }



    // List<blockBase> 를 2차원 형태로 사용하려는 목적으로 2차원 좌표를 명시적으로 표현하기위한 함수 
    public class Coordinate
    {
        int sizeofX = 0;
        // x좌표 최대 지정. x좌표가 최대를 넘으면 y좌표가 1 증가하는 형태
        public Coordinate(int _sizeofX)
        {
            sizeofX = _sizeofX;
        }
        //use in the two dimention array
        public int coord(int x, int y)
        {
            if (sizeofX <= 0)
            {
                Debug.LogError("sizeofX needs over 1 number.");
                return -1;
            }

            if (x >= sizeofX)
            {
                y += x / sizeofX;
                x %= sizeofX;
                Debug.LogWarning($"(Coordinate){this} 's x exceed the limit");
            }
            return x + y * sizeofX;
        }
    }


    public class MapGenerator : MonoBehaviour
    {
        //여기 말고 다른데에다 해두자
        [Header("Loading Bar Info")]
        public Text stateMassage;
        public Image prograssBar;

        [Header("Prefep Setting for Instantiate wall")]
        public List<GameObject> wallPrefab;
        public Transform wallParent;


        //수직, 수평에서의 벽 제거 확률 변수
        [Header("Wall Eilmination Probability")]
        [SerializeField]
        float columnMazeSideWallEilminationRate = 0.75f;
        [SerializeField]
        float columnMazeTopWallEilminationRate = 0.25f;
        [SerializeField]
        float rowMazeSideWallEilminationRate = 0.3f;
        [SerializeField]
        float rowMazeTopWallEilminationRate = 0.10f;



        private bool[,,] mapSchematicArray;
        public static float progress;
        public static State loadState = State.none;
        public readonly Vector3 inf = Vector3.positiveInfinity;


        //formalizatedMaze_Inside_WallSchematic(int,int) 에서 미로를 생성하기 위해 미로 구역을 벽에 따라 나눈 블록의 실체를 저장하는 리스트


        private void Start()
        {
            //Debug.Log("=============================================================================================================================================================");

            StartCoroutine(MapGenerate());




            //getSchementics(ref mapSchematicArray, Xsize, Ysize);


            #region for debugging
            //Debug.Log(string.Join(",", formalizatedMaze_Inside_WallSchematic(20, 20)));
            //Centers.instance.Print3D_Array<bool>(formalizatedMaze_Inside_WallSchematic(40, 40));
            //Debug.Log(debuggingDoubleWallDataMaze(formalizatedMaze_Inside_WallSchematic(30, 30)));

            // bool?[,,] 형태의 미로를 시각화시키는 함수를 로그창에 띄우는 부분
            //bool?[,] map = typecast_Blocklization(formalizatedMaze_Inside_WallSchematic(30, 20));
            //bool?[,] map2 = map;
            //bool?[,] map3 = map;



            //Debug.Log("general:\n" + debuggingDoubleWallDataMaze(map));

            //Centers.instance.Array2D_y_SymmetricTranspositon<bool?>(ref map);
            //mazeCleanup(ref map);
            //Debug.Log("y-axis:\n" + debuggingDoubleWallDataMaze(map));

            //Centers.instance.Array2D_clockwise_90_rotationalTransposition<bool?>(ref map2);
            //mazeCleanup(ref map);
            //Debug.Log("90 angle:\n" + debuggingDoubleWallDataMaze(map2));

            //Centers.instance.Array2D_y_SymmetricTranspositon<bool?>(ref map3);
            //Centers.instance.Array2D_clockwise_90_rotationalTransposition<bool?>(ref map3);
            //mazeCleanup(ref map3);
            //Debug.Log("y-axis and 90 angle:\n" + debuggingDoubleWallDataMaze(map3));


            //Debug.Log(typecast_Blocklization(mapSchematicArray));
            #endregion



            //Debug.Log(debuggingDoubleWallDataMaze(typecast_Blocklization(mapSchematicArray)));
            //wallPrefabGenerate(mapSchematicArray);


            //Debug.Log(debuggingDoubleWallDataMaze(typecast_Blocklization(formalizatedMaze_Inside_WallSchematic(3, 100))));

            //Debug.Log(debuggingDoubleWallDataMaze(typecast_Blocklization(mazeTypecast(blocks, size_X, size_Y))); the source to use debuging map 

        }

        private void Update()
        {
            //foreach(GameObject go in Centers.instance.topWalls)
            //{
            //    if (go != null)
            //        go.transform.rotation *= Quaternion.Euler(new Vector3(0, 90 * Time.deltaTime, 0));
            //}

            //foreach (GameObject go in Centers.instance.leftWalls)
            //{
            //    if (go != null)
            //        go.transform.rotation *= Quaternion.Euler(new Vector3(0, 90 * Time.deltaTime, 0));
            //}


        }

        IEnumerator MapGenerate()
        {
            //싱글톤에서 미로 크기 값을 받아오는 부분, 현재는 사실상 안쓰임
            int Xsize = Centers.instance.difficulty == Centers.Difficulty.Custom ? (int)Centers.instance.mazeSizeX : (int)Centers.instance.mazeSize;//(int)Centers.instance.mazeSizeX;
            int Ysize = Centers.instance.difficulty == Centers.Difficulty.Custom ? (int)Centers.instance.mazeSizeY : (int)Centers.instance.mazeSize;//(int)Centers.instance.mazeSizeY;
            Maze<NormalBlock> mapSchematic = new Maze<NormalBlock>(Xsize, Ysize, "row", isVoid: false);

            //mapSchematic = getSchementics<NormalBlock>(Xsize,Ysize);

            blockGenerate<NormalBlock>(mapSchematic);

            Centers.instance.startPoint = shuffleData(Centers.instance.centers)[0];
            Centers.instance.endPoint = getRandomDataCircle(Centers.instance.centers, 9 * Mathf.Sqrt(Xsize * Ysize) * Centers.instance.distanceForEnd, Centers.instance.startPoint); // 저기 9는 볼륨임
            Centers.instance.bonusPoints = getRandomData(Centers.instance.centers, Centers.instance.bonusFrequency);
            //foreach (Vector3 vc in Centers.instance.centers)
            //{
            //    Instantiate(wallPrefab[1], vc, wallPrefab[1].transform.rotation, wallParent);
            //}

            Centers.instance.mapGen = true;
            yield return null;
        }

        void blockGenerate<T>(Maze<T> map) where T: blockBase , new()
        {
            int prefabNumber = 0;

            foreach (var item in wallPrefab)
            {
                item.SetActive(true);
            }

            if (new T().Equals(new NormalBlock()))
            {
                prefabNumber = 0;
            }
            else
            {
                prefabNumber = 0;
            }




            var sizeX = map.getSize().Item1;
            var sizeY = map.getSize().Item2;

            var wallX = wallPrefab[prefabNumber].transform.localScale.x;
            var wallZ = wallPrefab[prefabNumber].transform.localScale.z;
            var Volumn = Mathf.Abs(wallX - wallZ);
            var offsetX = new Vector3(Volumn / 2, 0, 0);
            var offsetZ = new Vector3(0, 0, Volumn / 2);

            Centers.instance.topWalls.AddMultiple(null, sizeX * sizeY);
            Centers.instance.leftWalls.AddMultiple(null, sizeX * sizeY);
            Centers.instance.centers.AddMultiple(inf, sizeX * sizeY);

            Coordinate crd = new(map.getSize().Item1);

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    //Debug.Log($"{(x, y)} | {map.blocks[crd.coord(x,y)].family}");
                    
                    if (map.blocks[crd.coord(x, y)].mWallData.topWall || y==0)
                    {
                        Centers.instance.topWalls[crd.coord(x,y)] = Instantiate(wallPrefab[prefabNumber], offsetX + wallPrefab[prefabNumber].transform.position + new Vector3( Volumn * (sizeX - x),0,Volumn * y), wallPrefab[prefabNumber].transform.rotation, wallParent);
                    }

                    if (map.blocks[crd.coord(x, y)].mWallData.sideWall || x==0) // 오른쪽 끝 경계 검토 필요
                    {
                        Centers.instance.leftWalls[crd.coord(x, y)] = Instantiate(wallPrefab[prefabNumber], offsetZ + wallPrefab[prefabNumber].transform.position + new Vector3( Volumn * (sizeX - x + 1), 0, Volumn * y), wallPrefab[prefabNumber].transform.rotation * Quaternion.Euler(new Vector3(0, 90, 0)), wallParent);
                    }

                    Centers.instance.centers[crd.coord(x, y)] = offsetX + offsetZ + wallPrefab[prefabNumber].transform.position + new Vector3(Volumn * (sizeX - x), 0, Volumn * y);

                }
            }

            for(int x = 0; x<sizeX; x++)
            {
                Instantiate(wallPrefab[prefabNumber], offsetX + wallPrefab[prefabNumber].transform.position + new Vector3(Volumn * (sizeX - x), 0, Volumn * sizeY), wallPrefab[prefabNumber].transform.rotation, wallParent);
            }

            for (int y = 0; y < sizeY; y++)
            {
                Instantiate(wallPrefab[prefabNumber], offsetZ + wallPrefab[prefabNumber].transform.position + new Vector3(Volumn, 0, Volumn * y), wallPrefab[prefabNumber].transform.rotation * Quaternion.Euler(new Vector3(0, 90, 0)), wallParent);
            }

            //for (int y = 0; y < sizeY; y++)
            //{
            //    Instantiate(wallPrefab[prefabNumber], offsetZ + wallPrefab[prefabNumber].transform.position + new Vector3(Volumn * (sizeX + 1), 0, Volumn * y), wallPrefab[prefabNumber].transform.rotation * Quaternion.Euler(new Vector3(0, 90, 0)), wallParent);
            //}



        }


        Maze<T> getSchementics<T>(int sizeX, int sizeY) where T : blockBase, new()
        {
            int UX = Random.Range(3, 6);
            int DX = UX;
            int LX = Random.Range(5, sizeX - UX - 5);
            int RX = sizeX - (UX + LX);

            int UY = Random.Range(7, sizeY - 7);
            int DY = sizeY - UY;
            int LY = sizeY;
            int RY = sizeY;

            Maze<T> leftMaze = new Maze<T>(LX, LY, "column", isVoid: false, columnMazeTopWallEilminationRate, columnMazeSideWallEilminationRate);
            Maze<T> rightMaze = new Maze<T>(RX, RY, "column", isVoid: false, columnMazeTopWallEilminationRate, columnMazeSideWallEilminationRate);
            Maze<T> upMaze = new Maze<T>(UX, UY, "row", isVoid: false, rowMazeTopWallEilminationRate, rowMazeSideWallEilminationRate);
            Maze<T> downMaze = new Maze<T>(DX, DY, "row", isVoid: false, rowMazeTopWallEilminationRate, rowMazeSideWallEilminationRate);

            Maze<T> middleMaze = new Maze<T>(UX, sizeY, "row", isVoid: true);

            //leftMaze.removeBoundary();
            //rightMaze.removeBoundary();
            //upMaze.removeBoundary();
            //downMaze.removeBoundary();

            rightMaze.rotate().rotate();
            downMaze.rotate().rotate();

            middleMaze = upMaze + downMaze;
            middleMaze.ForceSetColumn();

            return ((leftMaze + middleMaze) + rightMaze);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="radius">this radius follow the unity's coordinate, not the custom coordinate or etc... </param>
        /// <param name="_center"> if you want to using random center system, you should set _center to inf that Vector3.positiveInfinity equals</param>
        /// <returns></returns>
        Vector3 getRandomDataCircle(List<Vector3> data, float radius, Vector3 _center)
        {
            Vector3 center = new Vector3();

            if (center == inf)
                center = shuffleData(data)[0];
            else
                center = _center;

            List<Vector3> aroundData = new List<Vector3>();

            foreach (Vector3 point in data)
            {
                if (Vector3.Distance(point, center) <= radius)
                {
                    aroundData.Add(point);
                }
            }

            return shuffleData<Vector3>(aroundData)[0];


        }

        //================= 

        // 입력받은 임의의 칸(좌표)들 중에서 임의의 점을 전체 중 
        //입력받은 임의의 rate 만큼 뽑아서 리스트 형태로 
        //출력하는 함수 
        List<T> getRandomData<T>(List<T> data, float rate = 1.0f)
        {
            List<T> result = new List<T>(data); // 입력 데이터를 변경하지 않고 복사본을 생성

            result = shuffleData(result); // 데이터를 섞음

            int elementsToKeep = (int)(result.Count * rate);

            // 삭제하는 대신 유지할 요소만 선택하여 반환
            return result.GetRange(0, elementsToKeep);


        }

        //================= 
        List<T> shuffleData<T>(List<T> data)
        {
            return data.OrderBy(x => Random.Range(0, int.MaxValue) % (10000 + data.Count)).ToList();
        }

        //=================

        //=================

        //================= 

        // 벽 생성하면서 MapGenerator 전역변수로 좌표들 수집 한 
        //다음에 위의 함수들을 이용하여 "미로 요소" 생성 변수 실행


        //void wallPrefabGenerate(bool[,,] maze)
        //{
        //    bool[,] topWalls = new bool[maze.GetLength(0), maze.GetLength(1)];
        //    bool[,] leftWalls = new bool[maze.GetLength(0), maze.GetLength(1)];

        //    var sizeVolumn = wallPrefab.transform.localScale.x - wallPrefab.transform.localScale.z;
        //    var offsetX = new Vector3(sizeVolumn / 2, 0, 0);
        //    var offsetZ = new Vector3(0, 0, sizeVolumn / 2);

        //    for (int i = 0; i < maze.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < maze.GetLength(1); j++)
        //        {
        //            topWalls[i, j] = maze[i, j, 0];
        //            leftWalls[i, j] = maze[i, j, 1];
        //        }
        //    }

        //    //0a
        //    for (int i = 0; i < topWalls.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < topWalls.GetLength(1); j++)
        //        {
        //            if (topWalls[i, j])
        //                Instantiate(wallPrefab, offsetX + new Vector3(sizeVolumn * j, 0, sizeVolumn * i), wallPrefab.transform.rotation, wallParent);
        //        }
        //    }

        //    //90a
        //    for (int i = 0; i < leftWalls.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < leftWalls.GetLength(1); j++)
        //        {
        //            if (leftWalls[i, j])
        //                Instantiate(wallPrefab, offsetZ + new Vector3(sizeVolumn * j, 0, sizeVolumn * i), wallPrefab.transform.rotation * Quaternion.Euler(new Vector3(0, 90, 0)), wallParent);
        //        }
        //    }

        // 좌표 , 각도
        // 각도             시작                             직렬방향                                           병렬방향
        // 0a -> ((size.X - size.Z)/2, 0, 0)   ( original + (size.X - size.Z), 0 , 0)          (0, 0, original + (size.X - size.Z))
        // 90a -> (0,0,size.X/2 - size.Z/2)    (0, 0, original + (size.X - size.Z))            ( original + (size.X - size.Z), 0 , 0)

        //}

        // 한방향으로만 주로 길쭉한게 아니꼬와서 미로 여러개 이어붙힘
        //void getSchementics(ref bool[,,] maze, int X_length, int Y_length)
        //{
        //    X_length += 2; // 결합 시 손실을 고려함 I consider the Loss when merging indexes

        //    int connectionPartColumnLength = Random.Range(3, 6);
        //    int rightPartColumnLength = Random.Range(5, X_length - connectionPartColumnLength - 5);
        //    int leftPartColumnLength = X_length - (rightPartColumnLength + connectionPartColumnLength);

        //    int upPartRowLength = Random.Range(3, Y_length - 3);
        //    int downPartRowLength = Y_length - upPartRowLength; //I consider the Loss when merging indexes

        //    sideWallEilminationRate = horizontalSideWallEilminationRate;
        //    downWallEilminationRate = horizontalDownWallEilminationRate;

        //    bool?[,] originalMaze = typecast_Blocklization(formalizatedMaze_Inside_WallSchematic(Y_length, leftPartColumnLength));
        //    bool?[,] yAxisSymMaze = typecast_Blocklization(formalizatedMaze_Inside_WallSchematic(Y_length, rightPartColumnLength));

        //    sideWallEilminationRate = verticalSideWallEilminationRate;
        //    downWallEilminationRate = verticalDownWallEilminationRate;

        //    bool?[,] angle90rotateMaze = typecast_Blocklization(formalizatedMaze_Inside_WallSchematic(connectionPartColumnLength, upPartRowLength));
        //    bool?[,] yAxisSymPlusAngle90rotateMaze = typecast_Blocklization(formalizatedMaze_Inside_WallSchematic(connectionPartColumnLength, downPartRowLength));

        //    Debug.Log(debuggingDoubleWallDataMaze(originalMaze));
        //    Debug.Log(debuggingDoubleWallDataMaze(yAxisSymMaze));
        //    Debug.Log(debuggingDoubleWallDataMaze(angle90rotateMaze));
        //    Debug.Log(debuggingDoubleWallDataMaze(yAxisSymPlusAngle90rotateMaze));

        //    //Debug.Log(debuggingDoubleWallDataMaze(angle90rotateMaze));
        //    //Debug.Log(debuggingDoubleWallDataMaze(yAxisSymPlusAngle90rotateMaze));

        //    // before amend
        //    //bool?[,] originalMaze = typecast_Blocklization(formalizatedMaze_Inside_WallSchematic(Y_length, leftPartColumnLength));
        //    //bool?[,] yAxisSymMaze = typecast_Blocklization(formalizatedMaze_Inside_WallSchematic(Y_length, rightPartColumnLength));
        //    //bool?[,] angle90rotateMaze = typecast_Blocklization(formalizatedMaze_Inside_WallSchematic(upPartRowLength, connectionPartColumnLength));
        //    //bool?[,] yAxisSymPlusAngle90rotateMaze = typecast_Blocklization(formalizatedMaze_Inside_WallSchematic(downPartRowLength, connectionPartColumnLength));

        //    bool[,] ConnectionPart;
        //    bool[,] mergedPart_1;
        //    bool[,] blocklizatedMaze;

        //    Matrix.Array2D_y_SymmetricTranspositon<bool?>(ref yAxisSymMaze); progress = 1f / 11f;
        //    Matrix.Array2D_clockwise_90_rotationalTransposition<bool?>(ref angle90rotateMaze); progress = 2f / 11f;
        //    Matrix.Array2D_y_SymmetricTranspositon<bool?>(ref yAxisSymPlusAngle90rotateMaze); progress = 3f / 11f;
        //    Matrix.Array2D_clockwise_90_rotationalTransposition<bool?>(ref yAxisSymPlusAngle90rotateMaze); progress = 4f / 11f;

        //    mazeCleanup(ref yAxisSymMaze); progress = 5f / 11f;
        //    mazeCleanup(ref angle90rotateMaze); progress = 6f / 11f;
        //    mazeCleanup(ref yAxisSymPlusAngle90rotateMaze); progress = 7f / 11f;


        //    bool[,] originalMazeNotNull = Enumerable.Range(0, originalMaze.GetLength(0)).Select(row => Enumerable.Range(0, originalMaze.GetLength(1)).Select(col => originalMaze[row, col] ?? false).ToArray()).ToArray().ToTwoDimensionalArray();
        //    bool[,] yAxisSymMazeNotNull = Enumerable.Range(0, yAxisSymMaze.GetLength(0)).Select(row => Enumerable.Range(0, yAxisSymMaze.GetLength(1)).Select(col => yAxisSymMaze[row, col] ?? false).ToArray()).ToArray().ToTwoDimensionalArray();
        //    bool[,] angle90rotateMazeNotNull = Enumerable.Range(0, angle90rotateMaze.GetLength(0)).Select(row => Enumerable.Range(0, angle90rotateMaze.GetLength(1)).Select(col => angle90rotateMaze[row, col] ?? false).ToArray()).ToArray().ToTwoDimensionalArray();
        //    bool[,] yAxisSymPlusAngle90rotateMazeNotNull = Enumerable.Range(0, yAxisSymPlusAngle90rotateMaze.GetLength(0)).Select(row => Enumerable.Range(0, yAxisSymPlusAngle90rotateMaze.GetLength(1)).Select(col => yAxisSymPlusAngle90rotateMaze[row, col] ?? false).ToArray()).ToArray().ToTwoDimensionalArray();
        //    // feat. Chat GPT
        //    progress = 8f / 11f;

        //    // Debug.Log(debuggingDoubleWallDataMaze(angle90rotateMaze));
        //    //Debug.Log(debuggingDoubleWallDataMaze(yAxisSymPlusAngle90rotateMazeNotNull));

        //    ConnectionPart = Matrix.booleanArray2D_merge(angle90rotateMazeNotNull, yAxisSymPlusAngle90rotateMazeNotNull, Matrix.Direction.Upward, 0); progress = 9f / 11f;

        //    //Debug.Log(debuggingDoubleWallDataMaze(ConnectionPart));

        //    mergedPart_1 = Matrix.booleanArray2D_merge(ConnectionPart, originalMazeNotNull, Matrix.Direction.Rightward, 0); progress = 10f / 11f;


        //    //Debug.Log(debuggingDoubleWallDataMaze(mergedPart_1));

        //    blocklizatedMaze = Matrix.booleanArray2D_merge(mergedPart_1, yAxisSymMazeNotNull, Matrix.Direction.Leftward, 0); progress = 1f;
        //    //mazeCleanup(ref blocklizatedMaze);
        //    maze = typecast_Wallization(blocklizatedMaze);

        //}


        //void mazeCleanup(ref bool?[,] _data)
        //{
        //    bool[,] processingCleanedupData = new bool[_data.GetLength(0) + 2, _data.GetLength(1) + 2];
        //    // 1: up-left , 2: up-right , 3: down-left, 4: down-right

        //    bool[,] data = new bool[_data.GetLength(0), _data.GetLength(1)];

        //    bool?[,] cleanedupData = new bool?[_data.GetLength(0), _data.GetLength(1)];

        //    for (int i = 0; i < data.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < data.GetLength(1); j++)
        //        {
        //            data[i, j] = _data[i, j] ?? false;
        //        }
        //    }

        //    for (int i = 0; i < data.GetLength(0) + 2; i++)
        //    {
        //        for (int j = 0; j < data.GetLength(1) + 2; j++)
        //        {
        //            processingCleanedupData[i, j] = false;
        //        }
        //    }

        //    //clean up
        //    for (int i = 0; i < data.GetLength(0) / 2; i++)
        //    {
        //        for (int j = 0; j < data.GetLength(1) / 2; j++)
        //        {

        //            // 1-3
        //            if (/*1*/data[2 * i, 2 * j] && /*2*/!data[2 * i, 2 * j + 1] && /*3*/data[2 * i + 1, 2 * j] && /*4*/!data[2 * i + 1, 2 * j + 1])
        //            {
        //                processingCleanedupData[2 * i, 2 * j] = true;
        //                processingCleanedupData[2 * i + 1, 2 * j] = true;
        //            }

        //            // 1-2
        //            if (/*1*/data[2 * i, 2 * j] && /*2*/data[2 * i, 2 * j + 1] && /*3*/!data[2 * i + 1, 2 * j] && /*4*/!data[2 * i + 1, 2 * j + 1])
        //            {
        //                processingCleanedupData[2 * i, 2 * j] = true;
        //                processingCleanedupData[2 * i, 2 * j + 1] = true;
        //            }
        //            // 2-4
        //            if (/*1*/!data[2 * i, 2 * j] && /*2*/data[2 * i, 2 * j + 1] && /*3*/!data[2 * i + 1, 2 * j] && /*4*/data[2 * i + 1, 2 * j + 1])
        //            {
        //                processingCleanedupData[2 * i, 2 * j + 2] = true;
        //                processingCleanedupData[2 * i + 1, 2 * j + 2] = true;
        //            }
        //            // 3-4
        //            if (/*1*/!data[2 * i, 2 * j] && /*2*/!data[2 * i, 2 * j + 1] && /*3*/data[2 * i + 1, 2 * j] && /*4*/data[2 * i + 1, 2 * j + 1])
        //            {
        //                processingCleanedupData[2 * i + 2, 2 * j] = true;
        //                processingCleanedupData[2 * i + 2, 2 * j + 1] = true;
        //            }

        //            // 1-2-3
        //            if (/*1*/data[2 * i, 2 * j] && /*2*/data[2 * i, 2 * j + 1] && /*3*/data[2 * i + 1, 2 * j] && /*4*/!data[2 * i + 1, 2 * j + 1])
        //            {
        //                processingCleanedupData[2 * i, 2 * j] = true;
        //                processingCleanedupData[2 * i + 1, 2 * j] = true;
        //                processingCleanedupData[2 * i, 2 * j + 1] = true;
        //            }

        //            // 1-2-4
        //            if (/*1*/data[2 * i, 2 * j] && /*2*/data[2 * i, 2 * j + 1] && /*3*/!data[2 * i + 1, 2 * j] && /*4*/data[2 * i + 1, 2 * j + 1])
        //            {
        //                processingCleanedupData[2 * i, 2 * j] = true;
        //                processingCleanedupData[2 * i, 2 * j + 1] = true;
        //                processingCleanedupData[2 * i, 2 * j + 2] = true;
        //                processingCleanedupData[2 * i + 1, 2 * j + 2] = true;
        //            }

        //            // 2-3-4
        //            if (/*1*/!data[2 * i, 2 * j] && /*2*/data[2 * i, 2 * j + 1] && /*3*/data[2 * i + 1, 2 * j] && /*4*/data[2 * i + 1, 2 * j + 1])
        //            {
        //                processingCleanedupData[2 * i, 2 * j + 2] = true;
        //                processingCleanedupData[2 * i + 1, 2 * j + 2] = true;
        //                processingCleanedupData[2 * i + 2, 2 * j] = true;
        //                processingCleanedupData[2 * i + 2, 2 * j + 1] = true;
        //            }

        //            // 1-3-4
        //            if (/*1*/data[2 * i, 2 * j] && /*2*/!data[2 * i, 2 * j + 1] && /*3*/data[2 * i + 1, 2 * j] && /*4*/data[2 * i + 1, 2 * j + 1])
        //            {
        //                processingCleanedupData[2 * i, 2 * j] = true;
        //                processingCleanedupData[2 * i + 1, 2 * j] = true;
        //                processingCleanedupData[2 * i + 2, 2 * j] = true;
        //                processingCleanedupData[2 * i + 2, 2 * j + 1] = true;

        //            }

        //        }
        //    }

        //    for (int i = 0; i < data.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < data.GetLength(1); j++)
        //        {
        //            cleanedupData[i, j] = processingCleanedupData[i, j];
        //        }
        //    }

        //    _data = cleanedupData;
        //}

        ////overload
        //void mazeCleanup(ref bool[,] _data)
        //{
        //    bool[,] processingCleanedupData = new bool[_data.GetLength(0) + 2, _data.GetLength(1) + 2];
        //    // 1: up-left , 2: up-right , 3: down-left, 4: down-right

        //    bool[,] data = new bool[_data.GetLength(0), _data.GetLength(1)];

        //    bool[,] cleanedupData = new bool[_data.GetLength(0), _data.GetLength(1)];

        //    for (int i = 0; i < data.GetLength(0) + 2; i++)
        //    {
        //        for (int j = 0; j < data.GetLength(1) + 2; j++)
        //        {
        //            processingCleanedupData[i, j] = false;
        //        }
        //    }

        //    //clean up
        //    for (int i = 0; i < data.GetLength(0) / 2; i++)
        //    {
        //        for (int j = 0; j < data.GetLength(1) / 2; j++)
        //        {

        //            // 1-3
        //            if (/*1*/data[2 * i, 2 * j] && /*2*/!data[2 * i, 2 * j + 1] && /*3*/data[2 * i + 1, 2 * j] && /*4*/!data[2 * i + 1, 2 * j + 1])
        //            {
        //                processingCleanedupData[2 * i, 2 * j] = true;
        //                processingCleanedupData[2 * i + 1, 2 * j] = true;
        //            }

        //            // 1-2
        //            if (/*1*/data[2 * i, 2 * j] && /*2*/data[2 * i, 2 * j + 1] && /*3*/!data[2 * i + 1, 2 * j] && /*4*/!data[2 * i + 1, 2 * j + 1])
        //            {
        //                processingCleanedupData[2 * i, 2 * j] = true;
        //                processingCleanedupData[2 * i, 2 * j + 1] = true;
        //            }
        //            // 2-4
        //            if (/*1*/!data[2 * i, 2 * j] && /*2*/data[2 * i, 2 * j + 1] && /*3*/!data[2 * i + 1, 2 * j] && /*4*/data[2 * i + 1, 2 * j + 1])
        //            {
        //                processingCleanedupData[2 * i, 2 * j + 2] = true;
        //                processingCleanedupData[2 * i + 1, 2 * j + 2] = true;
        //            }
        //            // 3-4
        //            if (/*1*/!data[2 * i, 2 * j] && /*2*/!data[2 * i, 2 * j + 1] && /*3*/data[2 * i + 1, 2 * j] && /*4*/data[2 * i + 1, 2 * j + 1])
        //            {
        //                processingCleanedupData[2 * i + 2, 2 * j] = true;
        //                processingCleanedupData[2 * i + 2, 2 * j + 1] = true;
        //            }

        //            // 1-2-3
        //            if (/*1*/data[2 * i, 2 * j] && /*2*/data[2 * i, 2 * j + 1] && /*3*/data[2 * i + 1, 2 * j] && /*4*/!data[2 * i + 1, 2 * j + 1])
        //            {
        //                processingCleanedupData[2 * i, 2 * j] = true;
        //                processingCleanedupData[2 * i + 1, 2 * j] = true;
        //                processingCleanedupData[2 * i, 2 * j + 1] = true;
        //            }

        //            // 1-2-4
        //            if (/*1*/data[2 * i, 2 * j] && /*2*/data[2 * i, 2 * j + 1] && /*3*/!data[2 * i + 1, 2 * j] && /*4*/data[2 * i + 1, 2 * j + 1])
        //            {
        //                processingCleanedupData[2 * i, 2 * j] = true;
        //                processingCleanedupData[2 * i, 2 * j + 1] = true;
        //                processingCleanedupData[2 * i, 2 * j + 2] = true;
        //                processingCleanedupData[2 * i + 1, 2 * j + 2] = true;
        //            }

        //            // 2-3-4
        //            if (/*1*/!data[2 * i, 2 * j] && /*2*/data[2 * i, 2 * j + 1] && /*3*/data[2 * i + 1, 2 * j] && /*4*/data[2 * i + 1, 2 * j + 1])
        //            {
        //                processingCleanedupData[2 * i, 2 * j + 2] = true;
        //                processingCleanedupData[2 * i + 1, 2 * j + 2] = true;
        //                processingCleanedupData[2 * i + 2, 2 * j] = true;
        //                processingCleanedupData[2 * i + 2, 2 * j + 1] = true;
        //            }

        //            // 1-3-4
        //            if (/*1*/data[2 * i, 2 * j] && /*2*/!data[2 * i, 2 * j + 1] && /*3*/data[2 * i + 1, 2 * j] && /*4*/data[2 * i + 1, 2 * j + 1])
        //            {
        //                processingCleanedupData[2 * i, 2 * j] = true;
        //                processingCleanedupData[2 * i + 1, 2 * j] = true;
        //                processingCleanedupData[2 * i + 2, 2 * j] = true;
        //                processingCleanedupData[2 * i + 2, 2 * j + 1] = true;

        //            }

        //        }
        //    }

        //    for (int i = 0; i < data.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < data.GetLength(1); j++)
        //        {
        //            cleanedupData[i, j] = processingCleanedupData[i, j];
        //        }
        //    }

        //    _data = cleanedupData;
        //}


        //// 미로 시각화 함수
        //string debuggingDoubleWallDataMaze(bool?[,] maze)
        //{
        //    string output = "";

        //    for (int i = 0; i < maze.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < maze.GetLength(1); j++)
        //        {
        //            if (maze[i, j] == null)
        //                output += "⬛";
        //            else
        //                output += (bool)maze[i, j] ? "⬜" : "🟦";
        //        }

        //        output += "\n";

        //    }



        //    return output;
        //}
        //string debuggingDoubleWallDataMaze(bool[,] maze)
        //{
        //    string output = "";

        //    for (int i = 0; i < maze.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < maze.GetLength(1); j++)
        //        {
        //            output += (bool)maze[i, j] ? "⬜" : "🟦";
        //        }

        //        output += "\n";

        //    }



        //    return output;
        //}

        //// [x,y,2] 형태의 벽을 위주로 저장하는 배열을 [2*x,2*y] 형태의 통로 또한 명시적으로 표현해주는 배열로 바꾸는 함수
        //bool?[,] typecast_Blocklization(bool?[,,] data) // [x,y,2] -> [x,y] on the other words, two walls in each blocks become individual nine blocks
        //{
        //    // 배열 크기 감지
        //    int dataXsize = data.GetLength(0);
        //    int dataYsize = data.GetLength(1);

        //    bool?[,] output = new bool?[2 * dataXsize, 2 * dataYsize];

        //    for (int i = 0; i < dataXsize; i++)
        //    {
        //        for (int j = 0; j < dataYsize; j++)
        //        {

        //            //output[2*i + 1, 2*j + 1] is middle(parent) block (5 rightward and downward blocks overlaped other middle block's children) hint: reversed x to y
        //            if (data[i, j, 0] is null)
        //            {
        //                output[2 * i, 2 * j] = null;
        //                output[2 * i + 1, 2 * j] = null;
        //                output[2 * i, 2 * j + 1] = null;
        //                output[2 * i + 1, 2 * j + 1] = null;
        //            }
        //            else
        //            {
        //                output[2 * i, 2 * j] = (data[i, j, 0] ?? false) || (data[i, j, 1] ?? false);

        //                output[2 * i + 1, 2 * j] = data[i, j, 0] ?? false;

        //                output[2 * i, 2 * j + 1] = data[i, j, 1] ?? false;

        //                output[2 * i + 1, 2 * j + 1] = false;
        //            }



        //        }
        //    }

        //    return output;

        //}

        //bool[,] typecast_Blocklization(bool[,,] data) // [x,y,2] -> [x,y] on the other words, two walls in each blocks become individual nine blocks
        //{
        //    // 배열 크기 감지
        //    int dataXsize = data.GetLength(0);
        //    int dataYsize = data.GetLength(1);

        //    bool[,] output = new bool[2 * dataXsize, 2 * dataYsize];

        //    for (int i = 0; i < dataXsize; i++)
        //    {
        //        for (int j = 0; j < dataYsize; j++)
        //        {

        //            //output[2*i + 1, 2*j + 1] is middle(parent) block (5 rightward and downward blocks overlaped other middle block's children) hint: reversed x to y
        //            output[2 * i, 2 * j] = data[i, j, 0] || data[i, j, 1];

        //            output[2 * i + 1, 2 * j] = data[i, j, 0];

        //            output[2 * i, 2 * j + 1] = data[i, j, 1];

        //            output[2 * i + 1, 2 * j + 1] = false;

        //        }
        //    }

        //    return output;

        //}

        ////안 씀
        //bool[,,] typecast_Wallization(bool[,] data) // [x,y] -> [x,y,2] on the other words, nine blocks near by become one block that has two walls 
        //{
        //    int dataXsize = data.GetLength(0) / 2;
        //    int dataYsize = data.GetLength(1) / 2;

        //    bool[,,] output = new bool[dataXsize, dataYsize, 2];

        //    for (int i = 0; i < dataXsize; i++)
        //    {
        //        for (int j = 0; j < dataYsize; j++)
        //        {
        //            output[i, j, 0] = data[2 * i + 1, 2 * j];
        //            output[i, j, 1] = data[2 * i, 2 * j + 1];
        //        }
        //    }

        //    return output;

        //}



    }

}

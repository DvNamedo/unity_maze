using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



namespace try2
{

    public class MapGenerator : MonoBehaviour
    {
        [Header("Loading Bar Info")]
        public Text stateMassage;
        public Image prograssBar;

        [Header("Prefep Setting for Instantiate wall")]
        public GameObject wallPrefap;

        //수직, 수평에서의 벽 제거 확률 변수
        [Header("Wall Eilmination Probability")]
        [SerializeField]
        float sideWallEilminationRate = 0.75f;
        [SerializeField]
        float downWallEilminationRate = 0.25f;

        private byte?[,] mapSchematicArray;
        int sizeofX;

        //formalizatedMaze_Inside_WallSchematic(int,int) 에서 미로를 생성하기 위해 미로 구역을 벽에 따라 나눈 블록의 실체를 저장하는 리스트
        static List<blockBase> blocks = new List<blockBase>();

        // 미로 구역을 벽에 따라 나눈 구역 중 한 부분
        class blockBase
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
            public blockBase(uint _family, bool top, bool side)
            {
                // 값 입력부
                mWallData.topWall = top;
                mWallData.sideWall = side;

                family = _family;

            }

        }

        private void Start()
        {

            //싱글톤에서 미로 크기 값을 받아오는 부분, 현재는 사실상 안쓰임
            mapSchematicArray = new byte?[Centers.instance.mazeSize, Centers.instance.mazeSize];
            //Debug.Log(string.Join(",", formalizatedMaze_Inside_WallSchematic(20, 20)));
            //Centers.instance.Print3D_Array<bool>(formalizatedMaze_Inside_WallSchematic(40, 40));
            //Debug.Log(debuggingDoubleWallDataMaze(formalizatedMaze_Inside_WallSchematic(30, 30)));
            
            // bool?[,,] 형태의 미로를 시각화시키는 함수를 로그창에 띄우는 부분
            Debug.Log(debuggingDoubleWallDataMaze(typecast_Blocklization(formalizatedMaze_Inside_WallSchematic(30, 30))));
            // [[],[],[]

            //Debug.Log(debuggingDoubleWallDataMaze(typecast_Blocklization(mazeTypecast(blocks, size_X, size_Y))); the source to use debuging map 

        }

        void setSchementics()
        {

        }

        // 미로 시각화 함수
        string debuggingDoubleWallDataMaze(bool?[,] maze)
        {
            string output = "";

            for (int i = 0; i < maze.GetLength(0); i++)
            {
                for (int j = 0; j < maze.GetLength(1); j++)
                {
                    if (maze[i, j] == null)
                        output += "⬛";
                    else
                        output += (bool)maze[i, j] ? "⬜" : "🟦";
                }

                output += "\n";

            }



            return output;
        }

        // [x,y,2] 형태의 벽을 위주로 저장하는 배열을 [2*x,2*y] 형태의 통로 또한 명시적으로 표현해주는 배열로 바꾸는 함수
        bool?[,] typecast_Blocklization(bool?[,,] data) // [x,y,2] -> [x,y] on the other words, two walls in each blocks become individual nine blocks
        {
            // 배열 크기 감지
            int dataXsize = data.GetLength(0);
            int dataYsize = data.GetLength(1);

            bool?[,] output = new bool?[2 * dataXsize + 1, 2 * dataYsize + 1];

            for (int i = 0; i < dataXsize; i++)
            {
                for (int j = 0; j < dataYsize; j++)
                {

                    //output[2*i + 1, 2*j + 1] is middle(parent) block (5 rightward and downward blocks overlaped other middle block's children) hint: reversed x to y
                    if(data[i,j,0] is null)
                    {
                        output[2 * i, 2 * j] = null;
                        output[2 * i + 1, 2 * j] = null;
                        output[2 * i, 2 * j + 1] = null;
                        output[2 * i + 1, 2 * j + 1] = null;
                    }
                    else
                    {
                        output[2 * i, 2 * j] = (data[i, j, 0] ?? false) || (data[i, j, 1] ?? false);

                        output[2 * i + 1, 2 * j] = data[i, j, 0] ?? false;

                        output[2 * i, 2 * j + 1] = data[i, j, 1] ?? false;

                        output[2 * i + 1, 2 * j + 1] = false;
                    }



                }
            }

            return output;

        }

        //안 씀
        bool[,,] typecast_Wallization(bool[,] data) // [x,y] -> [x,y,2] on the other words, nine blocks near by become one block that has two walls 
        {
            int dataXsize = (data.GetLength(0) - 1) / 2;
            int dataYsize = (data.GetLength(1) - 1) / 2;

            bool[,,] output = new bool[dataXsize, dataYsize, 2];

            for (int i = 0; i < dataXsize; i++)
            {
                for (int j = 0; j < dataYsize; j++)
                {
                    output[i, j, 0] = data[2 * i + 1, 2 * j];
                    output[i, j, 1] = data[2 * i, 2 * j + 1];
                }
            }

            return output;

        }

        // 오브젝트 까지 표시하기에 너무 길어서 간소화 하고자 쓴 델리게이트. 의미 없는 코드
        private delegate int Coord(int i, int j);

        /// <summary>
        /// it return Normal maze Array that each dimension is x coordination, y coordination and wall direction
        /// </summary>
        /// <param name="size_X">X coordination size</param>
        /// <param name="size_Y">Y coordination size</param>
        /// <returns></returns>
        bool?[,,] formalizatedMaze_Inside_WallSchematic(int size_X, int size_Y)
        {
            size_X = Mathf.Abs(size_X);
            size_Y = Mathf.Abs(size_Y);

            // 좌표
            Coordinate coordinate = new Coordinate(size_X);

            Coord coord = new Coord(coordinate.coord);

            // blocks 초기화
            for (int i = 0; i < coord(size_X - 1, size_Y - 1) + 1; i++)
            {
                blocks.Add(null);
            }

            // 중간 디버깅
            Debug.Log(debuggingDoubleWallDataMaze(typecast_Blocklization(mazeTypecast(blocks, size_X, size_Y))));
            
            // Create new blocks where none exist
            // After randomly removing the wall, the family value of all blocks that have the family value of the block to be integrated into the previous block is changed to the family value of the previous block.

            //각 행에 대하여 한번씩 돌되, 마지막 행은 따로 처리해줘야 하므로 마지막 전 행까지 돌아주는 코드
            for (int column = 0; column < size_Y - 1; column++)
            {
                // 행이 정해진 상태에서 각 열에서의 값을 null 이 아닐떄 blockBase 로 초기화 해주는 부분
                for (int row = 0; row < size_X; row++)
                {
                    //Debug.Log($"setblock in ({row},{column})");
                    blocks[coord(row, column)] = blocks[coord(row, column)] ?? new blockBase((uint)coord(row, column), top: true, side: true);

                }

                Debug.Log(debuggingDoubleWallDataMaze(typecast_Blocklization(mazeTypecast(blocks, size_X, size_Y))));

                //horizontal wall eliminating

                // 맵 끝부분인 첫번 째 열 부분은 벽이 무조건 존재해야 하므로 건들여도 의미 없는 정보이므로 건들지 않음. (맵의 최외각은 추후 따로 처리됨(이 코드에서는 작성 안된부분))
                for (int i = 0; i < size_X - 1; i++)
                {
                    if (Random.value < sideWallEilminationRate)
                    {
                        //벽이 삭제된다면 그 삭제된 쪽의 구역들은 전부 연결되게 되므로 연결되었다는 표시인 family 값이 같은 모든 blocks의 원소에 대하여 연결 시킨 원소의 family 값으로 통일 시키는 코드
                        foreach (int myFamily in listFindAtFamilyValue(blocks[coord(i + 1, column)].family))
                        {
                            blocks[myFamily].family = blocks[coord(i, column)].family;
                        }

                        // 왼쪽 벽 없앰을 표기하는 코드
                        blocks[coord(i + 1, column)].mWallData.sideWall = false;

                    }
                }

                Debug.Log(debuggingDoubleWallDataMaze(typecast_Blocklization(mazeTypecast(blocks, size_X, size_Y))));

                // virtical wall Eliminating ================================================================================================
                
                // 모든 연결된 각각의 큰 영역(family) 의 값들을 순서 없이 저장 하기 위한 family들의 집합, 즉 마을(town)
                HashSet<uint> townList = new HashSet<uint>();

                // 각각의 영역에 대하여 적어도 하나는 수직한 통로가 존재해야 하므로 한번 수정 되었는가를 표기하기 위한 지역 변수
                bool isModified = false;

                // 마을(town) 리스트에 모든 가족(family)을 등재
                for (int row = 0; row < size_X; row++)
                {
                    townList.Add(blocks[coord(row, column)].family);
                }

                // 마을에 등재된 모든 가족에 대하여 확인함
                foreach (uint eachFamily in townList)
                {
                    // 각각의 가족에 대한 수직통로가 있는가를 초기화
                    isModified = false;

                    // 그 가족구성원들(blocks 의 원소)을 저장하는 리스트
                    List<int> familyList = new List<int>();

                    for (int row = 0; row < size_X; row++)
                    {
                        if (blocks[coord(row, column)].family.Equals(eachFamily))
                        {
                            familyList.Add(row);
                        }
                    }


                    // 랜덤으로 각 가족구성원들에 대하여 수직통로를 생성
                    foreach (int fam in familyList)
                    {
                        if (Random.value <= downWallEilminationRate / Mathf.Clamp(familyList.Count, 1, 8))
                        {
                            Debug.Log("randomModified");
                            isModified = true;

                            //처음부터 같은 가족값을 같도록 생성
                            blocks[coord(fam, column + 1)] = new blockBase(eachFamily, top: false, side: true);
                        }
                    }

                    // 가족 구성원 중 적어도 하나는 얻도록 강제
                    if (!isModified)
                    {
                        //neccessary modified 
                        Debug.Log("nec-Modified{}");
                        int randomFam = familyList[Random.Range(0, familyList.Count - 1)];
                        Debug.Log("nec-Modified{coord(familyList[randomFam])}");
                        blocks[coord(randomFam, column + 1)] = new blockBase(eachFamily, top: false, side: true);
                    }


                }

                Debug.Log(debuggingDoubleWallDataMaze(typecast_Blocklization(mazeTypecast(blocks, size_X, size_Y))));

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

            // 마지막 줄은 전부 연결되어, 모든 가족들이 하나의 가족으로 통일 되도록 해야하므로(즉, 전부 연결되어야 하므로) 특수한 처리를 해줌
            //end process
            for (int row = 0; row < size_X; row++)
            {
                if (blocks[coord(row, size_Y - 1)] == null)
                {
                    blocks[coord(row, size_Y - 1)] = new blockBase((uint)coord(row, size_Y - 1), top: true, side: true);
                }
            }

            for (int i = 0; i < size_X - 1; i++)
            {
                blocks[coord(i + 1, size_Y - 1)].mWallData.sideWall = false;
            }

            // type Casting
            // 출력부
            return mazeTypecast(blocks, size_X, size_Y);


        }
        //=============================================================

        // List<blockBase> 형태를 출력을 위해 bool?[x,y,2] 형태로 변환하는 부분
        bool?[,,] mazeTypecast(List<blockBase> b, int X, int Y)
        {
            bool?[,,] typeCastedArray = new bool?[X, Y, 2]; // [0]:width [1]: height
            Coordinate crd = new Coordinate(X);

            for (int j = 0; j < Y; j++)
            {
                for (int i = 0; i < X; i++)
                {
                    typeCastedArray[i, j, 0] = b[crd.coord(i, j)]  is null ? null : b[crd.coord(i, j)].mWallData.topWall;
                    typeCastedArray[i, j, 1] = b[crd.coord(i, j)]  is null ? null : b[crd.coord(i, j)].mWallData.sideWall;
                    // + 
                }
            }

            // 맵의 가해 부분을 제거하려는 시도였으나 약간의 문제가 있어보여 일시적으로 주석처리
            //bool?[,,] sideCutArray = new bool?[X - 1, Y - 1, 2];

            //for (int i = 0; i < X-1; i++)
            //{
            //    for (int j = 0; j < Y-1; j++)
            //    {
            //        for (int k = 0; k < 2; k++) // non-meaning-practice
            //        {
            //            sideCutArray[i, j, k] = typeCastedArray[i + 1, j + 1, k];
            //        }

            //    }
            //}


            //return sideCutArray;
            return typeCastedArray;
        }

        // 같은 family 값을 같는 모든 blocks 안의 원소들을 인덱스 순서가 빠른 순으로 리스트 형태로 출력하는 함수
        List<int> listFindAtFamilyValue(uint _family)
        {

            List<int> coords = new List<int>();

            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[i] == null)
                    return coords;

                if (blocks[i].family.Equals(_family))
                    coords.Add(i);
            }

            return coords;
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
                x = x % sizeofX;
            }
            return x + y * sizeofX;
        }
    }

}

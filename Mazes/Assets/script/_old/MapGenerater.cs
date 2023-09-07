using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*

namespace try2
{

    public class MapGenerater : MonoBehaviour
    {
        [Header("Loading Bar Info")]
        public Text stateMassage;
        public Image prograssBar;

        [Header("Prefep Setting for Instantiate wall")]
        public GameObject wallPrefap;

        [Header("Wall Eilmination Probability")]
        [SerializeField]
        float sideWallEilminationRate = 0.75f;
        [SerializeField]
        float downWallEilminationRate = 0.25f;

        private byte[,] mapSchematicArray;
        int sizeofX;

        static List<blockBase> blocks = new List<blockBase>();

        class blockBase
        {
            /// <summary>
            /// uint.MaxValue implies meaning of 'nullValue' in this variable
            /// </summary>

            // The set to which the block belongs
            public uint family;
            // Wall of the block
            public struct _wallData
            {
                public bool topWall;
                public bool sideWall; //it usually refer to left wall

                public _wallData(bool t, bool s)
                {
                    topWall = t;
                    sideWall = s;
                }

            }

            public _wallData mWallData;


            /// <summary>
            /// It's an object for the structuralization of maze walls
            /// </summary>
            /// <param name="_family">Set for data proccessing</param>
            /// <param name="walls">Follow the formalities: "(bool,bool)" , Do Not here allow Blank.</param>
            public blockBase(uint _family, bool top, bool side)
            {
                mWallData.topWall = top;
                mWallData.sideWall = side;

                family = _family;

            }

        }

        private void Start()
        {
            mapSchematicArray = new byte[Centers.instance.mazeSize, Centers.instance.mazeSize];
            //Debug.Log(string.Join(",", formalizatedMaze_Inside_WallSchematic(20, 20)));
            //Centers.instance.Print3D_Array<bool>(formalizatedMaze_Inside_WallSchematic(40, 40));
            //Debug.Log(debuggingDoubleWallDataMaze(formalizatedMaze_Inside_WallSchematic(30, 30)));
            Debug.Log(debuggingDoubleWallDataMaze(typecast_Blocklization(formalizatedMaze_Inside_WallSchematic(30, 30))));
        }

        void setSchementics()
        {

        }

        string debuggingDoubleWallDataMaze(bool[,] maze)
        {
            string output = "";

            for (int i = 0; i < maze.GetLength(0); i++)
            {
                for (int j = 0; j < maze.GetLength(1); j++)
                {
                    output += maze[i, j] ? "■" : "□";
                }

                output += "\n";

            }



            return output;
        }

        bool[,] typecast_Blocklization(bool[,,] data) // [x,y,2] -> [x,y] on the other words, two walls in each blocks become individual nine blocks
        {
            int dataXsize = data.GetLength(0);
            int dataYsize = data.GetLength(1);

            bool[,] output = new bool[2 * dataXsize + 1, 2 * dataYsize + 1];

            for (int i = 0; i < dataXsize; i++)
            {
                for (int j = 0; j < dataYsize; j++)
                {
                    //output[2*i + 1, 2*j + 1] is middle(parent) block (5 rightward and downward blocks overlaped other middle block's children) 
                    output[2 * i, 2 * j] = data[i, j, 0] || data[i, j, 0];

                    output[2 * i + 1, 2 * j] = data[i, j, 0];
                    output[2 * i + 2, 2 * j] = data[i, j, 0];

                    output[2 * i, 2 * j + 1] = data[i, j, 1];
                    output[2 * i, 2 * j + 2] = data[i, j, 1];

                }
            }

            return output;

        }

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


        private delegate int Coord(int i, int j);

        /// <summary>
        /// it return Normal maze Array that each dimension is x coordination, y coordination and wall direction
        /// </summary>
        /// <param name="size_X">X coordination size</param>
        /// <param name="size_Y">Y coordination size</param>
        /// <returns></returns>
        bool[,,] formalizatedMaze_Inside_WallSchematic(int size_X, int size_Y)
        {
            size_X = Mathf.Abs(size_X);
            size_Y = Mathf.Abs(size_Y);

            Coordinate coordinate = new Coordinate(size_X);

            Coord coord = new Coord(coordinate.coord);

            for (int i = 0; i < coord(size_X - 1, size_Y - 1) + 1; i++)
            {
                blocks.Add(null);
            }

            // Create new blocks where none exist
            // After randomly removing the wall, the family value of all blocks that have the family value of the block to be integrated into the previous block is changed to the family value of the previous block.

            for (int column = 0; column < size_Y - 1; column++)
            {
                for (int row = 0; row < size_X; row++)
                {
                    if (blocks[coord(row, column)] == null)
                    {
                        blocks[coord(row, column)] = new blockBase((uint)coord(row, column), top: true, side: true);
                    }
                }

                //wall eliminating

                for (int i = 0; i < size_X - 1; i++)
                {
                    if (Random.value < sideWallEilminationRate)
                    {
                        foreach (int myFamily in listFindAtFamilyValue(blocks[coord(i + 1, column)].family))
                        {
                            blocks[myFamily].family = blocks[coord(i, column)].family;
                        }

                        blocks[coord(i + 1, column)].mWallData.sideWall = false;

                    }
                }

                // upon wall Eliminating ================================================================================================
                HashSet<uint> droughtyList = new HashSet<uint>();

                bool isModified = false;

                for (int row = 0; row < size_X; row++)
                {
                    droughtyList.Add(blocks[coord(row, column)].family);
                }

                foreach (uint droughty in droughtyList)
                {
                    isModified = false;

                    List<int> familyList = new List<int>();

                    for (int row = 0; row < size_X; row++)
                    {
                        if (blocks[coord(row, column)].family.Equals(droughty))
                        {
                            familyList.Add(row);
                        }
                    }


                    foreach (int fam in familyList)
                    {
                        if (Random.value <= downWallEilminationRate / Mathf.Clamp(familyList.Count, 1, 8))
                        {
                            Debug.Log("randomModified");
                            isModified = true;
                            blocks[coord(fam, column + 1)] = new blockBase(droughty, top: false, side: true);
                        }
                    }

                    if (!isModified)
                    {
                        Debug.Log("nec-Modified{}");
                        int randomFam = familyList[Random.Range(0, familyList.Count - 1)];
                        Debug.Log("nec-Modified{coord(familyList[randomFam])}");
                        blocks[coord(randomFam, column + 1)] = new blockBase(droughty, top: false, side: true);
                    }


                }


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
            }


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

            bool[,,] typeCastedArray = new bool[size_X, size_Y, 2]; // [0]:width [1]: height

            for (int j = 0; j < size_Y; j++)
            {
                for (int i = 0; i < size_X; i++)
                {
                    if (j != 0)
                    {
                        typeCastedArray[i, j - 1, 0] = blocks[coord(i, j)].mWallData.topWall; // n * m-1
                    }

                    if (i != 0)
                    {
                        typeCastedArray[i - 1, j, 1] = blocks[coord(i, j)].mWallData.sideWall; // m-1 * n
                    }
                }
            }

            return typeCastedArray;



        }

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

    public class Coordinate
    {
        int sizeofX = 0;
        public Coordinate(int _sizeofX)
        {
            sizeofX = _sizeofX;
        }
        //use in the two dimention array
        public int coord(int x, int y)
        {
            if (sizeofX == 0)
            {
                Debug.LogError("sizeofX 가 0이 될 수 없습니다.");
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

*/
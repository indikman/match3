using UnityEngine;

namespace IndiMatchThree
{
    public class GridSystem2D<T>
    {
        int width;
        int height;
        float cellSize;
        Vector3 origin;
        T[,] gridArray;


        public abstract class CoordinateConverter
        {
            public abstract Vector3 GridToWorld(int x, int y, float cellSize, Vector3 origin);

            public abstract Vector3 GridWorldCenter(int x, int y, float cellSize, Vector3 origin);

            public abstract Vector2Int WorldToGrid(Vector3 worldPosition, float cellSize, Vector3 origin);

            public abstract Vector3 Forward { get; }
        }
    }
}

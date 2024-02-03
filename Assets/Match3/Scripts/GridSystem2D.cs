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

            public abstract Vector3 GridToWorldCenter(int x, int y, float cellSize, Vector3 origin);

            public abstract Vector2Int WorldToGrid(Vector3 worldPosition, float cellSize, Vector3 origin);

            public abstract Vector3 Forward { get; }
        }

        public class VerticalConverter : CoordinateConverter
        {
            public override Vector3 Forward => Vector3.forward;

            public override Vector3 GridToWorld(int x, int y, float cellSize, Vector3 origin)
            {
                return new Vector3(x, y, 0) * cellSize + origin;
            }

            public override Vector3 GridToWorldCenter(int x, int y, float cellSize, Vector3 origin)
            {
                return new Vector3(x*cellSize + cellSize * 0.5f, y * cellSize + cellSize * 0.5f, 0) + origin;
            }

            public override Vector2Int WorldToGrid(Vector3 worldPosition, float cellSize, Vector3 origin)
            {
                var gridPosition = (worldPosition - origin) / cellSize;
                var x = Mathf.FloorToInt(gridPosition.x);
                var y = Mathf.FloorToInt(gridPosition.y);
                return new Vector2Int(x, y);
            }
        }
    }
}

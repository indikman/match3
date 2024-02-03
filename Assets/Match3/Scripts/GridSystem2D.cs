using System;
using TMPro;
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

        CoordinateConverter coordinateConverter;

        public GridSystem2D(int width, int height, float cellSize, Vector3 origin, T[,] gridArray, CoordinateConverter coordinateConverter, bool debug)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.origin = origin;
            this.coordinateConverter = coordinateConverter ?? new VerticalConverter();

            gridArray = new T[width, height];

            if(debug)
            {
                DrawDebugLines();
            }
        }

        // Set a value from a grid position


        // Get a value from a grid position

        // Is the input coordinates valid?
        bool IsValid(int x, int y) => x >= 0 &&y >= 0 && x < width && y < height;

        public Vector2Int GetXY(Vector3 worldPosition) => coordinateConverter.WorldToGrid(worldPosition, cellSize, origin);

        public Vector3 GetWorldPositionCenter(int x, int y) => coordinateConverter.GridToWorldCenter(x, y, cellSize, origin);

        Vector3 GetWorldPosition(int x, int y) => coordinateConverter.GridToWorld(x,y,cellSize,origin);

        private void DrawDebugLines()
        {
            const float duration = 100f;

            var parent = new GameObject("Debugging");

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    CreateWorldText(parent, x + "," + y, GetWorldPositionCenter(x, y), coordinateConverter.Forward);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y+1), Color.white, duration);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y),Color.white, duration);
                }
            }

            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, duration);
            Debug.DrawLine(GetWorldPosition(height, 0), GetWorldPosition(width, height), Color.white, duration);
        }

        TextMeshPro CreateWorldText(GameObject parent, string text, Vector3 position, Vector3 dir, int fontSize = 2, Color color = default, TextAlignmentOptions textAnchor = TextAlignmentOptions.Center, int sortingOrder = 0)
        {
            GameObject gameobjet = new GameObject("DebugText_" + text, typeof(TextMeshPro));
            gameobjet.transform.SetParent(parent.transform);
            gameobjet.transform.position = position;
            gameobjet.transform.forward = dir;

            TextMeshPro textMeshPro = gameobjet.GetComponent<TextMeshPro>();
            textMeshPro.text = text;
            textMeshPro.color= color;
            textMeshPro.sortingOrder = sortingOrder;
            textMeshPro.alignment= textAnchor;
            textMeshPro.GetComponent<MeshRenderer>().sortingOrder= sortingOrder;

            return textMeshPro;
        }

        public abstract class CoordinateConverter
        {
            public abstract Vector3 GridToWorld(int x, int y, float cellSize, Vector3 origin);

            public abstract Vector3 GridToWorldCenter(int x, int y, float cellSize, Vector3 origin);

            public abstract Vector2Int WorldToGrid(Vector3 worldPosition, float cellSize, Vector3 origin);

            public abstract Vector3 Forward { get; }
        }

        /// <summary>
        /// Coordinate converter
        /// </summary>
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

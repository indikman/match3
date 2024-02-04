using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndiMatchThree
{
    public class Match3 : MonoBehaviour
    {
        [SerializeField] int width = 8;
        [SerializeField] int height = 8;
        [SerializeField] float cellSize = 1.0f;
        [SerializeField] Vector3 originPosition = Vector3.zero;
        [SerializeField] bool debug = true;

        GridSystem2D<GridObject<Gem>> grid;

        private void Start()
        {
            //Create the grid system
            grid = GridSystem2D<GridObject<Gem>>.HorizontalGrid(width, height, cellSize, originPosition, debug);
        }
    }
}

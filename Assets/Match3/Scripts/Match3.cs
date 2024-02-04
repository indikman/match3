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

        [SerializeField] Gem gemPrefab;
        [SerializeField] GemType[] gemTypes;

        GridSystem2D<GridObject<Gem>> grid;

        Vector2Int selectedGem = new Vector2Int(-1, -1);

        InputReader inputReader;
        private void Awake()
        {
            inputReader = gameObject.AddComponent<InputReader>();
        }

        private void Start()
        {
            InitializeGrid();

            inputReader.Fire += OnSelectedGem;
        }

        private void OnDestroy()
        {
            inputReader.Fire -= OnSelectedGem;
        }

        private void OnSelectedGem()
        {
            var gridPos = grid.GetXY(Camera.main.ScreenToWorldPoint(inputReader.Selected));

            if(selectedGem == gridPos)
            {
                DeselectGem();
            }else if(selectedGem==Vector2Int.one * -1)
            {
                SelectGem(gridPos);
            }
            else
            {
                StartCoroutine(RunGameLoop(selectedGem, gridPos));
            }
        }

        IEnumerator RunGameLoop(Vector2Int gridPositionA, Vector2Int gridPositionB)
        {
            throw new System.NotImplementedException();
        }

        private void SelectGem(Vector2Int gridPos)
        {
            throw new System.NotImplementedException();
        }

        private void DeselectGem() => selectedGem = new Vector2Int(-1, -1);

        // Init grid
        void InitializeGrid()
        {
            //Create the grid system
            grid = GridSystem2D<GridObject<Gem>>.VerticalGrid(width, height, cellSize, originPosition, debug);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    CreateGem(x, y);
                }
            }
        }

        void CreateGem(int x, int y)
        {
            Gem gem =Instantiate(gemPrefab, grid.GetWorldPositionCenter(x, y), Quaternion.identity, transform);
            gem.SetType(gemTypes[Random.Range(0,gemTypes.Length)]);
            var gridObject = new GridObject<Gem>(grid, x, y);
            gridObject.SetValue(gem);
            grid.SetValue(x, y, gridObject);
        }

        // Read player input and swap gems

        // Start corouting

        // Swap animation

        // Matches?

        // Explode gems

        // Replace empty spot

        // Is Game over?

    }
}

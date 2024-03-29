using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
        [SerializeField] Ease ease=Ease.OutQuad;
        [SerializeField] Ease fallEase=Ease.OutQuad;
        [SerializeField] float animTimeSwap=0.5f;
        [SerializeField] float animTimeFall=0.1f;

        GridSystem2D<GridObject<Gem>> grid;

        Vector2Int selectedGem = new Vector2Int(-1, -1);

        InputReader inputReader;
        private void Awake()
        {
            inputReader = gameObject.GetComponent<InputReader>();
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

            if (!IsValidPosition(gridPos) || IsEmptyPosition(gridPos)) return;

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

        private bool IsEmptyPosition(Vector2Int gridPos) => grid.GetValue(gridPos.x, gridPos.y) == null;

        private bool IsValidPosition(Vector2Int gridPos) => gridPos.x >= 0 && gridPos.y >= 0 && gridPos.x <= width && gridPos.y <= height;

        IEnumerator RunGameLoop(Vector2Int gridPositionA, Vector2Int gridPositionB)
        {
            yield return StartCoroutine(SwapGems(gridPositionA, gridPositionB));

            List<Vector2Int> matches = FindMatches();

            yield return StartCoroutine(ExplodeGems(matches));

            yield return StartCoroutine(MakeGemsFall());

            yield return StartCoroutine(FillEmptySpots());


            DeselectGem();
        }

        IEnumerator FillEmptySpots()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if(grid.GetValue(x,y)==null)
                    {
                        CreateGem(x,y);
                        //play a sound
                        yield return new WaitForSeconds(animTimeFall);
                    }
                }
            }

            yield return null;
        }

        IEnumerator MakeGemsFall()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if(grid.GetValue(x,y) == null)
                    {
                        for (int i = y+1; i < height; i++)
                        {
                            if(grid.GetValue(x,i)!=null) // If there is a gem, swap values and move it down
                            {
                                var gem = grid.GetValue(x, i).GetValue();
                                grid.SetValue(x,y, grid.GetValue(x,i));
                                grid.SetValue(x, i, null);

                                gem.transform.DOLocalMove(grid.GetWorldPositionCenter(x, y), animTimeFall).SetEase(fallEase);

                                yield return new WaitForSeconds(animTimeFall);
                                break;
                            }
                        }
                    }
                }
            }
        }

        IEnumerator ExplodeGems(List<Vector2Int> matches)
        {
            // SFX Play sound

            foreach (var match in matches)
            {
                var gem = grid.GetValue(match.x, match.y).GetValue();
                grid.SetValue(match.x, match.y, null);

                // VFX Explode
                gem.transform.DOPunchScale(Vector3.one * 0.1f, 0.1f, 1, 0.5f);

                yield return new WaitForSeconds(.1f);
                gem.DestroyGem();
            }
        }

        IEnumerator SwapGems(Vector2Int gridPositionA, Vector2Int gridPositionB)
        {
            var gridObjectA = grid.GetValue(gridPositionA.x, gridPositionA.y);
            var gridObjectB = grid.GetValue(gridPositionB.x, gridPositionB.y);

            gridObjectA.GetValue().transform.DOLocalMove(grid.GetWorldPositionCenter(gridPositionB.x, gridPositionB.y), animTimeSwap).SetEase(ease);
            gridObjectB.GetValue().transform.DOLocalMove(grid.GetWorldPositionCenter(gridPositionA.x, gridPositionA.y), animTimeSwap).SetEase(ease);

            grid.SetValue(gridPositionA.x, gridPositionA.y, gridObjectB);
            grid.SetValue(gridPositionB.x, gridPositionB.y, gridObjectA);

            yield return new WaitForSeconds(animTimeSwap);
        }

        private void SelectGem(Vector2Int gridPos) => selectedGem = gridPos;

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

        Gem CreateGem(int x, int y)
        {
            Gem gem =Instantiate(gemPrefab, grid.GetWorldPositionCenter(x, y), Quaternion.identity, transform);
            gem.SetType(gemTypes[Random.Range(0,gemTypes.Length)]);
            var gridObject = new GridObject<Gem>(grid, x, y);
            gridObject.SetValue(gem);
            grid.SetValue(x, y, gridObject);
            return gem;
        }


        List<Vector2Int> FindMatches()
        {
            HashSet<Vector2Int> matches = new();

            // Horizontal
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var gemA = grid.GetValue(x, y);
                    var gemB = grid.GetValue(x+1, y);
                    var gemC = grid.GetValue(x+2, y);

                    if(gemA==null||gemB==null||gemC==null) continue;

                    if(gemA.GetValue().GetType() == gemB.GetValue().GetType() && gemB.GetValue().GetType() == gemC.GetValue().GetType())
                    {
                        matches.Add(new Vector2Int(x, y));
                        matches.Add(new Vector2Int(x+1, y));
                        matches.Add(new Vector2Int(x+2, y));
                    }
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var gemA = grid.GetValue(x, y);
                    var gemB = grid.GetValue(x, y+1);
                    var gemC = grid.GetValue(x, y+2);

                    if (gemA == null || gemB == null || gemC == null) continue;

                    if (gemA.GetValue().GetType() == gemB.GetValue().GetType() && gemB.GetValue().GetType() == gemC.GetValue().GetType())
                    {
                        matches.Add(new Vector2Int(x, y));
                        matches.Add(new Vector2Int(x, y+1));
                        matches.Add(new Vector2Int(x, y+2));
                    }
                }
            }

            return new List<Vector2Int>(matches);
        }


        // Explode gems

        // Replace empty spot

        // Is Game over?

    }
}

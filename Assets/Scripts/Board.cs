using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board instance;
    public List<Sprite> sprites = new List<Sprite>();
    public GameObject item;
    public int xSize, ySize;
    private GameObject[,] items;

    public bool IsMoving { get; set; }
    void Start()
    {
        instance = GetComponent<Board>();

        Vector2 offset = item.GetComponent<SpriteRenderer>().bounds.size;
        CreateBoard(offset.x, offset.y);
    }
    // create board with items on it
    private void CreateBoard(float xOffset, float yOffset)
    {
        items = new GameObject[xSize, ySize];

        float startX = transform.position.x;
        float startY = transform.position.y;

        Sprite[] previousLeft = new Sprite[ySize];
        Sprite previousBelow = null;

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                GameObject newItem = Instantiate(item, new Vector3(startX + (xOffset * x), startY + (yOffset * y), 0), Quaternion.identity);
                items[x, y] = newItem;
                newItem.gameObject.name = "item" + x + y;
                newItem.transform.parent = transform;

                List<Sprite> possibleSprites = new List<Sprite>();
                possibleSprites.AddRange(sprites);

                possibleSprites.Remove(previousLeft[y]);
                possibleSprites.Remove(previousBelow);

                Sprite newSprite = possibleSprites[Random.Range(0, possibleSprites.Count)];
                newItem.GetComponent<SpriteRenderer>().sprite = newSprite;
                previousLeft[y] = newSprite;
                previousBelow = newSprite;
            }
        }
    }
    // find tiles that are empty/null
    public IEnumerator FindNullItems()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (items[x, y].GetComponent<SpriteRenderer>().sprite == null)
                {
                    yield return StartCoroutine(MoveItemsDown(x, y));
                    break;
                }
            }
        }

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                items[x, y].GetComponent<Item>().ClearAllMatches();
            }
        }
    }
    // move tiles down
    private IEnumerator MoveItemsDown(int x, int yStart, float shiftDelay = .1f)
    {
        IsMoving = true;
        List<SpriteRenderer> renders = new List<SpriteRenderer>();
        int nullCount = 0;

        for (int y = yStart; y < ySize; y++)
        {
            SpriteRenderer render = items[x, y].GetComponent<SpriteRenderer>();
            if (render.sprite == null)
            {
                nullCount++;
            }
            renders.Add(render);
        }

        for (int i = 0; i < nullCount; i++)
        {
            yield return new WaitForSeconds(shiftDelay);
            for (int k = 0; k < renders.Count - 1; k++)
            {
                renders[k].sprite = renders[k + 1].sprite;
                renders[k + 1].sprite = GetNewSprite(x, ySize - 1);
            }
        }
        IsMoving = false;
    }
    // create new random sprite for item
    private Sprite GetNewSprite(int x, int y)
    {
        List<Sprite> possibleSprites = new List<Sprite>();
        possibleSprites.AddRange(sprites);

        if (x > 0)
        {
            possibleSprites.Remove(items[x - 1, y].GetComponent<SpriteRenderer>().sprite);
        }
        if (x < xSize - 1)
        {
            possibleSprites.Remove(items[x + 1, y].GetComponent<SpriteRenderer>().sprite);
        }
        if (y > 0)
        {
            possibleSprites.Remove(items[x, y - 1].GetComponent<SpriteRenderer>().sprite);
        }

        return possibleSprites[Random.Range(0, possibleSprites.Count)];
    }

}

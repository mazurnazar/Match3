using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private static Color selected = new Color(.5f, .5f, .5f, 1.0f);
    private static Item previousSelected = null;
    private SpriteRenderer render;
    private bool isSelected = false;
    private bool matchFound = false;
    private bool swapped = false;

    private Vector2[] directions = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
    void Awake()
    {
        render = GetComponent<SpriteRenderer>();
    }
    // select item
    private void Select()
    {
        isSelected = true;
        render.color = selected;
        previousSelected = gameObject.GetComponent<Item>();
    }
    // deselect item
    private void Deselect()
    {
        isSelected = false;
        render.color = Color.white;
        previousSelected = null;
    }
    
    void OnMouseDown()
    {
        // When sprite is null or board ismoving
        if (render.sprite == null || Board.instance.IsMoving)
        {
            return;
        }

        if (isSelected) // Is it already selected? then deselect
        { 
            Deselect();
        }
        else
        {
            if (previousSelected == null) // If it is the first item selected then select
            { 
                Select();
            }
            else
            {
                if (FindAllNeighbours().Contains(previousSelected.gameObject))
                { 
                    StartCoroutine(SwapSprite(previousSelected));
                    swapped = true;
                }
                else
                {
                    previousSelected.GetComponent<Item>().Deselect();
                    Select();
                }
            }
        }
    }

    IEnumerator SwapSprite(Item item2)
    {
        Vector2 firstPos = transform.position;
        Vector2 secondPos = item2.transform.position;
        float i = 0f;
        while (i <= 0.5f)
        {
            item2.transform.position = Vector2.Lerp(secondPos, firstPos, i*2);
            transform.position = Vector2.Lerp(firstPos, secondPos, i*2);
            i += 0.1f;
            yield return new WaitForSeconds(.05f);

        }
        previousSelected.ClearAllMatches();
        if (!matchFound || !previousSelected.matchFound)
        { 
            if (swapped) 
            { 
                StartCoroutine(SwapSprite(previousSelected)); swapped = false; 
            } 
            else previousSelected.Deselect(); 
        }
        else previousSelected.Deselect();
        ClearAllMatches();
    }
    private GameObject FindNeighbour(Vector2 castDir)
        {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
        if (hit.collider != null)
            {
                return hit.collider.gameObject;
            }
            return null;
        }
    // find matches in all  directions
    private List<GameObject> FindAllNeighbours()
    {
        List<GameObject> neighbours = new List<GameObject>();
        for (int i = 0; i < directions.Length; i++)
        {
            neighbours.Add(FindNeighbour(directions[i]));
        }
        return neighbours;
    }
    // find match in one direction
    private List<GameObject> FindMatch(Vector2 castDir)
    {
        List<GameObject> matchingTiles = new List<GameObject>();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
        while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == render.sprite)
        {
            matchingTiles.Add(hit.collider.gameObject);
            hit = Physics2D.Raycast(hit.collider.transform.position, castDir);
        }
        return matchingTiles;
    }
    // for a direction (horisontal or vertical) clear matches
    private void ClearMatch(Vector2[] directions)
    {
        List<GameObject> matchingItems= new List<GameObject>();
        for (int i = 0; i < directions.Length; i++) { matchingItems.AddRange(FindMatch(directions[i])); }
        if (matchingItems.Count >= 2)
        {
            for (int i = 0; i < matchingItems.Count; i++)
            {
                matchingItems[i].GetComponent<SpriteRenderer>().sprite = null;
            }
            matchFound = true;
        }
    }
    // Clear all matches
    public void ClearAllMatches()
    {
        if (render.sprite == null)
            return;

        ClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
        ClearMatch(new Vector2[2] { Vector2.up, Vector2.down });
        if (matchFound)
        {
            render.sprite = null;
            matchFound = false;
            StopCoroutine(Board.instance.FindNullItems()); 
            StartCoroutine(Board.instance.FindNullItems()); 

        }
    }

}

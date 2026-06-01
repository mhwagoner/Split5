using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RuneDraw : MonoBehaviour
{
    [SerializeField] private Rune[] runes;
    [SerializeField] private RunePoint[] runePoints;

    private RunePoint currentPoint;
    private RuneLine currentLine;
    private List<RuneLine> currentLines; // adjacency matrix might be better oops
    private bool drawing;

    [SerializeField] private InputActionReference clickAction;
    [SerializeField] private InputActionReference lookAction;
    private Vector2 mousePos;
    [SerializeField] private RectTransform canvas;

    [SerializeField] private GameObject lineImagePrefab;
    private List<Image> lineImages;
    private Image currentLineImage;
    private List<GameObject> orbs;
    [SerializeField] private GameObject orbPrefab;

    public Sprite drawSpriteBase;
    private Texture2D drawTexture;
    private Sprite drawSprite;
    [SerializeField] private Image drawImage;
    private Vector2Int? lastPixelPos = null;

    private const int BRUSH_SIZE = 4;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(RunePoint point in runePoints)
        {
            point.onPointerEnter += PointEntered;
            point.onPointerClick += PointClicked;
        }

        drawTexture = new(drawSpriteBase.texture.height, drawSpriteBase.texture.width, TextureFormat.RGBA32, false);
        ClearDrawTexture();
        drawSprite = Sprite.Create(
            drawTexture,
            new Rect(0.0f, 0.0f, drawTexture.width, drawTexture.height),
            new Vector2(0.0f, 0.0f),
            100.0f
        );
        drawImage.sprite = drawSprite;
        orbs = new();
    }

    // Update is called once per frame
    void Update()
    {
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, Mouse.current.position.value, Camera.main, out mousePos);
        //mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
        mousePos = Mouse.current.position.value;

        // Poop code, replace maybe
        if (clickAction.action.IsPressed())
        {
            if(!drawing)
            {
                /*RunePoint nearestPoint = runePoints[0];
                foreach (RunePoint point in runePoints)
                {
                    if(Vector2.Distance(point.transform.position, mousePos) < Vector2.Distance(nearestPoint.transform.position, mousePos))
                    {
                        nearestPoint = point;
                    }
                }*/
                //currentLine.point_a = currentPoint.pointPosition;
                currentLines = new();
                lineImages = new();

                //move and rename
                //currentLineImage = Instantiate(lineImagePrefab, canvas.transform).GetComponent<Image>();
            }
            //DrawLine(currentPoint.transform.position, mousePos, Color.white);
            DrawToTexture();
            drawing = true;
        }
        else
        {
            if(drawing)
            {
                if(CheckForRune())
                {
                    print("Rune Found!");
                    foreach (RuneLine line in currentLines)
                    {
                        print(line.point_a + " + " + line.point_b);
                    }
                }
                else
                {
                    print("Invalid Rune!");
                }

                /*foreach(Image lineImage in lineImages)
                {
                    Destroy(lineImage.gameObject);
                }
                Destroy(currentLineImage.gameObject);*/
                ClearDrawTexture();
            }
            drawing = false;
            lastPixelPos = null;
            currentPoint = null;
            currentLine = new();

            if(orbs.Count > 0)
            {
                foreach(GameObject orb in orbs)
                {
                    Destroy(orb);
                }
                orbs = new();
            }
        }
    }

    private void PointEntered(RunePoint point)
    {
        if (drawing)
        {
            AddPoint(point);
        }
    }

    private void AddPoint(RunePoint point)
    {
        // Current point not set
        if(currentPoint == null)
        {
            currentPoint = point;
            currentLine.point_a = point.pointPosition;
            AddOrb(point);
            return;
        }

        if (point != currentPoint)
        {
            // Use current line
            currentLine.point_b = point.pointPosition;
            //DrawLine(currentPoint.transform.position, point.transform.position, Color.white);
            currentPoint = point;
            currentLines.Add(currentLine);
            //lineImages.Add(currentLineImage);

            // Start new line
            currentLine = new();
            currentLine.point_a = currentPoint.pointPosition;
            //currentLineImage = Instantiate(lineImagePrefab, canvas.transform).GetComponent<Image>();
            //DrawLine(currentPoint.transform.position, mousePos, Color.white);
            AddOrb(point);
        }
    }

    public bool CheckForRune() // should cast spell if valid
    {
        if (currentLines.Count > 0)
        {
            foreach(Rune rune in runes)
            {
                bool runeMatch = true;
                foreach (RuneLine runeLine in rune.lines)
                {
                    bool lineFound = false;

                    foreach (RuneLine currentLine in currentLines)
                    {
                        // check both a -> b and b <- a
                        if((currentLine.point_a == runeLine.point_a && currentLine.point_b == runeLine.point_b) || (currentLine.point_a == runeLine.point_b && currentLine.point_b == runeLine.point_a))
                        {
                            lineFound = true;
                        }
                    }

                    if(!lineFound)
                    {
                        runeMatch = false;
                        break;
                    }
                }

                if(runeMatch)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // unused
    public void DrawLine(Vector2 positionOne, Vector2 positionTwo, Color color)
    {
        currentLineImage.color = color;

        Vector2 midpoint = (positionOne + positionTwo) / 2f;

        currentLineImage.transform.position = midpoint;

        Vector2 dir = positionOne - positionTwo;
        currentLineImage.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        currentLineImage.transform.localScale = new Vector3(dir.magnitude, 1f, 1f);
    }

    public void DrawToTexture()
    {
        Vector2 hitPos;
        bool hit = RectTransformUtility.ScreenPointToLocalPointInRectangle(drawImage.rectTransform, Mouse.current.position.value, null, out hitPos);

        // Perform raycast to find the exact interaction coordinate on the object
        if (hit)
        {
            // Convert UV texture hit (0.0 to 1.0) into discrete integer pixel indices
            Vector2 uv = hitPos;
            float normalizedX = ((uv.x - drawImage.rectTransform.rect.x) / drawImage.rectTransform.rect.width);
            float normalizedY = ((uv.y - drawImage.rectTransform.rect.y) / drawImage.rectTransform.rect.height);
            int pixelX = (int)(normalizedX * 140);
            int pixelY = (int)(normalizedY * 140);
            Vector2Int currentPixelPos = new Vector2Int(Math.Clamp(pixelX, 0, drawTexture.width - BRUSH_SIZE), Math.Clamp(pixelY, 0, drawTexture.height - BRUSH_SIZE));

            if (lastPixelPos.HasValue)
            {
                // Interpolate between the past frame and the current frame to prevent missing pixel gaps
                DrawInterpolatedLine(lastPixelPos.Value, currentPixelPos);
            }
            else
            {
                // Single dot draw for the initial click action
                DrawBrush(currentPixelPos);
            }

            // Force upload changes to the GPU cluster
            drawTexture.Apply();
            lastPixelPos = currentPixelPos;
        }
    }

    private void DrawBrush(Vector2Int center)
    {
        Color[] colorArray = new Color[16];
        Array.Fill(colorArray, Color.gray1);
        drawTexture.SetPixels(center.x, center.y, BRUSH_SIZE / 2, BRUSH_SIZE, colorArray);
        drawTexture.SetPixels(center.x, center.y, BRUSH_SIZE, BRUSH_SIZE / 2, colorArray);
    }

    private void DrawInterpolatedLine(Vector2Int start, Vector2Int end)
    {
        float distance = Vector2Int.Distance(start, end);

        // Calculate required steps based on distance so no single pixel index is bypassed
        int steps = Mathf.CeilToInt(distance);

        for (int i = 0; i <= steps; i++)
        {
            float t = steps == 0 ? 1f : (float)i / steps;
            Vector2 interpolatedPoint = Vector2.Lerp(start, end, t);
            Vector2Int pixelCoord = new Vector2Int(Mathf.RoundToInt(interpolatedPoint.x), Mathf.RoundToInt(interpolatedPoint.y));

            DrawBrush(pixelCoord);
        }
    }

    /// <summary>
    /// Set draw texture back to all transparent pixels
    /// </summary>
    private void ClearDrawTexture()
    {
        Color[] clearPixels = new Color[drawSpriteBase.texture.height * drawSpriteBase.texture.width];
        Array.Fill(clearPixels, Color.clear);
        drawTexture.SetPixels(clearPixels);
        drawTexture.Apply();
    }

    private void PointClicked(RunePoint point)
    {
        currentPoint = point;
        currentLine.point_a = point.pointPosition;
        AddOrb(point);
    }

    private void AddOrb(RunePoint point)
    {
        GameObject newOrb = Instantiate(orbPrefab, canvas.transform);
        newOrb.transform.position = point.transform.position;
        orbs.Add(newOrb);
    }
}

[System.Serializable]
public class Rune
{
    /*
     *        *
     *     *     *
     *  *     *     *
     *     *     *
     *        *
     *          [0,1]
     *  [-1, 1]         [1, 1]
     * [-1, 0]  [0, 0]   [1, 0]
     *  [-1, -1]        [1, -1]
     *          [0,-1]
     */

    public RuneLine[] lines;
}

[System.Serializable]
public class RuneLine
{
    public Vector2Int point_a;
    public Vector2Int point_b;
    //public bool empty = false;
}
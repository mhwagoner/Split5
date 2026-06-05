using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Unity.Collections.Unicode;

public class RuneDraw : MonoBehaviour
{
    //[SerializeField] private Rune[] runes;
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
    private Texture2D lineDrawTexture;
    private Sprite lineDrawSprite;
    [SerializeField] private Image drawImage;
    [SerializeField] private Image lineDrawImage;
    private Vector2Int? lastPixelPos = null;
    private AudioSource audioSource;
    private int drawAudioDelay = 0;

    private const int BRUSH_SIZE = 4;

    public Action<Spell> onSpellCast;
    [SerializeField] private bool writeToFile = false;

    private void Awake()
    {
        GameManager.Instance.runeDraw = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(RunePoint point in runePoints)
        {
            point.onPointerEnter += PointEntered;
            point.onPointerClick += PointClicked;
        }

        SetUpDrawTextures();

        orbs = new();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Mouse.current.position.value;

        // Poop code, replace maybe
        if (clickAction.action.IsPressed())
        {
            if(!drawing)
            {
                Vector2 hitPos;
                bool hit = RectTransformUtility.ScreenPointToLocalPointInRectangle(drawImage.rectTransform, Mouse.current.position.value, null, out hitPos);
                if (Vector2.Distance(hitPos, Vector2.zero) > 50)
                {
                    return;
                }
                currentLines = new();
                lineImages = new();
                audioSource.Play();
            }

            DrawToTexture(drawImage);
            drawing = true;
        }
        else
        {
            if(drawing)
            {
                if(CheckForRune())
                {
                    print("Rune Found!");
                }
                else
                {
                    print("Invalid Rune!");
                }

                // REMOVE FOR BUILD (i think you can actually do that as an IFDEF or something but idc)
                if(writeToFile)
                {
                    WriteDrawingToFile();
                    WriteLinesToTxt();
                }

                /*foreach(Image lineImage in lineImages)
                {
                    Destroy(lineImage.gameObject);
                }
                Destroy(currentLineImage.gameObject);*/
                ClearDrawTexture(drawTexture);
                ClearDrawTexture(lineDrawTexture);
                audioSource.Stop();
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
            // If passes through center, add connection there
            if (currentLine.point_a + point.pointPosition == Vector2Int.zero)
            {
                currentLine.point_b = Vector2Int.zero;
                DrawLine(currentPoint.transform.position, runePoints[0].transform.position, Color.white);
                currentLines.Add(currentLine);
                currentLine = new();
                currentLine.point_a = Vector2Int.zero;
                currentLine.point_b = point.pointPosition;
                DrawLine(runePoints[0].transform.position, point.transform.position, Color.white);
            }
            else
            {
                // Use current line
                currentLine.point_b = point.pointPosition;
                DrawLine(currentPoint.transform.position, point.transform.position, Color.white);
            }
            currentLines.Add(currentLine);
            currentLine = new();
            currentLine.point_a = point.pointPosition;
            currentPoint = point;
            //lineImages.Add(currentLineImage);

            // Start new line
            //currentLineImage = Instantiate(lineImagePrefab, canvas.transform).GetComponent<Image>();
            //DrawLine(currentPoint.transform.position, mousePos, Color.white);
            ClearDrawTexture(drawTexture);
            //AddOrb(point);
        }
    }

    public bool CheckForRune() // should cast spell if valid
    {
        RemoveDuplicateLines();

        if(GameManager.Instance.gameLost)
        {
            Rune rune = new Rune(new RuneLine[] { new RuneLine(-1, 1, 1, 1), new RuneLine(1, 1, 0, 0), new RuneLine(0, 0, -1, -1), new RuneLine(-1, -1, 1, -1), new RuneLine(1, -1, 0, 0), new RuneLine(0, 0, -1, 1), });
            if(CompareRune(rune))
            {
                // Reset scene
                GameManager.Instance.player.audioSource.PlayOneShot(GameManager.Instance.player.clockTickFast);
                StartCoroutine(GameManager.Instance.GoToSceneDelay(0.6f, SceneManager.GetActiveScene().name));
                return true;
            }
            rune = new Rune(new RuneLine[] { new RuneLine(-1, -1, 0, 0), new RuneLine(0, 0, 1, 1), new RuneLine(1, 1, 1, -1), new RuneLine(1, -1, 0, 0), new RuneLine(0, 0, -1, 1), new RuneLine(-1, 1, -1, -1), });
            if (CompareRune(rune))
            {
                // Return to title
                GameManager.Instance.player.audioSource.PlayOneShot(GameManager.Instance.player.clockTickFast);
                StartCoroutine(GameManager.Instance.GoToSceneDelay(0.6f, "MainMenu"));
                return true;
            }
        }
        else if (currentLines.Count > 0)
        {
            foreach (Spell spell in GameManager.Instance.spellManager.spells)
            {
                foreach (Rune rune in spell.runes)
                {
                    if (CompareRune(rune))
                    {
                        onSpellCast?.Invoke(spell);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void RemoveDuplicateLines()
    {
        Rune clearRune = new();
        clearRune.lines = new();

        foreach (RuneLine currentLine in currentLines)
        {
            bool lineFound = false;
            foreach (RuneLine runeLine in clearRune.lines)
            {
                // check both a -> b and b <- a
                if ((currentLine.point_a == runeLine.point_a && currentLine.point_b == runeLine.point_b) || (currentLine.point_a == runeLine.point_b && currentLine.point_b == runeLine.point_a))
                {
                    lineFound = true;
                }
            }

            if(!lineFound)
            {
                clearRune.lines.Add(currentLine);
            }
        }

        currentLines = clearRune.lines;
    }

    public bool CompareRune(Rune rune)
    {
        int lineCount = rune.lines.Count;
        foreach (RuneLine currentLine in currentLines)
        {
            bool lineFound = false;

            foreach (RuneLine runeLine in rune.lines)
            {
                // check both a -> b and b <- a
                if ((currentLine.point_a == runeLine.point_a && currentLine.point_b == runeLine.point_b) || (currentLine.point_a == runeLine.point_b && currentLine.point_b == runeLine.point_a))
                {
                    lineFound = true;
                    lineCount--;
                }
            }

            if (!lineFound)
            {
                return false;
            }
        }
        if (lineCount <= 0)
        {
            return true;
        }
        return false;
    }

    // unused
    public void DrawLine(Vector2 positionOne, Vector2 positionTwo, Color color)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(lineDrawImage.rectTransform, positionOne, null, out positionOne);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(lineDrawImage.rectTransform, positionTwo, null, out positionTwo);
        positionOne = ToImageCoords(lineDrawImage, positionOne);
        positionTwo = ToImageCoords(lineDrawImage, positionTwo);
        DrawInterpolatedLine(lineDrawTexture,Vector2Int.FloorToInt(positionOne - Vector2.one), Vector2Int.FloorToInt(positionTwo - Vector2.one));
        lineDrawTexture.Apply();

        return;
        currentLineImage.color = color;

        Vector2 midpoint = (positionOne + positionTwo) / 2f;

        currentLineImage.transform.position = midpoint;

        Vector2 dir = positionOne - positionTwo;
        currentLineImage.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        currentLineImage.transform.localScale = new Vector3(dir.magnitude, 1f, 1f);
    }

    private Vector2 ToImageCoords(Image image, Vector2 uv)
    {
        // Convert UV texture hit (0.0 to 1.0) into discrete integer pixel indices
        float normalizedX = ((uv.x - image.rectTransform.rect.x) / image.rectTransform.rect.width);
        float normalizedY = ((uv.y - image.rectTransform.rect.y) / image.rectTransform.rect.height);
        int pixelX = (int)(normalizedX * 140);
        int pixelY = (int)(normalizedY * 140);
        Vector2Int currentPixelPos = new Vector2Int(Math.Clamp(pixelX, 0, drawTexture.width - BRUSH_SIZE), Math.Clamp(pixelY, 0, drawTexture.height - BRUSH_SIZE));
        return currentPixelPos;
    }

    public void DrawToTexture(Image image)
    {
        Vector2 hitPos;
        bool hit = RectTransformUtility.ScreenPointToLocalPointInRectangle(image.rectTransform, Mouse.current.position.value, null, out hitPos);

        // Perform raycast to find the exact interaction coordinate on the object
        if (hit)
        {
            // Convert UV texture hit (0.0 to 1.0) into discrete integer pixel indices
            Vector2 uv = hitPos;
            float normalizedX = ((uv.x - image.rectTransform.rect.x) / image.rectTransform.rect.width);
            float normalizedY = ((uv.y - image.rectTransform.rect.y) / image.rectTransform.rect.height);
            int pixelX = (int)(normalizedX * 140);
            int pixelY = (int)(normalizedY * 140);
            Vector2Int currentPixelPos = new Vector2Int(Math.Clamp(pixelX, 0, drawTexture.width - BRUSH_SIZE), Math.Clamp(pixelY, 0, drawTexture.height - BRUSH_SIZE));

            if (lastPixelPos.HasValue)
            {
                // Interpolate between the past frame and the current frame to prevent missing pixel gaps
                DrawInterpolatedLine((Texture2D) image.mainTexture, lastPixelPos.Value, currentPixelPos);

                if(lastPixelPos != currentPixelPos)
                {
                    DrawAudio(true);
                }
                else
                {
                    DrawAudio(false);
                }
            }
            else
            {
                // Single dot draw for the initial click action
                DrawBrush((Texture2D)image.mainTexture, currentPixelPos);
            }

            // Force upload changes to the GPU cluster
            drawTexture.Apply();
            lastPixelPos = currentPixelPos;
        }
    }

    private void DrawBrush(Texture2D texture, Vector2Int center)
    {
        Color[] colorArray = new Color[16];
        Array.Fill(colorArray, Color.black);
        texture.SetPixels(center.x, center.y, BRUSH_SIZE / 2, BRUSH_SIZE, colorArray);
        texture.SetPixels(center.x, center.y, BRUSH_SIZE, BRUSH_SIZE / 2, colorArray);
    }

    private void DrawInterpolatedLine(Texture2D texture, Vector2Int start, Vector2Int end)
    {
        float distance = Vector2Int.Distance(start, end);

        // Calculate required steps based on distance so no single pixel index is bypassed
        int steps = Mathf.CeilToInt(distance);

        for (int i = 0; i <= steps; i++)
        {
            float t = steps == 0 ? 1f : (float)i / steps;
            Vector2 interpolatedPoint = Vector2.Lerp(start, end, t);
            Vector2Int pixelCoord = new Vector2Int(Mathf.RoundToInt(interpolatedPoint.x), Mathf.RoundToInt(interpolatedPoint.y));

            DrawBrush(texture, pixelCoord);
        }
    }

    /// <summary>
    /// Set draw texture back to all transparent pixels
    /// </summary>
    private void ClearDrawTexture(Texture2D texture)
    {
        Color[] clearPixels = new Color[drawSpriteBase.texture.height * drawSpriteBase.texture.width];
        Array.Fill(clearPixels, Color.clear);
        texture.SetPixels(clearPixels);
        texture.Apply();
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

    private void SetUpDrawTextures()
    {
        drawTexture = new(drawSpriteBase.texture.height, drawSpriteBase.texture.width, TextureFormat.RGBA32, false);
        ClearDrawTexture(drawTexture);
        drawSprite = Sprite.Create(
            drawTexture,
            new Rect(0.0f, 0.0f, drawTexture.width, drawTexture.height),
            new Vector2(0.0f, 0.0f),
            100.0f
        );
        drawImage.sprite = drawSprite;
        lineDrawTexture = new(drawSpriteBase.texture.height, drawSpriteBase.texture.width, TextureFormat.RGBA32, false);
        ClearDrawTexture(lineDrawTexture);
        lineDrawSprite = Sprite.Create(
            lineDrawTexture,
            new Rect(0.0f, 0.0f, drawTexture.width, drawTexture.height),
            new Vector2(0.0f, 0.0f),
            100.0f
        );
        lineDrawImage.sprite = lineDrawSprite;
    }

    private void WriteDrawingToFile()
    {
        byte[] bytes = lineDrawTexture.EncodeToPNG();

        // 4. Save the bytes to your project directory
        string path = Path.Combine(UnityEngine.Application.dataPath, "Export/" + Time.time.ToString() + ".png");
        File.WriteAllBytes(path, bytes);
    }

    private void WriteLinesToTxt()
    {
        string path = Path.Combine(UnityEngine.Application.dataPath, "Export/" + Time.time.ToString() + ".txt");
        string text = "new Rune( new RuneLine[] { ";
        foreach (RuneLine line in currentLines)
        {
            // IDK FIGURE OUT
            // new RuneLine[] { new RuneLine(0, 1, 0, 0) }
            text += "new RuneLine(" + line.point_a.x + ", " + line.point_a.y + ", " + line.point_b.x + ", " + line.point_b.y + "), ";
        }
        text.Remove(text.Length - 3);
        text += "} )";

        File.WriteAllText(path, text);
    }

    private void DrawAudio(bool draw)
    {
        if (draw)
        {
            audioSource.pitch = 1;
            drawAudioDelay = 0;
        }
        else
        {
            drawAudioDelay++;

            if (drawAudioDelay > 5)
            {
                audioSource.pitch = 0;
            }
        }
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

    public List<RuneLine> lines;

    public Rune()
    {

    }

    public Rune(RuneLine[] lines)
    {
        this.lines = new List<RuneLine>(lines);
    }
}

[System.Serializable]
public class RuneLine
{
    public Vector2Int point_a;
    public Vector2Int point_b;
    //public bool empty = false;

    public RuneLine()
    {

    }

    public RuneLine(int point_a_x, int point_a_y, int point_b_x, int point_b_y)
    {
        point_a = new Vector2Int(point_a_x, point_a_y);
        point_b = new Vector2Int(point_b_x, point_b_y);
    }
}
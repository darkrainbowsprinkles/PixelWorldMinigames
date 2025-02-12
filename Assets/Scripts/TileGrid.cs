using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileGrid : MonoBehaviour
{
    [SerializeField] GameObject tilePrefab;
    [SerializeField] bool staticColors = true;
    [SerializeField] bool staticNotes = true;
    AudioClip[] possibleNotes;
    AudioSource audioSource;
    Dictionary<Button, AudioClip> noteLookup = new();

    void Awake()
    {   
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        LoadNotes();
        FillTiles();
    }

    void LoadNotes()
    {
        possibleNotes = Resources.LoadAll<AudioClip>("");
    }

    void FillTiles()
    {
        int tileNumber = CalculateTileNumber();

        for(int tile = 0; tile < tileNumber; tile++)
        {
            Button tileInstance = Instantiate(tilePrefab, transform).GetComponent<Button>();
            SetRandomTileColor(tileInstance);
            SetRandomTileNote(tileInstance);
            HandleTilePressedEvent(tileInstance);
        }
    }


    void SetRandomTileColor(Button tile)
    {
        ColorBlock colors = tile.colors;
        colors.pressedColor = Random.ColorHSV(0, 1, 0.6f, 1, 0.7f, 1);
        tile.colors = colors;
    }

    void SetRandomTileNote(Button tile)
    {
        int randomNoteIndex = Random.Range(0, possibleNotes.Length);
        noteLookup[tile] = possibleNotes[randomNoteIndex];
    }

    void HandleTilePressedEvent(Button tile)
    {
        EventTrigger trigger = tile.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new();

        entry.eventID = EventTriggerType.PointerDown; 
        entry.callback.AddListener(a => OnTilePressed(tile));

        trigger.triggers.Add(entry);
    }

    void OnTilePressed(Button tile)
    {
        audioSource.PlayOneShot(noteLookup[tile]);

        if(!staticColors)
        {
            SetRandomTileColor(tile);
        }

        if(!staticNotes)
        {
            SetRandomTileNote(tile);
        }
    }

    int CalculateTileNumber()
    {
        RectTransform gridRect = GetComponent<RectTransform>();
        RectTransform tileRect = tilePrefab.GetComponent<RectTransform>();

        int width = Mathf.FloorToInt(gridRect.sizeDelta.x / tileRect.sizeDelta.x);
        int height = Mathf.FloorToInt(gridRect.sizeDelta.y / tileRect.sizeDelta.y);

        return width * height;
    }
}

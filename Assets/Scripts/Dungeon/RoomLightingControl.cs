using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(InstantiatedRoom))]
public class RoomLightingControl : MonoBehaviour
{
    private InstantiatedRoom instantiatedRoom;

    private void Awake()
    {
        // Load components
        instantiatedRoom = GetComponent<InstantiatedRoom>();
    }

    private void OnEnable()
    {
        //subscribe to room changed event
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        //unsubscribe from room changed event
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }


    /// <summary>
    /// Handle room changed event
    /// </summary>
    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        // If this is the room entered and the room isn't already lit, then fade in the room lighting
        if (roomChangedEventArgs.room == instantiatedRoom.room && !instantiatedRoom.room.isLit)
        {
            // Fade in room
            FadeInRoomLighting();

            // ensure enviroment decoration for room is activated
            instantiatedRoom.ActivateEnvironmentGameObjects();

            FadeInEnvironmentLighting();

            // Fade in the room doors lighting
            FadeInDoors();

            instantiatedRoom.room.isLit = true;

        }
    }

    /// <summary>
    /// Fade in the room lighting
    /// </summary>
    private void FadeInRoomLighting()
    {
        // Fade in the lighting for the room tilemaps
        StartCoroutine(FadeInRoomLightingRoutine(instantiatedRoom));
    }

    /// <summary>
    /// Fade in the room lighting coroutine
    /// </summary>
    private IEnumerator FadeInRoomLightingRoutine(InstantiatedRoom instantiatedRoom)
    {
        // Create new material to fade in
        Material material = new Material(GameResources.Instance.variableLitShader);

        instantiatedRoom.groundTilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.decoration1Tilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.decoration2Tilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.frontTilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.minimapTilemap.GetComponent<TilemapRenderer>().material = material;

        for (float i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fadeInTime)
        {
            material.SetFloat("Alpha_Slider", i);
            yield return null;
        }

        // Set material back to lit material
        instantiatedRoom.groundTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.decoration1Tilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.decoration2Tilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.frontTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.minimapTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;


    }

    /// <summary>
    /// Fade in enviorment objects
    /// </summary>
    private void FadeInEnvironmentLighting()
    {
        Material material = new Material(GameResources.Instance.variableLitShader);

        Environment[] environmentComponents = GetComponentsInChildren<Environment>();

        foreach (Environment environmentComponent in environmentComponents)
        {
            if (environmentComponent.spriteRenderer != null)
            {
                environmentComponent.spriteRenderer.material = material;
            }
        }

        StartCoroutine(FadeInEnvironmentLightingRoutine(material, environmentComponents));
    }

    private IEnumerator FadeInEnvironmentLightingRoutine(Material material, Environment[] environmentComponents)
    {
        for (float i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fadeInTime)
        {
            material.SetFloat("Alpha_Slider", i);
            yield return null;
        }

        foreach (Environment environmentComponent in environmentComponents)
        {
            if (environmentComponent.spriteRenderer != null)
            {
                environmentComponent.spriteRenderer.material = GameResources.Instance.litMaterial;
            }
        }
    }

    /// <summary>
    /// Fade in the doors
    /// </summary>
    private void FadeInDoors()
    {
        Door[] doorArray = GetComponentsInChildren<Door>();

        foreach (Door door in doorArray)
        {
            DoorLightingControl doorLightingControl = door.GetComponentInChildren<DoorLightingControl>();

            doorLightingControl.FadeInDoor(door);
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TakeSoylentSchizoController : MonoBehaviour
{
	
	public DollSchizo doll;
	public Image room1, room2, room3, room4;
	public RectTransform positionWhenEnteringRoom3, positionWhenEnteringRoom4;
	public RectTransform[] room1Colliders, room2Colliders, room3Colliders, room4Colliders;
	public float dollFramerateInRoom3, dollFramerateInRoom4;
	
	public Image fearsCover;
	public float timeBeforeDollFacesHisFears;
	public float timeBeforeFearsFlickersIntoEars;
	public AudioSource audioToStopAfterEnteringRoom3;
	public float timeBeforeEndingMinigameAfterEnteringRoom4;
	
	private float elapsedTime;
	private bool isDollFacingHisFears, isFearsFlickering;
	private Image currentRoom;
	private RectTransform dollRt, currentRoomRt;
	private RectTransform[] currentColliders;
	private float timeBeforeNextFlicker;
	private bool hasMinigameEnded;
	
    // Start is called before the first frame update
    void Start()
    {
		elapsedTime = 0f;
		isDollFacingHisFears = false;
		dollRt = doll.GetComponent<RectTransform>();
		UpdateRooms(null, room1);
		UpdateColliders(room1Colliders);
		fearsCover.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
		elapsedTime += Time.deltaTime;
		if (elapsedTime >= timeBeforeDollFacesHisFears && !isDollFacingHisFears)
		{
			elapsedTime = 0;
			isDollFacingHisFears = true;
			UpdateRooms(room1, room2);
			UpdateColliders(room2Colliders);
		}
		
		if (elapsedTime >= timeBeforeFearsFlickersIntoEars && isDollFacingHisFears && !isFearsFlickering)
		{
			elapsedTime = 0;
			isFearsFlickering = true;
			timeBeforeNextFlicker = UnityEngine.Random.Range(0.5f, 5f);
		}
		
		if (elapsedTime >= timeBeforeNextFlicker && isFearsFlickering && currentRoom == room2)
		{
			if (elapsedTime - timeBeforeNextFlicker < 0.2)
			{
				fearsCover.gameObject.SetActive(true);
			}
			else 
			{
				fearsCover.gameObject.SetActive(false);
				timeBeforeNextFlicker = UnityEngine.Random.Range(0.5f, 5f);
				elapsedTime = 0;
			}
		}
		
		if (elapsedTime >= timeBeforeEndingMinigameAfterEnteringRoom4 && currentRoom == room4 && !hasMinigameEnded)
		{
			hasMinigameEnded = true;
			GetComponent<TakeSoylentMinigameManager>().End();
		}
		
		if (dollRt.localPosition.y > (currentRoomRt.rect.height * 0.55f))
		{
			Vector3 localPosition = dollRt.localPosition;
			Vector3 newPosition = new Vector3(localPosition.x, localPosition.y, localPosition.z);
			
			float newFramerate = doll.framerate;
			Image nextRoom = null;
			Image previousRoom = currentRoom;
			RectTransform[] nextRoomColliders = null;
			if (currentRoom == room2)
			{
				audioToStopAfterEnteringRoom3.Stop();
				nextRoom = room3;
				nextRoomColliders = room3Colliders;
				newPosition.Set(positionWhenEnteringRoom3.localPosition.x, positionWhenEnteringRoom3.localPosition.y, positionWhenEnteringRoom3.localPosition.z);
				newFramerate = dollFramerateInRoom3;
			}
			else if (currentRoom == room3)
			{
				nextRoom = room4;
				nextRoomColliders = room4Colliders;
				newPosition.Set(positionWhenEnteringRoom4.localPosition.x, positionWhenEnteringRoom4.localPosition.y, positionWhenEnteringRoom4.localPosition.z);
				newFramerate = dollFramerateInRoom4;
			}
			UpdateRooms(previousRoom, nextRoom);
			UpdateColliders(nextRoomColliders);
			dollRt.localPosition = newPosition;
			doll.framerate = newFramerate;
			
			elapsedTime = 0;
		}
    }

	private void UpdateColliders(RectTransform[] colliders)
	{	
		if (this.currentColliders != null) {
			foreach (RectTransform collider in this.currentColliders)
			{
				collider.gameObject.SetActive(false);
			}
		}
		this.currentColliders = colliders;
		doll.SetColliders(colliders);
	}
	
	private void UpdateRooms(Image previousRoom, Image nextRoom)
	{
		if (previousRoom != null)
		{
			previousRoom.gameObject.SetActive(false);
		}
		if (nextRoom != null)
		{
			nextRoom.gameObject.SetActive(true);
		}
		this.currentRoom = nextRoom;
		this.currentRoomRt = this.currentRoom.GetComponent<RectTransform>();
	}
	
}

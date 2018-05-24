﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class FavListController : MonoBehaviour {		
	public BoxCollider2D TouchCollider;
	public float DetailLength = 2.2f;
	public GameObject ItemDetailPrefabs;
	public float Sensitive = 1.0f;
	public float Smooth = 1.0f;
	//	public int DetailSize = 5;
	public TextMeshPro TextData;
	
	private List<FavItem> itemDetailList = new List<FavItem> ();
	private MorphObject SnappingMorph;
	private float ItemMoveMent = 0.0f;
	private float MoveSpeed = 0.0f;
	private float currentMoveSpeed = 0.0f;
	private bool IsTouch = false;
	private Vector2 lastTouch;
	void Awake()
	{
		TouchCollider.size = new Vector2(TouchCollider.size.x,TouchCollider.size.y + ScreenRatio.SizeDiff()*2);
		TouchCollider.offset = new Vector2(TouchCollider.offset.x,TouchCollider.offset.y - ScreenRatio.SizeDiff());
	}

	// Use this for initialization
	void Start () {
		SnappingMorph = new MorphObject ();
	}
	
	public void CreateFavItem(ContentData[] dataList)
	{
		if (ItemDetailPrefabs == null)
			return;
		ClearItemDetail();
		for (int i = 0; i < dataList.Length; i++) {
			GameObject itemDetail = (GameObject)GameObject.Instantiate(ItemDetailPrefabs);
			itemDetail.SetActive(true);
			itemDetail.transform.parent = this.transform;
			itemDetail.transform.localPosition = new Vector3(0,-i*DetailLength,0);
			FavItem idt = itemDetail.GetComponent<FavItem>();
			idt.SetContentData(ContentPageFile.ModifiedDataForLoad( dataList[i]));
			idt.ReadMoreButton.box = this.GetComponent<Collider2D>();
			idt.WebButton.box = this.GetComponent<Collider2D>();
			idt.DeleteButton.box = this.GetComponent<Collider2D>();
			itemDetailList.Add(itemDetail.GetComponent<FavItem>());
		}
		if (dataList.Length == 0)
			TextData.text = "ไม่พบข้อมูล";
		else
			TextData.text = "จำนวนข้อมูลที่ถูกจัดเก็บ  "+dataList.Length+"/"+PageDetailGlobal.pGlobal.file.maxContent+"  เรื่อง";
	}
	public void ClearItemDetail()
	{
		Debug.Log("ClearItem Deatil List :"+itemDetailList.Count);
		if (itemDetailList == null)
			return;
		for (int i = 0; i < itemDetailList.Count; i++) {
			Debug.Log("Clear Fav Item : "+itemDetailList[i].name);
			Destroy(itemDetailList[i].gameObject);
		}
		itemDetailList.Clear ();
		ItemMoveMent = 0;
	}
	
	IEnumerator SnappingItem()
	{
		while (!SnappingMorph.IsFinish()) {
			SnappingMorph.Update ();
			ItemMoveMent = SnappingMorph.val;
			
			if (ItemMoveMent < 0)
				ItemMoveMent = 0;
			if (ItemMoveMent > (DetailLength * (itemDetailList.Count - 1)))
				ItemMoveMent = DetailLength * (itemDetailList.Count - 1);
			
			for (int i = 0; i < itemDetailList.Count; i++) {
				itemDetailList [i].transform.localPosition = new Vector3 (0,ItemMoveMent -i*DetailLength, 0);
			}
			yield return 0;
		}
	}
	
	void ItemListUpdate () {
		if (IsTouch) {
			Vector2 touchPos = TouchInterface.GetTouchPosition ();
			//calculate last touch
			ItemMoveMent += (touchPos.y - lastTouch.y) * (Sensitive/10.0f);
			MoveSpeed = (touchPos.y - lastTouch.y)* (Sensitive/10.0f) / (Time.deltaTime);
			currentMoveSpeed = MoveSpeed;
			if (ItemMoveMent < 0)
				ItemMoveMent = 0;
			if (ItemMoveMent > (DetailLength * (itemDetailList.Count - 1)))
				ItemMoveMent = DetailLength * (itemDetailList.Count - 1);
			lastTouch.y = touchPos.y;
			SnappingMorph.morphEasein (ItemMoveMent, ItemMoveMent+currentMoveSpeed, Mathf.Abs(MoveSpeed) / DetailLength + Smooth);
			//move all item
			for (int i = 0; i < itemDetailList.Count; i++) {
				itemDetailList [i].transform.localPosition = new Vector3 (0,ItemMoveMent -i*DetailLength, 0);
			}
		} 
	}
	
	// Update is called once per frame
	void Update () {
		if (PageDetailGlobal.state != DetailState.DS_LIST)
			return;
		if (TouchCollider == null)
			return;
		bool touchedDown = TouchInterface.GetTouchDown ();
		bool touchedUp = TouchInterface.GetTouchUp ();
		Vector2 touchPos = TouchInterface.GetTouchPosition ();
		RaycastHit2D hit = Physics2D.Raycast (touchPos, Vector2.zero);
		if (touchedDown) {
			if (TouchCollider == hit.collider) {
				StopCoroutine("SnappingItem");
				IsTouch = true;
				lastTouch = touchPos;
			}
		}
		else if (touchedUp) {
			if (IsTouch) {
				IsTouch = false;
				StopCoroutine("SnappingItem");
				StartCoroutine ("SnappingItem");
			}
		}
		ItemListUpdate ();
	}
}

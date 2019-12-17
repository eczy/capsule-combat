using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MPWinnerText : MonoBehaviour {

	Text text;

	void Start(){
		text = GetComponent<Text> ();
		text.text = "";
	}
		
	public void ShowWinner(string name){
		text.text = name + " WINS!";
	}
}

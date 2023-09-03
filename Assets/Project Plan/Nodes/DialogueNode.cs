using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using XNode;


public class DialogueNode : BaseNode {
	[Input] public Connection input;
	[Output] public Connection exit;

	public string speakerName;
	[TextArea] public string dialogueLine;
	//public Sprite sprite;
	public override string GetString(){ //overriding allows you to create a broad type of object that you can refer to but then get specific data from sub objects  
		return "DialogueNode/" + speakerName + "/" + dialogueLine; 
	}
	public override object GetValue(NodePort port){
		return null;
	}
 
	/* public override Sprite GetSprite(){
		return sprite;

	}  */
}
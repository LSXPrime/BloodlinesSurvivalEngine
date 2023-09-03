using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class SingleFunctionNode : BaseNode
{
	[Input] public Connection input;
	[TextArea] public string DialogueText;
	public CustomAnswer Answer;
	
	public override string GetString(){
		return "SingleFunctionNode/" + DialogueText + "/" + Answer;
	}

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {
		return null; // Replace this
	}
}
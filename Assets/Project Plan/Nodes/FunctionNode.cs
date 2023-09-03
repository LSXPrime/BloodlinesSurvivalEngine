using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class FunctionNode : BaseNode
{
	[Input] public Connection input;
	[TextArea] public string DialogueText;
	[Output(dynamicPortList = true)] public List<CustomAnswer> Answers;
	
	public override string GetString(){
		return "FunctionNode/" + DialogueText + "/" + Answers[0].Answer;
	}
	
	public override string GetReply(){
		return "";//GetPort("Answers " + 0);
	}
	
	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {
		return null; // Replace this
	}
}

[Serializable]
public class CustomAnswer
{
	//XNode	public CustomFunctions Function;
	[TextArea] public string Answer;
}
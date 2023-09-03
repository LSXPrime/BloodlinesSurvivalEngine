using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class StepNode : BaseNode
{
	[Input] public Connection input;
	[Output(dynamicPortList = true)] public List<CustomStep> Steps;
	
	public override string GetString(){
		return "FunctionNode/" + Steps[0].Step;
	}
	
	public override string GetReply(){
		return "";//GetPort("Answers " + 0);
	}
	
	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {
		return null; // Replace this
	}
}
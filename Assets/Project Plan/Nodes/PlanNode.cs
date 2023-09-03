using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class PlanNode : BaseNode
{
	[Input] public Connection input;
	[TextArea] public string Plan;
	[Output(dynamicPortList = true)] public List<CustomStep> Milestones;
	public bool Completed;
	
	public override string GetString(){
		return "FunctionNode/" + Plan + "/" + Milestones[0].Step;
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
public class CustomStep
{
	[TextArea] public string Step;
	public bool Done;
}
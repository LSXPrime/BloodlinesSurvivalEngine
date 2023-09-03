using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class BaseNode : Node {

	public virtual string GetString(){ //adding "virtual" keyword to a function lets you override in scripts that inherit from this one 
		return null;
	}
	
	public virtual string GetReply(){ //adding "virtual" keyword to a function lets you override in scripts that inherit from this one 
		return null;
	}

	public override object GetValue(NodePort port){
		return null;
	}

	public virtual string GetNodeType(){ 
		return null;
	}
}
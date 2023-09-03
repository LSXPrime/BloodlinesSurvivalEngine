using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class ExitNode : BaseNode{

    [Input] public Connection entry;
    //[Output] public Connection exit;
    public override string GetString()
    {
        return "CloseDialogue_ExitNode";
    }
    public override object GetValue(NodePort port){
		return null;
	}

    
}

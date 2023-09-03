using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[Serializable]
public struct Connection {}
public class ChoiceDialogueNode : BaseNode {
    [Input] public Connection input;
    //[Output] public Connection exit;
    [Output(dynamicPortList = true)] public List<string> Answers;
    public string speakerName; 
    [TextArea] public string DialogueText;
     
    public override string GetString(){
		return "ChoiceDialogueNode/" + speakerName + "/" + DialogueText + "/" + Answers[0];
	}

    public override object GetValue(NodePort port){
        return null;
    }


  
}

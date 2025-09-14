using Godot;
using System;

public partial class NPCManager:Node
{
    public Card currentNpcCard;
    public int customCounter;

    public async void GiveNpcCard(string cardName, int num)
    {
        for(int i = 0; i < num; i++)
        {
            await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
            GlobalManager.GetInfos().AddNewCard(cardName, GlobalManager.GetInfos().HandDeck,GlobalManager.GetInfos().HandDeck.GlobalPosition);
        }
    }

    public void EscDialogue(){
        currentNpcCard?.EscDialogue();
    }

    public void ClearCounter(){
        customCounter = 0;
    }

    public void Leave(){
    }
}

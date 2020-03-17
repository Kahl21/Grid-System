using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HistoryHandler
{
    static Stack<string> _history;
    static Stack<string> _alteredHistory;
    static Stack<string> _gridhistory;
    static Stack<string> _alteredGridHistory;

    static int _actionsTaken = 0;
    static string _actionSentence;

    public static void Init()
    {
        _history = new Stack<string>();
        _alteredHistory = new Stack<string>();
        _gridhistory = new Stack<string>();
        _alteredGridHistory = new Stack<string>();
        _actionSentence = "";
    }

    static public void DeclareDeath(Character deadCharacter)
    {
        AddToCurrentAction(" \n" + deadCharacter.Name + " has been slain!");
        FightHandler.LastManStanding(deadCharacter);
    }

    //add a section of the current action to the action statment
    static public void AddToCurrentAction(string addition)
    {
        _actionSentence += " " + addition;
    }

    static public void SaveCurrentGridLayout(string gridlayout)
    {
        _gridhistory.Push(gridlayout);
    }

    //add the current action to the history
    //then reset for next action
    static public void FinalizeAction()
    {
        _actionsTaken++;
        GridHandler.FinalizeGridLayoutForTurn();
        _history.Push(_actionSentence);
        _actionSentence = " ";
    }

    //Sends back currently view part of battle as string
    static public string GetCurrentHistory()
    {
        return _history.Peek();
    }

    static public string GetCurrentGridHistory()
    {
        return _gridhistory.Peek();
    }

    static public void SetToBeginning()
    {
        try
        {
            for (int i = _history.Count; i > 1; i = _history.Count)
            {
                _actionsTaken--;
                _alteredHistory.Push(_history.Peek());
                _alteredGridHistory.Push(_gridhistory.Peek());
                _history.Pop();
                _gridhistory.Pop();
            } 
        }
        catch
        {
            //Debug.Log("died in beginning");
        }
        
    }

    //removes the top of the history stack 
    //adds it to the altered stack incase we go back
    static public void ReverseHistory()
    {
        if (_history.Count > 1)
        {
            _actionsTaken--;
            _alteredHistory.Push(_history.Peek());
            _alteredGridHistory.Push(_gridhistory.Peek());
            _history.Pop();
            _gridhistory.Pop();
        }
    }

    //removes the top of the altered stack
    //adds it to the history stack
    static public void AdvanceHistory()
    {
        if (_alteredHistory.Count > 0)
        {
            _actionsTaken++;
            _history.Push(_alteredHistory.Peek());
            _gridhistory.Push(_alteredGridHistory.Peek());
            _alteredHistory.Pop();
            _alteredGridHistory.Pop();
        }
        else
        {
            if(!FightHandler.LastManStanding())
            {
                FightHandler.ContinueFight();
            }
        }
    }

    static public int GetCurrentActionNumber()
    {
        return _actionsTaken;
    }

}

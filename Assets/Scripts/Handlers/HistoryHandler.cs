using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HistoryHandler
{
    static Stack<string> _history;
    static Stack<string> _alteredHistory;
    static Stack<string> _gridhistory;
    static Stack<string> _alteredGridHistory;
    static Stack<string> _characterInfo;
    static Stack<string> _alteredCharacterInfo;

    static int _actionsTaken = 0;
    static string _actionSentence;

    static UIHolder _uiRef;

    public static void Init(UIHolder reference)
    {
        _uiRef = reference;
        _history = new Stack<string>();
        _alteredHistory = new Stack<string>();
        _gridhistory = new Stack<string>();
        _alteredGridHistory = new Stack<string>();
        _characterInfo = new Stack<string>();
        _alteredCharacterInfo = new Stack<string>();
        _actionSentence = "";

    }

    static public void DeclareDeath(Character deadCharacter)
    {
        AddToCurrentAction(" \n" + deadCharacter.Name + " has been slain!");
        FightHandler.LastManStanding();
    }

    //add a section of the current action to the action statment
    static public void AddToCurrentAction(string addition)
    {
        _actionSentence += " " + addition;
    }

    //add the current action to the history
    //then reset for next action
    //Called if no Character has acted
    static public void FinalizeAction()
    {
        _actionsTaken++;
        GridHandler.FinalizeGridLayoutForTurn();
        _characterInfo.Push("");
        _history.Push(_actionSentence);
        _actionSentence = " ";
    }

    //add the current grid layout, character stats, and action to the history
    //then reset for next action
    static public void FinalizeAction(Character characterThatActed)
    {
        _actionsTaken++;
        GridHandler.FinalizeGridLayoutForTurn();
        SaveCharacterInfo(characterThatActed);
        _history.Push(_actionSentence);
        _actionSentence = " ";
    }

    static public void SaveCurrentGridLayout(string gridlayout)
    {
        _gridhistory.Push(gridlayout);
    }
    
    static void SaveCharacterInfo(Character chara)
    {
        string characterInfo = "" + chara.Name + "\n"
                            + "Weapon: " + chara.HeldWeapon.Name + " --> " + chara.HeldWeapon.WeaponElement + "\n\n"
                            + "Health: " + chara.CurrHealth + "/" + chara.MaxHealth + "\n"
                            + "Strength: " + (chara.Offense.Strength + chara.HeldWeapon.StrengthMod) + " (" + chara.HeldWeapon.StrengthMod + ")" + "\n"
                            + "Magic: " + (chara.Offense.Strength + chara.HeldWeapon.MagicMod) + "(" + chara.HeldWeapon.MagicMod + ")" + "\n"
                            + "Defense: " + chara.Defense.BaseDefense + "\n"
                            + "Accuracy: " + (chara.Offense.Accuracy + chara.HeldWeapon.Accuracy) + "%" + " (" + chara.HeldWeapon.Accuracy + ")" + "\n"
                            + "Critical Chance: " + (chara.Offense.CriticalChance + chara.HeldWeapon.Crit) + "%" + " (" + chara.HeldWeapon.Crit + ")" + "\n"
                            + "Speed: " + chara.Speed + "\n"
                            + "Movement: " + chara.Movement + "\n\n"
                            + "Resistances: Fire/" + chara.Defense.FireRes + "    Ice/" + chara.Defense.IceRes + "\n"
                            + "             Thunder/" + chara.Defense.ThunderRes + "  Dark/" + chara.Defense.DarkRes + "\n";

        if (chara.LastAttacker != null)
        {
            characterInfo += "Current Target: " + chara.LastAttacker.Name + "\n";
        }
        else
        {
            characterInfo += "None" + "\n";
        }
        
        _characterInfo.Push(characterInfo);
    }

    //Sends back current view of battle history as string
    static public string GetCurrentHistory()
    {
        return _history.Peek();
    }

    //Sends back current view of Grid history as string
    static public string GetCurrentGridHistory()
    {
        return _gridhistory.Peek();
    }

    //Sends back current acting Character stats in history as string
    static public string GetCurrentCharacterInfo()
    {
        return _characterInfo.Peek();
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
                _alteredCharacterInfo.Push(_characterInfo.Peek());
                _history.Pop();
                _gridhistory.Pop();
                _characterInfo.Pop();
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
            _alteredCharacterInfo.Push(_characterInfo.Peek());
            _history.Pop();
            _gridhistory.Pop();
            _characterInfo.Pop();
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
            _characterInfo.Push(_alteredCharacterInfo.Peek());
            _alteredHistory.Pop();
            _alteredGridHistory.Pop();
            _alteredCharacterInfo.Pop();
        }
        else
        {
            if(!FightHandler.LastManStanding())
            {
                FightHandler.ContinueFight();
            }
            else
            {
                _uiRef.StopAuto();
            }
        }
    }

    static public int GetCurrentActionNumber()
    {
        return _actionsTaken;
    }

}

using System;
using System.Collections.Generic;
using System.IO;
public sealed class ScoreHandler
{
    private int participantsNumber;
    private string scoresPath = @"/src/Resources/scores.txt";
    private List<int> scores;
    private int challangeCount;
    public ScoreHandler(int currParticipantNumber, int challangeCount)
    {
        participantsNumber = currParticipantNumber;
        this.challangeCount = challangeCount;      
        DefineScores();
    }

    private void DefineScores(){
        string[] lines = File.ReadAllLines(scoresPath);

        foreach (var line in lines)
        {
            int s = Convert.ToInt32(line);
            if (s != -1)
            {
                scores.Add(s);
            }
        }
    }   

    public void AdjustScoreForAll(Dictionary<AuthToken, UserInfo> users){
        participantsNumber++;

        foreach (var user in users)
        {
            user.Value.Score++; 
        }
    }

    public void ScoreUser(UserInfo user, int problemNum){
        user.Score += scores[problemNum] + participantsNumber;

        scores[problemNum] = scores[problemNum] - 1;
    }
}
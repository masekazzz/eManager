using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

[DataContract]
public class Tournament
{
    [DataMember]
    public Dictionary<Team, string> invitedTeams = new Dictionary<Team, string>();

    [DataMember]
    public string name;

    [DataMember]
    public int prizePool;

    [DataMember]
    public int day = 1;

    [DataMember]
    public bool managerTeamInvited;

    [DataMember]
    public string dayResults;

    [DataMember]
    public int count = 1;

    public RectTransform tournamentTable;

    public Dictionary<int, string> currentPlace = new Dictionary<int, string>
    {
        [1] = "9-16",
        [2] = "5-8",
        [3] = "3-4",
        [4] = "2",
    };

    public void InviteTeams(List<Team> teams, Team managerTeam)
    {
        List<Team> newTeams = new List<Team>();
        newTeams.Add(managerTeam);
        foreach (var team in teams)
            newTeams.Add(team);
        newTeams.Sort();
        for (int i = 0; i < 16; i++)
        {
            invitedTeams.Add(newTeams[i], "stillPlaying");
        }
        managerTeamInvited = invitedTeams.ContainsKey(managerTeam);
    }

    public void NextDay()
    {
        dayResults = "";
        List<Team> stillPlayingTeams = (from t in invitedTeams.Keys
                                        where invitedTeams[t] == "stillPlaying"
                                        select t).ToList();
        if (day < 4)
        {
            for (int i = 0; i < stillPlayingTeams.Count; i += 2)
            {
                int score1 = 0;
                int score2 = 0;
                for (int j = 0; j < 3; j++)
                {
                    if (score1 == 2 || score2 == 2)
                        break;
                    if (stillPlayingTeams[i].MatchSimulation(stillPlayingTeams[i + 1]).Contains("lose"))
                    {
                        score2++;
                    }
                    else
                    {
                        score1++;
                    }
                }
                dayResults += stillPlayingTeams[i].TeamName + ";" + score1 + ";" + stillPlayingTeams[i + 1].TeamName + ";" + score2 + "\n";
                if (score1 < score2)
                {
                    invitedTeams[stillPlayingTeams[i]] = currentPlace[day];
                }
                else
                {
                    invitedTeams[stillPlayingTeams[i + 1]] = currentPlace[day];
                }
            }
        }
        else
        {
            int score1 = 0;
            int score2 = 0;
            for (int i = 0; i < 5; i++)
            {
                if (score1 == 3 || score2 == 3)
                    break;
                if (stillPlayingTeams[0].MatchSimulation(stillPlayingTeams[1]).Contains("lose"))
                {
                    score2++;
                }
                else
                {
                    score1++;
                }
            }
            dayResults += stillPlayingTeams[0].TeamName + ";" + score1 + ";" + stillPlayingTeams[1].TeamName + ";" + score2 + "\n";
            if (score1 < score2)
            {
                invitedTeams[stillPlayingTeams[0]] = currentPlace[day];
                invitedTeams[stillPlayingTeams[1]] = "1";
            }
            else
            {
                invitedTeams[stillPlayingTeams[1]] = currentPlace[day];
                invitedTeams[stillPlayingTeams[0]] = "1";
            }
        }
        day++;
    }
}

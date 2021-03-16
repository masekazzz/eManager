using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using TMPro;
using System.Linq;

[DataContract]
public class Team : IComparable<Team>
{
    [DataMember]
    public Dictionary<uint, Player> players = new Dictionary<uint, Player>();

    public void AddToTeam(Player player)
    {
        if (players == null)
        {
            players = new Dictionary<uint, Player>();
        }
        if (!players.ContainsKey(player.position))
            players.Add(player.position, player);
        else
            players[player.position] = player;
        player.inTeam = true;
        ChangeSkillLevels();
        ChangeGameStyle();
    }

    public void RemoveFromTeam(Player player)
    {
        if (players.Count == 1)
            players = new Dictionary<uint, Player>();
        else
            players.Remove(player.position);
        player.inTeam = false;
        daysInTeam = 1;
        teamPlayLevel -= 3;
        if (teamPlayLevel < 0)
            teamPlayLevel = 0;
        ChangeSkillLevels();
        ChangeGameStyle();
    }

    public void ChangeGameStyle()
    {
        int sum = 0;
        foreach (var player in players.Values)
        {
            switch (player.gameStyle)
            {
                case ("Aggresive"):
                    sum++;
                    break;
                case ("Passive"):
                    sum--;
                    break;
                default:
                    break;
            }
        }
        if (sum < -1)
            gameStyle = "Passive";
        else if (sum < 2)
            gameStyle = "Adaptable";
        else
            gameStyle = "Aggresive";
    }

    public void ChangeSkillLevels()
    {
        double sum = 0;
        foreach (Player player in players.Values)
        {
            sum += player.socialSkills;
        }
        if (players.Values.Count != 0)
            atmosphereLevel = ((sum + daysInTeam) / players.Values.Count) - 4;
        else
            atmosphereLevel = 0;
        if (atmosphereLevel < 0)
            atmosphereLevel = 0;
        if (players.Values.Count < 5)
            strategiesLevel = 0;
        else
            strategiesLevel = (0.6 * players[4].gamingSkills + 0.2 * players[5].gamingSkills + 0.15 * players[3].gamingSkills + 0.05 * players[2].gamingSkills + 0.2 * daysInTeam) - 4;
        if (strategiesLevel > 10)
            strategiesLevel = 10;
    }

    public void NextDay()
    {
        if (daysInTeam++ % 5 == 0)
            teamPlayLevel++;
        if (teamPlayLevel > 10)
            teamPlayLevel = 10;
        strategiesLevel = (0.6 * players[4].gamingSkills + 0.2 * players[5].gamingSkills + 0.15 * players[3].gamingSkills + 0.05 * players[2].gamingSkills + 0.2 * daysInTeam) - 4;
        double sum = 0;
        foreach (Player player in players.Values)
        {
            sum += player.socialSkills;
        }
        if (sum >= 30)
        {
            double temp = Player.rnd.NextDouble() * ((double)daysInTeam / (double)sum);
            temp *= onBootcamp > 0 ? 1.2 : 1;
            atmosphereLevel += Player.rnd.Next(2) == 1 ? 0 : temp;
            temp = Player.rnd.NextDouble() * ((double)daysInTeam / (double)sum) / 2;
            temp *= onBootcamp > 0 ? 0.8 : 1;
            atmosphereLevel -= Player.rnd.Next(2) == 1 ? 0 : temp;
        }
        else
        {
            double temp = Player.rnd.NextDouble() * ((double)daysInTeam / (double)sum);
            temp *= onBootcamp > 0 ? 0.8 : 1;
            atmosphereLevel -= Player.rnd.Next(2) == 1 ? 0 : temp;
            temp = Player.rnd.NextDouble() * ((double)daysInTeam / (double)sum) / 2;
            temp *= onBootcamp > 0 ? 1.2 : 1;
            atmosphereLevel += Player.rnd.Next(2) == 1 ? 0 : temp;
        }
    }

    public string MatchSimulation(Team team2)
    {
        double numberTeam1 = Player.rnd.NextDouble() * (0.3 * strategiesLevel + 0.3 * teamPlayLevel + 0.3 * atmosphereLevel + 0.1 * (from p in players.Values select p.gamingSkills).Sum());
        double numberTeam2 = Player.rnd.NextDouble() * (0.3 * team2.strategiesLevel + 0.3 * team2.teamPlayLevel + 0.3 * team2.atmosphereLevel + 0.1 * (from p in team2.players.Values select p.gamingSkills).Sum());
        double difference;
        string res;
        if (this > team2)
            difference = numberTeam1 * 1.3 - numberTeam2;
        else if (this == team2)
            difference = numberTeam1 - numberTeam2;
        else
            difference = numberTeam1 - numberTeam2 * 1.3;
        if (difference > 0)
            res = "win ";
        else
            res = "lose ";
        if (Math.Abs(difference) >= 2)
            res += "Total domination";
        else if (Math.Abs(difference) >= 1)
            res += "Equal game";
        else
            res += "Close game";
        return res;
    }

    public int CompareTo(Team other)
    {
        if ((gameStyle == "Passive" && other.gameStyle == "Adaptable") || (gameStyle == "Adaptable" && other.gameStyle == "Aggresive") || (gameStyle == "Aggresive" && other.gameStyle == "Passive"))
            return -1;
        else if (this == other)
            return 0;
        return 1;
    }

    public static bool operator >(Team team1, Team team2)
    {
        if ((team1.gameStyle == "Passive" && team2.gameStyle == "Adaptable") || (team1.gameStyle == "Adaptable" && team2.gameStyle == "Aggresive") || (team1.gameStyle == "Aggresive" && team2.gameStyle == "Passive"))
            return true;
        return false;
    }

    public static bool operator <(Team team1, Team team2)
    {
        if ((team1.gameStyle == "Passive" && team2.gameStyle == "Adaptable") || (team1.gameStyle == "Adaptable" && team2.gameStyle == "Aggresive") || (team1.gameStyle == "Aggresive" && team2.gameStyle == "Passive"))
            return false;
        return true;
    }

    public static bool operator ==(Team team1, Team team2) => team1.Equals(team2);

    public static bool operator !=(Team team1, Team team2) => !team1.Equals(team2);

    [DataMember]
    public string pathIcon { get; set; }

    [DataMember]
    public string TeamName { get; set; }

    [DataMember]
    public double teamPlayLevel = 0;

    [DataMember]
    public double strategiesLevel;

    [DataMember]
    public double atmosphereLevel;

    [DataMember]
    public string gameStyle;

    [DataMember]
    public int daysInTeam = 1;

    [DataMember]
    public int onBootcamp = 0;

    public Team(string teamName, string pathIcon)
    {
        TeamName = teamName;
        this.pathIcon = pathIcon;
    }
}

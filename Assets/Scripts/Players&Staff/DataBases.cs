using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using UnityEngine;
using UnityEngine.UI;

public class DataBases : MonoBehaviour
{
    public List<Team> teams;
    public List<Player> players;
    public List<Tournament> tournaments;

    public void ReadPlayers()
    {
        using (FileStream fs = new FileStream("Databases/players.ser", FileMode.Open, FileAccess.Read))
        {
            DataContractJsonSerializer format = new DataContractJsonSerializer(typeof(List<Player>));
            players = (List<Player>)format.ReadObject(fs);
        }
        foreach (var player in players)
        {
            player.pathIcon = $"Icons/{player.NickName}";
            player.status = "Relax";
        }
        ReadTeams();
    }

    public void ReadTeams()
    {
        using (FileStream fs = new FileStream("Databases/teams.ser", FileMode.Open, FileAccess.Read))
        {
            DataContractJsonSerializer format = new DataContractJsonSerializer(typeof(List<Team>));
            teams = (List<Team>)format.ReadObject(fs);
        }
        foreach (var team in teams)
        {
            team.pathIcon = $"Icons/{team.TeamName}";
            for (int i = 1; i < 6; i++)
            {
                var availablePlayers = players.FindAll(p => p.position == i && p.inTeam == false);
                var smth = availablePlayers[Player.rnd.Next(availablePlayers.Count)];
                team.AddToTeam(smth);
            }
        }
        ReadTournaments();
    }
    
    public void ReadTournaments()
    {
        using (FileStream fs = new FileStream("Databases/tournaments.ser", FileMode.Open, FileAccess.Read))
        {
            DataContractJsonSerializer format = new DataContractJsonSerializer(typeof(List<Tournament>));
            tournaments = (List<Tournament>)format.ReadObject(fs);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

[DataContract]
public class Player
{
    public static readonly System.Random rnd = new System.Random();

    public Manager manager;

    [DataMember]
    public uint age;

    [DataMember]
    public uint cost;

    [DataMember]
    public string Nationality { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string NickName { get; set; }

    [DataMember]
    public double gamingSkills;

    [DataMember]
    public double socialSkills;

    [DataMember]
    public double mediaSkills;

    [DataMember]
    public uint position { get; set; }

    [DataMember]
    public string gameStyle;

    [DataMember]
    public string pathIcon { get; set; }

    public bool inTeam;

    public string status;

    public int fatigue = 0;

    public void NextDay()
    {
        if (manager.team.players.ContainsValue(this))
        {
            if (fatigue > 4)
            {
                manager.mail.AddLetter(new Tuple<string, string>(NickName, $"Hello {manager.NickName}, I’m very tired of the last days of grueling training, could you give me some rest?\n\r" +
                    $"If I continue to work as much, my skills may decline.\n\rThank in Advance!"));
            }
            switch (status)
            {
                case ("Develop strategies"):
                    if (position == 4 || position == 5)
                        gamingSkills += rnd.Next(11) > 5 ? 0 : rnd.NextDouble() * 0.22;
                    else
                        gamingSkills += rnd.Next(11) > 5 ? 0 : rnd.NextDouble() * 0.11;
                    fatigue++;
                    break;
                case ("Exercise"):
                    if (!(position == 4 || position == 5))
                        gamingSkills += rnd.Next(11) > 5 ? 0 : rnd.NextDouble() * 0.22;
                    else
                        gamingSkills += rnd.Next(11) > 5 ? 0 : rnd.NextDouble() * 0.11;
                    fatigue++;
                    break;
                case ("Psychologist 1000$"):
                    if (socialSkills < 5 && socialSkills > -1)
                        socialSkills += rnd.Next(11) > 5 ? 0 : rnd.NextDouble() * (11 - socialSkills) / 18;
                    else
                        socialSkills += rnd.Next(11) > 5 ? 0 : rnd.NextDouble() * (11 - socialSkills) / 10;
                    fatigue++;
                    break;
                case ("Relax"):
                    fatigue -= 2;
                    if (fatigue < 0)
                        fatigue = 0;
                    break;
            }
            if (gamingSkills > 10)
                gamingSkills = 10;
            if (socialSkills > 10)
                socialSkills = 10;
            status = "Relax";
            if (fatigue > 5)
            {
                socialSkills -= 0.15 * (fatigue - 5);
                gamingSkills -= 0.15 * (fatigue - 5);
                if (gamingSkills < 0)
                    gamingSkills = 0;
                if (socialSkills < 0)
                    socialSkills = 0;
            }
        }
    }

}

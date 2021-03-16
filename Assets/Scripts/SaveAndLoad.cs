using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class SaveAndLoad : MonoBehaviour
{
    public Manager manager;
    public DataBases dataBases;
    public Mail mail;
    public HistoryMenu historyMenu;
    public PlayerCardsAppearance pca;
    public GameObject MainMenu, Game, RawImage;
    public void Save()
    {
        if (!Directory.Exists("Save"))
            Directory.CreateDirectory("Save");
        using (FileStream fs = new FileStream("Save/Save.ser", FileMode.Create))
        {
            DataContractJsonSerializer format = new DataContractJsonSerializer(typeof(GameSave));
            format.WriteObject(fs, new GameSave(new ManagerSave(manager.NickName, manager.sponsorsSalary, manager.money, manager.fans, manager.matches, manager.team, manager.day,
                manager.currentTournament, manager.currentTournamentTable, manager.team.pathIcon), new DataBasesSave(dataBases.teams, dataBases.players, dataBases.tournaments,
                (from t in dataBases.teams select t.players).ToList()), new MailSave(mail.letters, mail.firstTime), new HistoryMenuSave(historyMenu.tournaments, historyMenu.clanwars)));
        }
    }

    public void Load()
    {
        if (File.Exists("Save/Save.ser"))
        {
            Game.SetActive(true);
            MainMenu.SetActive(false);
            RawImage.SetActive(true);
            ManagerSave managerSave;
            DataBasesSave dataBasesSave;
            MailSave mailSave;
            HistoryMenuSave historyMenuSave;
            using (FileStream fs = new FileStream("Save/Save.ser", FileMode.Open, FileAccess.Read))
            {
                DataContractJsonSerializer format = new DataContractJsonSerializer(typeof(GameSave));
                var temp = format.ReadObject(fs);
                managerSave = ((GameSave)temp).managerSave;
                dataBasesSave = ((GameSave)temp).dataBasesSave;
                mailSave = ((GameSave)temp).mailSave;
                historyMenuSave = ((GameSave)temp).historyMenuSave;
            }
            RawImage.GetComponent<RawImage>().texture = Resources.Load<Texture2D>(managerSave.pathIcon);
            manager.NickName = managerSave.NickName;
            manager.sponsorsSalary = managerSave.sponsorsSalary;
            manager.money = managerSave.money;
            manager.fans = managerSave.fans;
            manager.matches = managerSave.matches;
            manager.team = managerSave.team;
            manager.team.pathIcon = managerSave.pathIcon;
            if (manager.team.players == null)
                manager.team.players = new Dictionary<uint, Player>();
            manager.day = managerSave.day;
            dataBases.teams = dataBasesSave.teams;
            for (int i = 0; i < dataBasesSave.teams.Count; i++)
            {
                dataBases.teams[i].players = dataBasesSave.teamsPlayers[i];
                dataBases.teams[i].ChangeSkillLevels();
                dataBases.teams[i].ChangeGameStyle();
                dataBases.teams[i].pathIcon = $"Icons/{dataBasesSave.teams[i].TeamName}";
            }
            dataBases.players = dataBasesSave.players;
            for (int i = 0; i < dataBasesSave.players.Count; i++)
            {
                dataBases.players[i].pathIcon = $"Icons/{dataBasesSave.players[i].NickName}";
            }
            dataBases.tournaments = dataBasesSave.tournaments;
            mail.letters = mailSave.letters;
            mail.firstTime = mailSave.firstTime;
            historyMenu.tournaments = historyMenuSave.tournaments;
            historyMenu.clanwars = historyMenuSave.clanwars;
            mail.firstTime = true;
            pca.OnClick();
            pca.SkillLevelLinesAppearance(manager);
            manager.StartGame();
            GameObject.Find("AmountOfDays").GetComponent<TextMeshProUGUI>().text = manager.day.ToString();
            manager.ChangeAmountOfFans();
            manager.ChangeMoney();
            manager.CreateEventNextDay();
            if (managerSave.currentTournament != null)
            {
                foreach (var team in managerSave.currentTournament.invitedTeams.Keys)
                {
                    team.players = dataBases.teams.Find(t => t.TeamName == team.TeamName).players;
                }
                manager.currentTournament = managerSave.currentTournament;
                manager.currentTournament.currentPlace = new Dictionary<int, string>
                {
                    [1] = "13-16",
                    [2] = "5-8",
                    [3] = "3-4",
                    [4] = "2",
                };
                manager.currentTournamentTable = GameObject.Instantiate(manager.TournamentTable);
                manager.currentTournamentTable.transform.SetParent(GameObject.Find("Canvas").transform, false);
                manager.currentTournamentTable.transform.Find("Button").GetComponent<Button>().onClick.AddListener(
                    () =>
                    {
                        manager.currentTournamentTable.gameObject.SetActive(false);
                        mail.MailRefresh();
                    }
                );
                managerSave.currentTournamentTable.CreateRectTransform(manager.currentTournamentTable);
                manager.currentTournamentTable.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = manager.currentTournament.name + $"\n\rDay {manager.currentTournament.day - 1}";
                manager.currentTournamentTable.gameObject.SetActive(false);
            }
        }
    }
    [DataContract]
    public class GameSave
    {
        [DataMember]
        public ManagerSave managerSave;
        [DataMember]
        public DataBasesSave dataBasesSave;
        [DataMember]
        public MailSave mailSave;
        [DataMember]
        public HistoryMenuSave historyMenuSave;

        public GameSave(ManagerSave ms, DataBasesSave dbs, MailSave Ms, HistoryMenuSave hms)
        {
            managerSave = ms;
            dataBasesSave = dbs;
            mailSave = Ms;
            historyMenuSave = hms;
        }
    }

    [DataContract]
    public class ManagerSave
    {
        [DataMember]
        public Tournament currentTournament;
        [DataMember]
        public string NickName;
        [DataMember]
        public int sponsorsSalary;
        [DataMember]
        public int money;
        [DataMember]
        public int fans;
        [DataMember]
        public List<Team> matches;
        [DataMember]
        public Team team;
        [DataMember]
        public int day;
        [DataMember]
        public HistoryMenu.TournamentTable currentTournamentTable;
        [DataMember]
        public string pathIcon;

        public ManagerSave(string NickName, int sponsorsSalary, int money, int fans, List<Team> matches, Team team, int day, Tournament currentTournament,RectTransform currentTournamentTable, string pathIcon)
        {
            this.NickName = NickName;
            this.sponsorsSalary = sponsorsSalary;
            this.money = money;
            this.fans = fans;
            this.matches = matches;
            this.team = team;
            this.day = day;
            this.currentTournament = currentTournament;
            if (currentTournamentTable != null)
                this.currentTournamentTable = new HistoryMenu.TournamentTable(currentTournamentTable);
            this.pathIcon = pathIcon;
        }
    }
    [DataContract]
    public class DataBasesSave
    {
        [DataMember]
        public List<Team> teams;
        [DataMember]
        public List<Player> players;
        [DataMember]
        public List<Tournament> tournaments;
        [DataMember]
        public List<Dictionary<uint, Player>> teamsPlayers;
        public DataBasesSave(List<Team> teams, List<Player> players, List<Tournament> tournaments, List<Dictionary<uint, Player>> teamsPlayers)
        {
            this.teams = teams;
            this.players = players;
            this.tournaments = tournaments;
            this.teamsPlayers = teamsPlayers;
        }
    }
    [DataContract]
    public class MailSave
    {
        [DataMember]
        public Dictionary<Tuple<string, string>, bool> letters;
        [DataMember]
        public bool firstTime;

        public MailSave(Dictionary<Tuple<string, string>, bool> letters, bool firstTime)
        {
            this.letters = letters;
            this.firstTime = firstTime;
        }
    }
    [DataContract]
    public class HistoryMenuSave
    {
        [DataMember]
        public List<HistoryMenu.TournamentTable> tournaments;
        [DataMember]
        public List<HistoryMenu.GameInformation> clanwars;
        public HistoryMenuSave(List<HistoryMenu.TournamentTable> tournaments, List<HistoryMenu.GameInformation> clanwars)
        {
            this.tournaments = tournaments;
            this.clanwars = clanwars;
        }
    }
}

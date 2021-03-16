using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    public Tournament currentTournament;

    public string NickName;

    public int sponsorsSalary = 10000;

    public int money;

    public int fans;

    public List<Team> matches = new List<Team>();

    public Team team;

    public int day;

    public DataBases dataBases;

    public delegate void EventHandler();

    private static event EventHandler nextDay;

    public RectTransform gameResults;

    public GameObject youLose;

    public RectTransform TournamentTable;

    public RectTransform currentTournamentTable;

    public Mail mail;

    public HistoryMenu historyMenu;

    public string iconPath;

    public void StartGame()
    {
         GameObject.Find("RawImage").GetComponent<RawImage>().transform.position = new Vector3(-8.23F, 4.35F, 
             GameObject.Find("RawImage").GetComponent<RawImage>().transform.position.z); // for build
        Color color = new Color(0, 0, 0);
        GameObject.Find("Main Camera").GetComponent<Camera>().backgroundColor = color;
    }
    public void NewGame()
    {
        day = 0;
        money = 60000;
        fans = 0;
        ChangeMoney();
        ChangeAmountOfFans();
        ChangeDay();
        if (currentTournamentTable != null)
            Destroy(currentTournamentTable.gameObject);
        if (currentTournament != null)
            currentTournament = null;
        historyMenu.tournaments = new List<HistoryMenu.TournamentTable>();
        historyMenu.clanwars = new List<HistoryMenu.GameInformation>();
        historyMenu.changed = true;
    }
    public void CreateEventNextDay()
    {
        if (GameObject.Find("MailButton(Clone)") == null)
            mail.firstTime = true;
        if (nextDay == null)
        {
            nextDay += new EventHandler(SimulateMatches);
            nextDay += new EventHandler(SimulateTournament);
            nextDay += new EventHandler(StartTournament);
            nextDay += new EventHandler(delegate ()
            {
                if (day % 14 == 0)
                {
                    money += sponsorsSalary;
                    foreach (var player in team.players.Values)
                    {
                        money -= (int)player.cost;
                    }
                }
            }
            );
            foreach (var player in dataBases.players)
            {
                nextDay += new EventHandler(player.NextDay);
                player.manager = this;
            }
            nextDay += new EventHandler(ChangeDay);
            nextDay += new EventHandler(ChangeMoney);
            nextDay += new EventHandler(ChangeAmountOfFans);
            nextDay += new EventHandler(BootCampButton);
            nextDay += new EventHandler(OtherTeamsAndPlayersActions);
            nextDay += new EventHandler(mail.MailRefresh);
            nextDay += new EventHandler(Lose);
        }
        if (mail != null)
            mail.letters = new Dictionary<Tuple<string, string>, bool>();
        mail.AddLetter(new Tuple<string, string>("Sponsors", "Congratulations on creating your team! " +
        "We are ready to help you financially, paying out 10000$ every 14 days, also depending " +
        "on your successes or failures, payments may increase or decrease accordingly"));
        mail.MailRefresh();
    }

    public void CreateTeam()
    {
        string TeamName = GameObject.Find("InputFieldForTeamName").transform.Find("Text Area").transform.Find("Text").GetComponent<TextMeshProUGUI>().text;
        NickName = GameObject.Find("InputFieldForNickName").transform.Find("Text Area").transform.Find("Text").GetComponent<TextMeshProUGUI>().text;
        if (TeamName.Length == 1)
            TeamName = "Team Unknown";
        if (NickName.Length == 1)
            NickName = "SomeCoolNickname";
        if (iconPath == "")
            iconPath = "Icons/1";
        team = new Team(TeamName, iconPath);
    }

    public void NextDay() => nextDay();

    public void BootCampButton()
    {
        if (team.onBootcamp != 0)
        {
            team.onBootcamp--;
            GameObject.Find("BootCampButton").GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = $"On bootcamp: {team.onBootcamp} days left";
        }
        else
        {
            var button = GameObject.Find("BootCampButton").GetComponent<Button>();
            button.interactable = true;
            button.GetComponentInChildren<TextMeshProUGUI>().text = "Bootcamp 5000$";
            button.GetComponentInChildren<TextMeshProUGUI>().enableAutoSizing = false;
            button.GetComponentInChildren<TextMeshProUGUI>().fontSize = 24;
        }
    }

    public void ChangeMoney() => GameObject.Find("Money").GetComponent<TextMeshProUGUI>().text = money + "$";

    public void ChangeAmountOfFans() => GameObject.Find("AmountOfFans").GetComponent<TextMeshProUGUI>().text = fans.ToString();

    public void ChangeDay() => GameObject.Find("AmountOfDays").GetComponent<TextMeshProUGUI>().text = (++day).ToString();

    public void SimulateMatches()
    {
        for (int i = matches.Count - 1; i >= 0; i--)
        {
            if (i == matches.Count - 4)
                break;
            int score1 = 0;
            int score2 = 0;
            var instance = GameObject.Instantiate(gameResults);
            instance.transform.SetParent(GameObject.Find("Canvas").transform, false);
            ScheduleMenu.MatchResultView mrv = new ScheduleMenu.MatchResultView(instance.transform);
            mrv.TeamIcon1.sprite = IMG2Sprite.LoadNewSprite(team.pathIcon);
            mrv.TeamIcon2.sprite = IMG2Sprite.LoadNewSprite(matches[i].pathIcon);
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            for (int j = 0; j < 3; j++)
            {
                if (score1 == 2 || score2 == 2)
                    break;
                var match = team.MatchSimulation(matches[i]);
                if (match.Contains("win"))
                {
                    score1++;
                    options.Add(new TMP_Dropdown.OptionData($"Game {j + 1}:{team.TeamName} {match.Substring(4)}"));
                }
                else
                {
                    score2++;
                    options.Add(new TMP_Dropdown.OptionData($"Game {j + 1}:{matches[i].TeamName} {match.Substring(5)}"));
                }
            }
            mrv.MoreDetails.ClearOptions();
            mrv.MoreDetails.AddOptions(options);
            mrv.Score.text = score1 + " : " + score2;
            mrv.OKButton.onClick.AddListener(
                () =>
                {
                    Destroy(instance.gameObject);
                }
            );
            fans += score1 > score2 ? Player.rnd.Next(101) : -Player.rnd.Next(51);
            historyMenu.clanwars.Add(new HistoryMenu.GameInformation(mrv.Score.text, team.TeamName, matches[i].TeamName, mrv.MoreDetails));
            historyMenu.changed = true;
        }
        for (int i = 0; i < 3 && matches.Count > 0; i++)
        {
            matches.RemoveAt(0);
        }
    }

    public void OtherTeamsAndPlayersActions()
    {
        foreach (var team in dataBases.teams)
        {
            if (Player.rnd.Next(2) == 1)
                team.strategiesLevel += Player.rnd.Next(2) == 1 ? 0 : Player.rnd.NextDouble() * 0.33;
            else
                team.strategiesLevel -= Player.rnd.Next(2) == 1 ? 0 : Player.rnd.NextDouble() * 0.22;
            if (team.strategiesLevel > 10)
                team.strategiesLevel = 10;
            if (team.strategiesLevel < 0)
                team.strategiesLevel = 0;
            if (Player.rnd.Next(2) == 1)
                team.atmosphereLevel += Player.rnd.Next(2) == 1 ? 0 : Player.rnd.NextDouble() * 0.33;
            else
                team.atmosphereLevel -= Player.rnd.Next(2) == 1 ? 0 : Player.rnd.NextDouble() * 0.22;
            if (team.atmosphereLevel > 10)
                team.atmosphereLevel = 10;
            if (team.atmosphereLevel < 0)
                team.atmosphereLevel = 0;
            if (Player.rnd.Next(2) == 1)
                team.teamPlayLevel += Player.rnd.Next(2) == 1 ? 0 : Player.rnd.NextDouble() * 0.33;
            else
                team.teamPlayLevel -= Player.rnd.Next(2) == 1 ? 0 : Player.rnd.NextDouble() * 0.22;
            if (team.teamPlayLevel > 10)
                team.teamPlayLevel = 10;
            if (team.teamPlayLevel < 0)
                team.teamPlayLevel = 0;
            if (this.team.players.Count == 5 && team.strategiesLevel - this.team.strategiesLevel < 3 &&
                team.atmosphereLevel - this.team.atmosphereLevel < 3 && team.teamPlayLevel - this.team.teamPlayLevel < 3 && Player.rnd.Next(1000) < 55)
            {
                mail.AddLetter(new Tuple<string, string>(team.TeamName, $"Hello, {NickName}! \n\rWe would like to play Clan War with you as soon as possible.Are you interested in this offer?"));
            }
        }
        foreach (var player in dataBases.players.FindAll(p => !team.players.ContainsValue(p)))
        {
            if (Player.rnd.Next(2) == 1)
                player.gamingSkills += Player.rnd.Next(2) == 1 ? 0 : Player.rnd.NextDouble() * 0.33;
            else
                player.gamingSkills -= Player.rnd.Next(2) == 1 ? 0 : Player.rnd.NextDouble() * 0.22;
            if (player.gamingSkills > 10)
                player.gamingSkills = 10;
            if (player.gamingSkills < 0)
                player.gamingSkills = 0;
            if (Player.rnd.Next(2) == 1)
                player.socialSkills += Player.rnd.Next(2) == 1 ? 0 : Player.rnd.NextDouble() * 0.33;
            else
                player.socialSkills -= Player.rnd.Next(2) == 1 ? 0 : Player.rnd.NextDouble() * 0.22;
            if (player.socialSkills > 10)
                player.socialSkills = 10;
            if (player.socialSkills < 0)
                player.socialSkills = 0;
            if (Player.rnd.Next(2) == 1)
                player.mediaSkills += Player.rnd.Next(2) == 1 ? 0 : Player.rnd.NextDouble() * 0.33;
            else
                player.mediaSkills -= Player.rnd.Next(2) == 1 ? 0 : Player.rnd.NextDouble() * 0.22;
            if (player.mediaSkills > 10)
                player.mediaSkills = 10;
            if (player.mediaSkills < 0)
                player.mediaSkills = 0;
        }
    }

    public void Lose()
    {
        if (money < 0)
            youLose.SetActive(true);
    }

    public void StartTournament()
    {
        if (day % 9 == 0)
        {
            currentTournament = new Tournament();
            if (day % 45 == 0)
            {
                currentTournament.name = dataBases.tournaments[4].name + " " + ++dataBases.tournaments[4].count;
                currentTournament.prizePool = dataBases.tournaments[4].prizePool;
            }
            else if (day % 36 == 0)
            {
                currentTournament.name = dataBases.tournaments[3].name + " " + ++dataBases.tournaments[3].count;
                currentTournament.prizePool = dataBases.tournaments[3].prizePool;
            }
            else if (day % 27 == 0)
            {
                currentTournament.name = dataBases.tournaments[2].name + " " + ++dataBases.tournaments[2].count;
                currentTournament.prizePool = dataBases.tournaments[2].prizePool;
            }
            else if (day % 18 == 0)
            {
                currentTournament.name = dataBases.tournaments[1].name + " " + ++dataBases.tournaments[1].count;
                currentTournament.prizePool = dataBases.tournaments[1].prizePool;
            }
            else
            {
                currentTournament.name = dataBases.tournaments[0].name + " " + ++dataBases.tournaments[0].count;
                currentTournament.prizePool = dataBases.tournaments[0].prizePool;
            }
            currentTournament.InviteTeams(dataBases.teams, team);
            if (currentTournament.managerTeamInvited)
                mail.AddLetter(new Tuple<string, string>($"Organizers of {currentTournament.name}", $"Congratulations! You are invited to {currentTournament.name} with prize pool {currentTournament.prizePool}$. " +
                    $"Games will start tomorrow"));
            else
                mail.AddLetter(new Tuple<string, string>($"Organizers of {currentTournament.name}", $"Hello, {NickName}! Unfortunately this time you are not invited to the {currentTournament.name}.Exercise harder!"));
        }
    }

    public void SimulateTournament()
    {
        if (currentTournament != null)
        {
            currentTournament.NextDay();
            TournamentTableAppearance(currentTournament.dayResults, currentTournament.day);
            if (currentTournament.day == 5)
            {
                if (currentTournament.invitedTeams.ContainsKey(team))
                {
                    currentTournament.tournamentTable = currentTournamentTable;
                    switch (currentTournament.invitedTeams[team])
                    {
                        case ("9-16"):
                            mail.AddLetter(new Tuple<string, string>($"Organizers of {currentTournament.name}", $"{currentTournament.name} is over and you have reached 13-16 place. " +
                                $"Unfortunately you don't get money for these places, but still a great result!"));
                            fans -= Player.rnd.Next(501);
                            ChangeAmountOfFans();
                            mail.AddLetter(new Tuple<string, string>($"Sponsors", $"We are disappointed with your game at the last tournament, so we lower your salary by 3000$. Train harder!"));
                            sponsorsSalary -= 3000;
                            break;
                        case ("5-8"):
                            mail.AddLetter(new Tuple<string, string>($"Organizers of {currentTournament.name}", $"{currentTournament.name} is over and you have reached 5-8 place. " +
                                $"You get {currentTournament.prizePool * 0.01875}$, congratulations!"));
                            money += (int)(currentTournament.prizePool * 0.01875);
                            ChangeMoney();
                            fans -= Player.rnd.Next(201);
                            ChangeAmountOfFans();
                            mail.AddLetter(new Tuple<string, string>($"Sponsors", $"Great job! Keep up the good work"));
                            break;
                        case ("3-4"):
                            mail.AddLetter(new Tuple<string, string>($"Organizers of {currentTournament.name}", $"{currentTournament.name} is over and you have reached 3-4 place. " +
                                $"You get {currentTournament.prizePool * 0.071875}$, congratulations!"));
                            money += (int)(currentTournament.prizePool * 0.071875);
                            ChangeMoney();
                            fans += Player.rnd.Next(301);
                            ChangeAmountOfFans();
                            mail.AddLetter(new Tuple<string, string>($"Sponsors", $"You are great fellows! Your salary has been increased by 2000$"));
                            sponsorsSalary += 2000;
                            break;
                        case ("2"):
                            mail.AddLetter(new Tuple<string, string>($"Organizers of {currentTournament.name}", $"{currentTournament.name} is over and you have reached 2 place. Great job! " +
                                $"You get {currentTournament.prizePool * 0.15625}$, congratulations!"));
                            money += (int)(currentTournament.prizePool * 0.15625);
                            ChangeMoney();
                            fans += Player.rnd.Next(801);
                            ChangeAmountOfFans();
                            mail.AddLetter(new Tuple<string, string>($"Sponsors", $"So close, but don't worry, this is very good result, we're very proud! Your salary increased by 4000$"));
                            sponsorsSalary += 4000;
                            break;
                        case ("1"):
                            mail.AddLetter(new Tuple<string, string>($"Organizers of {currentTournament.name}", $"{currentTournament.name} is over and you have won. " +
                                $"Congratulations! You get {currentTournament.prizePool * 0.625}$!"));
                            money += (int)(currentTournament.prizePool * 0.625);
                            ChangeMoney();
                            fans += Player.rnd.Next(2001);
                            ChangeAmountOfFans();
                            mail.AddLetter(new Tuple<string, string>($"Sponsors", $"Congratulations, just congratulations! Your salary increased by 6000$"));
                            sponsorsSalary += 6000;
                            break;
                    }
                    historyMenu.tournaments.Add(new HistoryMenu.TournamentTable(currentTournamentTable, currentTournament.invitedTeams[team]));
                    historyMenu.changed = true;
                }
                currentTournament = null;
                if (currentTournamentTable != null)
                    currentTournamentTable.Find("Button").GetComponent<Button>().onClick.AddListener(() => Destroy(currentTournamentTable.gameObject));
            }
        }
    }

    private GameObject gameInformation1, gameInformation2, gameInformation3;

    private void TournamentTableAppearance(string info, int day)
    {
        List<string> infoSplitted = new List<string>(info.Split('\n'));
        gameInformation1 = null;
        gameInformation2 = null;
        gameInformation3 = null;
        if (GameObject.Find("Canvas").transform.Find("GameInformation(Clone)") != null)
        {
            gameInformation1 = GameObject.Find("Canvas").transform.Find("GameInformation(Clone)").gameObject;
        }
        if (GameObject.Find("Canvas").transform.Find("GameInformation(Clone)") != null)
        {
            gameInformation2 = GameObject.Find("Canvas").transform.Find("GameInformation(Clone)").gameObject;
        }
        if (GameObject.Find("Canvas").transform.Find("GameInformation(Clone)") != null)
        {
            gameInformation3 = GameObject.Find("Canvas").transform.Find("GameInformation(Clone)").gameObject;
        }
        if (gameInformation1 != null)
            gameInformation1.SetActive(false);
        if (gameInformation2 != null)
            gameInformation2.SetActive(false);
        if (gameInformation3 != null)
            gameInformation3.SetActive(false);
        switch (day - 1)
        {
            case (1):
                currentTournamentTable = GameObject.Instantiate(TournamentTable);
                currentTournamentTable.transform.SetParent(GameObject.Find("Canvas").transform, false);
                currentTournamentTable.transform.Find("Button").GetComponent<Button>().onClick.AddListener(
                    () =>
                    {
                        currentTournamentTable.gameObject.SetActive(false);
                        mail.MailRefresh();
                        if (day - 1 == 4)
                        {
                            Destroy(currentTournamentTable.gameObject);
                        }
                        if (gameInformation1 != null)
                        {
                            gameInformation1.SetActive(true);
                            gameInformation1.transform.Find("Content").transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => gameInformation1 = null);
                        }
                        if (gameInformation2 != null)
                        {
                            gameInformation2.SetActive(true);
                            gameInformation2.transform.Find("Content").transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => gameInformation1 = null);
                        }
                        if (gameInformation3 != null)
                        {
                            gameInformation3.SetActive(true);
                            gameInformation3.transform.Find("Content").transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => gameInformation1 = null);
                        }
                    }
                );
                currentTournamentTable.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = currentTournament.name + $"\n\rDay {currentTournament.day - 1}";
                for (int i = 0, j = 0; i < 8; i++, j += 2)
                {
                    currentTournamentTable.transform.Find("Image").transform.Find("18").transform.Find("18 (" + j + ")").transform.GetComponent<TextMeshProUGUI>().text = infoSplitted[0].Split(';')[0];
                    currentTournamentTable.transform.Find("Image").transform.Find("18").transform.Find("18 (" + j + ")").transform.Find("Text").transform.GetComponent<TextMeshProUGUI>().text = infoSplitted[0].Split(';')[1];
                    currentTournamentTable.transform.Find("Image").transform.Find("18").transform.Find("18 (" + (int)(j + 1) + ")").transform.GetComponent<TextMeshProUGUI>().text = infoSplitted[0].Split(';')[2];
                    currentTournamentTable.transform.Find("Image").transform.Find("18").transform.Find("18 (" + (int)(j + 1) + ")").transform.Find("Text").transform.GetComponent<TextMeshProUGUI>().text = infoSplitted[0].Split(';')[3];
                    infoSplitted.RemoveAt(0);
                }
                break;
            case (2):
                currentTournamentTable.gameObject.SetActive(true);
                currentTournamentTable.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = currentTournament.name + $"\n\rDay {currentTournament.day - 1}";
                for (int i = 0, j = 0; i < 4; i++, j += 2)
                {
                    currentTournamentTable.transform.Find("Image").transform.Find("14").transform.Find("14 (" + j + ")").transform.GetComponent<TextMeshProUGUI>().text = infoSplitted[0].Split(';')[0];
                    currentTournamentTable.transform.Find("Image").transform.Find("14").transform.Find("14 (" + j + ")").transform.Find("Text").transform.GetComponent<TextMeshProUGUI>().text = infoSplitted[0].Split(';')[1];
                    currentTournamentTable.transform.Find("Image").transform.Find("14").transform.Find("14 (" + (int)(j + 1) + ")").transform.GetComponent<TextMeshProUGUI>().text = infoSplitted[0].Split(';')[2];
                    currentTournamentTable.transform.Find("Image").transform.Find("14").transform.Find("14 (" + (int)(j + 1) + ")").transform.Find("Text").transform.GetComponent<TextMeshProUGUI>().text = infoSplitted[0].Split(';')[3];
                    infoSplitted.RemoveAt(0);
                }
                break;
            case (3):
                currentTournamentTable.gameObject.SetActive(true);
                currentTournamentTable.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = currentTournament.name + $"\n\rDay {currentTournament.day - 1}";
                for (int i = 0, j = 0; i < 2; i++, j += 2)
                {
                    currentTournamentTable.transform.Find("Image").transform.Find("12").transform.Find("12 (" + j + ")").transform.GetComponent<TextMeshProUGUI>().text = infoSplitted[0].Split(';')[0];
                    currentTournamentTable.transform.Find("Image").transform.Find("12").transform.Find("12 (" + j + ")").transform.Find("Text").transform.GetComponent<TextMeshProUGUI>().text = infoSplitted[0].Split(';')[1];
                    currentTournamentTable.transform.Find("Image").transform.Find("12").transform.Find("12 (" + (int)(j + 1) + ")").transform.GetComponent<TextMeshProUGUI>().text = infoSplitted[0].Split(';')[2];
                    currentTournamentTable.transform.Find("Image").transform.Find("12").transform.Find("12 (" + (int)(j + 1) + ")").transform.Find("Text").transform.GetComponent<TextMeshProUGUI>().text = infoSplitted[0].Split(';')[3];
                    infoSplitted.RemoveAt(0);
                }
                break;
            case (4):
                currentTournamentTable.gameObject.SetActive(true);
                currentTournamentTable.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = currentTournament.name + $"\n\rDay {currentTournament.day - 1}";
                currentTournamentTable.transform.Find("Image").transform.Find("Final").transform.Find("Final (0)").transform.GetComponent<TextMeshProUGUI>().text = infoSplitted[0].Split(';')[0];
                currentTournamentTable.transform.Find("Image").transform.Find("Final").transform.Find("Final (0)").transform.Find("Text").transform.GetComponent<TextMeshProUGUI>().text = infoSplitted[0].Split(';')[1];
                currentTournamentTable.transform.Find("Image").transform.Find("Final").transform.Find("Final (1)").transform.GetComponent<TextMeshProUGUI>().text = infoSplitted[0].Split(';')[2];
                currentTournamentTable.transform.Find("Image").transform.Find("Final").transform.Find("Final (1)").transform.Find("Text").transform.GetComponent<TextMeshProUGUI>().text = infoSplitted[0].Split(';')[3];
                break;
        }
    }
}

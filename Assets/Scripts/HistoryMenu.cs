using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

public class HistoryMenu : MonoBehaviour
{
    public List<TournamentTable> tournaments = new List<TournamentTable>();
    public List<GameInformation> clanwars = new List<GameInformation>();

    public DataBases db;

    public RectTransform tournamentTable;
    public RectTransform gameInformation;

    public RectTransform content;
    public RectTransform contentClanWars;

    public RectTransform historyPrefab;

    public bool changed = true;

    public Manager manager;

    public void HistoryAppearance()
    {
        if (changed)
        {
            foreach (Transform child in content)
            {
                Destroy(child.gameObject);
            }
            foreach (var tournament in tournaments)
            {
                var instance = GameObject.Instantiate(historyPrefab);
                instance.transform.SetParent(content, false);
                instance.transform.Find("ClickButton").GetComponentInChildren<TextMeshProUGUI>().text = $"{tournament.place} on {tournament.TournamentName}";
                instance.transform.Find("ClickButton").GetComponent<Button>().onClick.AddListener(
                    () =>
                    {
                        var instancee = GameObject.Instantiate(tournamentTable);
                        instancee.transform.SetParent(GameObject.Find("Canvas").transform, false);
                        tournament.CreateRectTransform(instancee.transform);
                        instancee.Find("Button").GetComponent<Button>().onClick.AddListener(() => Destroy(instancee.gameObject));
                        instancee.Find("Text").GetComponent<TextMeshProUGUI>().text = tournament.TournamentName;
                    }
                );
            }
            foreach (Transform child in contentClanWars)
            {
                Destroy(child.gameObject);
            }
            foreach (var cw in clanwars)
            {
                var instance = GameObject.Instantiate(historyPrefab);
                instance.transform.SetParent(contentClanWars, false);
                instance.transform.Find("ClickButton").GetComponentInChildren<TextMeshProUGUI>().text = $"{cw.score} vs {cw.team2}";
                instance.transform.Find("ClickButton").GetComponent<Button>().onClick.AddListener(
                    () =>
                    {
                        var instancee = GameObject.Instantiate(gameInformation);
                        instancee.transform.SetParent(GameObject.Find("Canvas").transform, false);
                        cw.CreateRectTransform(instancee.transform, db.teams, manager.team);
                        instancee.Find("Content").transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => Destroy(instancee.gameObject));
                    }
                );
            }
            changed = false;
        }
    }
    [DataContract]
    public class TournamentTable
    {
        [DataMember]
        public List<string> information;

        [DataMember]
        public string TournamentName;

        [DataMember]
        public string place;

        public void CreateRectTransform(Transform currentTournamentTable)
        {
            for (int i = 0, j = 0; i < 16; i += 2, j++)
            {
                currentTournamentTable.transform.Find("Image").transform.Find("18").transform.Find("18 (" + i + ")").transform.GetComponent<TextMeshProUGUI>().text = information[j].Split(';')[0];
                currentTournamentTable.transform.Find("Image").transform.Find("18").transform.Find("18 (" + i + ")").transform.Find("Text").transform.GetComponent<TextMeshProUGUI>().text = information[j].Split(';')[1];
                currentTournamentTable.transform.Find("Image").transform.Find("18").transform.Find("18 (" + (int)(i + 1) + ")").transform.GetComponent<TextMeshProUGUI>().text = information[j].Split(';')[2];
                currentTournamentTable.transform.Find("Image").transform.Find("18").transform.Find("18 (" + (int)(i + 1) + ")").transform.Find("Text").transform.GetComponent<TextMeshProUGUI>().text = information[j].Split(';')[3];
            }
            for (int i = 8, j = 0; i < 12; i++, j += 2)
            {
                currentTournamentTable.transform.Find("Image").transform.Find("14").transform.Find("14 (" + j + ")").transform.GetComponent<TextMeshProUGUI>().text = information[i].Split(';')[0];
                currentTournamentTable.transform.Find("Image").transform.Find("14").transform.Find("14 (" + j + ")").transform.Find("Text").transform.GetComponent<TextMeshProUGUI>().text = information[i].Split(';')[1];
                currentTournamentTable.transform.Find("Image").transform.Find("14").transform.Find("14 (" + (int)(j + 1) + ")").transform.GetComponent<TextMeshProUGUI>().text = information[i].Split(';')[2];
                currentTournamentTable.transform.Find("Image").transform.Find("14").transform.Find("14 (" + (int)(j + 1) + ")").transform.Find("Text").transform.GetComponent<TextMeshProUGUI>().text = information[i].Split(';')[3];
            }
            for (int i = 12, j = 0; i < 14; i++, j += 2)
            {
                currentTournamentTable.transform.Find("Image").transform.Find("12").transform.Find("12 (" + j + ")").transform.GetComponent<TextMeshProUGUI>().text = information[i].Split(';')[0];
                currentTournamentTable.transform.Find("Image").transform.Find("12").transform.Find("12 (" + j + ")").transform.Find("Text").transform.GetComponent<TextMeshProUGUI>().text = information[i].Split(';')[1];
                currentTournamentTable.transform.Find("Image").transform.Find("12").transform.Find("12 (" + (int)(j + 1) + ")").transform.GetComponent<TextMeshProUGUI>().text = information[i].Split(';')[2];
                currentTournamentTable.transform.Find("Image").transform.Find("12").transform.Find("12 (" + (int)(j + 1) + ")").transform.Find("Text").transform.GetComponent<TextMeshProUGUI>().text = information[i].Split(';')[3];
            }
            currentTournamentTable.transform.Find("Image").transform.Find("Final").transform.Find("Final (0)").transform.GetComponent<TextMeshProUGUI>().text = information[14].Split(';')[0];
            currentTournamentTable.transform.Find("Image").transform.Find("Final").transform.Find("Final (0)").transform.Find("Text").transform.GetComponent<TextMeshProUGUI>().text = information[14].Split(';')[1];
            currentTournamentTable.transform.Find("Image").transform.Find("Final").transform.Find("Final (1)").transform.GetComponent<TextMeshProUGUI>().text = information[14].Split(';')[2];
            currentTournamentTable.transform.Find("Image").transform.Find("Final").transform.Find("Final (1)").transform.Find("Text").transform.GetComponent<TextMeshProUGUI>().text = information[14].Split(';')[3];
        }


        public TournamentTable(RectTransform currentTournamentTable, string place = null)
        {
            information = new List<string>();
            for (int i = 0; i < 16; i += 2)
            {
                //Debug.Log(currentTournamentTable.transform.Find("Image").transform.Find("18").transform.Find("18 (" + i + ")").transform.GetComponent<TextMeshProUGUI>().text);
                //Debug.Log(currentTournamentTable.transform.Find("Image").transform.Find("18").transform.Find("18 (" + i + ")").transform.Find("Text").transform.GetComponent<TextMeshProUGUI>().text == null);
                //Debug.Log(currentTournamentTable.transform.Find("Image").transform.Find("18").transform.Find("18 (" + (int)(i + 1) + ")").transform.GetComponent<TextMeshProUGUI>().text);
                //Debug.Log(currentTournamentTable.transform.Find("Image").transform.Find("18").transform.Find("18 (" + (int)(i + 1) + ")").transform.Find("Text").transform.GetComponent<TextMeshProUGUI>().text == null);
                information.Add(currentTournamentTable.transform.Find("Image").transform.Find("18").transform.Find("18 (" + i + ")").transform.GetComponent<TextMeshProUGUI>().text + ";" +
                    currentTournamentTable.transform.Find("Image").transform.Find("18").transform.Find("18 (" + i + ")").transform.Find("Text").transform.GetComponent<TextMeshProUGUI>().text + ";" +
                    currentTournamentTable.transform.Find("Image").transform.Find("18").transform.Find("18 (" + (int)(i + 1) + ")").transform.GetComponent<TextMeshProUGUI>().text + ";" +
                    currentTournamentTable.transform.Find("Image").transform.Find("18").transform.Find("18 (" + (int)(i + 1) + ")").transform.Find("Text").transform.GetComponent<TextMeshProUGUI>().text);
            }
            for (int i = 0; i < 8; i += 2)
            {
                //Debug.Log(currentTournamentTable.transform.Find("Image").transform.Find("14").transform.Find("14 (" + i + ")").transform.GetComponent<TextMeshProUGUI>().text);
                //Debug.Log(currentTournamentTable.transform.Find("Image").transform.Find("14").transform.Find("14 (" + i + ")").transform.Find("Text").transform.GetComponent<TextMeshProUGUI>().text);
                //Debug.Log(currentTournamentTable.transform.Find("Image").transform.Find("14").transform.Find("14 (" + (int)(i + 1) + ")").transform.GetComponent<TextMeshProUGUI>().text);
                //Debug.Log(currentTournamentTable.transform.Find("Image").transform.Find("14").transform.Find("14 (" + (int)(i + 1) + ")").transform.Find("Text").transform.GetComponent<TextMeshProUGUI>().text);
                information.Add(currentTournamentTable.transform.Find("Image").transform.Find("14").transform.Find("14 (" + i + ")").transform.GetComponent<TextMeshProUGUI>().text + ";" +
                    currentTournamentTable.transform.Find("Image").transform.Find("14").transform.Find("14 (" + i + ")").transform.Find("Text").transform.GetComponent<TextMeshProUGUI>().text + ";" +
                    currentTournamentTable.transform.Find("Image").transform.Find("14").transform.Find("14 (" + (int)(i + 1) + ")").transform.GetComponent<TextMeshProUGUI>().text + ";" +
                    currentTournamentTable.transform.Find("Image").transform.Find("14").transform.Find("14 (" + (int)(i + 1) + ")").transform.Find("Text").transform.GetComponent<TextMeshProUGUI>().text);
            }
            for (int i = 0; i < 4; i += 2)
            {
                information.Add(currentTournamentTable.transform.Find("Image").transform.Find("12").transform.Find("12 (" + i + ")").transform.GetComponent<TextMeshProUGUI>().text + ";" +
                    currentTournamentTable.transform.Find("Image").transform.Find("12").transform.Find("12 (" + i + ")").transform.Find("Text").transform.GetComponent<TextMeshProUGUI>().text + ";" +
                    currentTournamentTable.transform.Find("Image").transform.Find("12").transform.Find("12 (" + (int)(i + 1) + ")").transform.GetComponent<TextMeshProUGUI>().text + ";" +
                    currentTournamentTable.transform.Find("Image").transform.Find("12").transform.Find("12 (" + (int)(i + 1) + ")").transform.Find("Text").transform.GetComponent<TextMeshProUGUI>().text);
            }
            information.Add(currentTournamentTable.transform.Find("Image").transform.Find("Final").transform.Find("Final (0)").transform.GetComponent<TextMeshProUGUI>().text + ";" +
                currentTournamentTable.transform.Find("Image").transform.Find("Final").transform.Find("Final (0)").transform.Find("Text").transform.GetComponent<TextMeshProUGUI>().text + ";" +
                currentTournamentTable.transform.Find("Image").transform.Find("Final").transform.Find("Final (1)").transform.GetComponent<TextMeshProUGUI>().text + ";" +
                currentTournamentTable.transform.Find("Image").transform.Find("Final").transform.Find("Final (1)").transform.Find("Text").transform.GetComponent<TextMeshProUGUI>().text);
            TournamentName = currentTournamentTable.transform.Find("Text").GetComponent<TextMeshProUGUI>().text.Split('\n')[0];
            if (place != null)
                this.place = place;
        }
    }

    [DataContract]
    public class GameInformation
    {
        [DataMember]
        public string score;

        [DataMember]
        public string team1;

        [DataMember]
        public string team2;

        [DataMember]
        public List<string> options;


        public void CreateRectTransform(Transform currentGameInfo, List<Team> teams, Team team)
        {
            currentGameInfo.Find("Content").GetComponent<Image>().transform.Find("Score").GetComponent<TextMeshProUGUI>().text = score;
            currentGameInfo.Find("Content").GetComponent<Image>().transform.Find("Team1Icon").GetComponent<Image>().sprite = IMG2Sprite.LoadNewSprite(team.pathIcon);
            currentGameInfo.Find("Content").GetComponent<Image>().transform.Find("Team2Icon").GetComponent<Image>().sprite = IMG2Sprite.LoadNewSprite(teams.Find(t => t.TeamName == team2).pathIcon);
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            foreach (var option in this.options)
            {
                options.Add(new TMP_Dropdown.OptionData(option));
            }
            currentGameInfo.Find("Content").GetComponent<Image>().transform.Find("Dropdown").GetComponent<TMP_Dropdown>().ClearOptions();
            currentGameInfo.Find("Content").GetComponent<Image>().transform.Find("Dropdown").GetComponent<TMP_Dropdown>().AddOptions(options);
        }

        public GameInformation(string score, string team1, string team2, TMP_Dropdown dropdown)
        {
            options = new List<string>();
            this.score = score;
            this.team1 = team1;
            this.team2 = team2;
            foreach (var option in dropdown.options)
            {
                this.options.Add(option.text);
            }
        }
    }
}

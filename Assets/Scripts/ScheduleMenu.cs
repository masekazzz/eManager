using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScheduleMenu : MonoBehaviour
{
    public DataBases dataBases;
    public RectTransform TeamCard;
    public RectTransform ClanWarAnswer;
    public RectTransform Teams;
    public Manager manager;

    public PlayerCardsAppearance positions;

    public void OnClick()
    {
        try
        {
            Destroy(GameObject.Find("Teams(Clone)"));
        }
        catch (Exception)
        { }
        int day = int.Parse(GameObject.Find("AmountOfDays").GetComponent<TextMeshProUGUI>().text) + 2;
        GameObject.Find("DayAfterTomorrow").GetComponent<TextMeshProUGUI>().text = "Day " + day;
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        options.Add(new Dropdown.OptionData("Teams: "));
        foreach (var team in dataBases.teams)
        {
            options.Add(new Dropdown.OptionData(team.TeamName));
        }
        var instance = GameObject.Instantiate(Teams.gameObject);
        instance.transform.SetParent(GameObject.Find("ScheduleMenu").GetComponent<Transform>(), false);
        var dropdown = instance.GetComponentInChildren<Dropdown>();
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
        dropdown.onValueChanged.AddListener(delegate
        {
            TeamCardAppearance(dataBases.teams.Find(team => team.TeamName == dropdown.options[dropdown.value].text.ToString()));
            dropdown.ClearOptions();
            dropdown.AddOptions(options);
        });
        MatchesAppearance();
    }

    public RectTransform Image;

    public void MatchesAppearance()
    {
        foreach (Transform child in (GameObject.Find("TodayMatches").transform))
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in (GameObject.Find("TomorrowMatches").transform))
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in (GameObject.Find("DayAfterTomorrowMatches").transform))
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < manager.matches.Count; i++)
        {
            if (i < 3)
            {
                var instance = GameObject.Instantiate(Image);
                instance.transform.SetParent(GameObject.Find("TodayMatches").GetComponent<Image>().transform, false);
                instance.transform.GetComponent<Image>().sprite = IMG2Sprite.LoadNewSprite(manager.matches[i].pathIcon);
            }
            else if (i < 6)
            {
                var instance = GameObject.Instantiate(Image);
                instance.transform.SetParent(GameObject.Find("TomorrowMatches").GetComponent<Image>().transform, false);
                instance.transform.GetComponent<Image>().sprite = IMG2Sprite.LoadNewSprite(manager.matches[i].pathIcon);
            }
            else
            {
                var instance = GameObject.Instantiate(Image);
                instance.transform.SetParent(GameObject.Find("DayAfterTomorrowMatches").GetComponent<Image>().transform, false);
                instance.transform.GetComponent<Image>().sprite = IMG2Sprite.LoadNewSprite(manager.matches[i].pathIcon);
            }
        }
    }

    public void TeamCardAppearance(Team team)
    {
        var instance = GameObject.Instantiate(TeamCard);
        instance.transform.SetParent(GameObject.Find("Canvas").transform, false);
        TeamCardView tcv = new TeamCardView(instance.transform);
        tcv.TeamName.text = team.TeamName;
        tcv.TeamIcon.sprite = IMG2Sprite.LoadNewSprite(team.pathIcon);
        tcv.OKButton.onClick.AddListener(
            () =>
            {
                Destroy(instance.gameObject);
            }
        );
        tcv.OfferClanWarButton.onClick.AddListener(
            () =>
            {
                var instancee = GameObject.Instantiate(ClanWarAnswer);
                instancee.transform.SetParent(GameObject.Find("Canvas").transform, false);
                if (manager.matches.Count == 9)
                    instancee.transform.Find("Content").GetComponent<Image>().transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "You have planned the maximum number of Clan Wars!";
                else
                {
                    if (manager.team.players.Count == 5 && team.strategiesLevel - manager.team.strategiesLevel < 3 && team.atmosphereLevel - manager.team.atmosphereLevel < 3 && team.teamPlayLevel - manager.team.teamPlayLevel < 3)
                    {
                        manager.matches.Add(team);
                        string res = manager.matches.Count > 3 ? manager.matches.Count > 6 ? GameObject.Find("DayAfterTomorrow").GetComponent<TextMeshProUGUI>().text.ToString() : "tomorrow" : "today";
                        instancee.transform.Find("Content").GetComponent<Image>().transform.Find("Text").GetComponent<TextMeshProUGUI>().text = $"You have successfully assigned Clan War with {team.TeamName} for " + res;
                        MatchesAppearance();
                    }
                    else
                    {
                        instancee.transform.Find("Content").GetComponent<Image>().transform.Find("Text").GetComponent<TextMeshProUGUI>().text = $"{team.TeamName} rejected your Clan War offer";
                    }

                }
                instancee.transform.Find("Content").GetComponent<Image>().transform.Find("Button").GetComponent<Button>().onClick.AddListener(
                    () =>
                    {
                        Destroy(instancee.gameObject);
                    }
                );
            }
        );
        RectTransform[] pos = { positions.pos1, positions.pos2, positions.pos3, positions.pos4, positions.pos5 };
        for (uint i = 1; i < 6; i++)
        {
            var instancee = GameObject.Instantiate(pos[i - 1].gameObject) as GameObject;
            instancee.transform.SetParent(tcv.RosterPlayerCards.transform, false);
            Initialize(instancee, team.players[i]);
        }
        foreach (Transform child in GameObject.Find("StrategiesCardLine").transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in GameObject.Find("AtmosphereCardLine").transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in GameObject.Find("TeamPlayCardLine").transform)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < Math.Round(team.strategiesLevel, 0, MidpointRounding.AwayFromZero); i++)
        {
            var instanceee = GameObject.Instantiate(positions.good.gameObject) as GameObject;
            instanceee.transform.SetParent(GameObject.Find("StrategiesCardLine").transform, false);
        }
        for (int i = 0; i < Math.Round(team.atmosphereLevel, 0, MidpointRounding.AwayFromZero); i++)
        {
            var instanceee = GameObject.Instantiate(positions.good.gameObject) as GameObject;
            instanceee.transform.SetParent(GameObject.Find("AtmosphereCardLine").transform, false);
        }
        for (int i = 0; i < Math.Round(team.teamPlayLevel, 0, MidpointRounding.AwayFromZero); i++)
        {
            var instanceee = GameObject.Instantiate(positions.good.gameObject) as GameObject;
            instanceee.transform.SetParent(GameObject.Find("TeamPlayCardLine").transform, false);
        }
        for (int i = 0; i < 10 - Math.Round(team.strategiesLevel, 0, MidpointRounding.AwayFromZero); i++)
        {
            var instanceee = GameObject.Instantiate(positions.bad.gameObject) as GameObject;
            instanceee.transform.SetParent(GameObject.Find("StrategiesCardLine").transform, false);
        }
        for (int i = 0; i < 10 - Math.Round(team.atmosphereLevel, 0, MidpointRounding.AwayFromZero); i++)
        {
            var instanceee = GameObject.Instantiate(positions.bad.gameObject) as GameObject;
            instanceee.transform.SetParent(GameObject.Find("AtmosphereCardLine").transform, false);
        }
        for (int i = 0; i < 10 - Math.Round(team.teamPlayLevel, 0, MidpointRounding.AwayFromZero); i++)
        {
            var instanceee = GameObject.Instantiate(positions.bad.gameObject) as GameObject;
            instanceee.transform.SetParent(GameObject.Find("TeamPlayCardLine").transform, false);
        }
    }

    void Initialize(GameObject viewGameObject, Player player)
    {
        var view = new PlayerCardsAppearance.PlayerCardView(viewGameObject.transform);
        view.PlayerIcon.sprite = IMG2Sprite.LoadNewSprite(player.pathIcon);
        view.dropdown.gameObject.SetActive(false);
        view.button.gameObject.SetActive(false);
        view.gameStyle.text = player.gameStyle;
        view.Name.text = player.Name;
        view.NickName.text = player.NickName;
        view.Age.text = player.age.ToString();
        view.GS.text = $"Gaming Skills: {Math.Round(player.gamingSkills, 0, MidpointRounding.AwayFromZero)}";
        view.SS.text = $"Social Skills: {Math.Round(player.socialSkills, 0, MidpointRounding.AwayFromZero)}";
        view.MS.text = $"Media Skills: {Math.Round(player.mediaSkills, 0, MidpointRounding.AwayFromZero)}";
    }

    public class TeamCardView
    {
        public Image TeamIcon;

        public TextMeshProUGUI TeamName;

        public Button OKButton;

        public Button OfferClanWarButton;

        public Image RosterPlayerCards;

        public TeamCardView(Transform rootView)
        {
            TeamIcon = rootView.Find("TeamIcon").GetComponent<Image>();
            TeamName = rootView.Find("TeamName").GetComponent<TextMeshProUGUI>();
            OKButton = rootView.Find("OKButton").GetComponent<Button>();
            OfferClanWarButton = rootView.Find("OfferClanWarButton").GetComponent<Button>();
            RosterPlayerCards = rootView.Find("RosterPlayerCards").GetComponent<Image>();
        }
    }

    public class MatchResultView
    {
        public Image TeamIcon1;

        public Image TeamIcon2;

        public TextMeshProUGUI Score;

        public Button OKButton;

        public TMP_Dropdown MoreDetails;

        public MatchResultView(Transform rootView)
        {
            TeamIcon1 = rootView.Find("Content").GetComponent<Image>().transform.Find("Team1Icon").GetComponent<Image>();
            TeamIcon2 = rootView.Find("Content").GetComponent<Image>().transform.Find("Team2Icon").GetComponent<Image>();
            OKButton = rootView.Find("Content").GetComponent<Image>().transform.Find("Button").GetComponent<Button>();
            Score = rootView.Find("Content").GetComponent<Image>().transform.Find("Score").GetComponent<TextMeshProUGUI>();
            MoreDetails = rootView.Find("Content").GetComponent<Image>().transform.Find("Dropdown").GetComponent<TMP_Dropdown>();
        }
    }
}

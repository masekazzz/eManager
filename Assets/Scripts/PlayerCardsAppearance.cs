using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class PlayerCardsAppearance : MonoBehaviour
{
    public Manager manager;

    public ScrollViewAdapter scrollViewAdapter;
    public GameObject MarketMenu;

    public RectTransform pos1;
    public RectTransform pos2;
    public RectTransform pos3;
    public RectTransform pos4;
    public RectTransform pos5;

    public RectTransform content;

    public RectTransform good;
    public RectTransform bad;

    public RectTransform YDHEM;

    /// <summary>
    /// Спавнит картинки игроков в ростере.
    /// </summary>
    public void OnClick()
    {
        RectTransform[] positions = { pos1, pos2, pos3, pos4, pos5 };
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
        for (uint i = 1; i < 6; i++)
        {
            var instance = GameObject.Instantiate(positions[i - 1].gameObject) as GameObject;
            instance.transform.SetParent(content, false);
            if (!(manager.team != null && manager.team.players.ContainsKey(i) && manager.team.players[i] != null))
            {
                Initialize(instance, i);
            }
            else
            {
                Initialize(instance, i, manager.team.players[i]);
            }
        }
    }

    /// <summary>
    /// Продолжение появления картинок.
    /// </summary>
    /// <param name="viewGameObject"></param>
    /// <param name="player"></param>
    void Initialize(GameObject viewGameObject, uint position, Player player = null)
    {
        PlayerCardView view = new PlayerCardView(viewGameObject.transform);
        if (player == null)
        {
            view.Name.text = "";
            view.NickName.text = "";
            view.Age.text = "";
            view.GS.text = "";
            view.SS.text = "";
            view.MS.text = "";
            view.gameStyle.text = "";
            view.dropdown.gameObject.SetActive(false);
            view.button.onClick.AddListener(
                () =>
                {
                    GameObject.Find("Canvas").transform.Find("Game").transform.Find("Menus").transform.Find("RosterMenu").gameObject.SetActive(false);
                    MarketMenu.SetActive(true);
                    scrollViewAdapter.OnReceivedModels((from p in scrollViewAdapter.dataBases.players
                                      where p.position == position
                                      select p).ToList());
                    scrollViewAdapter.PlayersSort();
                }
            );
        }
        else
        {
            view.PlayerIcon.sprite = IMG2Sprite.LoadNewSprite(player.pathIcon);
            view.button.gameObject.SetActive(false);
            view.gameStyle.text = player.gameStyle;
            view.Name.text = player.Name;
            view.NickName.text = player.NickName;
            view.Age.text = player.age.ToString();
            view.GS.text = $"Gaming Skills: {Math.Round(player.gamingSkills, 0, MidpointRounding.AwayFromZero)}";
            view.SS.text = $"Social Skills: {Math.Round(player.socialSkills, 0, MidpointRounding.AwayFromZero)}";
            view.MS.text = $"Media Skills: {Math.Round(player.mediaSkills, 0, MidpointRounding.AwayFromZero)}";
            view.dropdown.ClearOptions();
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            options.Add(new Dropdown.OptionData("Relax"));
            options.Add(new Dropdown.OptionData("Develop strategies"));
            options.Add(new Dropdown.OptionData("Exercise"));
            if (manager.money >= 1000)
                options.Add(new Dropdown.OptionData("Psychologist 1000$"));
            options.Add(new Dropdown.OptionData("Fire"));
            view.dropdown.AddOptions(options);
            view.dropdown.onValueChanged.AddListener(delegate {
                if (view.dropdown.options[view.dropdown.value].text.ToString() == "Fire")
                {
                    manager.team.RemoveFromTeam(player);
                    OnClick();
                    SkillLevelLinesAppearance(manager);
                }
                else
                {
                    if (view.dropdown.options[view.dropdown.value].text.ToString() == "Psychologist 1000$")
                    {
                        manager.money -= 1000;
                        manager.ChangeMoney();
                    }
                    player.status = view.dropdown.options[view.dropdown.value].text.ToString();
                }
            });
        }
    }

    /// <summary>
    /// Появвление полосок навыков команды.
    /// </summary>
    /// <param name="manager"></param>
    public void SkillLevelLinesAppearance(Manager manager)
    {
        foreach (Transform child in GameObject.Find("StrategiesLine").transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in GameObject.Find("AtmosphereLine").transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in GameObject.Find("TeamPlayLine").transform)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < Math.Round(manager.team.strategiesLevel, 0, MidpointRounding.AwayFromZero); i++)
        {
            var instance = GameObject.Instantiate(good.gameObject) as GameObject;
            instance.transform.SetParent(GameObject.Find("StrategiesLine").transform, false);
        }
        for (int i = 0; i < Math.Round(manager.team.atmosphereLevel,0, MidpointRounding.AwayFromZero); i++)
        {
            var instance = GameObject.Instantiate(good.gameObject) as GameObject;
            instance.transform.SetParent(GameObject.Find("AtmosphereLine").transform, false);
        }
        for (int i = 0; i < Math.Round(manager.team.teamPlayLevel, 0, MidpointRounding.AwayFromZero) ; i++)
        {
            var instance = GameObject.Instantiate(good.gameObject) as GameObject;
            instance.transform.SetParent(GameObject.Find("TeamPlayLine").transform, false);
        }
        for (int i = 0; i < 10 - Math.Round(manager.team.strategiesLevel, 0, MidpointRounding.AwayFromZero) ; i++)
        {
            var instance = GameObject.Instantiate(bad.gameObject) as GameObject;
            instance.transform.SetParent(GameObject.Find("StrategiesLine").transform, false);
        }
        for (int i = 0; i < 10 - Math.Round(manager.team.atmosphereLevel, 0, MidpointRounding.AwayFromZero) ; i++)
        {
            var instance = GameObject.Instantiate(bad.gameObject) as GameObject;
            instance.transform.SetParent(GameObject.Find("AtmosphereLine").transform, false);
        }
        for (int i = 0; i < 10 - Math.Round(manager.team.teamPlayLevel, 0, MidpointRounding.AwayFromZero); i++)
        {
            var instance = GameObject.Instantiate(bad.gameObject) as GameObject;
            instance.transform.SetParent(GameObject.Find("TeamPlayLine").transform, false);
        }
    }
    /// <summary>
    /// Метод, отвечающий за кнопку буткемпа.
    /// </summary>
    public void BootcampButton()
    {
        if (manager.money >= 5000)
        {
            manager.team.onBootcamp = 7;
            var button = GameObject.Find("BootCampButton").GetComponent<Button>();
            button.interactable = false;
            button.GetComponentInChildren<TextMeshProUGUI>().text = "On bootcamp: 7 days left";
            button.GetComponentInChildren<TextMeshProUGUI>().enableAutoSizing = true;
            manager.money -= 5000;
            manager.ChangeMoney();
        }
        else
            YDHEMAppearance();
    }

    public void YDHEMAppearance()
    {
        GameObject ydhem = GameObject.Instantiate(YDHEM.gameObject) as GameObject;
        ydhem.transform.SetParent(GameObject.Find("Canvas").transform, false);
        StartCoroutine(ScrollViewAdapter.DestroyObj(ydhem));
    }

    /// <summary>
    /// Класс для карточек игроков.
    /// </summary>
    public class PlayerCardView
    {
        public Image PlayerIcon;

        public TextMeshProUGUI Name;

        public TextMeshProUGUI NickName;

        public TextMeshProUGUI Age;

        public TextMeshProUGUI GS;

        public TextMeshProUGUI SS;

        public TextMeshProUGUI MS;

        public Dropdown dropdown;

        public TextMeshProUGUI gameStyle;

        public Button button;

        public PlayerCardView(Transform rootView)
        {
            PlayerIcon = rootView.Find("PlayerIcon").GetComponent<Image>();
            Name = rootView.Find("Name").GetComponent<TextMeshProUGUI>();
            NickName = rootView.Find("NickName").GetComponent<TextMeshProUGUI>();
            Age = rootView.Find("Age").GetComponent<TextMeshProUGUI>();
            GS = rootView.Find("GS").GetComponent<TextMeshProUGUI>();
            SS = rootView.Find("SS").GetComponent<TextMeshProUGUI>();
            MS = rootView.Find("MS").GetComponent<TextMeshProUGUI>();
            dropdown = rootView.Find("Dropdown").GetComponent<Dropdown>();
            gameStyle = rootView.Find("GameStyle").GetComponent<TextMeshProUGUI>();
            button = rootView.Find("Button").GetComponent<Button>();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;
using System.Linq;

public class ScrollViewAdapter : MonoBehaviour
{
    private bool firstTime = true;
    public RectTransform prefab; // Префаб строчки игрока.
    public RectTransform content; // Место, куда помещаю строчки игроков.
    public DataBases dataBases; // Класс с игроками, командами и турнирами.

    public RectTransform good; // Префаб заполненного балла скилла.
    public RectTransform bad; // Префаб незаполненного балла скилла.
    public RectTransform playerCard; // Префаб карточки игрока.
    public RectTransform parentPlayerCard; // Место, куда будем помещать игрока.

    public Manager manager; // Класс менеджера

    public RectTransform YDHEM;
    public RectTransform FireDialogue;

    public void PlayersSort()
    {
        if (firstTime)
        {
            GameObject.Find("InputField").GetComponent<TMP_InputField>().onValueChanged.AddListener(
                (string s) =>
                {
                    OnReceivedModels((from p in dataBases.players
                                      where p.Name.ToLower().Contains(s.ToLower()) || p.NickName.ToLower().Contains(s.ToLower())
                                      select p).ToList());
                }
            );
            GameObject.Find("PositionButton").GetComponent<Button>().onClick.AddListener(
                () =>
                {
                    dataBases.players.Sort(delegate (Player p1, Player p2)
                    {
                        if (p1.position.CompareTo(p2.position) == 0)
                            return p1.NickName.CompareTo(p2.NickName);
                        return p1.position.CompareTo(p2.position);
                    });
                    OnReceivedModels(dataBases.players);
                }
            );
            GameObject.Find("NameButton").GetComponent<Button>().onClick.AddListener(
                () =>
                {
                    dataBases.players.Sort((p1, p2) => p1.Name.CompareTo(p2.Name));
                    OnReceivedModels(dataBases.players);
                }
            );
            GameObject.Find("NicknameButton").GetComponent<Button>().onClick.AddListener(
                () =>
                {
                    dataBases.players.Sort((p1, p2) => p1.NickName.CompareTo(p2.NickName));
                    OnReceivedModels(dataBases.players);
                }
            );
            firstTime = false;
        }
    }

    /// <summary>
    /// Метод запускает цепочку.
    /// </summary>
    public void UpdateItems()
    {
        if (firstTime)
        {
            OnReceivedModels(dataBases.players);
        }
    }

    /// <summary>
    /// Метод удаляет всех из скролл листа и заполняет новыми.
    /// </summary>
    /// <param name="players">Список игроков.</param>
    public void OnReceivedModels(List<Player> players)
    {
        // Удаление всех
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        foreach (var player in players)
        {
            // Проверка есть ли этот игрок в команде игрока.
            if (!manager.team.players.ContainsValue(player))
            {
                var instance = GameObject.Instantiate(prefab.gameObject) as GameObject;
                instance.transform.SetParent(content, false);
                InitializeItemView(instance, player);
            }
        }
    }

    /// <summary>
    /// Инициализация всех элементов префаба.
    /// </summary>
    /// <param name="viewGameObject">Префаб.</param>
    /// <param name="player">Игрок.</param>
    void InitializeItemView(GameObject viewGameObject, Player player)
    {
        ItemView view = new ItemView(viewGameObject.transform);
        view.clickButton.GetComponentInChildren<TextMeshProUGUI>().text = $"{player.Name.Split()[0]} \"{player.NickName}\" " +
            $"{player.Name.Split()[1]}";
        view.text.text = $"pos {player.position}";
        view.clickButton.onClick.AddListener(
            () =>
            {
                CreateHeroCard(player);
            }
         );
    }

    /// <summary>
    /// Класс с элементами префаба для строчки игрока.
    /// </summary>
    public class ItemView
    {
        public Button clickButton;

        public TextMeshProUGUI text;

        public ItemView(Transform rootView)
        {
            clickButton = rootView.Find("ClickButton").GetComponent<Button>();
            text = rootView.Find("ClickButton").transform.Find("TextPosition").GetComponent<TextMeshProUGUI>();
        }
    }

    /// <summary>
    /// Создание карточки игрока.
    /// </summary>
    /// <param name="player"></param>
    public void CreateHeroCard(Player player)
    {
        // Удаление текущей карточки игрока для устранения наложения.
        foreach (Transform child in parentPlayerCard)
        {
            Destroy(child.gameObject);
        }
        var instance = GameObject.Instantiate(playerCard.gameObject) as GameObject;
        instance.transform.SetParent(parentPlayerCard, false);
        Initialize(instance, player);
    }

    public static IEnumerator DestroyObj(GameObject ydhem)
    {
        yield return new WaitForSeconds(1.9f);
        Destroy(ydhem);
    }

    /// <summary>
    /// Инициализация всех элементов.
    /// </summary>
    /// <param name="viewGameObject"></param>
    /// <param name="player"></param>
    void Initialize(GameObject viewGameObject, Player player)
    {
        PlayerCardView view = new PlayerCardView(viewGameObject.transform);
        view.name.text = player.Name + $", {player.age}";
        view.nickName.text = player.NickName;
        view.cost.text = player.cost + "$";
        view.position.text = $"pos " + player.position;
        view.gameStyle.text = player.gameStyle;
        view.icon.sprite = IMG2Sprite.LoadNewSprite(player.pathIcon);
        view.flag.sprite = IMG2Sprite.LoadNewSprite($"Icons/{player.Nationality}");
        view.hireButton.onClick.AddListener(
            () =>
            {
                // Проверка на возможность покупки
                if (manager.money >= player.cost)
                {
                    if (manager.team.players.ContainsKey(player.position) && manager.team.players[player.position] != null)
                    {
                        viewGameObject.SetActive(false);
                        GameObject fireDialogue = GameObject.Instantiate(FireDialogue.gameObject) as GameObject;
                        fireDialogue.transform.SetParent(GameObject.Find("Canvas").transform, false);
                        fireDialogue.transform.Find("Image").Find("YesButton").GetComponent<Button>().onClick.AddListener(
                            () =>
                            {
                                Destroy(fireDialogue);
                                var team = dataBases.teams.Find(t => t.players.Values.Contains(player));
                                if (team is Team)
                                {
                                    team.RemoveFromTeam(player);
                                }
                                if (manager.team.players.ContainsKey(player.position))
                                    manager.team.RemoveFromTeam(manager.team.players[player.position]);
                                manager.team.AddToTeam(player);
                                manager.money -= (int)player.cost;
                                manager.ChangeMoney();
                                manager.fans += (int)(player.mediaSkills * 300 * Player.rnd.NextDouble());
                                manager.ChangeAmountOfFans();
                                viewGameObject.SetActive(true);
                                Destroy(viewGameObject);
                                OnReceivedModels(dataBases.players);
                                if (team is Team)
                                {
                                    var availablePlayers = dataBases.players.FindAll(p => p.position == player.position && p.inTeam == false);
                                    var smth = availablePlayers[Player.rnd.Next(availablePlayers.Count)];
                                    team.AddToTeam(smth);
                                }
                            }
                            );
                        fireDialogue.transform.Find("Image").Find("NoButton").GetComponent<Button>().onClick.AddListener(
                            () =>
                            {
                                Destroy(fireDialogue);
                                viewGameObject.SetActive(true);
                            }
                            );
                        fireDialogue.transform.Find("Image").Find("Dialogue").GetComponent<TextMeshProUGUI>().text = $"Are you sure want to fire {manager.team.players[player.position].NickName} and hire {player.NickName}";
                    }
                    else
                    {
                        var team = dataBases.teams.Find(t => t.players.Values.Contains(player));
                        if (team is Team)
                        {
                            team.RemoveFromTeam(player);
                        }
                        if (manager.team.players.ContainsKey(player.position))
                            manager.team.RemoveFromTeam(manager.team.players[player.position]);
                        manager.team.AddToTeam(player);
                        manager.money -= (int)player.cost;
                        manager.ChangeMoney();
                        manager.fans += (int)(player.mediaSkills * 300 * Player.rnd.NextDouble());
                        manager.ChangeAmountOfFans();
                        // Удаление окна покупки и строчки игрока при покупке
                        Destroy(viewGameObject);
                        OnReceivedModels(dataBases.players);
                        if (team is Team)
                        {
                            var availablePlayers = dataBases.players.FindAll(p => p.position == player.position && p.inTeam == false);
                            var smth = availablePlayers[Player.rnd.Next(availablePlayers.Count)];
                            team.AddToTeam(smth);
                        }
         
                    }
                }
                else
                {
                    GameObject ydhem = GameObject.Instantiate(YDHEM.gameObject) as GameObject;
                    ydhem.transform.SetParent(GameObject.Find("Canvas").transform, false);
                    StartCoroutine(DestroyObj(ydhem));
                }

            }
        );
        // Костыль для создания полосок оценок
        for (int i = 0; i < Math.Round(player.gamingSkills, 0, MidpointRounding.AwayFromZero); i++)
        {
            var instance = GameObject.Instantiate(good.gameObject) as GameObject;
            instance.transform.SetParent(view.gamingSkillsLine.transform, false);
        }
        for (int i = 0; i < Math.Round(player.socialSkills, 0, MidpointRounding.AwayFromZero); i++)
        {
            var instance = GameObject.Instantiate(good.gameObject) as GameObject;
            instance.transform.SetParent(view.socialSkillsLine.transform, false);
        }
        for (int i = 0; i < Math.Round(player.mediaSkills, 0, MidpointRounding.AwayFromZero); i++)
        {
            var instance = GameObject.Instantiate(good.gameObject) as GameObject;
            instance.transform.SetParent(view.mediaSkillsLine.transform, false);
        }
        for (int i = 0; i < 10 - Math.Round(player.gamingSkills, 0, MidpointRounding.AwayFromZero); i++)
        {
            var instance = GameObject.Instantiate(bad.gameObject) as GameObject;
            instance.transform.SetParent(view.gamingSkillsLine.transform, false);
        }
        for (int i = 0; i < 10 - Math.Round(player.socialSkills, 0, MidpointRounding.AwayFromZero); i++)
        {
            var instance = GameObject.Instantiate(bad.gameObject) as GameObject;
            instance.transform.SetParent(view.socialSkillsLine.transform, false);
        }
        for (int i = 0; i < 10 - Math.Round(player.mediaSkills, 0, MidpointRounding.AwayFromZero); i++)
        {
            var instance = GameObject.Instantiate(bad.gameObject) as GameObject;
            instance.transform.SetParent(view.mediaSkillsLine.transform, false);
        }
    }

    /// <summary>
    /// Класс для карточки игрока.
    /// </summary>
    public class PlayerCardView
    {
        public Button hireButton;

        public Image icon;

        public TextMeshProUGUI name;

        public TextMeshProUGUI nickName;

        public Image flag;

        public TextMeshProUGUI cost;

        public Image gamingSkillsLine;

        public Image socialSkillsLine;

        public Image mediaSkillsLine;

        public TextMeshProUGUI position;

        public TextMeshProUGUI gameStyle;

        public PlayerCardView(Transform rootView)
        {
            hireButton = rootView.Find("HireButton").GetComponent<Button>();
            icon = rootView.Find("Icon").GetComponent<Image>();
            name = rootView.Find("Name").GetComponent<TextMeshProUGUI>();
            nickName = rootView.Find("Nick").GetComponent<TextMeshProUGUI>();
            flag = rootView.Find("Flag").GetComponent<Image>();
            cost = rootView.Find("Cost").GetComponent<TextMeshProUGUI>();
            gamingSkillsLine = rootView.Find("GamingSkillsLine").GetComponent<Image>();
            socialSkillsLine = rootView.Find("SocialSkillsLine").GetComponent<Image>();
            mediaSkillsLine = rootView.Find("MediaSkillsLine").GetComponent<Image>();
            position = rootView.Find("Position").GetComponent<TextMeshProUGUI>();
            gameStyle = rootView.Find("GameStyle").GetComponent<TextMeshProUGUI>();
        }
    }
}

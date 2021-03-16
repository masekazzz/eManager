using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Mail : MonoBehaviour
{
    public RectTransform readed;
    public RectTransform notReaded;
    public RectTransform mailButton;
    public RectTransform mailScroll;
    public RectTransform mailPrefab;
    public RectTransform letterPrefab;
    public RectTransform ClanWar;
    public Manager manager;
    public ScheduleMenu refreshTeams;
    public bool firstTime = true;
    public RectTransform mailButtonn;

    public Dictionary<Tuple<string, string>, bool> letters = new Dictionary<Tuple<string, string>, bool>();

    public void AddLetter(Tuple<string, string> letter)
    {
        if (!letters.ContainsKey(letter))
            letters.Add(letter, false);
        else
            letters[letter] = false;
    }
    public void ReadLetter(Tuple<string, string> letter) => letters[letter] = true;

    public void DeleteLetter(Tuple<string, string> letter) => letters.Remove(letter);

    public void MailRefresh()
    {
        if (firstTime && GameObject.Find("Canvas").transform.Find("MailButton(Clone)") == null)
        {
            mailButtonn = GameObject.Instantiate(mailButton);
            mailButtonn.transform.SetParent(GameObject.Find("Canvas").transform, false);
            mailButtonn.GetComponent<Button>().onClick.AddListener(
                () =>
                {
                    if (GameObject.Find("MailScroll(Clone)") != null)
                        Destroy(GameObject.Find("MailScroll(Clone)"));
                    else
                    {
                        InitializeMail(GameObject.Instantiate(mailScroll.gameObject));
                    }
                    if (GameObject.Find("LetterPrefab(Clone)") != null)
                        Destroy(GameObject.Find("LetterPrefab(Clone)"));
                }
            );
            firstTime = false;
        }
        if (letters.ContainsValue(false))
            mailButtonn.GetComponent<RawImage>().texture = notReaded.GetComponent<RawImage>().texture;
        else
            mailButtonn.GetComponent<RawImage>().texture = readed.GetComponent<RawImage>().texture;

    }

    public void InitializeMail(GameObject MailScroll)
    {
        foreach (Transform child in MailScroll.transform.Find("MailScroll").transform.Find("Content").transform)
        {
            Destroy(child.gameObject);
        }
        MailScroll.transform.SetParent(GameObject.Find("Canvas").transform, false);
        foreach (var letter in letters.Keys)
        {
            var instance = GameObject.Instantiate(mailPrefab.gameObject);
            instance.transform.SetParent(MailScroll.transform.Find("MailScroll").transform.Find("Content").transform, false);
            instance.GetComponentInChildren<TextMeshProUGUI>().text = letter.Item1.ToString();
            if (letters[letter])
                instance.GetComponentInChildren<RawImage>().gameObject.SetActive(false);
            instance.GetComponent<Button>().onClick.AddListener(
                () =>
                {
                    Destroy(GameObject.Find("LetterPrefab(Clone)"));
                    letters[letter] = true;
                    Destroy(MailScroll);
                    MailRefresh();
                    InitializeMail(GameObject.Instantiate(mailScroll.gameObject));
                    var instancee = GameObject.Instantiate(letterPrefab.gameObject);
                    instancee.transform.SetParent(GameObject.Find("Canvas").transform, false);
                    instancee.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = letter.Item2.ToString();
                    instancee.transform.Find("Author").GetComponent<TextMeshProUGUI>().text = $"From: {letter.Item1.ToString()}";
                    instancee.transform.Find("TrashCanButton").GetComponent<Button>().onClick.AddListener(
                        () =>
                        {
                            letters.Remove(letter);
                            Destroy(GameObject.Find("MailScroll(Clone)"));
                            MailRefresh();
                            InitializeMail(GameObject.Instantiate(mailScroll.gameObject));
                            Destroy(instancee);
                        }
                    );
                    if (letter.Item2.ToString().Contains("Clan"))
                    {
                        instancee.transform.Find("YESButton").GetComponent<Button>().onClick.AddListener(
                            () =>
                            {
                                if (manager.matches.Count < 9)
                                {
                                    manager.matches.Add(manager.dataBases.teams.Find(t => t.TeamName == letter.Item1.ToString()));
                                    letters.Remove(letter);
                                    Destroy(GameObject.Find("MailScroll(Clone)"));
                                    MailRefresh();
                                    InitializeMail(GameObject.Instantiate(mailScroll.gameObject));
                                    Destroy(instancee);
                                    try
                                    {
                                        refreshTeams.MatchesAppearance();
                                    }
                                    catch { }
                                }
                                else
                                {
                                    var instanceee = GameObject.Instantiate(ClanWar);
                                    instanceee.transform.SetParent(GameObject.Find("Canvas").transform, false);
                                    instanceee.transform.Find("Content").GetComponent<Image>().transform.Find("Text").GetComponent<TextMeshProUGUI>().text = $"You have planned the maximum number of Clan Wars! You cannot accept this offer at this time.";
                                    instanceee.transform.Find("Content").GetComponent<Image>().transform.Find("Button").GetComponent<Button>().onClick.AddListener(
                                        () =>
                                        {
                                            Destroy(instanceee.gameObject);
                                        }
                                    );
                                }
                            }
                        );
                        instancee.transform.Find("NOButton").GetComponent<Button>().onClick.AddListener(
                            () =>
                            {
                                letters.Remove(letter);
                                Destroy(GameObject.Find("MailScroll(Clone)"));
                                MailRefresh();
                                InitializeMail(GameObject.Instantiate(mailScroll.gameObject));
                                Destroy(instancee);
                            }
                        );
                        instancee.transform.Find("OKButton").gameObject.SetActive(false);
                    }
                    else
                    {
                        instancee.transform.Find("YESButton").gameObject.SetActive(false);
                        instancee.transform.Find("NOButton").gameObject.SetActive(false);
                        instancee.transform.Find("OKButton").GetComponent<Button>().onClick.AddListener(
                            () =>
                            {
                                Destroy(instancee);
                            }
                        );
                    }
                }
            );
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;
using UnityEngine.UI;

public class IconKitScript : MonoBehaviour {

    public KMAudio audio;
    public KMBombInfo bomb;

    public KMSelectable[] buttons;
    public GameObject[] sceneChanges;
    public SpriteRenderer[] slots;
    public SpriteRenderer[] selections;
    public Sprite[] icons;
    public Sprite[] ships;
    public Sprite[] balls;
    public Sprite[] ufos;
    public Sprite[] waves;
    public Sprite[] robots;
    public Sprite[] spiders;
    public TextMesh[] stats;
    public Text[] shards;
    public MeshRenderer background;
    public Material sceneTwoBacking;

    private bool isSceneTwo;
    private bool shardsMenuOpen;
    private List<int> stageOrder = new List<int>();
    private string[] names = { "cube", "ship", "gravity ball", "ufo", "wave", "robot", "spider" };
    private int[] iconOrder = { 12, 16, 11, 5, 18, 19, 10, 9, 20, 3, 7, 17, 6, 1, 15, 8, 4, 14, 13, 2, 0 };
    private int[] shipOrder = { 15, 12, 17, 16, 13, 7, 14, 8, 9, 5, 3, 6, 1, 4, 11, 10, 2, 0 };
    private int[] ballOrder = { 12, 11, 13, 5, 8, 6, 4, 2, 3, 1, 10, 7, 9, 0 };
    private int[] ufoOrder = { 11, 4, 9, 6, 8, 5, 10, 2, 7, 3, 1, 0 };
    private int[] waveOrder = { 6, 7, 9, 3, 8, 4, 2, 5, 1, 0 };
    private int[] robotOrder = { 6, 3, 5, 7, 1, 2, 4, 0 };
    private int[] spiderOrder = { 5, 2, 1, 3, 4, 0 };
    private int[] usedPositions = new int[5];
    private int[] generatedStats = new int[10];
    private int stage;
    private int correctPos;

    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        moduleSolved = false;
        foreach (KMSelectable obj in buttons){
            KMSelectable pressed = obj;
            pressed.OnInteract += delegate () { PressButton(pressed); return false; };
        }
    }

    void Start () {
        for (int i = 0; i < 7; i++)
            selections[i].sprite = null;
        sceneChanges[1].SetActive(false);
        sceneChanges[2].SetActive(false);
        buttons[1].gameObject.SetActive(false);
        Debug.LogFormat("[The Icon Kit #{0}] Press the button to begin disarming the module.", moduleId);
    }

    void PressButton(KMSelectable pressed)
    {
        if (moduleSolved != true)
        {
            if (Array.IndexOf(buttons, pressed) == 0)
            {
                isSceneTwo = true;
                Debug.LogFormat("[The Icon Kit #{0}] The button has been pressed, generating icons and statistics...", moduleId);
                sceneChanges[0].SetActive(false);
                sceneChanges[1].SetActive(true);
                background.material = sceneTwoBacking;
                buttons[1].gameObject.SetActive(true);
                GenerateStats();
                GenerateStage(stageOrder[0]);
            }
            else if (Array.IndexOf(buttons, pressed) == 1 && !shardsMenuOpen)
            {
                shardsMenuOpen = true;
                sceneChanges[2].SetActive(true);
                sceneChanges[3].SetActive(true);
            }
            else if (Array.IndexOf(buttons, pressed) == 2 && shardsMenuOpen)
            {
                shardsMenuOpen = false;
                sceneChanges[2].SetActive(false);
                sceneChanges[3].SetActive(false);
            }
            else if (!shardsMenuOpen)
            {
                List<string> namedItems = new List<string>();
                for (int i = 0; i < 5; i++)
                {
                    string prefix = names[stageOrder[stage]];
                    if (prefix.Equals("gravity ball"))
                        prefix = "ball";
                    if (usedPositions[i] == 0)
                        namedItems.Add(prefix + "def");
                    else
                        namedItems.Add(prefix + usedPositions[i]);
                }
                if (Array.IndexOf(buttons, pressed) - 3 == correctPos)
                {
                    Debug.LogFormat("[The Icon Kit #{0}] You selected {1}, which is correct.", moduleId, namedItems[Array.IndexOf(buttons, pressed) - 3]);
                    selections[stageOrder[stage]].sprite = slots[correctPos].sprite;
                    if (stage == 0 && selections[0].sprite == icons[15])
                        selections[0].gameObject.transform.localPosition = new Vector3(-0.065f, 0.0154f, -0.009f);
                    stage++;
                    if (stage == 4)
                    {
                        moduleSolved = true;
                        GetComponent<KMBombModule>().HandlePass();
                        Debug.LogFormat("[The Icon Kit #{0}] Module disarmed.", moduleId);
                    }
                    else
                        GenerateStage(stageOrder[stage]);
                }
                else
                {
                    Debug.LogFormat("[The Icon Kit #{0}] You selected {1}, which is incorrect. Strike!", moduleId, namedItems[Array.IndexOf(buttons, pressed) - 3]);
                    GetComponent<KMBombModule>().HandleStrike();
                }
            }
        }
    }

    void GenerateStats()
    {
        for (int i = 0; i < 3; i++)
        {
            int rando = UnityEngine.Random.Range(1, 7);
            while (stageOrder.Contains(rando))
                rando = UnityEngine.Random.Range(1, 7);
            stageOrder.Add(rando);
        }
        stageOrder.Insert(0, 0);
        Debug.LogFormat("[The Icon Kit #{0}] The missing icons are the {1}, {2}, {3}, and {4}.", moduleId, names[stageOrder[0]], names[stageOrder[1]], names[stageOrder[2]], names[stageOrder[3]]);
        for (int i = 1; i < 7; i++)
        {
            if (!stageOrder.Contains(i))
            {
                switch (i)
                {
                    case 1:
                        selections[i].sprite = ships[UnityEngine.Random.Range(0, ships.Length)];
                        break;
                    case 2:
                        selections[i].sprite = balls[UnityEngine.Random.Range(0, balls.Length)];
                        break;
                    case 3:
                        selections[i].sprite = ufos[UnityEngine.Random.Range(0, ufos.Length)];
                        break;
                    case 4:
                        selections[i].sprite = waves[UnityEngine.Random.Range(0, waves.Length)];
                        break;
                    case 5:
                        selections[i].sprite = robots[UnityEngine.Random.Range(0, robots.Length)];
                        break;
                    case 6:
                        selections[i].sprite = spiders[UnityEngine.Random.Range(0, spiders.Length)];
                        break;
                }
            }
        }
        string[] statArr = { "Stars: ", "Secret Coins: ", "User Coins: ", "Demons: ", "Diamonds: ", "Fire Shards: ", "Ice Shards: ", "Poison Shards: ", "Shadow Shards: ", "Lava Shards: " };
        Debug.LogFormat("[The Icon Kit #{0}] ==Generated Statistics==", moduleId);
        for (int i = 0; i < statArr.Length; i++)
        {
            switch (i)
            {
                case 0:
                    int type = UnityEngine.Random.Range(0, 2);
                    if (type == 0)
                        generatedStats[i] = UnityEngine.Random.Range(100, 1000);
                    else
                        generatedStats[i] = UnityEngine.Random.Range(1000, 11001);
                    break;
                case 1:
                    generatedStats[i] = UnityEngine.Random.Range(10, 131);
                    break;
                case 2:
                    type = UnityEngine.Random.Range(0, 2);
                    if (type == 0)
                        generatedStats[i] = UnityEngine.Random.Range(10, 100);
                    else
                        generatedStats[i] = UnityEngine.Random.Range(100, 1101);
                    break;
                case 3:
                    generatedStats[i] = UnityEngine.Random.Range(1, 71);
                    break;
                case 4:
                    type = UnityEngine.Random.Range(0, 2);
                    if (type == 0)
                        generatedStats[i] = UnityEngine.Random.Range(100, 1000);
                    else
                        generatedStats[i] = UnityEngine.Random.Range(1000, 4101);
                    break;
                default:
                    generatedStats[i] = UnityEngine.Random.Range(0, 81);
                    break;
            }
            if (i < 5)
                stats[i].text = generatedStats[i].ToString();
            else
                shards[i - 5].text = generatedStats[i].ToString();
            Debug.LogFormat("[The Icon Kit #{0}] {1}{2}", moduleId, statArr[i], generatedStats[i].ToString());
        }
    }

    void GenerateStage(int type)
    {
        switch (type)
        {
            case 0:
                Debug.LogFormat("[The Icon Kit #{0}] ==Generated Cubes==", moduleId);
                slots[4].sprite = icons[0];
                break;
            case 1:
                Debug.LogFormat("[The Icon Kit #{0}] ==Generated Ships==", moduleId);
                slots[4].sprite = ships[0];
                slots[4].gameObject.transform.localScale = new Vector3(0.3645455f, 0.3645462f, 10000);
                break;
            case 2:
                Debug.LogFormat("[The Icon Kit #{0}] ==Generated Gravity Balls==", moduleId);
                slots[4].sprite = balls[0];
                slots[4].gameObject.transform.localScale = new Vector3(0.4545455f, 0.4545462f, 10000);
                break;
            case 3:
                Debug.LogFormat("[The Icon Kit #{0}] ==Generated UFOs==", moduleId);
                slots[4].sprite = ufos[0];
                slots[4].gameObject.transform.localScale = new Vector3(0.3945455f, 0.3945462f, 10000);
                break;
            case 4:
                Debug.LogFormat("[The Icon Kit #{0}] ==Generated Waves==", moduleId);
                slots[4].sprite = waves[0];
                slots[4].gameObject.transform.localScale = new Vector3(0.1845455f, 0.1845462f, 10000);
                break;
            case 5:
                Debug.LogFormat("[The Icon Kit #{0}] ==Generated Robots==", moduleId);
                slots[4].sprite = robots[0];
                slots[4].gameObject.transform.localScale = new Vector3(0.3045455f, 0.3045462f, 10000);
                break;
            case 6:
                Debug.LogFormat("[The Icon Kit #{0}] ==Generated Spiders==", moduleId);
                slots[4].sprite = spiders[0];
                slots[4].gameObject.transform.localScale = new Vector3(0.3845455f, 0.3845462f, 10000);
                break;
        }
        List<int> used = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            int choice = -1;
            regen:
            if (type == 0)
                choice = UnityEngine.Random.Range(1, icons.Length);
            else if (type == 1)
                choice = UnityEngine.Random.Range(1, ships.Length);
            else if (type == 2)
                choice = UnityEngine.Random.Range(1, balls.Length);
            else if (type == 3)
                choice = UnityEngine.Random.Range(1, ufos.Length);
            else if (type == 4)
                choice = UnityEngine.Random.Range(1, waves.Length);
            else if (type == 5)
                choice = UnityEngine.Random.Range(1, robots.Length);
            else if (type == 6)
                choice = UnityEngine.Random.Range(1, spiders.Length);
            if (!used.Contains(choice))
            {
                used.Add(choice);
                usedPositions[i] = choice;
                switch (type)
                {
                    case 0:
                        slots[i].sprite = icons[choice];
                        if (choice == 15)
                            slots[i].gameObject.transform.localPosition = new Vector3(0, 1, 0.1f);
                        break;
                    case 1:
                        slots[i].sprite = ships[choice];
                        slots[i].gameObject.transform.localPosition = new Vector3(0, 1, 0);
                        slots[i].gameObject.transform.localScale = new Vector3(0.3645455f, 0.3645462f, 10000);
                        break;
                    case 2:
                        slots[i].sprite = balls[choice];
                        slots[i].gameObject.transform.localScale = new Vector3(0.4545455f, 0.4545462f, 10000);
                        break;
                    case 3:
                        slots[i].sprite = ufos[choice];
                        slots[i].gameObject.transform.localScale = new Vector3(0.3945455f, 0.3945462f, 10000);
                        break;
                    case 4:
                        slots[i].sprite = waves[choice];
                        slots[i].gameObject.transform.localScale = new Vector3(0.1845455f, 0.1845462f, 10000);
                        break;
                    case 5:
                        slots[i].sprite = robots[choice];
                        slots[i].gameObject.transform.localScale = new Vector3(0.3045455f, 0.3045462f, 10000);
                        break;
                    case 6:
                        slots[i].sprite = spiders[choice];
                        slots[i].gameObject.transform.localScale = new Vector3(0.3845455f, 0.3845462f, 10000);
                        break;
                }
            }
            else
                goto regen;
        }
        List<string> namedItems = new List<string>();
        for (int i = 0; i < 5; i++)
        {
            string prefix = names[stageOrder[stage]];
            if (prefix.Equals("gravity ball"))
                prefix = "ball";
            if (prefix.Equals("cube"))
                prefix = "icon";
            if (usedPositions[i] == 0)
                namedItems.Add(prefix + "def");
            else
                namedItems.Add(prefix + usedPositions[i]);
        }
        Debug.LogFormat("[The Icon Kit #{0}] {1}", moduleId, namedItems.Join(" "));
        correctPos = GetCorrectButton(type);
        Debug.LogFormat("[The Icon Kit #{0}] The correct {1} is {2}.", moduleId, names[stageOrder[stage]], namedItems[correctPos]);
    }

    int GetCorrectButton(int type)
    {
        for (int i = 0; i < 21; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                switch (type)
                {
                    case 0:
                        if (iconOrder[i] == usedPositions[j])
                        {
                            switch (usedPositions[j])
                            {
                                case 0:
                                    return j;
                                case 1:
                                    if (generatedStats[3] >= 4) return j;
                                    break;
                                case 2:
                                    if (generatedStats[0] >= 300) return j;
                                    break;
                                case 3:
                                    if (generatedStats[0] >= 1000) return j;
                                    break;
                                case 4:
                                    if (generatedStats[1] >= 25) return j;
                                    break;
                                case 5:
                                    if (generatedStats[3] >= 30) return j;
                                    break;
                                case 6:
                                    if (generatedStats[1] >= 60) return j;
                                    break;
                                case 7:
                                    if (generatedStats[2] >= 150) return j;
                                    break;
                                case 8:
                                    if (generatedStats[2] >= 60) return j;
                                    break;
                                case 9:
                                    if (generatedStats[1] >= 110) return j;
                                    break;
                                case 10:
                                    if (generatedStats[0] >= 3000) return j;
                                    break;
                                case 11:
                                    if (generatedStats[0] >= 5500) return j;
                                    break;
                                case 12:
                                    if (generatedStats[0] >= 9000) return j;
                                    break;
                                case 13:
                                    if (generatedStats[6] >= 5) return j;
                                    break;
                                case 14:
                                    if (generatedStats[4] >= 250) return j;
                                    break;
                                case 15:
                                    if (generatedStats[9] >= 15) return j;
                                    break;
                                case 16:
                                    if (generatedStats[2] >= 500) return j;
                                    break;
                                case 17:
                                    if (generatedStats[5] >= 5 && generatedStats[6] >= 5 && generatedStats[7] >= 5 && generatedStats[8] >= 5 && generatedStats[9] >= 5) return j;
                                    break;
                                case 18:
                                    if (generatedStats[2] >= 225) return j;
                                    break;
                                case 19:
                                    if (generatedStats[4] >= 2000) return j;
                                    break;
                                default:
                                    if (generatedStats[7] >= 35) return j;
                                    break;
                            }
                        }
                        break;
                    case 1:
                        if (shipOrder[i] == usedPositions[j])
                        {
                            switch (usedPositions[j])
                            {
                                case 0:
                                    return j;
                                case 1:
                                    if (generatedStats[3] >= 5) return j;
                                    break;
                                case 2:
                                    if (generatedStats[0] >= 500) return j;
                                    break;
                                case 3:
                                    if (generatedStats[3] >= 15) return j;
                                    break;
                                case 4:
                                    if (generatedStats[1] >= 55) return j;
                                    break;
                                case 5:
                                    if (generatedStats[1] >= 90) return j;
                                    break;
                                case 6:
                                    if (generatedStats[0] >= 1500) return j;
                                    break;
                                case 7:
                                    if (generatedStats[0] >= 3500) return j;
                                    break;
                                case 8:
                                    if (generatedStats[2] >= 180) return j;
                                    break;
                                case 9:
                                    if (generatedStats[1] >= 115) return j;
                                    break;
                                case 10:
                                    if (generatedStats[2] >= 30) return j;
                                    break;
                                case 11:
                                    if (generatedStats[8] >= 15) return j;
                                    break;
                                case 12:
                                    if (generatedStats[0] >= 7000) return j;
                                    break;
                                case 13:
                                    if (generatedStats[6] >= 65) return j;
                                    break;
                                case 14:
                                    if (generatedStats[4] >= 2500) return j;
                                    break;
                                case 15:
                                    if (generatedStats[2] >= 800) return j;
                                    break;
                                case 16:
                                    if (generatedStats[5] >= 35 && generatedStats[6] >= 35 && generatedStats[7] >= 35 && generatedStats[8] >= 35 && generatedStats[9] >= 35) return j;
                                    break;
                                default:
                                    if (generatedStats[3] >= 60) return j;
                                    break;
                            }
                        }
                        break;
                    case 2:
                        if (ballOrder[i] == usedPositions[j])
                        {
                            switch (usedPositions[j])
                            {
                                case 0:
                                    return j;
                                case 1:
                                    if (generatedStats[1] >= 35) return j;
                                    break;
                                case 2:
                                    if (generatedStats[3] >= 20) return j;
                                    break;
                                case 3:
                                    if (generatedStats[1] >= 70) return j;
                                    break;
                                case 4:
                                    if (generatedStats[0] >= 2000) return j;
                                    break;
                                case 5:
                                    if (generatedStats[3] >= 40) return j;
                                    break;
                                case 6:
                                    if (generatedStats[1] >= 105) return j;
                                    break;
                                case 7:
                                    if (generatedStats[2] >= 40) return j;
                                    break;
                                case 8:
                                    if (generatedStats[2] >= 190) return j;
                                    break;
                                case 9:
                                    if (generatedStats[7] >= 5) return j;
                                    break;
                                case 10:
                                    if (generatedStats[5] >= 15) return j;
                                    break;
                                case 11:
                                    if (generatedStats[0] >= 6000) return j;
                                    break;
                                case 12:
                                    if (generatedStats[2] >= 900) return j;
                                    break;
                                default:
                                    if (generatedStats[2] >= 425) return j;
                                    break;
                            }
                        }
                        break;
                    case 3:
                        if (ufoOrder[i] == usedPositions[j])
                        {
                            switch (usedPositions[j])
                            {
                                case 0:
                                    return j;
                                case 1:
                                    if (generatedStats[1] >= 20) return j;
                                    break;
                                case 2:
                                    if (generatedStats[1] >= 80) return j;
                                    break;
                                case 3:
                                    if (generatedStats[2] >= 50) return j;
                                    break;
                                case 4:
                                    if (generatedStats[3] >= 50) return j;
                                    break;
                                case 5:
                                    if (generatedStats[2] >= 160) return j;
                                    break;
                                case 6:
                                    if (generatedStats[3] >= 120) return j;
                                    break;
                                case 7:
                                    if (generatedStats[7] >= 15) return j;
                                    break;
                                case 8:
                                    if (generatedStats[5] >= 15 && generatedStats[6] >= 15 && generatedStats[7] >= 15 && generatedStats[8] >= 15 && generatedStats[9] >= 15) return j;
                                    break;
                                case 9:
                                    if (generatedStats[4] >= 3000) return j;
                                    break;
                                case 10:
                                    if (generatedStats[8] >= 35) return j;
                                    break;
                                default:
                                    if (generatedStats[2] >= 1000) return j;
                                    break;
                            }
                        }
                        break;
                    case 4:
                        if (waveOrder[i] == usedPositions[j])
                        {
                            switch (usedPositions[j])
                            {
                                case 0:
                                    return j;
                                case 1:
                                    if (generatedStats[2] >= 20) return j;
                                    break;
                                case 2:
                                    if (generatedStats[2] >= 120) return j;
                                    break;
                                case 3:
                                    if (generatedStats[2] >= 170) return j;
                                    break;
                                case 4:
                                    if (generatedStats[0] >= 2500) return j;
                                    break;
                                case 5:
                                    if (generatedStats[9] >= 5) return j;
                                    break;
                                case 6:
                                    if (generatedStats[0] >= 4500) return j;
                                    break;
                                case 7:
                                    if (generatedStats[4] >= 4000) return j;
                                    break;
                                case 8:
                                    if (generatedStats[4] >= 1500) return j;
                                    break;
                                default:
                                    if (generatedStats[2] >= 600) return j;
                                    break;
                            }
                        }
                        break;
                    case 5:
                        if (robotOrder[i] == usedPositions[j])
                        {
                            switch (usedPositions[j])
                            {
                                case 0:
                                    return j;
                                case 1:
                                    if (generatedStats[2] >= 200) return j;
                                    break;
                                case 2:
                                    if (generatedStats[2] >= 100) return j;
                                    break;
                                case 3:
                                    if (generatedStats[0] >= 10000) return j;
                                    break;
                                case 4:
                                    if (generatedStats[4] >= 500) return j;
                                    break;
                                case 5:
                                    if (generatedStats[2] >= 700) return j;
                                    break;
                                case 6:
                                    if (generatedStats[5] >= 65 && generatedStats[6] >= 65 && generatedStats[7] >= 65 && generatedStats[8] >= 65 && generatedStats[9] >= 65) return j;
                                    break;
                                default:
                                    if (generatedStats[8] >= 65) return j;
                                    break;
                            }
                        }
                        break;
                    default:
                        if (spiderOrder[i] == usedPositions[j])
                        {
                            switch (usedPositions[j])
                            {
                                case 0:
                                    return j;
                                case 1:
                                    if (generatedStats[0] >= 8000) return j;
                                    break;
                                case 2:
                                    if (generatedStats[7] >= 65) return j;
                                    break;
                                case 3:
                                    if (generatedStats[6] >= 35) return j;
                                    break;
                                case 4:
                                    if (generatedStats[2] >= 300) return j;
                                    break;
                                default:
                                    if (generatedStats[9] >= 65) return j;
                                    break;
                            }
                        }
                        break;
                }
            }
        }
        return -1;
    }

    //twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} start [Presses the button to start the module] | !{0} shards [Opens/closes the shards menu] | !{0} select <pos> [Selects the option in the specified position] | Valid positions are 1-5 from left to right";
    #pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        if (Regex.IsMatch(command, @"^\s*start\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (isSceneTwo)
                yield return "sendtochaterror The module has already been started!";
            buttons[0].OnInteract();
            yield break;
        }
        if (Regex.IsMatch(command, @"^\s*shards\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (!isSceneTwo)
                yield return "sendtochaterror The module must be started first!";
            if (!shardsMenuOpen)
                buttons[1].OnInteract();
            else
                buttons[2].OnInteract();
            yield break;
        }
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*select\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (parameters.Length == 1)
            {
                yield return "sendtochaterror Please specify the position of an option to select!";
            }
            else if (parameters.Length == 2)
            {
                int temp = -1;
                if (!int.TryParse(parameters[1], out temp))
                {
                    yield return "sendtochaterror!f The specified position '" + parameters[1] + "' is invalid!";
                    yield break;
                }
                if (temp < 1 || temp > 5)
                {
                    yield return "sendtochaterror The specified position '" + parameters[1] + "' is out of range 1-5!";
                    yield break;
                }
                if (!isSceneTwo)
                    yield return "sendtochaterror The module must be started first!";
                buttons[temp + 2].OnInteract();
            }
            else if (parameters.Length > 2)
            {
                yield return "sendtochaterror Too many parameters!";
            }
            yield break;
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        if (!isSceneTwo)
        {
            buttons[0].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
        else if (shardsMenuOpen)
        {
            buttons[2].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
        int start = stage;
        for (int i = start; i < 4; i++)
        {
            buttons[correctPos + 3].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
    }
}

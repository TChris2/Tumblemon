using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Battle : MonoBehaviour
{
    [SerializeField]
    private float waitTime = 1;
    private MoveDatabase moveDatabase;
    private TypeDatabase typeDatabase;
    private PartySelect partySelect;
    // Used if a mon has run out power points on all of their moves
    private MoveInfo struggle;
    public MonParty Trainer1Party;
    public MonParty Trainer2Party;
    TrainerInfo winningTrainer = null;
    TrainerInfo lossingTrainer = null;
    // Displays Action Text
    TMPro.TMP_Text ActionText;
    private ChristinaCreatesGames.Typography.Typewriter.TypewriterEffect typewriter;
    private bool isTextDone = false;
    private bool isAnimationDone = false;
    private bool isCountingDone = false;
    private bool isAttackingDone = true;
    private Animator fadeAnimator;

    // HP health colors
    [SerializeField]
    private Color green;
    [SerializeField]
    private Color yellow;
    [SerializeField]
    private Color red;
    private AudioSource audiosource;
    [SerializeField]
    private AudioClip winMusic;
    private bool isFirstTurn = true;
    AudioClip clip;


    // Start is called before the first frame update
    void Start()
    {
        // Gets struggle from the move datebase
        moveDatabase = FindObjectOfType<MoveDatabase>();
        typeDatabase = FindObjectOfType<TypeDatabase>();
        audiosource = moveDatabase.GetComponent<AudioSource>();
        struggle = moveDatabase.GetMoveByName("Struggle");
        partySelect = moveDatabase.GetComponent<PartySelect>();
        Trainer1Party = partySelect.Trainer1Party;
        Trainer2Party = partySelect.Trainer2Party;
        
        // Gets Text Boxes
        ActionText = GameObject.Find("Action Text").GetComponent<TMPro.TMP_Text>();
        typewriter = ActionText.GetComponent<ChristinaCreatesGames.Typography.Typewriter.TypewriterEffect>();

        // Trainer 1
        // Text Stuff
        Trainer1Party.HealthBar = GameObject.Find("Back Health Bar").GetComponent<Image>();
        Trainer1Party.NameText = GameObject.Find("Back Name Text").GetComponent<TMPro.TMP_Text>();
        Trainer1Party.HealthText = GameObject.Find("Back Health Text").GetComponent<TMPro.TMP_Text>();
        Trainer1Party.TotalHealthText = GameObject.Find("Back Total Health Text").GetComponent<TMPro.TMP_Text>();
        Trainer1Party.LevelText = GameObject.Find("Back Lvl Text").GetComponent<TMPro.TMP_Text>();
        // Animators
        Trainer1Party.MonAnimator = GameObject.Find("Back").GetComponent<Animator>();
        Trainer1Party.audiosource = Trainer1Party.MonAnimator.GetComponent<AudioSource>();
        Trainer1Party.TextBoxCanvas = GameObject.Find("Back Mon TextBox").GetComponent<CanvasGroup>();
        Trainer1Party.AttackAnimator = GameObject.Find("Back Attack").GetComponent<Animator>();
        // Trainer 2
        // Text Stuff
        Trainer2Party.HealthBar = GameObject.Find("Fore Health Bar").GetComponent<Image>();
        Trainer2Party.NameText = GameObject.Find("Fore Name Text").GetComponent<TMPro.TMP_Text>();
        Trainer2Party.LevelText = GameObject.Find("Fore Lvl Text").GetComponent<TMPro.TMP_Text>();
        // Animators
        Trainer2Party.MonAnimator = GameObject.Find("Fore").GetComponent<Animator>();
        Trainer2Party.audiosource = Trainer2Party.MonAnimator.GetComponent<AudioSource>();
        Trainer2Party.TextBoxCanvas = GameObject.Find("Fore Mon TextBox").GetComponent<CanvasGroup>();
        Trainer2Party.AttackAnimator = GameObject.Find("Fore Attack").GetComponent<Animator>();
        
        fadeAnimator = GameObject.Find("Canvas").GetComponent<Animator>();

        // Starts Battles
        StartCoroutine(TumblemonBattle());
    }

    // Waits for animation
    private IEnumerator WaitForAnimation(Animator animator, string stateName)
    {
        // Wait for the animator to enter the state
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            yield return null;

        // Wait until it finishes
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            yield return null;
    }

    // Stuff used by typewriter script
    private void OnTypewriterDone()
    {
        isTextDone = true;
    }

    void OnEnable()
    {
        ChristinaCreatesGames.Typography.Typewriter.TypewriterEffect.CompleteTextRevealed += OnTypewriterDone;
    }

    void OnDisable()
    {
        ChristinaCreatesGames.Typography.Typewriter.TypewriterEffect.CompleteTextRevealed -= OnTypewriterDone;
    }

    // Updates textboxes with new information during a swap / send in of a mon
    IEnumerator UpdateMonTextboxSwap(MonParty ActingParty, string text, bool isTrainer1, bool isIntro)
    {
        Debug.Log($"{ActingParty.Trainer.name} has entered UpdateMonTextboxSwap");
        isAnimationDone = false;
        
        // Updates textboxes
        ActingParty.NameText.text = ActingParty.MonTeam[ActingParty.currentMon].name;
        ActingParty.LevelText.text = $"{ActingParty.MonTeam[ActingParty.currentMon].level}";
        // Updates visible HP for Trainer 1
        if (isTrainer1) 
        {
            ActingParty.HealthText.text = $"{ActingParty.MonTeam[ActingParty.currentMon].stats.health}";
            ActingParty.TotalHealthText.text = $"{ActingParty.MonTeam[ActingParty.currentMon].stats.total_health}";
        }

        // Updates to show mon's hp amt
        ActingParty.HealthBar.fillAmount = (float)ActingParty.MonTeam[ActingParty.currentMon].stats.health / 
            ActingParty.MonTeam[ActingParty.currentMon].stats.total_health;
        // Updates color of hp bar
        ActingParty.HealthBar.color = GetHPBarColor(ActingParty);

        // Gives a pause to display the text
        isTextDone = false;
        ActionText.text = text;
        typewriter.SendMessage("PrepareForNewText", ActionText);
        if (!isTrainer1 && isIntro)
        {
            clip = Resources.Load<AudioClip>($"SFX/{ActingParty.Trainer.introSFX}");
            ActingParty.audiosource.PlayOneShot(clip);
            yield return new WaitForSeconds(waitTime/4);
        }
        yield return new WaitUntil(() => isTextDone);
        if (!isIntro)
            yield return new WaitForSeconds(waitTime);

        // Sends out the new mon
        ActingParty.MonAnimator.Play($"{ActingParty.MonTeam[ActingParty.currentMon].name}");
        if (isIntro)
            ActingParty.MonAnimator.Play("Trainer Exit", -1, 0f);
        else
            ActingParty.MonAnimator.Play("Send");

        if (isTrainer1)
            yield return new WaitForSeconds(2f);
        else
            yield return new WaitForSeconds(.7f);

        ActingParty.TextBoxCanvas.alpha = 1;
        ActingParty.MonAnimator.Play("TextBox Move Onscreen", -1, 0f);
        clip = Resources.Load<AudioClip>($"SFX/Intro SFX/{ActingParty.MonTeam[ActingParty.currentMon].name}");
        ActingParty.audiosource.PlayOneShot(clip);

        // Waits till animation is done
        if (isIntro)
            yield return StartCoroutine(WaitForAnimation(ActingParty.MonAnimator, "Trainer Exit"));
        else
            yield return StartCoroutine(WaitForAnimation(ActingParty.MonAnimator, "Send"));

        Debug.Log($"{ActingParty.Trainer.name} has exitted UpdateMonTextboxSwap");
        // Sends a signal to move on the next thing
        isAnimationDone = true;
    }

    // Updates the HP Bar
    private IEnumerator HealthBarTicker(MonParty DefParty, int amt, bool isTrainer1, bool isHealing)
    {
        Debug.Log($"{DefParty.Trainer.name} has entered HealthBarTicker");
        int currentHealth = 0;
        // If healing
        if (isHealing)
            currentHealth = DefParty.MonTeam[DefParty.currentMon].stats.health;
        // If being attacked
        else 
        {
            currentHealth = DefParty.MonTeam[DefParty.currentMon].stats.health - amt;
            // Ensures hp stays at 0
            if (currentHealth <= 0)
                currentHealth = 0;
        }

        //Debug.Log($"Health tick {currentHealth}");

        // Loops until hp matches
        do
        {
            if (DefParty.MonTeam[DefParty.currentMon].stats.health <= 0)
                break;
            // Slight pause for the ticker
            yield return new WaitForSeconds(.025f);

            // Also updates trainer1's hp textboxes
            if (isTrainer1) 
            {
                DefParty.HealthText.text = $"{DefParty.MonTeam[DefParty.currentMon].stats.health}";
                DefParty.TotalHealthText.text = $"{DefParty.MonTeam[DefParty.currentMon].stats.total_health}";
            }

            // Updates hp bar
            DefParty.HealthBar.fillAmount = (float)DefParty.MonTeam[DefParty.currentMon].stats.health / DefParty.MonTeam[DefParty.currentMon].stats.total_health;
            // Updates color of hp bar
            DefParty.HealthBar.color = GetHPBarColor(DefParty);
            // Increases health if healing
            if (isHealing)
                DefParty.MonTeam[DefParty.currentMon].stats.health++;
            // Decrease's health if getting attacked
            else
                DefParty.MonTeam[DefParty.currentMon].stats.health--;

        } while (DefParty.MonTeam[DefParty.currentMon].stats.health != currentHealth);

        // Also updates trainer1's hp textboxes
        if (isTrainer1) 
        {
            DefParty.HealthText.text = $"{DefParty.MonTeam[DefParty.currentMon].stats.health}";
            DefParty.TotalHealthText.text = $"{DefParty.MonTeam[DefParty.currentMon].stats.total_health}";
        }
        // Updates hp bar
        DefParty.HealthBar.fillAmount = (float)DefParty.MonTeam[DefParty.currentMon].stats.health / DefParty.MonTeam[DefParty.currentMon].stats.total_health;
        // Updates color of hp bar
        DefParty.HealthBar.color = GetHPBarColor(DefParty);
        
        Debug.Log($"{DefParty.Trainer.name} has exitted HealthBarTicker");
        // Sends a signal signifying counting is done
        isCountingDone = true;
    }

    // Gets color of hp bar depending on how much health the user has
    Color GetHPBarColor(MonParty ActingParty)
    {
        // Green if above 50%
        if (ActingParty.HealthBar.fillAmount >= .5f)
        {
            return green;
        }
        // Yellow if between 50-20%
        else if (ActingParty.HealthBar.fillAmount < .5f && ActingParty.HealthBar.fillAmount >= .20f)
        {
            return yellow;
        }
        // Red for below 20%
        else
        {
            return red;
        }
    }
    
    // Battle itself
    private IEnumerator TumblemonBattle()
    {
        yield return StartCoroutine(WaitForAnimation(fadeAnimator, "Intro"));
        Debug.Log("Start of Battle");
        Debug.Log("-------------------------------------------------------------------------------------");

        // Starting Text
        Debug.Log($"{Trainer1Party.Trainer.name} is challenged by {Trainer2Party.Trainer.name}!");
        isTextDone = false;
        ActionText.text = $"{Trainer1Party.Trainer.name} is challenged by {Trainer2Party.Trainer.name}!";
        typewriter.SendMessage("PrepareForNewText", ActionText);
        yield return new WaitUntil(() => isTextDone);
        yield return new WaitForSeconds(waitTime);

        yield return StartCoroutine(UpdateMonTextboxSwap(Trainer2Party, $"{Trainer2Party.Trainer.name} sent out {Trainer2Party.MonTeam[Trainer2Party.currentMon].name}!", false, true));
        yield return new WaitUntil(() => isTextDone && isAnimationDone);
        yield return new WaitForSeconds(waitTime);

        yield return StartCoroutine(UpdateMonTextboxSwap(Trainer1Party, $"Go {Trainer1Party.MonTeam[Trainer1Party.currentMon].name}!", true, true));
        yield return new WaitUntil(() => isTextDone && isAnimationDone);
        yield return new WaitForSeconds(waitTime);

        // Loops until on of the trainers runs out of mons
        while (true)
        {
            Debug.Log("Trainers are deciding...");
            Debug.Log("-------------------------------------------------------------------------------------");

            isTextDone = false;
            ActionText.text = $"Trainers are deciding...";
            typewriter.SendMessage("PrepareForNewText", ActionText);
            yield return new WaitUntil(() => isTextDone);
            yield return new WaitForSeconds(waitTime);

            // Trainer 1 action decision
            var (T1action, T1moveSelected, T1currentMon) = TrainerActionDecide(Trainer1Party, Trainer2Party);
            Trainer1Party.action = T1action;
            Trainer1Party.moveSelected = T1moveSelected;
            Debug.Log($"{Trainer1Party.Trainer.name} has decided to {Trainer1Party.action}");
            Debug.Log("-------------------------------------------------------------------------------------");

            // Trainer 2 action decision
            var (T2action, T2moveSelected, T2currentMon) = TrainerActionDecide(Trainer2Party, Trainer1Party);
            Trainer2Party.action = T2action;
            Trainer2Party.moveSelected = T2moveSelected;
            Debug.Log($"{Trainer2Party.Trainer.name} has decided to {Trainer2Party.action}");
            Debug.Log("-------------------------------------------------------------------------------------");

            yield return null;
            if (isFirstTurn)
            {
                isFirstTurn = false;
                fadeAnimator.enabled = false;
            }

            // Swapping current mon
            if (Trainer1Party.action == "Swap")
            {
                Debug.Log($"{Trainer1Party.Trainer.name} is swapping {Trainer1Party.MonTeam[Trainer1Party.currentMon].name} to {Trainer1Party.MonTeam[T1currentMon].name}");
                Debug.Log("-------------------------------------------------------------------------------------");

                yield return StartCoroutine(Swap(Trainer1Party, T1currentMon, $"{Trainer1Party.MonTeam[Trainer1Party.currentMon].name}, switch out!\nCome Back!"));
                yield return new WaitUntil(() => isTextDone && isAnimationDone);
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(UpdateMonTextboxSwap(Trainer1Party, $"Go {Trainer1Party.MonTeam[Trainer1Party.currentMon].name}!", true, false));
                yield return new WaitUntil(() => isTextDone && isAnimationDone);
                yield return new WaitForSeconds(waitTime);
            }
            if (Trainer2Party.action == "Swap")
            {
                Debug.Log($"{Trainer2Party.Trainer.name} is swapping {Trainer2Party.MonTeam[Trainer2Party.currentMon].name} to {Trainer2Party.MonTeam[T2currentMon].name}");
                Debug.Log("-------------------------------------------------------------------------------------");
                yield return StartCoroutine(Swap(Trainer2Party, T2currentMon, $"{Trainer2Party.Trainer.name} switched out {Trainer2Party.MonTeam[Trainer2Party.currentMon].name}!"));
                yield return new WaitUntil(() => isTextDone && isAnimationDone);
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(UpdateMonTextboxSwap(Trainer2Party, $"{Trainer2Party.Trainer.name} sent out {Trainer2Party.MonTeam[Trainer2Party.currentMon].name}!", false, false));
                yield return new WaitUntil(() => isTextDone && isAnimationDone);
                yield return new WaitForSeconds(waitTime);
            }
            
            // Healing current mon
            if (Trainer1Party.action == "Heal")
            {
                Debug.Log($"{Trainer1Party.Trainer.name} is healing {Trainer1Party.MonTeam[Trainer1Party.currentMon].name}");
                Debug.Log("-------------------------------------------------------------------------------------");
                yield return StartCoroutine(Heal(Trainer1Party, $"{Trainer1Party.Trainer.name} used a Full Restore.", true));
                yield return new WaitUntil(() => isTextDone && isAnimationDone && isCountingDone && isAttackingDone);
                yield return new WaitForSeconds(waitTime);
            }
            if (Trainer2Party.action == "Heal")
            {
                Debug.Log($"{Trainer2Party.Trainer.name} is healing {Trainer2Party.MonTeam[Trainer2Party.currentMon].name}");
                Debug.Log("-------------------------------------------------------------------------------------");
                yield return StartCoroutine(Heal(Trainer2Party, $"{Trainer2Party.Trainer.name} used a Full Restore.", false));
                yield return new WaitUntil(() => isTextDone && isAnimationDone && isCountingDone && isAttackingDone);
                yield return new WaitForSeconds(waitTime);
            }

            bool Trainer1First = Trainer1Party.MonTeam[Trainer1Party.currentMon].stats.speed > Trainer2Party.MonTeam[Trainer2Party.currentMon].stats.speed
                || Trainer1Party.MonTeam[Trainer1Party.currentMon].stats.speed == Trainer2Party.MonTeam[Trainer2Party.currentMon].stats.speed && UnityEngine.Random.value < 0.5f;

            // Trainer 1 goes first
            if (Trainer1First)
            {
                yield return new WaitForSeconds(waitTime);

                Debug.Log($"{Trainer1Party.Trainer.name} GOES FIRST! {Trainer2Party.Trainer.name} GOES SECOND!");
                Debug.Log("-------------------------------------------------------------------------------------");

                if (Trainer1Party.action == "Attack") 
                {
                    Debug.Log($"{Trainer1Party.Trainer.name} is attacking with {Trainer1Party.moveSelected.name}");
                    Debug.Log("-------------------------------------------------------------------------------------");
                    yield return StartCoroutine(Attack(Trainer1Party, Trainer2Party, true));
                    yield return new WaitUntil(() => isTextDone && isAnimationDone && isCountingDone && isAttackingDone);
                    yield return new WaitForSeconds(waitTime);
                }

                if (Trainer2Party.action == "Attack" && Trainer2Party.MonTeam[Trainer2Party.currentMon].stats.health > 0) 
                {
                    Debug.Log($"{Trainer2Party.Trainer.name} is attacking with {Trainer2Party.moveSelected.name}");
                    Debug.Log("-------------------------------------------------------------------------------------");
                    yield return StartCoroutine(Attack(Trainer2Party, Trainer1Party, false));
                    yield return new WaitUntil(() => isTextDone && isAnimationDone && isCountingDone && isAttackingDone);
                    yield return new WaitForSeconds(waitTime);
                }
            }
            // Trainer 2 goes first
            else
            {
                Debug.Log($"{Trainer2Party.Trainer.name} GOES FIRST! {Trainer1Party.Trainer.name} GOES SECOND!");
                Debug.Log("-------------------------------------------------------------------------------------");

                if (Trainer2Party.action == "Attack") 
                {
                    Debug.Log($"{Trainer2Party.Trainer.name} is attacking with {Trainer2Party.moveSelected.name}");
                    Debug.Log("-------------------------------------------------------------------------------------");
                    yield return StartCoroutine(Attack(Trainer2Party, Trainer1Party, false));
                    yield return new WaitUntil(() => isTextDone && isAnimationDone && isCountingDone);
                    yield return new WaitForSeconds(waitTime);
                }

                if (Trainer1Party.action == "Attack" && Trainer1Party.MonTeam[Trainer1Party.currentMon].stats.health > 0) 
                {
                    Debug.Log($"{Trainer1Party.Trainer.name} is attacking with {Trainer1Party.moveSelected.name}");
                    Debug.Log("-------------------------------------------------------------------------------------");
                    yield return StartCoroutine(Attack(Trainer1Party, Trainer2Party, true));
                    yield return new WaitUntil(() => isTextDone && isAnimationDone && isCountingDone);
                    yield return new WaitForSeconds(waitTime);
                }
            }
            
            // If their current mon has fainted
            if (Trainer1Party.MonTeam[Trainer1Party.currentMon].stats.health <= 0)
            {
                yield return StartCoroutine(FaintedMonSwap(Trainer1Party, Trainer2Party));
                yield return new WaitUntil(() => isTextDone && isAnimationDone);
                yield return new WaitForSeconds(waitTime);

                // Checks to see if the trainer has run out of mons after they both do their actions
                if (IsPartyDead(Trainer1Party))
                {
                    winningTrainer = Trainer2Party.Trainer;
                    lossingTrainer = Trainer1Party.Trainer;
                    break;
                }
                // Allows them to swap to another mon
                else
                {
                    yield return StartCoroutine(UpdateMonTextboxSwap(Trainer1Party, $"Go {Trainer1Party.MonTeam[Trainer1Party.currentMon].name}!", true, false));
                    yield return new WaitUntil(() => isTextDone && isAnimationDone);
                    yield return new WaitForSeconds(waitTime);
                }
            }
            if (Trainer2Party.MonTeam[Trainer2Party.currentMon].stats.health <= 0)
            {
                yield return StartCoroutine(FaintedMonSwap(Trainer2Party, Trainer1Party));
                yield return new WaitUntil(() => isTextDone && isAnimationDone);
                yield return new WaitForSeconds(waitTime);

                // Checks to see if the trainer has run out of mons after they both do their actions
                if (IsPartyDead(Trainer2Party))
                {
                    winningTrainer = Trainer1Party.Trainer;
                    lossingTrainer = Trainer2Party.Trainer;
                    break;
                }
                // Allows them to swap to another mon
                else 
                {
                    yield return StartCoroutine(UpdateMonTextboxSwap(Trainer2Party, $"{Trainer2Party.Trainer.name} sent out {Trainer2Party.MonTeam[Trainer2Party.currentMon].name}!", false, false));
                    yield return new WaitUntil(() => isTextDone && isAnimationDone);
                    yield return new WaitForSeconds(waitTime);
                }
                
            }

            Debug.Log("End of turn");
            Debug.Log("-------------------------------------------------------------------------------------");
            ActionText.text = " \n\n\n\n\n\naaaaaaa";
            yield return null;
        }
        Debug.Log("-------------------------------------------------------------------------------------");
        Debug.Log("BATTLE OVER");
        Debug.Log("-------------------------------------------------------------------------------------");

        audiosource.clip = winMusic;
        audiosource.Play();

        isTextDone = false;
        Debug.Log($"{winningTrainer.name} defeated {lossingTrainer.name}!");
        ActionText.text = $"{winningTrainer.name} defeated {lossingTrainer.name}!";
        typewriter.SendMessage("PrepareForNewText", ActionText);
        Trainer2Party.MonAnimator.Play("Trainer Enter");
        yield return new WaitUntil(() => isTextDone);
        yield return new WaitForSeconds(waitTime);

        isTextDone = false;

        if (winningTrainer.name == Trainer2Party.Trainer.name)
            ActionText.text = $"{Trainer2Party.Trainer.winText}";
        else
            ActionText.text = $"{Trainer2Party.Trainer.loseText}";

        typewriter.SendMessage("PrepareForNewText", ActionText);
        yield return new WaitUntil(() => isTextDone);
        yield return new WaitForSeconds(waitTime+10);
        
        fadeAnimator.enabled = true;
        fadeAnimator.Play("Fade To Black");
    }

    // Checks to see if trainer has run of mons
    bool IsPartyDead(MonParty p) => p.MonTeam.TrueForAll(mon => mon.stats.health <= 0);

    // If a trainer's mon died in battle but has not run out of mons to swap to
    IEnumerator FaintedMonSwap(MonParty ActingParty, MonParty OpParty)
    {
        Debug.Log($"{ActingParty.Trainer.name} has entered FaintedMonSwap");
        isAnimationDone = false;
        
        Debug.Log($"{ActingParty.Trainer.name}'s {ActingParty.MonTeam[ActingParty.currentMon].name} fainted!");
        Debug.Log("-------------------------------------------------------------------------------------");

        isTextDone = false;
        ActionText.text = $"{ActingParty.MonTeam[ActingParty.currentMon].name} fainted!";
        typewriter.SendMessage("PrepareForNewText", ActionText);
        yield return new WaitUntil(() => isTextDone);
        yield return new WaitForSeconds(waitTime);

        ActingParty.MonAnimator.Play($"Faint");
        clip = Resources.Load<AudioClip>($"SFX/Faint SFX/{ActingParty.MonTeam[ActingParty.currentMon].name}");
        ActingParty.audiosource.PlayOneShot(clip);
        yield return StartCoroutine(WaitForAnimation(ActingParty.MonAnimator, $"Faint"));

        ActingParty.TextBoxCanvas.alpha = 0;

        ActingParty.MonTeam.RemoveAt(ActingParty.currentMon);

        // Clones parties to simulate them
        MonParty simMy = ActingParty.Clone();
        MonParty simOpponent = OpParty.Clone();

        Minimax(simMy, simOpponent, 5, true, float.NegativeInfinity, float.PositiveInfinity, true,
                out string action, out MoveInfo move, out int swapIndex);

        ActingParty.currentMon = swapIndex;

        Debug.Log($"{ActingParty.Trainer.name} has exited FaintedMonSwap");
        isAnimationDone = true;
    }

    // Enacts Trainer Attack
    IEnumerator Attack(MonParty AtkParty, MonParty DefParty, bool isTrainer1)
    {
        Debug.Log($"{AtkParty.Trainer.name} has entered Attack");
        isAnimationDone = false;
        isCountingDone = false;
        isAttackingDone = false;

        int trainerDmg = 0;

        //Debug.Log($"{AtkParty.Trainer.name}'s {AtkParty.MonTeam[AtkParty.currentMon].name} USES {AtkParty.moveSelected.name}");
        if (AtkParty.moveSelected.name == "Struggle")
        {
            isTextDone = false;
            ActionText.text = $"{AtkParty.MonTeam[AtkParty.currentMon].name} has no moves left it can use!";
            typewriter.SendMessage("PrepareForNewText", ActionText);
            yield return new WaitUntil(() => isTextDone);
            yield return new WaitForSeconds(waitTime);
        }

        isTextDone = false;
        ActionText.text = $"{AtkParty.MonTeam[AtkParty.currentMon].name} used {AtkParty.moveSelected.name}!";
        typewriter.SendMessage("PrepareForNewText", ActionText);
        yield return new WaitUntil(() => isTextDone);
        yield return new WaitForSeconds(waitTime);
        AtkParty.TextBoxCanvas.alpha = 0;
        DefParty.TextBoxCanvas.alpha = 0;
        yield return new WaitForSeconds(waitTime/5);

        AtkParty.AttackAnimator.Play($"{AtkParty.moveSelected.name}", -1, 0f);

        yield return StartCoroutine(WaitForAnimation(AtkParty.AttackAnimator, $"{AtkParty.moveSelected.name}"));
        yield return new WaitForSeconds(waitTime/2);

        isAnimationDone = true;
        
        // Decreases the attacking mon move's power points
        AtkParty.moveSelected.pp--;

        // Gets damage info
        trainerDmg = DamageCal(AtkParty, DefParty, AtkParty.moveSelected);

        // Updates damage for counter moves
        if (isTrainer1)
        {
            AtkParty.currentDmg = trainerDmg;
        }
        else
        {
            DefParty.currentDmg = trainerDmg;
        }

        AtkParty.TextBoxCanvas.alpha = 1;
        DefParty.TextBoxCanvas.alpha = 1;
        
        if (trainerDmg > 0)
            yield return StartCoroutine(HealthBarTicker(DefParty, trainerDmg, !isTrainer1, false));

        if (AtkParty.moveSelected.recoilType != "None")
        {
            Debug.Log($"{AtkParty.MonTeam[AtkParty.currentMon].name} is damaged by recoil!");
            isTextDone = false;
            ActionText.text = $"{AtkParty.MonTeam[AtkParty.currentMon].name} is damaged by recoil!";
            typewriter.SendMessage("PrepareForNewText", ActionText);
            yield return new WaitUntil(() => isTextDone);
            yield return new WaitForSeconds(waitTime);

            if (AtkParty.moveSelected.recoilType != "Total Health")
                trainerDmg = (int)(AtkParty.MonTeam[AtkParty.currentMon].stats.total_health * AtkParty.moveSelected.recoil);

            yield return StartCoroutine(HealthBarTicker(AtkParty, trainerDmg, isTrainer1, false));
        }

        float type_modifier1 = TypeCheck(AtkParty.moveSelected.type, DefParty.MonTeam[DefParty.currentMon].type1);
        float type_modifier2;

        if (!string.IsNullOrEmpty(DefParty.MonTeam[DefParty.currentMon].type2))
            type_modifier2 = TypeCheck(AtkParty.moveSelected.type, DefParty.MonTeam[DefParty.currentMon].type2);
        else 
            type_modifier2 = 1;

        // Checks for type effectiveness
        float type_modifier = type_modifier1 * type_modifier2;

        if (type_modifier >= 2)
        {
            isTextDone = false;
            ActionText.text = $"It's super effective!";
            typewriter.SendMessage("PrepareForNewText", ActionText);
            yield return new WaitUntil(() => isTextDone);
            yield return new WaitForSeconds(waitTime);
        }
        if (type_modifier == .5)
        {
            isTextDone = false;
            ActionText.text = $"It's was not very effective...";
            typewriter.SendMessage("PrepareForNewText", ActionText);
            yield return new WaitUntil(() => isTextDone);
            yield return new WaitForSeconds(waitTime);
        }
        if (type_modifier == 0)
        {
            isTextDone = false;
            ActionText.text = $"It doesn't effect {DefParty.MonTeam[DefParty.currentMon].name}";
            typewriter.SendMessage("PrepareForNewText", ActionText);
            yield return new WaitUntil(() => isTextDone);
            yield return new WaitForSeconds(waitTime);
        }
        
        Debug.Log($"{AtkParty.Trainer.name} has exitted Attack");
        isAttackingDone = true;
    }

    // Heals Current Mon
    IEnumerator Heal(MonParty ActingParty, string text, bool isTrainer1)
    {   
        Debug.Log($"{ActingParty.Trainer.name} has entered Heal");
        isAnimationDone = false;
        isCountingDone = false;

        isTextDone = false;
        ActionText.text = text;
        typewriter.SendMessage("PrepareForNewText", ActionText);
        yield return new WaitUntil(() => isTextDone);
        yield return new WaitForSeconds(waitTime);

        if (isTrainer1)
            ActingParty.MonAnimator.Play("Heal");

        yield return StartCoroutine(WaitForAnimation(ActingParty.MonAnimator, "Heal"));

        isAnimationDone = true;

        yield return StartCoroutine(HealthBarTicker(ActingParty, 0, isTrainer1, true));

        isTextDone = false;
        ActionText.text = $"{ActingParty.MonTeam[ActingParty.currentMon].name} HP was restored.";
        typewriter.SendMessage("PrepareForNewText", ActionText);
        yield return new WaitUntil(() => isTextDone);
        yield return new WaitForSeconds(waitTime);

        ActingParty.MonTeam[ActingParty.currentMon].stats.health = ActingParty.MonTeam[ActingParty.currentMon].stats.total_health;
        ActingParty.MonTeam[ActingParty.currentMon].status = "None";
        ActingParty.potionCount--;    

        Debug.Log($"{ActingParty.Trainer.name} has exited Heal");
    }

    // Swaps Current Mon
    IEnumerator Swap(MonParty ActingParty, int newMon, string text)
    {
        Debug.Log($"{ActingParty.Trainer.name} has entered Swap");
        isAnimationDone = false;

        isTextDone = false;
        ActionText.text = text;
        typewriter.SendMessage("PrepareForNewText", ActionText);
        yield return new WaitUntil(() => isTextDone);
        yield return new WaitForSeconds(waitTime);
        
        ActingParty.TextBoxCanvas.alpha = 0;

        ActingParty.MonAnimator.Play("Recall");
        Debug.Log("Playing recall");

        yield return StartCoroutine(WaitForAnimation(ActingParty.MonAnimator, "Recall"));

        ActingParty.currentMon = newMon; 

        Debug.Log($"{ActingParty.Trainer.name} has exitted Swap");
        isAnimationDone = true;       
    }

    // Decides Trainer action
    (string, MoveInfo, int) TrainerActionDecide(MonParty myParty, MonParty opponentParty)
    {
        Debug.Log($"{myParty.Trainer.name}'s currentMon: {myParty.currentMon} (HP: {myParty.MonTeam[myParty.currentMon].stats.health})");
        // Clones parties to simulate them
        MonParty simMy = myParty.Clone();
        MonParty simOpponent = opponentParty.Clone();

        Minimax(simMy, simOpponent, 5, true, float.NegativeInfinity, float.PositiveInfinity, false,
                out string action, out MoveInfo move, out int swapIndex);
        Debug.Log($"AI decided: action={action}, move={move?.name}, swapIndex={swapIndex}");

        return (action, move, swapIndex);
    }

    // Does the minimax
    float Minimax(MonParty my, MonParty opp, int depth, bool isMax, float alpha, float beta, bool isSwapping,
                  out string bestAction, out MoveInfo bestMove, out int bestSwap)
    {
        bestAction = "Attack"; bestMove = null; bestSwap = -1;
        if (depth == 0 || IsPartyDead(my) || IsPartyDead(opp))
            return EvaluateGameState(my, opp);

        float bestScore = isMax ? float.NegativeInfinity : float.PositiveInfinity;
        var actions = GetAllActions(my, opp, isSwapping);
        
        foreach (var (act, mv, idx) in actions)
        {
            float bonus = 0;
            var myClone = my.Clone();
            var oppClone = opp.Clone();
            ApplyAction(myClone, oppClone, act, mv, idx);
            float score = Minimax(oppClone, myClone, depth - 1, !isMax, alpha, beta, isSwapping, out _, out _, out _);

            if (act == "Attack")
            {
                if (mv.name == "Struggle")
                {
                    bonus = 0.25f * my.Trainer.attack_priority;
                }
                else
                {   
                    int predictedDamage = DamageCal(my, opp, mv);
                    int targetHP = opp.MonTeam[opp.currentMon].stats.health;
                    if (predictedDamage >= targetHP || my.MonTeam[my.currentMon].stats.health >= 1)
                    {
                        bonus = 2 * my.Trainer.attack_priority;
                    }
                    else
                    {
                        bonus = my.Trainer.attack_priority;
                    }
                }
            }
            else if (act == "Heal")
            {
                float healthRatio = my.MonTeam[my.currentMon].stats.health / (float)my.MonTeam[my.currentMon].stats.total_health;
                if (healthRatio <= my.Trainer.heal_threshold / 100f){
                    bonus = 2 * my.Trainer.heal_priority;
                }
                else {
                    bonus = my.Trainer.heal_priority;
                }
            }
            else if (act == "Swap")
            {
                bonus = my.Trainer.swap_priority;
            }

            score = isMax ? score + bonus : score - bonus;

            if (isMax && score > bestScore)
            {
                bestScore = score; bestAction = act; bestMove = mv; bestSwap = idx;
                alpha = Mathf.Max(alpha, score);
            }
            else if (!isMax && score < bestScore)
            {
                bestScore = score;
                beta = Mathf.Min(beta, score);
            }
            if (beta <= alpha) break;
        }
        return bestScore;
    }

    // Gets all possible actions the trainer can do that turn
    List<(string, MoveInfo, int)> GetAllActions(MonParty party, MonParty opponent, bool isSwapping)
    {
        var actions = new List<(string, MoveInfo, int)>();
        if (party.currentMon < 0 || party.currentMon >= party.MonTeam.Count)
        {
            party.currentMon = 0;
        }
        var mon = party.MonTeam[party.currentMon];

        //Debug.Log($"[AI] {party.Trainer.name}'s simulated currentMon is: {party.currentMon}, HP: {party.MonTeam[party.currentMon].stats.health}");


        if (mon.stats.health <= 0 || party.MonTeam[party.currentMon].stats.health <= 0)
        {
            // Only add swap options
            for (int i = 0; i < party.MonTeam.Count; i++)
            {
                if (i != party.currentMon && party.MonTeam[i].stats.health > 0)
                    actions.Add(("Swap", null, i));
            }
            return actions;
        }

        if (!isSwapping && mon.stats.health > 0)
        {
            // Gets all moves that have power points
            foreach (var move in mon.moves)
                if (move.pp > 0) actions.Add(("Attack", move, -1));
            
            // If the current mon has 0 power points in any of their moves
            if (actions.Count == 0) actions.Add(("Attack", struggle, -1));

            // Adds heal action if the mon is at a certian threshold for healing
            if (party.potionCount > 0 && mon.stats.health / (float)mon.stats.total_health <= party.Trainer.heal_threshold / 100f)
                actions.Add(("Heal", null, -1));
        }

        // Gets all available mons which have not fainted yet
        for (int i = 0; i < party.MonTeam.Count; i++)
        {
            if (i != party.currentMon && party.MonTeam[i].stats.health > 0 || party.MonTeam[i].stats.health > 0 && isSwapping)
            {
                //Debug.Log($"{party.Trainer.name}'s {party.MonTeam[i].name}");
                actions.Add(("Swap", null, i));
            }
        }

        return actions;
    }

    void ApplyAction(MonParty my, MonParty opp, string action, MoveInfo move, int swapIndex)
    {
        switch (action)
        {
            case "Attack":
                my.moveSelected = move;
                AttackSim(my, opp);
                break;
            case "Heal":
                var mon = my.MonTeam[my.currentMon];
                mon.stats.health = mon.stats.total_health;
                mon.status = "None";
                my.potionCount--;
                break;
            case "Swap":
                my.currentMon = swapIndex;
                break;
        }
    }

    void AttackSim(MonParty AtkParty, MonParty DefParty)
    {
        int dmg = DamageCal(AtkParty, DefParty, AtkParty.moveSelected);
        DefParty.MonTeam[DefParty.currentMon].stats.health -= dmg;
        if (DefParty.MonTeam[DefParty.currentMon].stats.health < 0)
            DefParty.MonTeam[DefParty.currentMon].stats.health = 0;
    }

    // Caculates damage dealt to defending party
    int DamageCal(MonParty AtkParty, MonParty DefParty, MoveInfo move) 
    {
        float attack = 0;
        float defense = 0;
        float stab = 0;
        float crit = 1;
        
        if (move.attackType == "Physical")
        {
            attack = AtkParty.MonTeam[AtkParty.currentMon].stats.attack;
            defense = DefParty.MonTeam[DefParty.currentMon].stats.defense;
        }
        if (move.attackType == "Special")
        {
            attack = AtkParty.MonTeam[AtkParty.currentMon].stats.special_attack;
            defense = DefParty.MonTeam[DefParty.currentMon].stats.special_defense;
        }

        float type_modifier1 = TypeCheck(move.type, DefParty.MonTeam[DefParty.currentMon].type1);
        float type_modifier2;

        if (!string.IsNullOrEmpty(DefParty.MonTeam[DefParty.currentMon].type2))
            type_modifier2 = TypeCheck(move.type, DefParty.MonTeam[DefParty.currentMon].type2);
        else 
            type_modifier2 = 1;

        // Checks for type effectiveness
        float type_modifier = type_modifier1 * type_modifier2;
        
        // Stab
        if (move.type == AtkParty.MonTeam[AtkParty.currentMon].type1 || 
            (AtkParty.MonTeam[AtkParty.currentMon].type2 != null &&
            move.type == AtkParty.MonTeam[AtkParty.currentMon].type2))
        {
            stab = 1.5f;
        }
        else
        {
            stab = 1;
        }
       
        float random = UnityEngine.Random.Range(85f, 101f) / 100f;

        if (UnityEngine.Random.Range(1, 256) == 1)
        {
            crit = ((2*AtkParty.MonTeam[AtkParty.currentMon].level) + 5) / 
                ((AtkParty.MonTeam[AtkParty.currentMon].level) + 5);
        }

        float damage = ( ((((2*AtkParty.MonTeam[AtkParty.currentMon].level)/5)+2) * move.power
            * (attack/defense)) / (50) + 2) * crit * random * stab * type_modifier;

        return (int)damage;
    }

    // Checks type effectiveness for type modifier
    float TypeCheck(string moveType, string defType)
    {
        TypeInfo targetType = typeDatabase.GetTypeByName(moveType);
        if (moveType == "Typeless")
            return 1f;
        // No 2nd type
        if (defType == null)
            return 1f;
        // Super Effective
        if (targetType.effective.Contains(defType))
            return 2f;
        // Not Very Effective
        if (targetType.weak.Contains(defType))
            return 0.5f;
        // No Effect
        if (targetType.no_effect.Contains(defType))
            return 0f;
        // Normal
        else
            return 1f;
    }

    float EvaluateGameState(MonParty myParty, MonParty opponentParty)
{
    float myScore = 0;
    float opponentScore = 0;

    foreach (var mon in myParty.MonTeam)
    {
        float ratio = mon.stats.health / (float)mon.stats.total_health;
        myScore += mon.stats.health + (ratio < 0.3f ? -20 : 0); // Penalize low-HP mons
    }

    foreach (var mon in opponentParty.MonTeam)
    {
        float ratio = mon.stats.health / (float)mon.stats.total_health;
        opponentScore += mon.stats.health + (ratio < 0.3f ? -20 : 0);
    }

    return myScore - opponentScore;
}
}

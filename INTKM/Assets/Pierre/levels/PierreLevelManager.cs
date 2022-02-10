using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PierreLevelManager : MonoBehaviour
{
    public PierreManagerAI entitiesManager;
    private AudioSource audio;

    public AudioClip miaou;
    public AudioClip ouaf;

    public Canvas can;
    public Canvas generique;
    public Text label;

    public PierrePlayer player1;
    public PierrePlayer player2;

    public Canvas gameover;

    public bool generiqueDone = false;
    public bool gameDone = false;
    public bool introDone = false;

    public bool failed = false;

    public int currentText = 0;

    private string[] text = { "Player 1 (Cat): These lizards think that the street belongs to them, let's put our differences aside and show them who is in charge here !\n\n(Press any key to continue)",
                             "Player 2 (Dog): If one of us is beaten, the other one will have to help him get up, just stay close.\n\n(Press any key to continue)",
                             "Player 1 (Cat): I know I know, I move with 'ZQSD' you with 'OKLM', I sprint with 'F', you with 'J', I hit with 'A' and you with 'I'.\n\n(Press any key to continue)",
                             "Player 2 (Dog): Yes, yes, anyway the keys are in the option menu. Let's show them who this street belongs to!\n\n(Press any key to continue)"
    };


    // Start is called before the first frame update
    void Start()
    {
        label.text = text[0];
        audio = GetComponent<AudioSource>();
        generique.enabled = false;
        gameover.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {


       
        if (introDone && !failed && player1.IsDead() && player2.IsDead())
        {
            can.enabled = false;
            failed = true;
            generique.enabled = false;
            gameover.enabled = true;
        }


        if (Input.anyKeyDown)
        {

            if (failed)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                return;
            }

            if (!introDone)
            {
                currentText++;
                if (currentText == text.Length)
                {
                    introDone = true;
                    can.enabled = false;
                    entitiesManager.EnableAll();
                    return;
                }
                label.text = text[currentText];
                audio.Stop();
                if (currentText % 2 == 0)
                    audio.clip = miaou;
                else audio.clip = ouaf;
                audio.Play();
                
            } else if (gameDone && !generiqueDone)
            {
                // show generic
                can.enabled = false;
                generique.enabled = true;
                generiqueDone = true;

            } else if(generiqueDone)
            {
                // quit generic
                SceneManager.LoadScene("MainMenu");
            }
        }

        if (introDone && entitiesManager.allLizardAreDead() && !gameDone)
        {
            foreach (PierrePlayer p in entitiesManager.players) p.activated = false;
            gameDone = true;
            can.enabled = true;
            label.text = "Piece of cake !\n\n(Press any key to continue)";
        }

    }
}

using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;


public class UIController : MonoBehaviour
{
    public VisualElement root;
    public Button startButton;
    public Button howToPlayButton;
    public Label howToPlayLabel;

    // Start is called before the first frame update
    private void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        startButton = root.Q<Button>("start-button");
        startButton.Focus();
        startButton.Focus();
        howToPlayButton = root.Q<Button>("how-to-play-button");
        howToPlayLabel = root.Q<Label>("how-to-play-label");
        startButton.clicked += StartButtonPressed;
        howToPlayButton.clicked += HowToPlayButtonPressed;
        Debug.Log(root.focusController);
    }

    // Update is called once per frame
    private void StartButtonPressed()
    {
        howToPlayLabel.visible = false;
        root.visible = false;
        SceneManager.LoadScene(0);
    }

    private void HowToPlayButtonPressed()
    {
        if (howToPlayLabel.visible)
        {
            howToPlayLabel.visible = false;
        }
        else
        {
            howToPlayLabel.visible = true;
            howToPlayLabel.text =
                "Objective: Bump all of the enemies off of the island. \n" +
                "Controls: Press 'J' to jump, 'Spacebar' to use powerups, and 'P' to pause.";
            howToPlayLabel.style.display = DisplayStyle.Flex;
        }
    }
}
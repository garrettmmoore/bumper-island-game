using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    public VisualElement root;
    public Button startButton;
    public Button howToPlayButton;
    public Label howToPlayLabel;
    private GameManager _gameManager;

    // Start is called before the first frame update
    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        root = GetComponent<UIDocument>().rootVisualElement;
        startButton = root.Q<Button>("start-button");
        howToPlayButton = root.Q<Button>("how-to-play-button");
        howToPlayLabel = root.Q<Label>("how-to-play-label");

        startButton.clicked += StartButtonPressed;
        howToPlayButton.clicked += HowToPlayButtonPressed;
    }

    // Update is called once per frame
    private void StartButtonPressed()
    {
        howToPlayLabel.visible = false;
        root.visible = false;
        _gameManager.StartGame();
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
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton instance of GameManager
    private Opponent currentOpponent;

    private Controller currentController;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Method to set the current active opponent
    public void SetOpponent(Opponent opponent)
    {
        currentOpponent = opponent;
    }

    public void setController(Controller controller) {
        currentController = controller;

    }

    public Controller getController() {
        return currentController;
    }
    // Method to get the current active opponent
    public Opponent GetOpponent()
    {
        return currentOpponent;
    }

    // Method to clear the opponent reference when it dies
    public void ClearOpponent()
    {
        currentOpponent = null;
    }
}

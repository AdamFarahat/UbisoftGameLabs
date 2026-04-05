using UnityEngine;


public class DifficultyTracker : MonoBehaviour
{
    private string difficulty = "Normal";
    public string Difficulty => difficulty;

    public void SetDifficulty(string difficulty)
    {
        this.difficulty = difficulty;
        gameObject.SetActive(false);
    }
}

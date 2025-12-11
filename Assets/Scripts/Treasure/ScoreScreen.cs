using TMPro;
using UnityEngine;

public class ScoreScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeoutsText;
    [SerializeField] private TextMeshProUGUI hitsText;
    [SerializeField] private TextMeshProUGUI missesText;
    [SerializeField] private TextMeshProUGUI accText;
    [SerializeField] private TextMeshProUGUI treasureCountText;
    [SerializeField] private TextMeshProUGUI scoreText;

    void Awake()
    {
        updateScreenValues(0, 0, 0, 0, 0, 0, 0);
    }

    public void updateScreenValues(int timeOuts, int hits, int misses, float acc, int treasures, int total, int score)
    {
        timeoutsText.text = "Crit points timed out: " + timeOuts;
        hitsText.text = "Crit points hit: " + hits;
        missesText.text = "Crit points missed: " + misses;
        accText.text = "Average distance: " + acc;
        treasureCountText.text = "Treasures found: " + treasures + "/" + total;
        scoreText.text = "Score: " + score;
    }
}

using UnityEngine;

public class ScoreManager : MonoBehaviour
{

    private int critPointsTimedOut = 0;
    private int critPointsHit = 0;
    private int critPointsMiss = 0;
    private float critPointAccuracy = 0; // of the hit points, how close to center on average
    [SerializeField] float maxDistForMiss = 2;


    private int treasuresFound = 0;
    private int treasureTotal = 0;
    private int score;

    [SerializeField] private ScoreScreen screen;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void updateScoreScreen()
    {
        if (screen != null)
        {
            screen.updateScreenValues(critPointsTimedOut,critPointsHit, critPointsMiss, critPointAccuracy, treasuresFound, treasureTotal, score);
        }
    }



    public int getTimedOutPoints() { return critPointsTimedOut; }
    public int getPointsHit() {  return critPointsHit; }
    public int getPointsMissed() { return critPointsMiss; }
    public float getPointAccuracy() { return critPointAccuracy; }
    public int getScore() { return score; }
    public int getTreasuresFound() {  return treasuresFound; }
    public int getTreasureTotal() { return treasureTotal; }


    public void addTimedOutPoint() { 
        critPointsTimedOut++;
        updateScoreScreen();
    }
    public void addPointsHit(float distance) {
        critPointAccuracy = (critPointAccuracy * (critPointsHit + critPointsMiss) + distance) / (critPointsHit + critPointsMiss + 1);
        critPointsHit++;
        updateScoreScreen();
    }
    public void addPointsMiss(float distance) {
        if (distance > maxDistForMiss) { return; }
        critPointAccuracy = (critPointAccuracy * (critPointsHit + critPointsMiss) + distance) / (critPointsHit + critPointsMiss + 1);
        critPointsMiss++;
        updateScoreScreen();
    }

    public void addTreasureFound() { 
        treasuresFound++;
        updateScoreScreen();
    }

    public void addScore(int val) { 
        score+=val;
        updateScoreScreen();
    }

    public void setTreasureTotal(int tot) { 
        treasureTotal = tot;
        updateScoreScreen();
    }

}

[System.Serializable]
public class TaskResult
{
    public string taskName;
    public string timePressureMode;
    public bool success;
    public int moveCount;
    public int illegalMoveCount;
    public int invalidDropCount;
    public float timeToCompletion;
    public float timeLimit;
}
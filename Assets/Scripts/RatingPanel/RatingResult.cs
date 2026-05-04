namespace DataLogging
{
    [System.Serializable]
    public class RatingResult
    {
        public string ratingStage;
        public string taskName;
        public string timePressureMode;

        public int stressRating;
        public int calmnessRating;
        public int moodRating;
        public int pressureRating;
        public int difficultyRating;

        public string timestamp;
    }
}
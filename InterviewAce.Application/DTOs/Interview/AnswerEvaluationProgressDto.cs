namespace InterviewAce.Application.DTOs.Interview;

public class AnswerEvaluationProgressDto
{
    public int LatestScore { get; set; }

    public int PreviousScore { get; set; }

    public int BestScore { get; set; }

    public double AverageScore { get; set; }

    public int EvaluationCount { get; set; }

    public int ScoreChange { get; set; }
}
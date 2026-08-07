namespace InterviewAce.Application.DTOs.AI;

public class GeminiResponseDto
{
    public List<GeminiCandidateDto>? Candidates { get; set; }
}


public class GeminiCandidateDto
{
    public GeminiContentDto? Content { get; set; }
}


public class GeminiContentDto
{
    public List<GeminiPartDto>? Parts { get; set; }
}


public class GeminiPartDto
{
    public string? Text { get; set; }
}
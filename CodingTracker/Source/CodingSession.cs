namespace vcesario.CodingTracker;

public class CodingSession
{
    private DateTime m_Start;
    private DateTime m_End;

    public CodingSession(DateTime startDateTime, DateTime endDateTime)
    {
        m_Start = startDateTime;
        m_End = endDateTime;
    }

    public bool Validate()
    {
        // end should be greater than start, at least 1 second long
        // end should be now or less

        return true;
    }
}
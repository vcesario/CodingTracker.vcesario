namespace vcesario.CodingTracker;

public class CodingSession
{
    public long Id => m_Id;
    public DateTime Start => m_Start;
    public DateTime End => m_End;

    private long m_Id;
    private DateTime m_Start;
    private DateTime m_End;

    public CodingSession(DateTime startDateTime, DateTime endDateTime)
    {
        m_Id = -1;
        m_Start = startDateTime;
        m_End = endDateTime;
    }

    public CodingSession(long rowid, DateTime start_date, DateTime end_date)
    {
        m_Id = rowid;
        m_Start = start_date;
        m_End = end_date;
    }

    public bool Validate()
    {
        // end should be greater than start, at least 1 second long
        // end should be now or less

        return true;
    }

    public TimeSpan GetDuration()
    {
        TimeSpan duration = End - Start;
        return duration;
    }
}
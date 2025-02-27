namespace vcesario.CodingTracker;

public class CodingGoal
{
    private int m_Value;
    private DateOnly m_StartDate;
    private DateOnly m_DueDate;

    public int Value => m_Value;
    public DateOnly StartDate => m_StartDate;
    public DateOnly DueDate => m_DueDate;

    public CodingGoal(int value, DateTime start_date, DateTime due_date)
    {
        m_Value = value;
        m_StartDate = DateOnly.FromDateTime(start_date);
        m_DueDate = DateOnly.FromDateTime(due_date);
    }
}
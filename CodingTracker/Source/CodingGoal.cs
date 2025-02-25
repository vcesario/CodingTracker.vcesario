namespace vcesario.CodingTracker;

public class CodingGoal
{
    private uint m_Value;
    private DateOnly m_StartDate;
    private DateOnly m_DueDate;

    public uint Value => m_Value;
    public DateOnly StartDate => m_StartDate;
    public DateOnly DueDate => m_DueDate;

    public CodingGoal(int value, DateTime start_date, DateTime due_date)
    {
        m_Value = (uint)value;
        m_StartDate = DateOnly.FromDateTime(start_date);
        m_DueDate = DateOnly.FromDateTime(due_date);
    }
}
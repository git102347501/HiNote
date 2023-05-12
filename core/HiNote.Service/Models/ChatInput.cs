namespace HiNote.Service.Models;

public class ChatInput
{
    public string role { get; set; }

    public string content { get; set; }

    public ChatInput()
    {
        
    }

    public ChatInput(string role, string content)
    {
        this.role = role;
        this.content = content;
    }
}
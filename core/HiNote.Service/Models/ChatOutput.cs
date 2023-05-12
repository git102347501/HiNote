using System.Collections.Generic;

namespace HiNote.Service.Models;

public class ChatOutput
{
    public string id { get; set; }

    public List<ChatOutputChoice> choices { get; set; }
}

public class ChatOutputChoice
{
    public int index { get; set; }
    
    public ChatOutputMessage message { get; set; }

    public string finish_reason { get; set; }
}

public class ChatOutputMessage
{
    public string role { get; set; }

    public string content { get; set; }
}
namespace HiNote.Service.Models;

// 定义响应模型
public class OpenAIResponse
{
    public OpenAIChoice[] choices { get; set; }
}

public class OpenAIChoice
{
    public string text { get; set; }
}
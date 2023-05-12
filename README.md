# HiNote 
Win11 native application notebook based on WinUi3 design
基于WinUi3 设计的现代化Win11笔记本应用

Official website：https://note.magicalconch.com

![microsoft](https://user-images.githubusercontent.com/37917403/206961341-fadbdff9-e178-4d83-9245-9bb94dc97816.png)

[Download from Microsoft Store](https://apps.microsoft.com/store/detail/hellonote/9N94LT5S8FD9?hl=zh-cn&gl=cn)

# How to use（开始使用项目）

- # Git Clone（克隆项目）

   `` git clone https://github.com/git102347501/HiNote.git ``

- # Env（建立项目环境）
  - VS2022
  - Winui3 SDK
  - UWP DeskTop Develop Tool

- # Introduction（项目分层介绍）
  - # api
    
    Based on the backend interface layer of ABP vnext, it implements the addition, deletion, modification, and authentication services of the journal foundation
  
  - # core
    
    Interface call abstraction layer, you can use this layer to implement abstraction to connect to your own API, or you can use the project's built-in API, just      configure the API address
    
    Config to: HiNote.Core.Services.BasicService
    ```
     public class BasicService
     {
        public HttpClient HttpClient;

        public const string AuthUrl = "https://www.auth.com";
        public const string ApiUrl = "https://www.api.com";
     }
    
    ```
  
  - # winui3
    
    Winui3 page layer, this is the front-end page implementation layer. If you want to preview the development of the front-end, please proceed on this layer
    
  - # markdown-web
    
    A simple markdown web layer for rendering editors in MD format. If you need this type of editor, you need to deploy this site and point the markdown editor        address of Winui3 to your deployment address

# Call Relationship（调用关系）：

Winui3 uses the nuget method to use the interface services provided by the core layer, which implements abstraction and calls the API layer's basic services using an HTTP client.

Completed functions



Light and dark theme support

- Directory visualization drag and drop management

- Chinese and English multilingual support

- Winui3 Rich Text Editor Support

- Markdown online web editor support (requires deployment of web pages)

- Text Insertion Generation Based on OpenAI


# Note Main Page (主页面):
![image](https://github.com/git102347501/HiNote/assets/37917403/ffa1b104-83d8-4d06-8973-d10463721c24)
![image](https://github.com/git102347501/HiNote/assets/37917403/039b62ac-64b3-43c6-a96a-e70cfc2d15e5)
![image](https://github.com/git102347501/HiNote/assets/37917403/b21b8e26-a856-4017-8cc2-d777cebf89f9)
![image](https://github.com/git102347501/HiNote/assets/37917403/22e6d6de-8de5-43ed-ac88-2fcd93d81422)


# Note Setting Page (设置页面):
![3](https://user-images.githubusercontent.com/37917403/204701252-6d3e6d2e-9a95-4e68-8e49-7df3056d6788.png)

# Note Bright theme (明亮主题):
![4](https://user-images.githubusercontent.com/37917403/204701255-46b4240b-d28a-4916-8336-80ae15e19125.png)


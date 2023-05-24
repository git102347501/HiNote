# HiNote 

# Win11 native application notebook based on WinUi3 design
  基于WinUi3 设计的现代化Win11笔记本应用

# Official website (官网直达): 
  https://note.magicalconch.com

# Microsoft Store (微软商店直达)
[![microsoft](https://user-images.githubusercontent.com/37917403/206961341-fadbdff9-e178-4d83-9245-9bb94dc97816.png)
](https://apps.microsoft.com/store/detail/hellonote/9N94LT5S8FD9?hl=zh-cn&gl=cn)

# How to use（开始使用项目）

- # Git Clone（克隆项目）

   `` git clone https://github.com/git102347501/HiNote.git ``

- # Env（建立项目环境）
  - VS2022
  - Winui3 SDK
  - UWP DeskTop Develop Tool
  - Windows(10/11) SDK
  - Net 6.0 SDK

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
![38b61eca49a9d1ff382b45ed24725e0](https://github.com/git102347501/HiNote/assets/37917403/64fb3932-6a15-440b-8377-d66c3e064611)
![794a16ec101a5b93266f399e8dcc518](https://github.com/git102347501/HiNote/assets/37917403/deedb233-0285-4d16-b74a-09eec9afbe94)

# Multi-Language Support (多语言支持):
![8dfb875514988afbce0015a407a1b61](https://github.com/git102347501/HiNote/assets/37917403/b60082b1-f215-4124-8e45-bfac1a26d914)
![67eaf34df253cc3262c1291bdfdfc8c](https://github.com/git102347501/HiNote/assets/37917403/b91c4277-2471-47fa-98d3-b54b91936139)


# Full Windows (全屏模式)：
![image](https://github.com/git102347501/HiNote/assets/37917403/98638a85-c660-4615-a920-3c2ca3709dff)

# Note Setting Page (设置页面):
![1684805164722](https://github.com/git102347501/HiNote/assets/37917403/f4a97c48-4f18-473c-b804-343d9f0c0136)

# Note Bright theme (明亮主题):
![4](https://user-images.githubusercontent.com/37917403/204701255-46b4240b-d28a-4916-8336-80ae15e19125.png)


# AI-Powered Quiz Application with ASP.NET Core MVC and OpenAI

### Project Overview
This project combines the power of ASP.NET Core MVC, C#, and OpenAI’s GPT-4 to deliver an AI-driven quiz application. Developed in Visual Studio 2022 with MS SQL Server 20, this application dynamically generates quiz questions on a range of topics using the OpenAI API, creating an engaging, personalized quiz experience.

### Key Features
- **Dynamic, AI-Generated Quizzes**: Using the OpenAI API, each quiz generates fresh questions in real-time. Users are not limited to predefined questions, making every quiz unique and engaging.
- **Flexible Topic Selection**: Users can choose from a variety of topics, including popular programming languages (SQL, C#, Python, JavaScript, PHP) or explore general knowledge, history, movies, wildlife, and more.
- **Customized Difficulty and Language Options**: Users can adjust the quiz difficulty (Easy, Medium, or Hard) and select from over 40 languages, ensuring an inclusive, customized experience.
- **Automatic Scoring and Result Options**: Upon submission, users receive instant feedback on their quiz performance, with options to download a detailed PDF report or receive their score via email.
- **User Authorization and Security**: The quiz is accessible only to registered users, offering a secure, personalized experience.

### Quick Customization for Developers
For developers, it’s easy to change the OpenAI model version. To modify the AI model for generating questions, update this section in `OpenAiService.cs`:
```csharp
var requestBody = new
{
    model = "gpt-4",
    messages = messages,
    max_tokens = 1500
};
```

## Technology Stack
This project is crafted with a modern technology stack optimized for performance and a smooth user experience:

- **ASP.NET Core MVC**: A scalable and structured framework for building robust applications.
- **Entity Framework Core**: Streamlines database operations with MS SQL Server.
- **OpenAI GPT-4**: Generates dynamic quiz questions, offering a unique and engaging experience.
- **PdfSharpCore**: Creates downloadable PDF reports for quiz results.
- **MailKit**: Enables email delivery of quiz results upon user request.
- **HTML, CSS, JavaScript, Bootstrap**: Provides a responsive, polished front-end interface.
- **GitHub**: Facilitates version control for collaborative development.
- **C# (OOP)**: Utilizes object-oriented programming for organized and maintainable backend logic.

## Why This Project Stands Out
This project exemplifies a full-stack web application that incorporates AI capabilities within the .NET 8 framework. It showcases advanced backend development using Entity Framework Core and MS SQL Server, combined with front-end best practices to deliver an intuitive and engaging user experience. The integration of AI with .NET technology positions this project as a leading-edge example of modern web development.

## Tags
- **Artificial Intelligence**
- **OpenAI**
- **ChatGPT-4.0**
- **.NET Core**
- **ASP.NET Core MVC**
- **Entity Framework Core**
- **MS SQL Server**
- **MailKit**
- **GitHub**
- **HTML, CSS, JavaScript, Bootstrap**
- **C# (OOP)**
- **Razor Pages**

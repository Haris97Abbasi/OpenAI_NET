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

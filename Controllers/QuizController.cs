using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OpenAiProject.Models;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Drawing;
using System.Xml.Linq;

[Authorize]
public class QuizController : Controller
{
    private readonly OpenAiService _openAiService;

    public QuizController(OpenAiService openAiService)
    {
        _openAiService = openAiService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new QuizRequest());
    }

    [HttpPost]
    public async Task<IActionResult> GenerateQuiz(QuizRequest quizRequest)
    {
        if (ModelState.IsValid)
        {
            var quiz = await _openAiService.GenerateQuizAsync(quizRequest.Topic, quizRequest.Difficulty, quizRequest.Language);

            // Ensure Questions is not null before passing to view
            if (quiz.Questions == null || quiz.Questions.Count == 0)
            {
                quiz.Questions = new List<Question>(); // Initialize with an empty list
                Console.WriteLine("No questions were returned from OpenAI or deserialization failed.");
            }

            return View("Quiz", quiz); // Pass the quiz data to the Quiz view
        }
        return View("Index", quizRequest); // Return to form if input is invalid
    }


    [HttpPost]
    public IActionResult SubmitQuiz(QuizResponse quizResponse)
    {
        int score = 0;

        // Ensure all selected option indices are within range
        foreach (var question in quizResponse.Questions)
        {
            if(question == null)
            {
                Console.WriteLine("Question is null");
            }
            else
            {
                Console.WriteLine(question.Text);
                Console.WriteLine(question.SelectedOptionIndex);
                Console.WriteLine(question.CorrectOptionIndex);
            }
            if (question.SelectedOptionIndex.HasValue &&
                question.SelectedOptionIndex.Value >= 0 &&
                question.SelectedOptionIndex.Value < question.Options.Count)
            {
                // Compare selected option with the correct option
                if (question.SelectedOptionIndex == question.CorrectOptionIndex)
                {
                    score++;
                }
            }
            else
            {
                Console.WriteLine($"Invalid selection for question: {question.Text}");
            }
        }

        // tempData will be used in downloading pdf
        TempData["QuizResponse"] = JsonConvert.SerializeObject(quizResponse); 
        ViewBag.Score = score;
        ViewBag.TotalQuestions = quizResponse.Questions.Count;
        return View("Results", quizResponse);
    }

    public async Task<IActionResult> EmailQuizScore(int score, int totalQuestions, string receiverEmail)
    {
        string ReceiverEmail = receiverEmail;
        await _openAiService.Send(score.ToString(), totalQuestions.ToString(), ReceiverEmail);
        return RedirectToAction("Index");
    }

    public IActionResult DownloadResult(int score, int totalQuestions)
    {
        // Retrieve QuizResponse from TempData
        var quizResponseJson = TempData["QuizResponse"] as string;
        if (string.IsNullOrEmpty(quizResponseJson))
        {
            return NotFound("Quiz data not found.");
        }
        var quizResponse = JsonConvert.DeserializeObject<QuizResponse>(quizResponseJson);

        var document = new PdfDocument();
        var page = document.AddPage();
        var graphics = XGraphics.FromPdfPage(page);
        var fontTitle = new XFont("Arial", 18, XFontStyle.Bold);
        var fontText = new XFont("Arial", 12, XFontStyle.Regular);

        // Write the quiz result information
        graphics.DrawString("Quiz Results", fontTitle, XBrushes.Black, new XRect(0, 0, page.Width, 40), XStringFormats.TopCenter);
        graphics.DrawString($"Score: {score}/{totalQuestions}", fontText, XBrushes.Black, new XRect(40, 60, page.Width - 80, 0), XStringFormats.TopLeft);

        int yPoint = 100; // Starting Y position for questions
        int count = 1;

        foreach (var question in quizResponse.Questions)
        {
            // Question text
            graphics.DrawString($"Question {count}: {question.Text}", fontText, XBrushes.Black, new XRect(40, yPoint, page.Width - 80, 0), XStringFormats.TopLeft);
            yPoint += 20;
            count++;

            // Selected answer and correctness
            if (question.SelectedOptionIndex.HasValue &&
                question.SelectedOptionIndex.Value >= 0 &&
                question.SelectedOptionIndex.Value < question.Options.Count)
            {
                string userAnswer = question.Options[question.SelectedOptionIndex.Value];
                bool isCorrect = question.SelectedOptionIndex == question.CorrectOptionIndex;
                string resultText = isCorrect ? "Correct" : "Incorrect";

                // User answer with result
                graphics.DrawString($"Your Answer: {userAnswer} - {resultText}", fontText, isCorrect ? XBrushes.Green : XBrushes.Red, new XRect(40, yPoint, page.Width - 80, 0), XStringFormats.TopLeft);
            }
            else
            {
                graphics.DrawString("Your Answer: No valid answer selected", fontText, XBrushes.Red, new XRect(40, yPoint, page.Width - 80, 0), XStringFormats.TopLeft);
            }

            yPoint += 20;

            // Correct answer
            graphics.DrawString($"Correct Answer: {question.Options[question.CorrectOptionIndex]}", fontText, XBrushes.Black, new XRect(40, yPoint, page.Width - 80, 0), XStringFormats.TopLeft);
            yPoint += 40; // Extra space between questions
            if (yPoint > page.Height - 100)
            {
                page = document.AddPage();
                graphics = XGraphics.FromPdfPage(page);
                yPoint = 40;
            }
        }

        // Save the document into a memory stream
        using (var stream = new MemoryStream())
        {
            document.Save(stream, false);
            var fileBytes = stream.ToArray();
            return File(fileBytes, "application/pdf", "QuizResults.pdf");
        }
    }

}

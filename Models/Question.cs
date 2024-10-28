namespace OpenAiProject.Models
{
    public class Question
    {
        public string Text { get; set; }
        public List<string> Options { get; set; } = new List<string>();
        public int CorrectOptionIndex { get; set; }
        public int? SelectedOptionIndex { get; set; }
    }
}

namespace ProductService.Domain.Entities.ValueObjects
{
    public class Qna
    {
        public List<Question>? Questions { get; private set; } = [];
        public int TotalQuestions { get; private set; } = 0;

        internal void AddQuestion(Question question)
        {
            Questions ??= [];
            Questions.Add(question);
            UpdateTotalQuestions();
        }
        internal void RemoveQuestion(Question question)
        {
            if (Questions != null && Questions.Contains(question))
            {
                Questions.Remove(question);
                UpdateTotalQuestions();
            }

        }
        private void UpdateTotalQuestions()
        {
            TotalQuestions = Questions?.Count ?? 0;
        }
    }
}

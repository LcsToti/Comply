using System;
using System.Collections.Generic;
using System.Text;

namespace ProductService.Domain.Entities.ValueObjects
{
    public class Question
    {
        public Guid QuestionId { get; private set; }
        public Guid UserId { get; private set; }
        public string QuestionText { get; private set; }
        public DateTime AskedAt { get; private set; }
        public Answer? Answer { get; private set; }


        internal Question(Guid userId, string questionText)
        {
            QuestionId = Guid.NewGuid();
            UserId = userId;
            QuestionText = questionText;
            AskedAt = DateTime.UtcNow;
        }

        internal void UpdateQuestion(string questionText)
        {
            if (string.IsNullOrWhiteSpace(questionText))
            {
                throw new ArgumentException("Question text cannot be empty.");
            }

            QuestionText = questionText;
            AskedAt = DateTime.UtcNow;
        }

        internal void AddAnswer(string answerText)
        {
            if (Answer != null)
            {
                throw new InvalidOperationException("Question already answered.");
            }

            Answer = new Answer(answerText);
        }

        internal void RemoveAnswer()
        {
            if (Answer == null)
            {
                throw new InvalidOperationException("No answer to remove.");
            }
            Answer = null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ProductService.Domain.Entities.ValueObjects
{
    public class Answer
    {
        public string AnswerText { get; private set; }
        public DateTime AnsweredAt { get; private set; }

        internal Answer(string text)
        {
            AnswerText = text;
            AnsweredAt = DateTime.UtcNow;
        }

        internal void UpdateAnswer(string newText)
        {
            AnswerText = newText;
            AnsweredAt = DateTime.UtcNow;
        }

    }
}

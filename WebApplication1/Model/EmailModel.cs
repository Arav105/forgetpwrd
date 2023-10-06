using System;

namespace WebApplication1.Model
{
    public class EmailModel
    {
        internal string content;

        public string To { get; set; }
        public string Subject { get; set; }
        public string  Content { get; set; }
        public EmailModel(string to,string subject,string content)
        {
            To = to;
            Subject = subject;
            Content = content;  
        }
    }
}
